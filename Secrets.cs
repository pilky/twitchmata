using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Twitchmata {
    public static class Secrets {
        public static string ClientID() {
            return ReadString("client_id.txt");
        }

        public static string ClientSecret() {
            return ReadString("client_secret.txt");
        }

        public static string BotAccessToken() {
            return ReadString("bot_access_token.txt");
        }

        public static void SetBotAccessToken(string newToken) {
            WriteString(newToken, "bot_access_token.txt");
        }

        public static string BotRefreshToken() {
            return ReadString("bot_refresh_token.txt");
        }

        public static void SetBotRefreshToken(string newToken) {
            WriteString(newToken, "bot_refresh_token.txt");
        }

        public static string AccountAccessToken() {
            return ReadString("account_access_token.txt");
        }

        public static void SetAccountAccessToken(string newToken) {
            WriteString(newToken, "account_access_token.txt");
        }

        public static string AccountRefreshToken() {
            return ReadString("account_refresh_token.txt");
        }

        public static void SetAccountRefreshToken(string newToken) {
            WriteString(newToken, "account_refresh_token.txt");
        }


        //MARK: - Helpers
        private static string ReadString(string name) {
            string path = Config.secretsPath + name;
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            string contents = reader.ReadToEnd();
            reader.Close();
            return contents;
        }

        private static void WriteString(string value, string name) {
            string path = Config.secretsPath + name;
            StreamWriter writer = new StreamWriter(path);
            writer.Write(value);
            writer.Close();
        }
    }
}
