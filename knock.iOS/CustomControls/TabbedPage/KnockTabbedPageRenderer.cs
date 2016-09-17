using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using knock.HotelManagementModule;
using knock.iOS;
using System.Linq;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using knock.CustomControls;

[assembly: ExportRenderer (typeof(KnockTabbedPage), typeof(KnockTabbedPageRenderer))]
namespace knock.iOS
{
    public class KnockTabbedPageRenderer : TabbedRenderer
    {
        private readonly List<UIImageView> _images = new List<UIImageView>();
        private static UIColor SelectedColor = Color.FromHex("#DADADC").ToUIColor();

        public override void ViewWillLayoutSubviews()
        {
            this.UpdateTabBarFrame();
            base.ViewWillLayoutSubviews();
            this.UpdateImages();
        }

        private void UpdateImages()
        {
            var step = (int)this.View.Bounds.Width / 
                this.TabBar.Items.Count() + 1;
            int i = 0;
            foreach (var image in this._images)
            {
                if (image.Superview == null)
                    this.TabBar.AddSubview(image);
                var frame = image.Frame;
                var x = step * i++ + (step - frame.Width) / 2;
                image.Frame = new CGRect(x, frame.Y, frame.Width, frame.Height);
            }

            this.TabBar.SelectionIndicatorImage = 
                AppearanceCustomization.ImageWithRect(SelectedColor, 
                    new CGRect(0, .5, step, this.TabBar.Frame.Height));
        }

        private void UpdateTabBarFrame()
        {
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                var tabFrame = this.TabBar.Frame;
                tabFrame.Height = 80;
                tabFrame.Y = this.View.Frame.Height - 80;
                this.TabBar.Frame = tabFrame;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            foreach (var item in this.TabBar.Items)
            {
                if (Device.Idiom == TargetIdiom.Phone)
                    item.Title = string.Empty;

                item.Image = null;
            }
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            this.TabBar.ItemPositioning = UIKit.UITabBarItemPositioning.Fill;
            this._images.Clear();
            var tabbedPage = this.Element as TabbedPage;
            var rect = new OnIdiom<CGRect>()
                {
                    Tablet = new CGRect(0, 2, 56, 56),
                    Phone = new CGRect(0, 4, 40, 40),
                };
            foreach (var page in tabbedPage.Children)
            {
                var img = UIImage.FromBundle(page.Icon.File);
                var imageView = new UIImageView(rect)
                    {
                        Image = img,
                        ContentMode = UIViewContentMode.ScaleAspectFit
                    };
                this._images.Add(imageView);
            }
        }
    }
}

