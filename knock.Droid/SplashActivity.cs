
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Gcm.Client;
using knock.Droid;

using Xamarin;

namespace knock.Droid
{
	[Activity( //Indicates the theme to use for this activity
		Label = "Knock",
		Icon = "@drawable/icon",
		MainLauncher = true, //Set it as boot activity
		NoHistory = false)] //Doesn't place it in back stack
	public class SplashActivity : Activity
	{
		private void doDestroy ()
		{
			Log.Error("Knock", "LOW MEMORY from Splash activity");
			var notificationManager = 
				GetSystemService(Context.NotificationService) as NotificationManager;
			var intent0 = new Intent(this, typeof(MainActivity));
			intent0.AddFlags (ActivityFlags.SingleTop);
			PendingIntent contentIntent = PendingIntent.GetActivity (this, 0, intent0, PendingIntentFlags.OneShot);
			var builder = new Notification.Builder(this);
			builder.SetAutoCancel(true);
			builder.SetContentTitle("Knock");
			builder.SetContentText("Low memory. from splash activity");


			builder.SetDefaults (NotificationDefaults.Sound | NotificationDefaults.Vibrate);

			builder.SetSmallIcon(Resource.Drawable.Icon);
			builder.SetContentIntent(contentIntent);
			builder.SetAutoCancel (true);
			var notification = builder.Build();
			notificationManager.Notify(1, notification);
			Finish ();	
		}
		protected override void OnDestroy ()
		{
			//doDestroy ();
			base.OnDestroy ();
		}
		public override void OnLowMemory ()
		{	
			doDestroy ();
			base.OnLowMemory ();
		}
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.SplashScreen);
			//System.Threading.Thread.Sleep(3000); //Let's wait awhile...
			try{
				GcmClient.CheckDevice(this);
				GcmClient.CheckManifest(this);
				if (GcmClient.IsRegistered (this) == false) {
					GcmClient.Register (this, GcmBroadcastReceiver.SENDER_IDS);
				}
			}
			catch(Exception ex) {
				//no GCM
				Toast.MakeText (this, "Unable to register for notifications", ToastLength.Long).Show();
			}

			//StartService(new Android.Content.Intent(this, typeof(knock.Droid.GcmServiceSticky)));

			Insights.Initialize("0c82d437187c7c4247ab65f05b28a3b7da3dbff8", this.ApplicationContext);
			Insights.HasPendingCrashReport += (sender, isStartupCrash) =>
			{
				if (isStartupCrash) {
					Insights.PurgePendingCrashReports().Wait();
				}
			};
			this.StartActivity(typeof(MainActivity));
			System.Threading.Thread.Sleep(200);
			Finish ();
		}
	}
}

