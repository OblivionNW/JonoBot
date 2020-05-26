using System;
using System.Collections.Generic;
using System.Text;

namespace JonoBot {
    public class StringUtil {

        /// <summary>
        /// Replaces the first instance of a word in a string.
        /// </summary>
        /// <param name="Text">The string to replace the word in.</param>
        /// <param name="Search">The word to replace.</param>
        /// <param name="Replace">The word to put in its place.</param>
        /// <returns>The modified string.</returns>
        public static string ReplaceFirst(string Text, string Search, string Replace) {
            int Position = Text.IndexOf(Search);
            if (Position < 0) {
                return Text;
            }
            return Text.Substring(0, Position) + Replace + Text.Substring(Position + Search.Length);
        }
    }
}
