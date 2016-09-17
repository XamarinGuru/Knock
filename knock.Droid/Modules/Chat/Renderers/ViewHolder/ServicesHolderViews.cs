using Java.Lang;
using Android.App;
using Android.Widget;
using knock.Droid;

namespace Xamarin.Forms.Chat.Droid
{
    public class IncomingServiceHolderView : ImageHolderView
    {
        public IncomingServiceHolderView() : base ()
        {
			var resource = Resource.Layout.item_conversation_msg_service_incoming;
            var view = (Forms.Context as Activity).LayoutInflater.Inflate(resource, null);
            this.AddView(view);
            this.ImageView = view.FindViewById<ImageView>(Resource.Id.thumnail_image);
            this.DateView = view.FindViewById<TextView>(Resource.Id.msg_timestamp);
			this.AuthorView=view.FindViewById<TextView>(Resource.Id.msg_author);
			this.MessageView = view.FindViewById<TextView>(Resource.Id.msg_text);
        }
    }

	public class OutgoingServiceHolderView : ImageHolderView
	{
		public OutgoingServiceHolderView() : base ()
		{
			var resource = Resource.Layout.item_conversation_msg_service_outgoing;
			var view = (Forms.Context as Activity).LayoutInflater.Inflate(resource, null);
			this.AddView(view);
			this.ImageView = view.FindViewById<ImageView>(Resource.Id.thumnail_image);
			this.DateView = view.FindViewById<TextView>(Resource.Id.msg_timestamp);
			this.AuthorView=view.FindViewById<TextView>(Resource.Id.msg_author);
			this.MessageView = view.FindViewById<TextView>(Resource.Id.msg_text);
		}
	}
}
