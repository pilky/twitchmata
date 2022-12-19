using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Twitchmata {
    public class Secrets {
        public string RootPath { get; private set; }
        public Secrets(string rootPath) {
            this.RootPath = rootPath;
        }

        public string ClientID() {
            return this.ReadString("client_id.txt");
        }

        public string ClientSecret() {
            return this.ReadString("client_secret.txt");
        }

        public string BotAccessToken() {
            return this.ReadString("bot_access_token.txt");
        }

        public void SetBotAccessToken(string newToken) {
            this.WriteString(newToken, "bot_access_token.txt");
        }

        public string BotRefreshToken() {
            return this.ReadString("bot_refresh_token.txt");
        }

        public void SetBotRefreshToken(string newToken) {
            this.WriteString(newToken, "bot_refresh_token.txt");
        }

        public string AccountAccessToken() {
            return this.ReadString("account_access_token.txt");
        }

        public void SetAccountAccessToken(string newToken) {
            this.WriteString(newToken, "account_access_token.txt");
        }

        public string AccountRefreshToken() {
            return this.ReadString("account_refresh_token.txt");
        }

        public void SetAccountRefreshToken(string newToken) {
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
