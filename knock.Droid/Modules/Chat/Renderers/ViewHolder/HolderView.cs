using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;

namespace Xamarin.Forms.Chat.Droid
{
    public abstract class HolderView : LinearLayout
    {
        protected TextView MessageView { get; set; }
		protected TextView DateView { get; set; }
		protected TextView AuthorView { get; set; }
		protected global::Android.Widget.ProgressBar LoadingBar { get; set; }

        protected HolderView() : base(Forms.Context)
        {
            this.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent, 1f);
        }

        public virtual void Bind(MessageViewModel viewModel)
        {
            this.DateView.Text = viewModel.DateStr;
			this.AuthorView.Text = viewModel.Author;

			if (viewModel.Author != null && viewModel.Author.Length > 1) {
				//this.AuthorView.Visibility = ViewStates.Visible;
			} else {
				this.AuthorView.SetHeight (0);
			}
        }
    }
}

