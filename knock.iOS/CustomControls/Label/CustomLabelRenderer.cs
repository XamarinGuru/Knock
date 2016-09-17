using System;
using knock;
using Xamarin.Forms;
using knock.iOS;
using Xamarin.Forms.Platform.iOS;
using XLabs.Forms.Controls;

[assembly:ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace knock.iOS
{
	public class CustomLabelRenderer : ExtendedLabelRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Label> e)
		{

			base.OnElementChanged(e);
			var element = this.Element as CustomLabel;
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

