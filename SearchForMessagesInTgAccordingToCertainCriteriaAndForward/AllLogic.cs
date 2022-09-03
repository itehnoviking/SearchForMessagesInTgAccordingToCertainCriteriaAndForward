using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SearchForMessagesInTgAccordingToCertainCriteriaAndForward
{
    class AllLogic
    {
        public async Task<IList<string>> CreatedListAndFiltrationMatchCollectionAsync(MatchCollection matches)
        {
            var words = new List<string>();

            foreach (Match match in matches)
            {
                if (!words.Contains(match.Value) & matches.Where(m => m.Value == match.Value).Count() < 2)
                {
                    words.Add(match.Value);
                }
            }

            return words;
        }
    }
}
