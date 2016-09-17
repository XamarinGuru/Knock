using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Visual1993;
using knock.iOS;
using knock;
using UIKit;

[assembly: ExportRenderer (typeof(Views.ZoomWebView), typeof(knock.iOS.ZoomWebViewRenderer))]
namespace knock.iOS
{
	public class ZoomWebViewRenderer : WebViewRenderer
	{
		protected override void OnElementChanged (VisualElementChangedEventArgs e)
		{
			if (this.Element == null) {
				return;
			}
			//var webView = (this.Element as WebView);
			this.ScalesPageToFit=true;

			base.OnElementChanged (e);
		}

	}
}