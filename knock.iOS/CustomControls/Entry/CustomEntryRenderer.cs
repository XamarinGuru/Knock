using System;
using knock;
using Xamarin.Forms;
using knock.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace knock.iOS
{
    public class CustomEntryRenderer : FixedExtendedEntryRendeder
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            var element = this.Element as CustomEntry;
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
        }
    }
}

