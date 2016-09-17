using System;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Visual1993;

[assembly: ExportRenderer(typeof(Views.ColoredPicker), typeof(knock.Droid.ColoredPickerRenderer))]
namespace knock.Droid
{
	public class ColoredPickerRenderer: PickerRenderer
	{
		/*protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Picker> e)
		{
			base.OnElementChanged (e);
			if (Control != null) {
				Control.SetBackgroundColor (global::Android.Graphics.Color.LightGreen);
			}
		}
		*/

		protected override void OnElementChanged (ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged (e);
			//var xe = e as ColoredPicker;
			if (Control != null) {
				Control.SetTextColor (Tema.coloreRosa.ToAndroid()); //global::Android.Graphics.Color.LightGreen
				Control.SetHintTextColor(Tema.coloreRosa.ToAndroid());
				Control.SetLinkTextColor(Tema.coloreRosa.ToAndroid());
			}
		}

	}


}