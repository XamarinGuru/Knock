using System;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using knock.Droid;
using AudioService;

namespace Xamarin.Forms.Chat.Droid
{
    public abstract class AudioHolderView : HolderView
    {
        protected ImageView ImageView { get; set; }
		protected ImageButton imageButton { get; set; }
		protected SeekBar seekBar { get; set; }

		public async override void Bind(MessageViewModel viewModel)
		{
			base.Bind(viewModel);
			#region audio player

			viewModel.UpdateCell += delegate
			{
				DateView.Text = viewModel.DateStr;

				//update progress
				if (viewModel.audioPosition < 0)
				{
					viewModel.audioPosition = 0;
				}
				if (viewModel.audioPosition > 100)
				{
					viewModel.audioPosition = 100;
				}
				seekBar.Progress = viewModel.audioPosition;

				//update button
				var pause = global::Android.Resource.Drawable.IcMediaPause;
				var play = global::Android.Resource.Drawable.IcMediaPlay;
				switch (viewModel.audioState)
				{
					case AudioPlayerStatus.Stopped:
						{
							if (imageButton.Drawable != Resources.GetDrawable(play))
							{
								imageButton.SetImageDrawable(Resources.GetDrawable(play));
							}
							break;
						}
					case AudioPlayerStatus.Playing:
						{
							if (imageButton.Drawable != Resources.GetDrawable(pause))
							{
								imageButton.SetImageDrawable(Resources.GetDrawable(pause));
							}
							break;
						}
				}
			};
			seekBar.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
			{
				viewModel.OnSeekBarClicked(e.Progress);
			};

			imageButton.Click+= delegate {
				//var stop = global::Android.Resource.Drawable.bu;
				var pause = global::Android.Resource.Drawable.IcMediaPause;
				var play = global::Android.Resource.Drawable.IcMediaPlay;
				switch (viewModel.audioState)
				{
					case AudioPlayerStatus.Stopped: { 
							viewModel.OnMediaButtonClicked(AudioPlayerStatus.Stopped);
							imageButton.SetImageDrawable(Resources.GetDrawable(pause));
							break;
						}
					case AudioPlayerStatus.Playing:
						{
							viewModel.OnMediaButtonClicked(AudioPlayerStatus.Playing);
							imageButton.SetImageDrawable(Resources.GetDrawable(play));
							break;
						}
				}
			};
			#endregion

			#region image
            if (viewModel.ThumbnailImageSource == null)
                return;

            this.ImageView.SetImageBitmap(null);

            var sourceHandler = new ImageLoaderSourceHandler();
			try{
            	var bitmap = await sourceHandler.LoadImageAsync(viewModel.ThumbnailImageSource, Forms.Context);
				this.ImageView.SetImageBitmap(bitmap);
			}
			catch(Exception ex) {
				this.ImageView.SetImageBitmap(null);
			}
			#endregion
            
        }
    }
}

