using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Acr.UserDialogs;
using Xamarin;
using Xamarin.Forms.Platform.Android;

using Android.Gms.Common;
using Android.Util;
using Gcm.Client;
using Xamarin.Facebook.AppEvents;

using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using System.Collections.Generic;

/*
using Android.Gms.Gcm.Iid;
using Android.Gms.Gcm;

using Gcm.Client;

[assembly: Permission (Name="@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission ("@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission (Android.Manifest.Permission.Internet)]
[assembly: UsesPermission (Android.Manifest.Permission.GetAccounts)]
[assembly: UsesPermission (Android.Manifest.Permission.WakeLock)]
[assembly: UsesPermission ("com.google.android.c2dm.permission.RECEIVE")]
*/
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/app_id")]
[assembly: MetaData("com.facebook.sdk.ApplicationName", Value = "@string/app_name")]
namespace knock.Droid
{
	[Activity (Label = "Knock-notification", MainLauncher = false/*, LaunchMode = Android.Content.PM.LaunchMode.SingleTop*/)]
	public class NotificationActivity : Activity
	// global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		
		static void HandleExceptions(object sender,UnhandledExceptionEventArgs e)
		{
			Exception d = (Exception)e.ExceptionObject;
			Xamarin.Insights.Report(d);
			Console.WriteLine("globalExceptionHandled");
			//http://stackoverflow.com/questions/13287163/android-stop-background-service-after-activity-crash
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += HandleExceptions;

			//http://stackoverflow.com/questions/13287163/android-stop-background-service-after-activity-crash

			if (Intent.Extras != null) {
				int chatID = Intent.Extras.GetInt ("chatID", 0);
				if (chatID != 0) {
					Xamarin.Forms.MessagingCenter.Subscribe<Visual1993.Messenger> (App.messenger, "mainAvviato", (sender) => {
						Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]> (App.messenger, "notificaChat", new[] {
							"chatID",
							chatID.ToString ()
						});
						Xamarin.Forms.MessagingCenter.Unsubscribe<Visual1993.Messenger> (App.messenger, "mainAvviato");
						Finish ();
					});
				}
				int promozioneID = Intent.Extras.GetInt ("promozioneID", 0);
				if (promozioneID != 0) {
					Xamarin.Forms.MessagingCenter.Subscribe<Visual1993.Messenger> (App.messenger, "mainAvviato", (sender) => {
						Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]> (App.messenger, "notificaPromozione", new[] {
							"promozioneID",
							promozioneID.ToString ()
						});
						Xamarin.Forms.MessagingCenter.Unsubscribe<Visual1993.Messenger> (App.messenger, "mainAvviato");
						Finish ();
					});
				}

				StartActivity (typeof(MainActivity));

