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

namespace Gcm.Client
{
	[Android.Runtime.Preserve(AllMembers=true)]
	public abstract class GcmServiceBase : Service
	{
		//inizio new
		private volatile Looper mServiceLooper;
		private volatile ServiceHandler mServiceHandler;
		private String mName;
		private bool mRedelivery;

		private sealed class ServiceHandler : Handler {
			private GcmServiceBase sis;
			public ServiceHandler(Looper looper, GcmServiceBase sis): base(looper) {
				this.sis = sis;
			}

			public override void HandleMessage(Message msg) {
				sis.OnHandleIntent((Intent)msg.Obj);
			}
		}

		public void setIntentRedelivery(bool enabled) {
			mRedelivery = enabled;
		}

		public override void OnCreate()
		{
			// TODO: It would be nice to have an option to hold a partial wakelock
			// during processing, and to have a static startService(Context, Intent)
			// method that would launch the service & hand off a wakelock.

			base.OnCreate();
			HandlerThread thread = new HandlerThread("IntentService[" + mName + "]");
			thread.Start();

			mServiceLooper = thread.Looper;
			mServiceHandler = new ServiceHandler(mServiceLooper,this);

		}

		public override void OnStart(Intent intent, int startId) {
			Message msg = mServiceHandler.ObtainMessage();
			msg.Arg1 = startId;
			msg.Obj = intent;
			mServiceHandler.SendMessage(msg);
		}
		public override StartCommandResult OnStartCommand(Intent intent,  StartCommandFlags flags, int startId)
		{
			Message msg = mServiceHandler.ObtainMessage();
			msg.Arg1 = startId;
			msg.Obj = intent;
			mServiceHandler.SendMessage(msg);
			//OnStart(intent, startId);
			return StartCommandResult.Sticky;
		}

		public override void OnDestroy() {
			mServiceLooper.Quit();
		}

		/**
     * Unless you provide binding for your service, you don't need to implement this
     * method, because the default implementation returns null. 
     * @see android.app.Service#onBind
     */
		public override IBinder OnBind(Intent intent) {
			return null;
		}

		//fine new

		const string WAKELOCK_KEY = "GCM_LIB";
		static PowerManager.WakeLock sWakeLock;

		static object LOCK = new object();
		static int serviceId = 1;

        /// <summary>
        /// The GCM Sender Ids to use. Set by the constructor taking parameters but not by the one that doesn't. Be very careful changing this value, preferably only set it in your constructor and only once.
        /// </summary>
		protected string[] SenderIds = new string[] {};

		//int sCounter = 1;
		Random sRandom = new Random();

		const int MAX_BACKOFF_MS = 3600000; //1 hour

		string TOKEN = "";
		const string EXTRA_TOKEN = "token";

		protected GcmServiceBase() : base() {}

		/*public GcmServiceBase(params string[] senderIds)
			: base("GCMIntentService-" + (serviceId++).ToString())
		{
			SenderIds = senderIds;
		}*/

		public GcmServiceBase(params string[] senderIds):base()
		{
			SenderIds = senderIds;
		}


		protected abstract void OnMessage(Context context, Intent intent);

		protected virtual void OnDeletedMessages(Context context, int total)
		{
		}

		protected virtual bool OnRecoverableError(Context context, string errorId)
		{
			return true;
		}

		protected abstract void OnError(Context context, string errorId);

		protected abstract void OnRegistered(Context context, string registrationId);

		protected abstract void OnUnRegistered(Context context, string registrationId);


