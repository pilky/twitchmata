using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Web;
using Twitchmata;

namespace Twitchmata {
    public class TwitchmataAuthentication : EditorWindow {

        [MenuItem("Window/Twitchmata/Authenticate"), MenuItem("CONTEXT/TwitchManager/Twitchmata/Authenticate")]
        public static void ShowWindow() {
            var window = GetWindow<TwitchmataAuthentication>();
            window.titleContent = new GUIContent("Authenticate Twitchmata");
        }

        private TwitchManager TwitchManager;

        public VisualTreeAsset WindowXML;
        public void CreateGUI() {
            this.TwitchManager = Selection.activeGameObject.GetComponent<TwitchManager>();
            if (this.TwitchManager == null) {
                var label = new Label("You must select a GameObject containing a TwitchManager before opening this window");
                label.style.unityFontStyleAndWeight = FontStyle.Bold;
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                label.style.whiteSpace = WhiteSpace.Normal;
                label.style.marginTop = 20;
                this.rootVisualElement.Add(label);
                return;
            }


            var ui = this.WindowXML.CloneTree();
            this.rootVisualElement.Add(ui);

            var authChannelButton = ui.Query<Button>("authenticate_channel").First();
            authChannelButton.RegisterCallback<MouseUpEvent>(OpenAuthenticateChannel);

            var authBotButton = ui.Query<Button>("authenticate_chat_bot").First();
            authBotButton.RegisterCallback<MouseUpEvent>(OpenAuthenticateBot);

            var saveChannelAuthButton = ui.Query<Button>("save_channel_auth").First();
            saveChannelAuthButton.RegisterCallback<MouseUpEvent>(SaveChannelAuth);

            var saveBotAuthButton = ui.Query<Button>("save_bot_auth").First();
            saveBotAuthButton.RegisterCallback<MouseUpEvent>(SaveBotAuth);

            this.ChannelAuthField = ui.Query<TextField>("channel_auth_field").First();
            this.BotAuthField = ui.Query<TextField>("bot_auth_field").First();
            this.ErrorLabel = ui.Query<Label>("error_label").First();
        }

        private TextField ChannelAuthField;
        private TextField BotAuthField;
        private Label ErrorLabel;

        private string ChannelAuthState;
        private void OpenAuthenticateChannel(MouseUpEvent evt) {
            string clientID = this.TwitchManager.ConnectionConfig.ClientID;
            this.ChannelAuthState = Guid.NewGuid().ToString();
            var loginURL = "https://id.twitch.tv/oauth2/authorize?response_type=token&client_id="+ clientID + "&redirect_uri=http://localhost:3000&scope=bits%3Aread+channel%3Aread%3Aredemptions+channel%3Amanage%3Aredemptions+channel%3Amoderate+chat%3Aread+chat%3Aedit+user%3Amanage%3Awhispers+moderation%3Aread+channel%3Amanage%3Araids+moderator%3Aread%3Achatters+channel%3Aread%3Avips+channel%3Aread%3Asubscriptions+moderator%3Amanage%3Aannouncements+moderator%3Amanage%3Ashoutouts&force_verify=true&state=" + this.ChannelAuthState;
            Application.OpenURL(loginURL);
        }

        private string BotAuthState;
        private void OpenAuthenticateBot(MouseUpEvent evt) {
            string clientID = this.TwitchManager.ConnectionConfig.ClientID;
            this.BotAuthState = Guid.NewGuid().ToString();
            var loginURL = "https://id.twitch.tv/oauth2/authorize?response_type=token&client_id=" + clientID + "&redirect_uri=http://localhost:3000&scope=bits%3Aread+channel%3Aread%3Aredemptions+channel%3Amanage%3Aredemptions+channel%3Amoderate+chat%3Aread+chat%3Aedit+user%3Amanage%3Awhispers+moderation%3Aread+channel%3Amanage%3Araids+moderator%3Aread%3Achatters+channel%3Aread%3Avips+channel%3Aread%3Asubscriptions+moderator%3Amanage%3Aannouncements+moderator%3Amanage%3Ashoutouts&force_verify=true&state=" + this.BotAuthState;
            Application.OpenURL(loginURL);
        }

        private void SaveChannelAuth(MouseUpEvent evt) {
            try { 
                var channelURI = new Uri(this.ChannelAuthField.value);
                var channelToken = this.ExtractTokenFromFragment(channelURI.Fragment, this.ChannelAuthState);

                var persistencePath = this.TwitchManager.PersistencePath;
                if (persistencePath == null || persistencePath.Length == 0) {
                    persistencePath = Application.persistentDataPath;
                }
                var persistence = new Persistence(persistencePath);
                persistence.AccountAccessToken = channelToken;
            } catch {
                this.DisplayError("Could not complete authentication. Please double-check the pasted URL");
            }
        }

        private void SaveBotAuth(MouseUpEvent evt) {
            try {
                var botURI = new Uri(this.BotAuthField.value);
                var botToken = this.ExtractTokenFromFragment(botURI.Fragment, this.BotAuthState);

                var persistencePath = this.TwitchManager.PersistencePath;
                if (persistencePath == null || persistencePath.Length == 0)
                {
                    persistencePath = Application.persistentDataPath;
                }
                var persistence = new Persistence(persistencePath);
                persistence.BotAccessToken = botToken;
            } catch {
                this.DisplayError("Could not complete authentication. Please double-check the pasted URL");
            }
        }

        private string ExtractTokenFromFragment(string fragment, string expectedState) {
            string token = null;
            string state = null;
            var components = fragment.Substring(1).Split('&');
            foreach (var component in components) {
                var splitComponent = component.Split('=');
                if (splitComponent.Length != 2) {
                    continue;
                }
                if (splitComponent[0] == "access_token") {
                    token = splitComponent[1];
                }
                if (splitComponent[0] == "state") {
                    state = splitComponent[1];
                }
            }

            if (state != null && state != expectedState) {
                return null;
            }
            return token;
        }

        private void DisplayError(string error) {
            this.ErrorLabel.text = error;
        }
    }
}
