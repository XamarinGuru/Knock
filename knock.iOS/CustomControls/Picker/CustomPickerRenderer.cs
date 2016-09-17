using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using knock;
using knock.iOS;
using UIKit;

[assembly:ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer))]
namespace knock.iOS
{
    public class CustomPickerRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            var element = this.Element as CustomPicker;
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