		public /*protected override*/ void OnHandleIntent(Intent intent)
		{
			try
			{
				var context = this.ApplicationContext;
				var action = intent.Action;

				if (action.Equals(GCMConstants.INTENT_FROM_GCM_REGISTRATION_CALLBACK))
				{
					handleRegistration(context, intent);
				}
				else if (action.Equals(GCMConstants.INTENT_FROM_GCM_MESSAGE))
				{
					// checks for special messages
					var messageType = intent.GetStringExtra(GCMConstants.EXTRA_SPECIAL_MESSAGE);
					if (messageType != null)
					{
						if (messageType.Equals(GCMConstants.VALUE_DELETED_MESSAGES))
						{
							var sTotal = intent.GetStringExtra(GCMConstants.EXTRA_TOTAL_DELETED);
							if (!string.IsNullOrEmpty(sTotal))
							{
								int nTotal = 0;
								if (int.TryParse(sTotal, out nTotal))
								{
									Logger.Debug("Received deleted messages notification: " + nTotal);
									OnDeletedMessages(context, nTotal);
								}
								else
									Logger.Debug("GCM returned invalid number of deleted messages: " + sTotal);
							}
						}
						else
						{
							// application is not using the latest GCM library
							Logger.Debug("Received unknown special message: " + messageType);
						}
					}
					else
					{
						OnMessage(context, intent);
					}
				}
				else if (action.Equals(GCMConstants.INTENT_FROM_GCM_LIBRARY_RETRY))
				{
					var token = intent.GetStringExtra(EXTRA_TOKEN);

					if (!string.IsNullOrEmpty(token) && !TOKEN.Equals(token))
					{
						// make sure intent was generated by this class, not by a
						// malicious app.
						Logger.Debug("Received invalid token: " + token);
						return;
					}

					// retry last call
					if (GcmClient.IsRegistered(context))
						GcmClient.internalUnRegister(context);
					else
						GcmClient.internalRegister(context, SenderIds);
				}
			}
			catch (Exception ex) {
				Log.Error ("notifiche","ECCEZIONE IN HandleIntent:"+ex.Message);
			}
			finally
			{
				// Release the power lock, so phone can get back to sleep.
				// The lock is reference-counted by default, so multiple
				// messages are ok.

				// If OnMessage() needs to spawn a thread or do something else,
				// it should use its own lock.
				lock (LOCK)
				{
					//Sanity check for null as this is a public method
					if (sWakeLock != null)
					{
						Logger.Debug("Releasing Wakelock");
						try{
						sWakeLock.Release();
						}
						catch (Exception ex) {
							Log.Error("Notifiche","Wakelock exception in releasing. Bug in GCM. "+ex.Message);
						}
					}
					else
					{
						//Should never happen during normal workflow
						Logger.Debug("Wakelock reference is null");
					}
				}
			}
		}



		internal static void RunIntentInService(Context context, Intent intent, Type classType) 
		{
			lock (LOCK) 
			{
				if (sWakeLock == null) 
				{
					// This is called from BroadcastReceiver, there is no init.
					var pm = PowerManager.FromContext(context);
					sWakeLock = pm.NewWakeLock(WakeLockFlags.Partial, WAKELOCK_KEY);
				}
			}

			Logger.Debug("Acquiring wakelock");
			sWakeLock.Acquire();
			//intent.SetClassName(context, className);
			intent.SetClass(context, classType);

			context.StartService(intent);
		}

		private void handleRegistration(Context context, Intent intent)
		{
			var registrationId = intent.GetStringExtra(GCMConstants.EXTRA_REGISTRATION_ID);
			var error = intent.GetStringExtra(GCMConstants.EXTRA_ERROR);
			var unregistered = intent.GetStringExtra(GCMConstants.EXTRA_UNREGISTERED);

			Logger.Debug("handleRegistration: registrationId = " + registrationId + ", error = " + error + ", unregistered = " + unregistered);

			// registration succeeded
			if (registrationId != null)
			{
				GcmClient.ResetBackoff(context);
				GcmClient.SetRegistrationId(context, registrationId);
				OnRegistered(context, registrationId);
				return;
			}

			// unregistration succeeded
			if (unregistered != null)
			{
				// Remember we are unregistered
				GcmClient.ResetBackoff(context);
				var oldRegistrationId = GcmClient.ClearRegistrationId(context);
				OnUnRegistered(context, oldRegistrationId);
				return;
			}

			// last operation (registration or unregistration) returned an error;
			Logger.Debug("Registration error: " + error);
			// Registration failed
			if (GCMConstants.ERROR_SERVICE_NOT_AVAILABLE.Equals(error))
			{
				var retry = OnRecoverableError(context, error);

				if (retry)
				{
					int backoffTimeMs = GcmClient.GetBackoff(context);
					int nextAttempt = backoffTimeMs / 2 + sRandom.Next(backoffTimeMs);

					Logger.Debug("Scheduling registration retry, backoff = " + nextAttempt + " (" + backoffTimeMs + ")");

					var retryIntent = new Intent(GCMConstants.INTENT_FROM_GCM_LIBRARY_RETRY);
					retryIntent.PutExtra(EXTRA_TOKEN, TOKEN);

					var retryPendingIntent = PendingIntent.GetBroadcast(context, 0, retryIntent, PendingIntentFlags.OneShot);

					var am = AlarmManager.FromContext(context);
					am.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + nextAttempt, retryPendingIntent);

					// Next retry should wait longer.
					if (backoffTimeMs < MAX_BACKOFF_MS)
					{
						GcmClient.SetBackoff(context, backoffTimeMs * 2);
					}
				}
				else
				{
					Logger.Debug("Not retrying failed operation");
				}
			}
			else
			{
				// Unrecoverable error, notify app
				OnError(context, error);
			}
		}

	}
}