using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Xamarin.Forms;
using knock;
using knock.iOS;
using System.ComponentModel;
using CoreGraphics;
using Acr.UserDialogs;

[assembly:ExportRenderer(typeof(ImageViewer), typeof(ImageViewerRenderer))]
namespace knock.iOS
{
    public class ImageViewerRenderer : ViewRenderer<ImageViewer, ImageScrollView>
    {

        protected override void OnElementChanged(ElementChangedEventArgs<ImageViewer> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= Element_PropertyChanged;
            }

            if (e.NewElement != null)
            {
                var element = e.NewElement;
                element.PropertyChanged += Element_PropertyChanged;
                if (this.Control == null)
                {
                    this.SetNativeControl(new ImageScrollView());
                }
                this.UpdateImage();
            }
        }

        private async void UpdateImage()
        {
            var fileImageSource = this.Element.Source as FileImageSource;
            UIImage image;
            if (fileImageSource != null)
                image = UIImage.FromFile(fileImageSource.File);
            else
            {
				this.Element.IsLoaded = false;
                //UserDialogs.Instance.ShowLoading();
                var loader = new ImageLoaderSourceHandler();
                image = await loader.LoadImageAsync(this.Element.Source);
				this.Element.IsLoaded = true;
				//Visual1993.Dialogs.HideLoading ();
            }
			try{
            this.Control.DisplayImage(image);
			}
			catch(Exception) {
				//cannot display image
				this.Element.IsLoaded = false; 
				Visual1993.Dialogs.Alert2 ("Error","Cannot display image","ok");
			}
        }

        private void Element_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ImageViewer.Source))
                return;
            this.UpdateImage();
        }

        private UIImageView _imageView;
    }
}

