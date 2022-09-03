using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TL;

namespace SearchForMessagesInTgAccordingToCertainCriteriaAndForward
{
    class TgClient
    {
        private WTelegram.Client Client { get; set; }
        private User My { get; set; }
        private readonly Dictionary<long, User> Users = new();
        private readonly Dictionary<long, ChatBase> Chats = new();

        public TgClient(string _paternRegular)
        {
            paternRegular = _paternRegular;
        }

        private long ChatForMessages { get; set; }
        private string paternRegular { get; set; }


        private string Config(string what)
        {
            switch (what)
            {
                case "api_id": Console.Write("API Id: "); return "9650623";
                case "api_hash": Console.Write("API Hash: "); return "ec5bb72cbd45cb422e20ced2346ffba6";
                case "phone_number": Console.Write("Phone number: "); return Console.ReadLine();
                case "verification_code": Console.Write("Verification code: "); return Console.ReadLine();  // if sign-up is required
                case "password": Console.Write("Password: "); return Console.ReadLine();     // if user has enabled 2FA
                default: return null;                  // let WTelegramClient decide the default config
            }
        }

        public async Task CopyMashine()
        {

            Client = new WTelegram.Client(Config);

            using (Client)
            {
                Client.OnUpdate += Client_OnUpdate;
                My = await Client.LoginUserIfNeeded();

                var chats = await Client.Messages_GetAllChats();
                foreach (var (id, chat) in chats.chats)
                {
                    switch (chat)
                    {
                        case Chat basicChat when basicChat.IsActive:
                            Console.WriteLine($"{id}:  Basic chat: {basicChat.title} with {basicChat.participants_count} members");
                            break;
                        case Channel group when group.IsGroup:
                            Console.WriteLine($"{id}: Group {group.username}: {group.title}");
                            break;
                        case Channel channel:
                            Console.WriteLine($"{id}: Channel {channel.username}: {channel.title}");
                            break;
                    }
                }

                Console.WriteLine("Введите ID чатa/каналa в которыq вы хотите копировать сообщения: ");
                ChatForMessages = Convert.ToInt64(Console.ReadLine());

                Users[My.id] = My;

                var dialogs = await Client.Messages_GetAllDialogs();
                dialogs.CollectUsersChats(Users, Chats);
                Console.ReadKey();
            }
        }

        private async Task Client_OnUpdate(IObject arg)
        {
            if (arg is not UpdatesBase updates)
            {
                return;
            }
            updates.CollectUsersChats(Users, Chats);
            foreach (var update in updates.UpdateList)
                switch (update)
                {
                    case UpdateNewMessage
                    unm:
                        await DisplayMessage(unm.message);
                        break;
                    default: Console.WriteLine(update.GetType().Name); break;
                }
        }

        private async Task DisplayMessage(MessageBase messageBase, bool edit = false)
        {
            if (edit) Console.Write("(Edit): ");
            switch (messageBase)
            {
                case Message m:
                    if (m.peer_id.ID != ChatForMessages)
                    {
                        MatchCollection matches = Regex.Matches(m.message, paternRegular);

                        var words = await new AllLogic().CreatedListAndFiltrationMatchCollectionAsync(matches);

                        try
                        {
                            foreach (var word in words)
                            {
                                var text = $"<code>{word}</code>";
                                var entities = Client.HtmlToEntities(ref text);
                                await Client.SendMessageAsync(Chats[ChatForMessages], text, entities: entities);
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    break;
            }
        }
    }
}
