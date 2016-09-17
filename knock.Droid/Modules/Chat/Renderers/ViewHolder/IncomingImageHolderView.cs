using Java.Lang;
using Android.App;
using Android.Widget;
using knock.Droid;

namespace Xamarin.Forms.Chat.Droid
{
    public class IncomingImageHolderView : ImageHolderView
    {
        public IncomingImageHolderView() : base ()
        {
            var resource = Resource.Layout.item_conversation_msg_image_incoming;
            var view = (Forms.Context as Activity).LayoutInflater.Inflate(resource, null);
            this.AddView(view);
            this.ImageView = view.FindViewById<ImageView>(Resource.Id.thumnail_image);
            this.DateView = view.FindViewById<TextView>(Resource.Id.msg_timestamp);
			this.AuthorView=view.FindViewById<TextView>(Resource.Id.msg_author);
			this.MessageView = view.FindViewById<TextView>(Resource.Id.msg_text);
        }
    }

	public class IncomingThumbHolderView : ImageHolderView
	{
		public IncomingThumbHolderView() : base ()
		{
			var resource = Resource.Layout.item_conversation_msg_image_incoming_micro;
			var view = (Forms.Context as Activity).LayoutInflater.Inflate(resource, null);
			this.AddView(view);
			this.ImageView = view.FindViewById<ImageView>(Resource.Id.thumnail_image);
			this.DateView = view.FindViewById<TextView>(Resource.Id.msg_timestamp);
			this.AuthorView=view.FindViewById<TextView>(Resource.Id.msg_author);
		}
	}
}
