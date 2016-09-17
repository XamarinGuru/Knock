using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Visual1993;
using knock.iOS;
using knock;
using UIKit;
using System.Linq;
using System;

//[assembly:ExportRenderer(typeof(ContentPage), typeof(ContentPageRenderer))]
namespace knock.iOS
{
	public class ContentPageRenderer : PageRenderer
	{
		public void setFonts()
		{
			if (this.Element == null) {
				return;
			}
			var itemsInfo = (this.Element as ContentPage).ToolbarItems;

			var navigationItem = this.NavigationController.TopViewController.NavigationItem;
			var leftNativeButtons = (navigationItem.LeftBarButtonItems ?? new UIBarButtonItem[]{ }).ToList();
			var rightNativeButtons = (navigationItem.RightBarButtonItems ?? new UIBarButtonItem[]{ }).ToList();

			MessagingCenter.Unsubscribe<Messenger> (App.messenger, "hideBack");
			MessagingCenter.Subscribe<Visual1993.Messenger> (App.messenger, "hideBack", (sender0) => {
				Device.BeginInvokeOnMainThread (() => {
					try {
						navigationItem.HidesBackButton = true;
					} catch (Exception ex) {
						//
					}
				});
			});
			MessagingCenter.Unsubscribe<Messenger> (App.messenger, "showBack");
			MessagingCenter.Subscribe<Visual1993.Messenger>(App.messenger, "showBack", (sender1) => {
				Device.BeginInvokeOnMainThread(() => {
					try{	navigationItem.HidesBackButton=false;
					}
					catch (Exception ex)
					{
						//
					}
				});
			});

			rightNativeButtons.ToList().ForEach(nativeItem =>
				{
					nativeItem.SetTitleTextAttributes(new UITextAttributes()
						{
							//TextColor = UIColor.Red,
							Font = UIFont.FromName("Avenir Next Condensed",20)
						},UIControlState.Normal);

					rightNativeButtons.Remove(nativeItem);
					rightNativeButtons.Add(nativeItem);

				});
			/*
			leftNativeButtons.ToList().ForEach(nativeItem =>
				{
					nativeItem.SetTitleTextAttributes(new UITextAttributes()
						{
							TextColor = UIColor.Red,
							Font = UIFont.FromName("Avenir Next Condensed",20)
						},UIControlState.Normal);

					leftNativeButtons.Remove(nativeItem);
					leftNativeButtons.Add(nativeItem);

				});
			*/
			if (navigationItem.RightBarButtonItem != null) {
				navigationItem.RightBarButtonItem.SetTitleTextAttributes (new UITextAttributes () {
					//TextColor = UIColor.Red,
					Font = UIFont.FromName ("Avenir Next Condensed", 20)
				}, UIControlState.Normal);
			}
			/*
			if (navigationItem.LeftBarButtonItem != null) {
				navigationItem.LeftBarButtonItem.SetTitleTextAttributes (new UITextAttributes () {
					TextColor = UIColor.Red,
					Font = UIFont.FromName ("Avenir Next Condensed", 20)
				}, UIControlState.Normal);
			}
			*/

			navigationItem.RightBarButtonItems = rightNativeButtons.ToArray();
			//navigationItem.LeftBarButtonItems = leftNativeButtons.ToArray();

			UIBarButtonItem bottoneIndietro = new UIBarButtonItem("BACK",UIBarButtonItemStyle.Plain,buttonIndietroHandler);
			bottoneIndietro.SetTitleTextAttributes (new UITextAttributes () {
				Font = UIFont.FromName ("Avenir Next Condensed", 20)
			}, UIControlState.Normal);
			if (App.navigation.CurrentPage != App.navigation.Navigation.NavigationStack.First ()) {
				//navigationItem.LeftBarButtonItem = bottoneIndietro;
				navigationItem.BackBarButtonItem = bottoneIndietro;
				navigationItem.HidesBackButton = false;
			}
		}
		static async void buttonIndietroHandler(object sender, EventArgs e)
		{
			if (App.navigation.Navigation.NavigationStack.Count > 1) {
				try{
					await App.navigation.PopAsync ();
				}
				catch(Exception ex)
				{
					//cannot go back
					MessagingCenter.Send<Messenger> (App.messenger, "hideBack");
				}
			}
		}
		public override void ViewWillAppear(bool animated)
		{
			MessagingCenter.Subscribe<Visual1993.Messenger>(App.messenger, "setFonts", (sender) => {
				Device.BeginInvokeOnMainThread(() => {
					try{	setFonts ();
					}
					catch (Exception ex)
					{
						//
					}
				});
			});
			setFonts ();
			base.ViewWillAppear(animated);
		}

		/*private ToolbarItem GetButtonInfo(IList<ToolbarItem> items, string name)
		{
			if (string.IsNullOrEmpty(name) || items == null)
				return null;

			return items.ToList().Where(itemData => name.Equals(itemData.Name)).FirstOrDefault();
		}*/
	}
}
