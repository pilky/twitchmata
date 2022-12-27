using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Twitchmata {
    public class Persistence {
        internal string RootPath { get; private set; }
        public Persistence(string rootPath) {
            this.RootPath = rootPath;
            Directory.CreateDirectory(rootPath + "/channels/");
            Directory.CreateDirectory(rootPath + "/auth/");
        }

        internal string ChannelIDForChannel(string channelName) {
            try { 
                return this.ReadString("channels/" + channelName + ".txt");
            } catch {
                return null;
            }
        }

        internal void SetChannelIDForChannel(string channelName, string channelID) {
            this.WriteString(channelID, "channels/" + channelName + ".txt");
        }

        public string BotAccessToken {
            get { return this.ReadString("auth/bot_access_token.txt"); }
            set { this.WriteString(value, "auth/bot_access_token.txt"); }
        }

        public string AccountAccessToken {
            get { return this.ReadString("auth/account_access_token.txt"); }
            set { this.WriteString(value, "auth/account_access_token.txt"); }
        }

        //MARK: - Helpers
        private string ReadString(string name) {
            string path = this.RootPath + "/" + name;
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            string contents = reader.ReadToEnd();
            reader.Close();
            return contents;
        }

        private void WriteString(string value, string name) {
            string path = this.RootPath + "/" + name;
            StreamWriter writer = new StreamWriter(path);
            writer.Write(value);
            writer.Close();
        }
    }
}
