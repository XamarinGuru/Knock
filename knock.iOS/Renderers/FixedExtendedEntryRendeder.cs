using System;
using XLabs.Forms.Controls;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;

namespace knock.iOS
{
    public class FixedExtendedEntryRendeder : ExtendedEntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            var element = this.Element;
            if (element != null)
            {
				element.FontFamily=Device.OnPlatform (
					iOS:      "Avenir Next Condensed",
					Android:  "Droid Sans",
					WinPhone: "Comic Sans MS"
				);
				if (element.FontFamily != null) {
					this.Control.Font = UIFont.FromName (element.FontFamily, (float)element.FontSize);
				}
            }
        }
    }
}

