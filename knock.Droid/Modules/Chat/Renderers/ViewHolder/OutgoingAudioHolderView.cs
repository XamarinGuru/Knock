using Java.Lang;
using Android.App;
using Android.Widget;
using knock.Droid;

namespace Xamarin.Forms.Chat.Droid
{
    public class OutgoingAudioHolderView : AudioHolderView
    {
        public OutgoingAudioHolderView() : base ()
        {
			var resource = Resource.Layout.item_conversation_msg_audio_outgoing;
            var view = (Forms.Context as Activity).LayoutInflater.Inflate(resource, null);
            this.AddView(view);
            this.ImageView = view.FindViewById<ImageView>(Resource.Id.thumnail_image);
            this.DateView = view.FindViewById<TextView>(Resource.Id.msg_timestamp);
			this.AuthorView=view.FindViewById<TextView>(Resource.Id.msg_author);
			this.imageButton = view.FindViewById<ImageButton>(Resource.Id.toggleButton);
			this.seekBar = view.FindViewById<SeekBar>(Resource.Id.seekBar1);
        }
    }

}
