using System;
using Android.Widget;
using knock.Droid;

namespace Xamarin.Forms.Chat.Droid
{
    public abstract class TextHolderView : HolderView
    {
        protected TextView MessageView { get; set; }

        public override void Bind(MessageViewModel viewModel)
        {
			try
			{
				this.MessageView.Text = viewModel.Content;
			}
			catch (Exception) { }
            base.Bind(viewModel);
        }
    }
}