				return;
			}
		}
	}

	[Activity (Label = "Knock", Theme = "@style/MyCustomTheme", Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	[IntentFilter(new[] { Android.Content.Intent.ActionView },
	DataScheme = "https",
	DataHost = "api.letknock.com",
	              DataPathPrefix="/event",
	              Categories = new[] { Android.Content.Intent.CategoryDefault,Intent.CategoryBrowsable  })]
	[IntentFilter(new[] { Android.Content.Intent.ActionView },
	DataScheme = "letknock",
	DataHost = "event",
				  Categories = new[] { Android.Content.Intent.CategoryDefault, Intent.CategoryBrowsable })]
	[IntentFilter(new[] { Android.Content.Intent.ActionSend },
	              Categories = new[] { Android.Content.Intent.CategoryDefault },
	              DataMimeType="image/*"
				  )]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		ICallbackManager callbackManager;

		private void doDestroy ()
		{
			Log.Error("Knock", "LOW MEMORY ");
			var notificationManager = 
				GetSystemService(Context.NotificationService) as NotificationManager;
			var intent0 = new Intent(this, typeof(MainActivity));
			intent0.AddFlags (ActivityFlags.SingleTop);
			PendingIntent contentIntent = PendingIntent.GetActivity (this, 0, intent0, PendingIntentFlags.OneShot);
			var builder = new Notification.Builder(this);
			builder.SetAutoCancel(true);
			builder.SetContentTitle("Knock");
			builder.SetContentText("Low memory.");


			builder.SetDefaults (NotificationDefaults.Sound | NotificationDefaults.Vibrate);

			builder.SetSmallIcon(Resource.Drawable.Icon);
			builder.SetContentIntent(contentIntent);
			builder.SetAutoCancel (true);
			var notification = builder.Build();
			notificationManager.Notify(1, notification);
			Finish ();	
		}

		public bool IsPlayServicesAvailable ()
		{
			string msgText = "";
			int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable (this);
			if (resultCode != ConnectionResult.Success)
			{
				if (GoogleApiAvailability.Instance.IsUserResolvableError (resultCode))
					msgText = GoogleApiAvailability.Instance.GetErrorString (resultCode);
				else
				{
					msgText = "Sorry, this device is not supported";
					Finish ();
				}
				return false;
			}
			else
			{
				msgText = "Google Play Services is available.";
				return true;
			}
		}

		void HandleExceptions(object sender,UnhandledExceptionEventArgs e)
		{
			Exception d = (Exception)e.ExceptionObject;
			Xamarin.Insights.Report(d);
			Console.WriteLine("globalExceptionHandled");
			//http://stackoverflow.com/questions/13287163/android-stop-background-service-after-activity-crash

			var intent = new Intent(this, typeof(MainActivity));
			intent.SetFlags(ActivityFlags.ClearTop);
			StartActivity(intent);
			this.Finish ();
		}

		public override void OnActivityReenter (int resultCode, Intent data)
		{
			base.OnActivityReenter (resultCode, data);
		}

	/*public override async void OnBackPressed ()
		{
			if  (App.navigation == null || App.navigation.Navigation == null) {
				return;
			}
			if (App.navigation.Navigation.NavigationStack.Count > 1) {
				base.OnBackPressed ();
			} else {
				var answer = await Visual1993.Dialogs.ShowConfirm ("App closing", "If you close the app, you'll not receive notifications until you opne it again. Please use the Home button to put the app in background."+System.Environment.NewLine+"Are you sure you want to close the app?", "Yes", "No");
				if (answer == true) {
					base.OnBackPressed ();
				}
			}
		}
		*/

		protected override void OnStop ()
		{
			Xamarin.Forms.MessagingCenter.Unsubscribe<Messenger> (App.messenger, "notificaChat");

			base.OnStop ();
		}
		/*protected override void OnResume()
		{
			base.OnResume();
			try { AppEventsLogger.ActivateApp(this); }
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
		protected override void OnPause()
		{
			base.OnPause();
			try { AppEventsLogger.DeactivateApp(this); }
			catch (Exception ex) { }
		}*/
		protected override void OnPostResume ()
		{
			base.OnPostResume ();
			//Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger>(App.messenger, "mainAvviato");
			//DispatchNotification ();
		}
		protected override void OnRestart ()
		{
			//DispatchNotification ();
			base.OnRestart ();
		}

		private void DispatchNotification(Bundle Extras = null)
		{
			/*
			if(intent==null){
				Console.WriteLine("onNewIntent called from stopped");
				Log.Info ("activity","onNewIntent called from stopped");
				intent = Intent;
			};
			*/
			if (Extras != null) {
				int chatID = Extras.GetInt ("chatID", 0);
				if (chatID != 0) {
						Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]> (App.messenger, "notificaChat", new[] {
							"chatID",
							chatID.ToString ()
						});
					}
				string extraData = Extras.GetString ("extraData", "{}");
				if (extraData != "{}") {
					Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]> (App.messenger, "notificaGenerica", new[] {
							"extraData",
						extraData
						});
				}

				//intent.RemoveExtra("chatID"); 

				return;
			}
		}

		protected override void OnNewIntent (Intent intent)
		{
			//this is called when an instance of knock is running in background.
			Console.WriteLine("onNewIntent");
			Log.Info ("activity","onNewIntent");
			DispatchNotification (intent.Extras);
			intent.RemoveExtra("chatID"); 
			base.OnNewIntent (intent);
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			FacebookSdk.SdkInitialize(this.ApplicationContext);
			callbackManager = CallbackManagerFactory.Create();

			LoginManager.Instance.RegisterCallback(callbackManager, new FacebookCallback<Xamarin.Facebook.Login.LoginResult>()
			{
				HandleSuccess = loginResult =>
				{
					var accessToken = loginResult.AccessToken;
					Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(Constants.msecWaitUIFacebook), () =>
								{
									App.OnFacebookAuthSuccess(accessToken.Token);
									return false;
								});
				},
				HandleCancel = () =>
				{
					Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(Constants.msecWaitUIFacebook), () =>
								{
									App.OnFacebookAuthFailed();
									return false;
								});
				},
				HandleError = loginError =>
				{
					Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(Constants.msecWaitUIFacebook), () =>
								{
									App.OnFacebookAuthFailed();
									return false;
								});
				}
			});
			/*
			try{
				GcmClient.CheckDevice(this);
				GcmClient.CheckManifest(this);
				if (GcmClient.IsRegistered (this) == false) {
					GcmClient.Register (this, GcmBroadcastReceiver.SENDER_IDS);
				}
			}
			catch(Exception ex) {
				//no GCM
				Android.Widget.Toast.MakeText (this, "Unable to register for notifications", Android.Widget.ToastLength.Long).Show();
			}
			*/
			//service creation moved to Splash Screen activity

			AppDomain currentDomain = AppDomain.CurrentDomain;
			//currentDomain.UnhandledException += HandleExceptions;

			Xamarin.Insights.Initialize("0c82d437187c7c4247ab65f05b28a3b7da3dbff8", this);

			DevExpress.Mobile.Forms.Init();
			global::Xamarin.Forms.Forms.Init(this, bundle);
			ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();
			MR.Gestures.Android.Settings.LicenseKey = "";
			global::AudioService.Droid.Platform.Init();
			UserDialogs.Init(this);
			FormsMaps.Init(this, bundle);
			FFImageLoading.Forms.Droid.CachedImageRenderer.Init();
			AdvancedTimer.Forms.Plugin.Droid.AdvancedTimerImplementation.Init();
			Context context = this.ApplicationContext;
			string versione = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
			int chatID = 0;
			int smsID = 0;
			string pathUri = "";
			var Extras = Intent.Extras;

			if (Intent.Data != null)
			{
				var uriObj = Intent.Data;
				pathUri = uriObj.ToString();
				//var appLinkData = Intent.GetStringExtra("al_applink_data");
				//var alUrl = new Rivets.AppLinkUrl(Intent.Data.ToString(), appLinkData);
			}
			String action = Intent.Action;
			String type = Intent.Type;

			if (action == Intent.ActionSend && string.IsNullOrWhiteSpace(type) == false)
			{
				if (type.Contains("image/")) { handleSendImage(Intent); }
				if (type == "text/plain") { handleSendText(Intent); }
			}
		
			try{Intent.RemoveExtra("chatID"); 
			}catch(Exception){
			}

			if (Extras != null) {
				
				chatID = Extras.GetInt ("chatID", 0);
				smsID = Extras.GetInt ("smsID", 0);
				//new
				ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences (context);
				if (prefs.GetInt ("lastSMS", 0) != smsID) {
					
					ISharedPreferencesEditor editor = prefs.Edit ();
					editor.PutInt ("lastSMS", smsID);
					editor.Apply (); 

					//end new
				} else {
					Xamarin.Forms.MessagingCenter.Unsubscribe<Visual1993.Messenger> (App.messenger, "mainAvviato");
				}

			}
			Xamarin.Forms.MessagingCenter.Unsubscribe<Visual1993.Messenger>(App.messenger, "mainAvviato");
			Xamarin.Forms.MessagingCenter.Subscribe<Visual1993.Messenger>(App.messenger, "mainAvviato", (sender) =>
			{
				Log.Info("notifica", "chiamo Dispatch da MainActivity");
				if (chatID != 0)
				{
					DispatchNotification(Extras);
				}
				if (string.IsNullOrWhiteSpace(pathUri)==false)
				{
					var extraData = Newtonsoft.Json.JsonConvert.SerializeObject(new NotificationExtra { uriCallback = pathUri, tipoNotifica = NotificationExtra.TipoNotifica.UriCallback});
					Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]>(App.messenger, "notificaGenerica", new[] {
							"extraData",
						extraData
						});
				}
			});
			LoadApplication (new knock.App ("", 0, versione));
			//this.ActionBar.SetBackgroundDrawable (new Android.Graphics.Drawables.ColorDrawable (Tema.coloreSfondoScuro.ToAndroid()));

		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
		}

		void handleSendText(Intent intent)
		{
			string sharedText = intent.GetStringExtra(Intent.ExtraText);
			if (string.IsNullOrWhiteSpace(sharedText)==false)
			{
				// Update UI to reflect text being shared
			}
		}
		void handleSendImage(Intent intent)
		{
			var imageUri = (Android.Net.Uri)intent.GetParcelableExtra(Intent.ExtraStream);
			if (string.IsNullOrWhiteSpace(imageUri.ToString()) == false)
			{
				if (App.isStarted == true)
				{
					sendImageToApp(imageUri.ToString());
				}
				Xamarin.Forms.MessagingCenter.Subscribe<Visual1993.Messenger>(App.messenger, "mainAvviato", (sender) =>
				{
					sendImageToApp(imageUri.ToString());
					Xamarin.Forms.MessagingCenter.Unsubscribe<Visual1993.Messenger>(App.messenger, "mainAvviato");
				});
			}
		}
		private void sendImageToApp(string imageUri)
		{
			var extraData = Newtonsoft.Json.JsonConvert.SerializeObject(new NotificationExtra { uriCallback = imageUri, tipoNotifica = NotificationExtra.TipoNotifica.ImageShare });
			Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]>(App.messenger, "notificaGenerica", new[] {
							"extraData",
						extraData
						});
		}

		void handleSendMultipleImages(Intent intent)
		{
			/*var  imageUris = (System.Collections.ArrayList<Android.Net.Uri>)intent.GetParcelableExtra(Intent.ExtraStream);
			if (imageUris != null)
			{
				// Update UI to reflect multiple images being shared
			}*/
		}

		internal class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
		{
			public Action HandleCancel { get; set; }
			public Action<FacebookException> HandleError { get; set; }
			public Action<TResult> HandleSuccess { get; set; }

			public void OnCancel()
			{
				var c = HandleCancel;
				if (c != null)
					c();
			}

			public void OnError(FacebookException error)
			{
				var c = HandleError;
				if (c != null)
					c(error);
			}

			public void OnSuccess(Java.Lang.Object result)
			{
				var c = HandleSuccess;
				if (c != null)
					c(result.JavaCast<TResult>());
			}

		}

	}
}
