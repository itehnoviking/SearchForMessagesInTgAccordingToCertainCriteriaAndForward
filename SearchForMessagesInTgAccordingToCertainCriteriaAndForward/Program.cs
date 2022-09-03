using SearchForMessagesInTgAccordingToCertainCriteriaAndForward;
using System;
using TL;

static class Program
{
    static async Task Main(string[] _)
    {
        var paternRegular = @"\b[*0-9a-zA-Z*][*0-9a-zA-Z*][*0-9a-zA-Z*][*0-9a-zA-Z*][*0-9a-zA-Z*][*0-9a-zA-Z*][*0-9a-zA-Z*][*0-9a-zA-Z*]\b";
        var client = new TgClient(paternRegular);
        var task = Task.Run(() => client.CopyMashine());

        await task;
    }
}