using System;
using knock.Droid;
using knock;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;

[assembly:ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
namespace knock.Droid
{
    public class CustomPickerRenderer : Xamarin.Forms.Platform.Android.PickerRenderer
    {
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            var element = this.Element as CustomPicker;
            if (element == null)
                return;

            this.MakeRoundCorners();

            var fontFamily = Device.OnPlatform (
                iOS:      "Avenir Next Condensed",
                Android:  "Droid Sans",
                WinPhone: "Comic Sans MS"
            );
            this.Control.Typeface = Typeface.Create(fontFamily, TypefaceStyle.Normal);
            this.Control.SetTextColor(Tema.coloreSfondoScuro.ToAndroid());
        }
    }
}

