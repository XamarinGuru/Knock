using Java.Lang;
using Android.App;
using Android.Widget;
using knock.Droid;

namespace Xamarin.Forms.Chat.Droid
{
    public class OutgoingImageHolderView : ImageHolderView
    {
        public OutgoingImageHolderView()
        {
            var resource = Resource.Layout.item_conversation_msg_image_outgoing;
            var view = (Forms.Context as Activity).LayoutInflater.Inflate(resource, null);
            this.AddView(view);
            this.ImageView = view.FindViewById<ImageView>(Resource.Id.thumnail_image);
            this.DateView = view.FindViewById<TextView>(Resource.Id.msg_timestamp);
			this.AuthorView=view.FindViewById<TextView>(Resource.Id.msg_author);
			this.LoadingBar = view.FindViewById<global::Android.Widget.ProgressBar>(Resource.Id.progressBar1);
			if (this.LoadingBar != null) { this.LoadingBar.Visibility = global::Android.Views.ViewStates.Invisible;}
			this.MessageView = view.FindViewById<TextView>(Resource.Id.msg_text);
        }
    }

	public class OutgoingThumbHolderView : ImageHolderView
	{
		public OutgoingThumbHolderView()
		{
			var resource = Resource.Layout.item_conversation_msg_image_outgoing_micro;
			var view = (Forms.Context as Activity).LayoutInflater.Inflate(resource, null);
			this.AddView(view);
			this.ImageView = view.FindViewById<ImageView>(Resource.Id.thumnail_image);
			this.DateView = view.FindViewById<TextView>(Resource.Id.msg_timestamp);
			this.AuthorView=view.FindViewById<TextView>(Resource.Id.msg_author);
		}
	}

}
