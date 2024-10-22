using System;
using System.Collections.Generic;
using System.Linq;
using static BetterChat.BetterChat;

namespace BetterChat
{
    public static class Api
    {
        internal static Dictionary<string, Func<ulong, string>> formattings = [];

        public static bool RegisterFormatting(string formatting, Func<ulong, string> func)
        {
            formatting = $"{{{formatting}}}";
            if (formattings.ContainsKey(formatting))
                return false;

            formattings.Add(formatting, func);
            return true;
        }

        public static string Format(ulong clientId)
        {
            List<ValueTuple<string, int>> formattingTodo = [];
            foreach (string formatting in formattings.Keys)
            {
                int startIndex = 0;
                while (true)
                {
                    if (startIndex == Instance.usernameFormatting.Length)
                        break;

                    int index = Instance.usernameFormatting.IndexOf(formatting, startIndex);
                    if (index == -1)
                        break;

                    formattingTodo.Add(new(formatting, index));
                    startIndex = index + formatting.Length;
                }
            }

            string formattedUsername = Instance.usernameFormatting;
            int indexOffset = 0;
            foreach (ValueTuple<string, int> format in formattingTodo.OrderBy(format => format.Item2))
            {
                int index = format.Item2 + indexOffset;
                string formatting = format.Item1;
                string formatted = formattings[formatting](clientId);

                formattedUsername = $"{formattedUsername[..index]}{formatted}{formattedUsername[(index + formatting.Length)..]}";
                indexOffset += formatted.Length - formatting.Length;
            }
            return formattedUsername;
        }
    }
}