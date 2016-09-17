using System;
using Xamarin.Forms;
using knock;
using knock.Droid;
using com.refractored.monodroidtoolkit;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Acr.UserDialogs;
using Android.Graphics;

[assembly:ExportRenderer(typeof(ImageViewer), typeof(ImageViewerRenderer))]
namespace knock.Droid
{
    public class ImageViewerRenderer : ViewRenderer<ImageViewer, ScaleImageView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ImageViewer> e)
        {
            base.OnElementChanged(e);

            var element = e.NewElement;
            element.PropertyChanged += Element_PropertyChanged;
            if (this.Control == null)
                this.SetNativeControl(new ScaleImageView(this.Context, null));
            this.UpdateImage();
        }

        private async void UpdateImage()
        {
            var fileImageSource = this.Element.Source as FileImageSource;
			this.Element.IsLoaded = false;

            Bitmap image;
            if (fileImageSource != null)
                image = await BitmapFactory.DecodeFileAsync(fileImageSource.File);
            else
            {
                var loader = new ImageLoaderSourceHandler();
                image = await loader.LoadImageAsync(this.Element.Source, this.Context);
            }
            this.Control.SetImageBitmap(image);
			this.Element.IsLoaded = true;
			//Visual1993.Dialogs.HideLoading ();
        }

        private void Element_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ImageViewer.Source))
                return;
            this.UpdateImage();
        }
    }
}

