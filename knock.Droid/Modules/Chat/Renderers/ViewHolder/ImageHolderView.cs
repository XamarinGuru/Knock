using System;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.Chat.Droid
{
    public abstract class ImageHolderView : HolderView
    {
        protected ImageView ImageView { get; set; }

        public async override void Bind(MessageViewModel viewModel)
        {
            base.Bind(viewModel);

			doSetImage(viewModel);

			doUpdateCell(viewModel);
			viewModel.UpdateCell += delegate
			{
				doUpdateCell(viewModel);
			};

        }

		public async void doSetImage(MessageViewModel viewModel)
		{
			if (viewModel.ThumbnailImageSource == null)
				return;

			this.ImageView.SetImageBitmap(null);

			var sourceHandler = new ImageLoaderSourceHandler();
			try
			{
				var bitmap = await sourceHandler.LoadImageAsync(viewModel.ThumbnailImageSource, Forms.Context);
				this.ImageView.SetImageBitmap(bitmap);
			}
			catch (Exception ex)
			{
				this.ImageView.SetImageBitmap(null);
			}
			if (this.MessageView != null) { this.MessageView.Text = ""; this.MessageView.Visibility = global::Android.Views.ViewStates.Invisible; }
			if (string.IsNullOrWhiteSpace(viewModel.Content) == false)
			{
				if (this.MessageView != null) { this.MessageView.Text = viewModel.Content; this.MessageView.Visibility = global::Android.Views.ViewStates.Visible; }
			}

		}
		public void doUpdateCell(MessageViewModel viewModel)
		{
			if (viewModel.IsOutgoing)
			{
				if (this.LoadingBar != null)
				{
					switch (viewModel.isUploaded)
					{
						case true: { this.LoadingBar.Visibility = global::Android.Views.ViewStates.Invisible; break; }
						case false: { if (viewModel.isAnImageToUpload == false) { break; } this.LoadingBar.Visibility = global::Android.Views.ViewStates.Visible; break; }
					}
				}
			}
			doSetImage(viewModel);
		}
    }
}

