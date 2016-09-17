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

[assembly: ExportRenderer(typeof(Views.ZoomWebView), typeof(knock.Droid.WebViewZoomRenderer))]
namespace knock.Droid
{
	public class WebViewZoomRenderer: WebViewRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<WebView> e)
		{
			base.OnElementChanged (e);
			//var xe = e as ColoredPicker;
			if (Control != null) {
				Control.Settings.BuiltInZoomControls = true;
				Control.Settings.DisplayZoomControls = false;
			}
		}

	}


}

