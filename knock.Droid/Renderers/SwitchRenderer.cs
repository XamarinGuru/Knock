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

[assembly: ExportRenderer(typeof(Views.ColoredSwitch), typeof(knock.Droid.ColoredSwitchRenderer))]
namespace knock.Droid
{
	public class ColoredSwitchRenderer: SwitchRenderer
	{
		
		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (Control != null) {
				//Control.color
				Android.Graphics.Color colorOn = Tema.coloreRosa.ToAndroid();
				Android.Graphics.Color colorOff = Android.Graphics.Color.Black;
				Android.Graphics.Color colorDisabled = Android.Graphics.Color.DarkGray;

				Android.Graphics.Drawables.StateListDrawable drawable = new Android.Graphics.Drawables.StateListDrawable();
				drawable.AddState(new int[] { Android.Resource.Attribute.StateChecked }, new Android.Graphics.Drawables.ColorDrawable(colorOn));
				drawable.AddState(new int[] { -Android.Resource.Attribute.StateEnabled }, new Android.Graphics.Drawables.ColorDrawable(colorDisabled));
				drawable.AddState(new int[] { }, new Android.Graphics.Drawables.ColorDrawable(colorOff));

				Control.ThumbDrawable = drawable;
			}
		}
	
	}


}