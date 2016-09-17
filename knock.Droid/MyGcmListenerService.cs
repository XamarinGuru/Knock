using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using knock.Droid;

[assembly: Permission(Name = "com.visual1993.knock.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
[assembly: UsesPermission(Name = "com.visual1993.knock.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

namespace knock.Droid
{
	[Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
	[IntentFilter(new[] { Intent.ActionBootCompleted })] // Allow GCM on boot and when app is closed  
	public class MyGcmListenerService : GcmListenerService
	{
		
		public override void OnCreate ()
		{
			Log.Debug ("MyGcmListenerService", "Created mygcm");
			base.OnCreate ();
		}
		public override void OnMessageReceived (string from, Bundle data)
		{
			var message = data.GetString ("message");
			Log.Debug ("MyGcmListenerService", "From:    " + from);
			Log.Debug ("MyGcmListenerService", "Message: " + message);
			//SendNotification (message);
			createNotification (from, message, 0, this);
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

		void SendNotification (string message)
		{
			var intent = new Intent (this, typeof(MainActivity));
			intent.AddFlags (ActivityFlags.ClearTop);
			var pendingIntent = PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.OneShot);

			var notificationBuilder = new Notification.Builder(this)
				.SetContentTitle ("GCM Message")
				.SetContentText (message)
				.SetAutoCancel (true)
				.SetContentIntent (pendingIntent);

			var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
			notificationManager.Notify (0, notificationBuilder.Build());
		}
	}
}