using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Visual1993;
using knock.iOS;
using knock;
using UIKit;

[assembly: ExportRenderer (typeof(Views.ColoredPicker), typeof(knock.iOS.ColoredPickerRenderer))]
namespace knock.iOS
{
	public class ColoredPickerRenderer : PickerRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged (e);

			if (Control != null) {
				// do whatever you want to the UITextField here!
				Control.TextColor=Tema.coloreRosa.ToUIColor();
				Control.Font = UIFont.FromName ("Avenir Next Condensed", 14);
				//Control.TintColor=Tema.coloreRosa.ToUIColor();
				//Control.BackgroundColor = UIColor.FromRGB (204, 153, 255);
				//Control.BorderStyle = UITextBorderStyle.Line;
			}
		}
	}
}