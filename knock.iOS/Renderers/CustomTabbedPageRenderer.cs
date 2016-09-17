using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Visual1993;
using knock.iOS;
using knock;
using UIKit;
using System.Linq;

[assembly:ExportRenderer(typeof(Views.CustomTabbedPage), typeof(CustomTabbedPageRenderer))]
namespace knock.iOS
{
	public class CustomTabbedPageRenderer: TabbedRenderer
	{
		public CustomTabbedPageRenderer ()
		{
		}

		readonly nfloat imageYOffset = 1.0f;

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			var txtFont = new UITextAttributes()
			{
			 Font = UIFont.FromName("Avenir Next Condensed",14)
			};
			if (TabBar.Items != null)
			{
				foreach (var item in TabBar.Items)
				{
					//item.Title = null;
					//item.Title = "";
					item.ImageInsets=UIEdgeInsets.Zero;

					//item.ImageInsets = new UIEdgeInsets(imageYOffset, 0, -imageYOffset, 0);
					item.SetTitleTextAttributes( txtFont,UIControlState.Normal);
				}
			}


		}
	}
}
	

