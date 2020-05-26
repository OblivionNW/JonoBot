using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JonoBot {
    class JonoBotSettings {
        public static string BOT_TOKEN = "";
        public const TokenType TOKEN_TYPE = Discord.TokenType.Bot;

        public const string BOT_NAME = "Jono Bot";
        public const char COMMAND_PREFIX = '%';

        public static string SQL_CONNECTION = "";

        /// <summary>
        /// Loads the bots settings from an external config files.
        /// </summary>
        /// <param name="ConfigFilePath">The settings config file path.</param>
        public static void InitializeSettings(String ConfigFilePath) {
            Dictionary<string, string> Settings = new Dictionary<string, string>();

            // Read in the config file and store it in a dictionary containing name value pairs.
            using (StreamReader Reader = new StreamReader(ConfigFilePath)) {
                string Line;
                while (!string.IsNullOrEmpty(Line = Reader.ReadLine())) {
                    string[] SettingPair = Line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    Settings[SettingPair[0]] = SettingPair[1];
                }
            }

            // Load the BotToken from the config file.
            if (Settings.ContainsKey("BotToken")) {
                BOT_TOKEN = Settings["BotToken"];
            } else {
                //TODO: Handle non existance.
            }

            // Load the SQL connection string from the config file.
            if (Settings.ContainsKey("SQLServer") && Settings.ContainsKey("SQLPort") && Settings.ContainsKey("SQLUser")
                && Settings.ContainsKey("SQLPassword") && Settings.ContainsKey("SQLDB")) {
                SQL_CONNECTION = "Server=" + Settings["SQLServer"] + ";"
                               + "Port=" + Settings["SQLPort"] + ";"
                               + "Database=" + Settings["SQLDB"] + ";"
                               + "User=" + Settings["SQLUser"] + ";"
                               + "Password=" + Settings["SQLPassword"] + ";";
            } else {
                //TODO: Handle non existance.
            }
        }
    }
}
