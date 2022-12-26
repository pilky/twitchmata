using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Twitchmata {
    internal class Secrets {
        internal string RootPath { get; private set; }
        internal Secrets(string rootPath) {
            this.RootPath = rootPath;
        }

        //TODO: Remove
        internal string ClientID() {
            return this.ReadString("client_id.txt");
        }

        internal string ClientSecret() {
            return this.ReadString("client_secret.txt");
        }

        internal string BotAccessToken() {
            return this.ReadString("bot_access_token.txt");
        }

        internal void SetBotAccessToken(string newToken) {
            this.WriteString(newToken, "bot_access_token.txt");
        }

        //TODO: Remove
        internal string BotRefreshToken() {
            return this.ReadString("bot_refresh_token.txt");
        }

        internal void SetBotRefreshToken(string newToken) {
            this.WriteString(newToken, "bot_refresh_token.txt");
        }

        internal string AccountAccessToken() {
            return this.ReadString("account_access_token.txt");
        }

        internal void SetAccountAccessToken(string newToken) {
            this.WriteString(newToken, "account_access_token.txt");
        }

        //TODO: Remove
        internal string AccountRefreshToken() {
            return this.ReadString("account_refresh_token.txt");
        }

        internal void SetAccountRefreshToken(string newToken) {
            this.WriteString(newToken, "account_refresh_token.txt");
        }


        //MARK: - Helpers
        private string ReadString(string name) {
            string path = this.RootPath + "/keys/" + name;
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            string contents = reader.ReadToEnd();
            reader.Close();
            return contents;
        }

        private void WriteString(string value, string name) {
            string path = this.RootPath + "/keys/" + name;
            StreamWriter writer = new StreamWriter(path);
            writer.Write(value);
            writer.Close();
        }
    }
}
