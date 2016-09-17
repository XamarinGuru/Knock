using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Visual1993;
using knock.iOS;
using knock;
using UIKit;
using Facebook.LoginKit;
using Facebook.CoreKit;
using System.Collections.Generic;
using CoreGraphics;

[assembly: ExportRenderer(typeof(Views.FBLoginButton), typeof(knock.iOS.FBLoginViewRenderer))]
namespace knock.iOS
{
	public class FBLoginViewRenderer : ButtonRenderer
	{
		// To see the full list of permissions, visit the following link:
		// https://developers.facebook.com/docs/facebook-login/permissions/v2.3

		// This permission is set by default, even if you don't add it, but FB recommends to add it anyway
		List<string> readPermissions = new List<string> { "public_profile", "user_about_me" };


	LoginButton loginView;
	ProfilePictureView pictureView;
	UILabel nameLabel;

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
		{
			// If was send true to Profile.EnableUpdatesOnAccessTokenChange method
			// this notification will be called after the user is logged in and
			// after the AccessToken is gotten
			Profile.Notifications.ObserveDidChange((sender, e1) =>
			{

				if (e1.NewProfile == null)
					return;
			});

			// Set the Read and Publish permissions you want to get
			loginView = new LoginButton(new CGRect(51, 0, 218, 46))
			{
				LoginBehavior = LoginBehavior.Native,
				ReadPermissions = readPermissions.ToArray()
			};

			// Handle actions once the user is logged in
			loginView.Completed += (sender, e2) =>
			{
				if (e2.Error != null)
				{
				// Handle if there was an error
			}

				if (e2.Result.IsCancelled)
				{
				// Handle if the user cancelled the login request
			}

			// Handle your successful login
		};
			// Handle actions once the user is logged out
			loginView.LoggedOut += (sender, e3) =>
			{
			// Handle your logout
		};
			SetNativeControl(loginView);

		}
	/*public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		
		
		// Add views to main view
		View.AddSubview(loginView);
		View.AddSubview(pictureView);
		View.AddSubview(nameLabel);
	}
*/
	}
}