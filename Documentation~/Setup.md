# Setting up Twitchmata

*Note: This guide assume you already have your Unity project set up and are familiar with the basics of Unity and C#*

## 1. Get Twitchmata

The first step is to download Twitchmata. You can find the latest version [here](https://github.com/pilky/twitchmata/releases). Download the zip file of the release at the top of the page.

Once downloaded you need to unzip the release. Next, create a folder in your Unity project's Assets folder and call it "External". Drag the unzipped release of Twitchmata into this External folder (ensure folder is just called "Twitchmata").

## 2. Setting up the Game Object

In Unity, add an empty GameObject to your scene. Name it something like "Twitch", "TwitchManager", or "Twitchmata" so you can find it easily. 

Next, select this GameObject in the Hierarchy, then click **Add Component** in the Inspector. Add the `TwitchManager` script. You should now see some additional fields in the inspector, we need to fill these out.

### Configure Account Names
First, enter the name of the channel you're streaming from in the **Channel Name** field

Next, enter the name of the chat bot account you want to use in the **Bot Name** field. This can be the same as your streaming account, but it is **highly** recommended you create a separate account for this bot.

Optionally, you can set where you want Twitchmata to store any local files on disk by putting a file path in the **Persistence Path** field. If you leave this blank the files will be stored to the standard persistence path Unity sets up (this is the recommended setup).

Finally we have the Client ID, but that requires a bit more setup first

## 3. Setting up your Twitch App

In order to communicate with the Twitch API you need to set up a Twitch App. You can find instructions on how to set up an app [here](https://dev.twitch.tv/docs/authentication/register-app). Follow steps 1 through 9 on that page (step 10 is not relevant). For Step 4, set the OAuth Redirect URL to "http://localhost:3000"

Once you have set up your app, copy the Client ID for your app and paste it into the Client ID field of your TwitchManager GameObject in Unity. You are now nearly setup. The last thing to do is authenticate your accounts.

## 4. Authenticate

Twitchmata provides a convenient UI for authenticating with your accounts. To access this, select your TwitchManager GameObject and then select **Window > Twitchmata > Authenticate** from the menu bar.

First you need to authenticate your channel. Click on the Authenticate Channel with Twitch button, which will open a link in your web browser. Make sure you are logged into the account you stream from and then click Authorize at the bottom of the web page. You will be re-directed to a page that doesn't exist, but that is ok.

Copy the URL in your web browser's address bar (make sure you copy the whole thing) and paste it into the text field in the "Authenticate Channel" section. Then click **Save Channel Auth** to save your details.

Now repeat the same for the Authenticate Chat Bot section, making sure that you log into your chat bot's account.

Once you have saved your authentication details you are good to go!

## 5. Test

To perform a quick test, select the your TwitchManager GameObject and set the **Log Level** in the inspector to "Info". This will cause Twitchmata to show all its logs, which can be useful for debugging.

Now build and run your overlay and check the Console in Unity. You should see some of the following messages:

```
[TWITCHMATA] Resetting connection
[TWITCHMATA] Connecting client: <your bot name>
[TWITCHMATA] Fetching user info
[TWITCHMATA] PubSub Connected
```

If you have no errors from Twitchmata then you are good to go. You can set the Log Level back to "Error" for now. Next you need to set up some FeatureManagers, but you can find information on that in the ReadMe.