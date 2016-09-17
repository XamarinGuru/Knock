using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using knock.iOS;
using knock;
using UIKit;

[assembly:ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]
namespace knock.iOS
{
    public class CustomDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            var element = this.Element as CustomDatePicker;
            if (element == null)
                return;

            if (element.BorderWidth > 0)
            {
                this.Control.Layer.BorderWidth = element.BorderWidth;
                this.Control.Layer.BorderColor = element.BorderColor.ToCGColor();
            }

            if (element.CornerRadius > 0)
            {
                this.Control.Layer.CornerRadius = element.CornerRadius;
            }

            var fontFamily = Device.OnPlatform (
                iOS:      "Avenir Next Condensed",
                Android:  "Droid Sans",
                WinPhone: "Comic Sans MS"
            );
            this.Control.Font = UIFont.FromName (fontFamily, (float)Tema.fontSizeMedium);
        }
    }
}

