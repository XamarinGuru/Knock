using Java.Lang;
using Android.App;
using Android.Widget;
using knock.Droid;

namespace Xamarin.Forms.Chat.Droid
{
    public class OutgoingTextHolderView : TextHolderView
	{
        public OutgoingTextHolderView()
        {
			var resource = Resource.Layout.item_conversation_msg_outgoing;
            var view = (Forms.Context as Activity).LayoutInflater.Inflate(resource, null);
            this.AddView(view);
			this.MessageView = view.FindViewById<TextView>(Resource.Id.msg_text);
            this.DateView = view.FindViewById<TextView>(Resource.Id.msg_timestamp);
			this.AuthorView=view.FindViewById<TextView>(Resource.Id.msg_author);
        }
	}
}

