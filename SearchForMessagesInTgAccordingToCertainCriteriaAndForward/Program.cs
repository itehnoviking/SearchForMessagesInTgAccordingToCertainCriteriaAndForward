using SearchForMessagesInTgAccordingToCertainCriteriaAndForward;
using System;
using TL;

static class Program
{
    static async Task Main(string[] _)
    {
        var paternRegular = @"(?<![^\s]+)[*0-9a-zA-Z*]{8}(?![^\s]+)";
        var client = new TgClient(paternRegular);
        var task = Task.Run(() => client.CopyMashine());

        await task;
    }
}