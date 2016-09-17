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
	[IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
	//[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
	//[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]


	[Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
	//[Service] //Must use the service tag
	public class GcmServiceSticky : StickyIntentService
	{

		public GcmServiceSticky(): base(TAG) { }

		const string TAG = "GCM-STICKY";
		public override void OnCreate ()
		{
			Log.Info(TAG, "GCM-PERSONAL Created!");
			base.OnCreate ();
		}
		public override void OnStart (Intent intent, int startId)
		{
			Log.Info(TAG, "GCM-PERSONAL Started!");
			base.OnStart (intent, startId);
		}
		protected override void OnHandleIntent (Intent intent)
		{
			//createNotification ("new message", "new message", 0, context);
			Log.Info(TAG, "GCM-PERSONAL Message Received!"+intent.Action);
		/*
		var msg = new StringBuilder();
		var title = "";
		var descr = "";
		int chatID = 0;

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
			}
		}

		//Store the message
		//var prefs = GetSharedPreferences(context.PackageName, FileCreationMode.Private);
		//var edit = prefs.Edit();
		//edit.PutString("last_msg", msg.ToString());
		//edit.Commit();
		createNotification (title, descr, chatID, context);
*/
	}
	

	public void createNotification(string title, string desc, int chatID, Context context)
	{

		var notificationManager = 
			GetSystemService(Context.NotificationService) as NotificationManager;

		//var intent0 = new Intent(this, typeof(MainActivity));
		//intent0.AddFlags (ActivityFlags.SingleTop | ActivityFlags.ClearTop);
		//var intent0 = new Intent(this, typeof(NotificationActivity));
		var intent0 = new Intent(this, typeof(MainActivity));
		intent0.PutExtra("chatID",chatID);
		intent0.AddFlags (ActivityFlags.SingleTop);
		//intent0.AddFlags (ActivityFlags.ClearTask|ActivityFlags.NewTask); //arguments arrivano ma cancella forms e apre una nuova schermata
		//intent0.AddFlags (ActivityFlags.ClearTop|ActivityFlags.SingleTop|ActivityFlags.NewTask); //arguments a
		//intent0.AddFlags (ActivityFlags.BroughtToFront);
		//intent0.AddFlags (ActivityFlags.BroughtToFront|ActivityFlags.top );
		//ActivityFlags.SingleTop | ActivityFlags.ClearTop| 
		// Create a new intent to show the notification in the UI. 
		//PendingIntent contentIntent = PendingIntent.GetActivity (context, 0, intent0, PendingIntentFlags.CancelCurrent);
		PendingIntent contentIntent = PendingIntent.GetActivity (context, 0, intent0, PendingIntentFlags.OneShot);


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

