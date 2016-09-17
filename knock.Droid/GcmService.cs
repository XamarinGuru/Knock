using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using Gcm.Client;
using System.IO;


[assembly: Permission(Name = "com.visual1993.knock.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
[assembly: UsesPermission(Name = "com.visual1993.knock.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]

//VERY VERY VERY IMPORTANT NOTE!!!!
// Your package name MUST NOT start with an uppercase letter.
// Android does not allow permissions to start with an upper case letter
// If it does you will get a very cryptic error in logcat and it will not be obvious why you are crying!
// So please, for the love of all that is kind on this earth, use a LOWERCASE first letter in your Package Name!!!!
namespace knock.Droid
{
	//You must subclass this!
	[BroadcastReceiver(Permission=GCMConstants.PERMISSION_GCM_INTENTS)]
	[IntentFilter(new[] { Intent.ActionBootCompleted })] // Allow GCM on boot and when app is closed  
	[IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })] //registers with  this but hanlde message with Sticky
	[IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
	public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
	{
		//IMPORTANT: Change this to your own Sender ID!
		//The SENDER_ID is your Google API Console App Project ID.
		//  Be sure to get the right Project ID from your Google APIs Console.  It's not the named project ID that appears in the Overview,
		//  but instead the numeric project id in the url: eg: https://code.google.com/apis/console/?pli=1#project:785671162406:overview
		//  where 785671162406 is the project id, which is the SENDER_ID to use!
		public static string[] SENDER_IDS = new string[] {"944308070832"};

		public const string TAG = "PushSharp-GCM";
	}

	[Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
	//[Service] //Must use the service tag
	public class PushHandlerService : GcmServiceBase
	{
		protected override void OnDeletedMessages (Context context, int total)
		{
			Log.Error(TAG, "GCM-Xamarin Deleted messages: " + total.ToString());
			base.OnDeletedMessages (context, total);
		}

		public PushHandlerService() : base(GcmBroadcastReceiver.SENDER_IDS) { }

		const string TAG = "GCM-SAMPLE";

		protected override async void OnRegistered (Context context, string registrationId)
		{
			Log.Verbose(TAG, "GCM-Xamarin new GCMtoken Registered: " + registrationId);
			try{
				await App.registraNotifURI ("android", registrationId);
			}
			catch(Exception ex) {
				Log.Error ("GCM-Xamarin","Exception in registrazione ARN");
				if (App.utente == null)
				{
					return;
				}
				await Visual1993.Dialogs.Alert2("Error", "Error setting up notifications: " + ex.Message, "Retry");
			}
			//createNotification("GCM Registered...", "The device has been Registered, Tap to View!");
		}

		protected override void OnUnRegistered (Context context, string registrationId)
		{
			Log.Verbose(TAG, "GCM-Xamarin Unregistered: " + registrationId);
			//Remove from the web service
			//	var wc = new WebClient();
			//	var result = wc.UploadString("http://your.server.com/api/unregister/", "POST",
			//		"{ 'registrationId' : '" + lastRegistrationId + "' }");

			//createNotification("GCM Unregistered...", "The device has been unregistered, Tap to View!");
		}

		protected override void OnMessage (Context context, Intent intent)
		{
			//createNotification ("new message", "new message", 0, context);
			Log.Info(TAG, "GCM-Xamarin Message Received!");
			//WRITE MESSAGE ON LOG WITH DATE TIME NOW
			/*StreamWriter file2 = new StreamWriter(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath+"knock-notifications.txt", true);
			file2.WriteLine(DateTime.UtcNow.ToString()+"");
			file2.Close();
			*/

			
			/*if (App.utente == null) {
				return;		
			} //if logged out
			*/
			var msg = new StringBuilder();
			var title = "";
			var descr = "";
			int chatID = 0;
			int smsID = 0;
			string extraData = "{}";

			if (intent != null && intent.Extras != null)
			{
				foreach (var key in intent.Extras.KeySet()) {
					msg.AppendLine (key + "=" + intent.Extras.Get (key).ToString ());
					if (key == "message") {
						descr = intent.Extras.Get (key).ToString ();
					}
					if (key == "title") {
						title = intent.Extras.Get (key).ToString ();
					}
					if (key == "chatID") {
						chatID = Convert.ToInt32(intent.Extras.Get (key).ToString ());
					}
					if (key == "smsID") {
						smsID = Convert.ToInt32(intent.Extras.Get (key).ToString ());
					}
					if (key == "extraData") {
						extraData = intent.Extras.Get (key).ToString ();
					}
				}
			}

			//Store the message
			//var prefs = GetSharedPreferences(context.PackageName, FileCreationMode.Private);
			//var edit = prefs.Edit();
			//edit.PutString("last_msg", msg.ToString());
			//edit.Commit();
		var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		var pathUtente= Path.Combine(docsPath, "utente.json");
			if (File.Exists (pathUtente)) {

				ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences (context);
				if (prefs.GetInt ("currentChatID", 0) != chatID) { //se sto nella chat con quell'ID evita di mostrarmi la notifica
				createNotification (title, descr, chatID, context, smsID, extraData);
				}

			try{
				Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger> (App.messenger, "newMessage"); //NEW 1.0.9
			}
			catch(Exception ex) {
				
				}
			} else {
				Log.Warn ("notifiche-Xamarin","notification called when utente.json does not exist");
			}
			/*
			if (false) { //App.isForeground
				//QUESTO PERMETTE ALL'APP DI ANDARE IMMEDIATAMENTE SULLA CHAT SENZA NOTIFICARE
			//	Xamarin.Forms.MessagingCenter.Send<Visual1993.Messenger, string[]> (App.messenger, "notificaChat", new [] {"chatID",chatID.ToString() });
			} else {//qui è giusto e manda 77
				if (ChatNewPage.CurrentChatId != chatID) {
					createNotification (title, descr, chatID, context);
				} else {
					Xamarin.Insights.Report (new Exception ("notificationNotDisplayedDueToCurrentChatId"));
				}
			}
			*/
		}
		public bool AppInForeground()
		{
			Context context = Application.Context;
			ActivityManager activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
			System.Collections.Generic.IList<Android.App.ActivityManager.RunningAppProcessInfo> appProcesses = activityManager.RunningAppProcesses;
			if (appProcesses == null)
			{
				return false;
			}
			string packageName = context.PackageName;
			foreach (Android.App.ActivityManager.RunningAppProcessInfo appProcess in appProcesses)
			{
				if (appProcess.Importance == Importance.Background && appProcess.ProcessName == packageName)
				{
					return true;
				}
			}
			return false;
		}

		protected override bool OnRecoverableError (Context context, string errorId)
		{
			Log.Warn(TAG, "GCM-Xamarin Recoverable Error: " + errorId);

			return base.OnRecoverableError (context, errorId);
		}

		protected override void OnError (Context context, string errorId)
		{
			Log.Error(TAG, "GCM-Xamarin Error: " + errorId);
		}

	public void createNotification(string title, string desc, int chatID, Context context, int smsID, string extra="{}")
		{
			
			var notificationManager = 
				GetSystemService(Context.NotificationService) as NotificationManager;

			//var intent0 = new Intent(this, typeof(MainActivity));
			//intent0.AddFlags (ActivityFlags.SingleTop | ActivityFlags.ClearTop);
			//var intent0 = new Intent(this, typeof(NotificationActivity));
			var intent0 = new Intent(this, typeof(MainActivity));
		intent0.PutExtra("smsID",smsID);	
		intent0.PutExtra("chatID",chatID);
		intent0.PutExtra("extraData",extra);

			intent0.AddFlags (ActivityFlags.SingleTop);
			//intent0.AddFlags (ActivityFlags.ClearTask|ActivityFlags.NewTask); //arguments arrivano ma cancella forms e apre una nuova schermata
			//intent0.AddFlags (ActivityFlags.ClearTop|ActivityFlags.SingleTop|ActivityFlags.NewTask); //arguments a
			//intent0.AddFlags (ActivityFlags.BroughtToFront);
			//intent0.AddFlags (ActivityFlags.BroughtToFront|ActivityFlags.top );
			//ActivityFlags.SingleTop | ActivityFlags.ClearTop| 
			// Create a new intent to show the notification in the UI. 
		PendingIntent contentIntent = PendingIntent.GetActivity (context, 0, intent0, PendingIntentFlags.CancelCurrent);
		//PendingIntent contentIntent = PendingIntent.GetActivity (context, 0, intent0, PendingIntentFlags.OneShot);
			

			// Create the notification using the builder.
			var builder = new Notification.Builder(context);
			builder.SetAutoCancel(true);
			builder.SetContentTitle(title);
			builder.SetContentText(desc);


			builder.SetDefaults (NotificationDefaults.Sound | NotificationDefaults.Vibrate);

			builder.SetSmallIcon(Resource.Drawable.Icon);
			builder.SetContentIntent(contentIntent);
			builder.SetAutoCancel (true);
			var notification = builder.Build();

			// Display the notification in the Notifications Area.
			notificationManager.Notify(1, notification);
		}

	}
}

