using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MonoDroidToolkit.ImageLoader;

namespace knock.Droid.Renderers
{
    public static class ImageLoaderCache
    {
        //TODO change to a proper dictionary
        static ImageLoader _onlyLoader;

        public static ImageLoader GetImageLoader(VisualImageRenderer imageRenderer)
        {
            //TODO
            if (_onlyLoader == null)
            {
                _onlyLoader = new ImageLoader(Android.App.Application.Context, 64, 40);
            }
            return _onlyLoader;
        }
    }
    public class VisualImageRenderer : ImageRenderer
    {
        ImageLoader _imageLoader;

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            //			if (e.OldElement != null) {
            //				((FastImage)e.OldElement).ImageProvider = null;
            //			}
            if (e.NewElement != null)
            {
                var fastImage = e.NewElement as Visual1993Image;
                _imageLoader = ImageLoaderCache.GetImageLoader(this);
                SetImageUrl(fastImage.ImageUrl);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "ImageUrl")
            {
                var fastImage = Element as Visual1993Image;
                SetImageUrl(fastImage.ImageUrl);
            }
        }


        public void SetImageUrl(string imageUrl)
        {
            if (Control == null)
            {
                return;
            }
            if (imageUrl != null)
            {
                _imageLoader.DisplayImage(imageUrl, Control, -1);

            }
        }
    }
}