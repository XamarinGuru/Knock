using System;
using XLabs.Forms.Controls;
using Xamarin.Forms;
using knock;
using knock.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Content.Res;
using Android.Support.V4.Content;

[assembly:ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace knock.Droid
{
	public class CustomLabelRenderer : ExtendedLabelRender
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Label> e)
		{
			base.OnElementChanged(e);

			var element = this.Element as CustomLabel;
			if (element == null)
				return;

			this.MakeRoundCorners();
		}
	}
}

