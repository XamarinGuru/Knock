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


[assembly: ExportRenderer(typeof(TabbedPage), typeof(knock.Droid.TabbedPageRenderer))]
namespace knock.Droid
{
	public class TabbedPageRenderer: TabbedRenderer 
	{
		/*
		public override void OnWindowFocusChanged(bool hasWindowFocus)
		{
			Activity activity = this.Context as Activity;
			var element = this.Element;
			if (null == element)
			{
				return;
			}

			if ((null != activity) && (null != activity.ActionBar) && (activity.ActionBar.TabCount > 0)) {
				for (int i = 0; i < element.Children.Count; i += 1) {
					var tab = activity.ActionBar.GetTabAt (i);
					var page = element.Children [i];
					if ((null != tab) && (null != page) && (null != page.Icon)) {
						
						tab.SetIcon (this.Context.Resources.GetDrawable (page.Icon.File));
					}
				}
			}
		}
		*/
	}
}