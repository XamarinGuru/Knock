using System;
using knock.Droid;
using knock;
using Xamarin.Forms;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;

[assembly:ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]
namespace knock.Droid
{
    public class CustomDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            var element = this.Element as CustomDatePicker;
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

