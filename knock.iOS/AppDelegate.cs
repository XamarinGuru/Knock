using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using Xamarin;
using CoreSpotlight;

using Facebook.CoreKit;

namespace knock.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public static bool applicazioneLanciata = false;
		public static string uriString ="";
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{
			return base.ContinueUserActivity(application, userActivity, completionHandler);
		}
		async void processNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			if (App.utente == null) {	return;		} //if logged out
			//Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey(new NSString("aps")))
			{
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;
				NSDictionary alert = options.ObjectForKey(new NSString("aps")) as NSDictionary;

				//string alert = string.Empty;
				string title = string.Empty;
				string body = string.Empty;
				int chatID = -1;
				int promozioneID = -1;
				string extraData = "{}";

				try{
				//Extract the alert text
				//NOTE: If you're using the simple alert by just specifying "  aps:{alert:"alert msg here"}  "
				//      this will work fine.  But if you're using a complex alert with Localization keys, etc., your "alert" object from the aps dictionary
				//      will be another NSDictionary... Basically the json gets dumped right into a NSDictionary, so keep that in mind
				if (aps.ContainsKey (new NSString ("alert")))
					alert = aps.ObjectForKey (new NSString ("alert")) as NSDictionary;

				if (alert.ContainsKey(new NSString("title")))
					title = (alert[new NSString("title")] as NSString).ToString();
				if (alert.ContainsKey(new NSString("body")))
					body = (alert[new NSString("body")] as NSString).ToString();
				
				/*
				//Extract the sound string
				if (aps.ContainsKey(new NSString("chatID")))
					sound = (aps[new NSString("chatID")] as NSString).ToString();
				*/
				//Extract the chatID
				if (aps.ContainsKey(new NSString("chatID")))
				{
					string chatIDStr = (aps[new NSString("chatID")] as NSObject).ToString();
					int.TryParse(chatIDStr, out chatID);
				}
				if (aps.ContainsKey(new NSString("promozioneID")))
				{
					string promozioneIDStr = (aps[new NSString("promozioneID")] as NSObject).ToString();
					int.TryParse(promozioneIDStr, out promozioneID);
				}
				if (aps.ContainsKey(new NSString("extraData")))
				{
					 extraData = (aps[new NSString("extraData")] as NSObject).ToString();
				}

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching) {
					//Manually show an alert
					if (!string.IsNullOrEmpty (title)) {
				if (ChatNewPage.CurrentChatId != chatID) {
								var answer = await Visual1993.Dialogs.ShowConfirm (title, body+Environment.NewLine+"Would you like to open the notification?", "Yes", "No");
								if (answer == true) {
									if(chatID!=-1){
										Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]> (App.messenger, "notificaChat", new[] {
											"chatID",
											chatID.ToString ()
										});
									}
									if(extraData!="{}"){
										Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]> (App.messenger, "notificaGenerica", new[] {
											"extraData",
											extraData
										});
								}
								else{
									return;
								}
								if(chatID!=-1){
									//NEW 1.0.9
									Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger> (App.messenger, "newMessage");
								}
						//UIAlertView avAlert = new UIAlertView (title, body, null, "OK", null);
						//avAlert.Show ();
						
							}
					}
				} else {
					if(chatID!=-1){
						Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]> (App.messenger, "notificaChat", new[] {
							"chatID",
							chatID.ToString ()
						});
							Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger> (App.messenger, "newMessage");
					}
						if(extraData!="{}"){
							Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]> (App.messenger, "notificaGenerica", new[] {
								"extraData",
								extraData
							});
						}
					}
				}
					//prima il passaggio era qui
				}
				catch (Exception ex) {
				}
			}
			//from: https://roycornelissen.wordpress.com/2011/05/12/push-notifications-in-ios-with-monotouch/

		}
		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			//This method gets called whenever the app is already running and receives a push notification
			// YOU MUST HANDLE the notifications in this case.  Apple assumes if the app is running, it takes care of everything
			// this includes setting the badge, playing a sound, etc.
			processNotification(userInfo, false);
		}

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Xamarin.Forms.MessagingCenter.Subscribe<Visual1993.Messenger>(App.messenger, "clearNotificationBadge", (sender) => {
				app.ApplicationIconBadgeNumber=0;
			});
			app.ApplicationIconBadgeNumber=0;
			//Websockets.Ios.WebsocketConnection.Link();
			global::Xamarin.Forms.Forms.Init ();

            FormsMaps.Init();
			FFImageLoading.Forms.Touch.CachedImageRenderer.Init();

			//for facebook login
			Profile.EnableUpdatesOnAccessTokenChange(true);
			Settings.AppID = "244707605861845";
			Settings.DisplayName = "Knock";


			MR.Gestures.iOS.Settings.LicenseKey = "";
			 global::AudioService.iOS.Platform.Init();
			new Syncfusion.SfRating.XForms.iOS.SfRatingRenderer ();
			new Syncfusion.SfRangeSlider.XForms.iOS.SfRangeSliderRenderer ();
            DevExpress.Mobile.Forms.Init ();


			ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init();

			ObjCRuntime.Class.ThrowOnInitFailure = false;
			LoadApplication (new knock.App (uriString, 0, ""));
			applicazioneLanciata = true;
			Xamarin.Forms.MessagingCenter.Subscribe<Visual1993.Messenger>(App.messenger, "demoNotification", (sender) => {
			/*	NSString notificationString = (NSString)@"{""aps"":{""alert"":""Test alert"",""sound"":""default""}}";
				NSData notificationData = new NSData(notificationString,NSDataBase64DecodingOptions.None);
				NSError error;
				NSDictionary testNotification = (NSDictionary)NSJsonSerialization.Deserialize(notificationData,0,out error);

				processNotification(testNotification, true);
				return;
			*/
			});
			processNotification(options, true);



            AppearanceCustomization.Perform();

			return base.FinishedLaunching (app, options);
		}

		#region Facebook

		#endregion

		public override void RegisteredForRemoteNotifications (
			UIApplication application, NSData deviceToken)
		{
			// Get current device token
			var DeviceToken = deviceToken.Description;
			if (!string.IsNullOrWhiteSpace(DeviceToken)) {
				DeviceToken = DeviceToken.Trim('<').Trim('>');
			}

			// Get previous device token
			var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

			// Has the token changed?
			if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
			{
				//TODO: Put your own logic here to notify your server that the device token has changed/been created!
			}

			// Save new device token 
			NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");

			//ora mandalo al server
			App.registraNotifURI ("ios", DeviceToken);
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application , NSError error)
		{
			new UIAlertView("Error registering push notifications", error.LocalizedDescription, null, "OK", null).Show();
		}

		public override bool OpenUrl (UIApplication application, NSUrl url,
			string sourceApplication, NSObject annotation)
		{
			Console.WriteLine (url);
			/* now store the url somewhere in the app’s context. The url is in the url NSUrl object. The data is in url.Host if the link as a scheme as superduperapp://something_interesting */
			if (applicazioneLanciata == false) {
				uriString = url.ParameterString; //todo: da verificare
			
			}
			// We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
			//return base.OpenUrl(application,url,sourceApplication,annotation); 
			//for facebook login
			return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
		}

	}

}
