using System;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using CoreGraphics;

namespace knock.iOS
{
    public static class AppearanceCustomization
    {
        public static void Perform()
        {
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
                {
                    TextColor = UIColor.White,
                    Font = UIFont.FromName("Avenir Next Condensed",20)
                });

            UIBarButtonItem.Appearance.SetTitleTextAttributes(new UITextAttributes()
                {
                    TextColor = UIColor.White,
                    Font = UIFont.FromName("Avenir Next Condensed", 20)
                }, UIControlState.Normal);

            UITabBarItem.Appearance.SetTitleTextAttributes(new UITextAttributes()
                {
                    TextColor = new OnIdiom<UIColor>()
                        {
                            Tablet = Tema.coloreViolaSfondo.ToUIColor(),
                            Phone = Tema.coloreSfondoChiaro.ToUIColor(),
                        },
                    Font = UIFont.FromName("Avenir Next Condensed",16)
                }, UIControlState.Normal);
                        
            UITabBar.Appearance.BackgroundImage = UIImage.FromBundle("TabBar");
        }

        public static UIImage ImageWithRect(this UIColor color, CGRect rect)
        {
            UIGraphics.BeginImageContext(rect.Size);
            var context = UIGraphics.GetCurrentContext();
            context.SetFillColor(color.CGColor);
            context.FillRect(rect);
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }
    }
}

