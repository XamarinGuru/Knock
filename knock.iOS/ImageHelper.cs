using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;

namespace Xamarin.Forms.Chat.iOS
{
    public static class ImageHelper
    {
        private static readonly UIEdgeInsets Cap = new UIEdgeInsets {
            Top = 17f,
            Left = 26f,
            Bottom = 17f,
            Right = 21f,
        };

        public static UIImage CreateBubbleImage(UIImage mask, UIColor color)
        {
            return CreateColoredImage (color, mask).CreateResizableImage (Cap, UIImageResizingMode.Stretch);
        }

        public static CALayer Mask(UIImage maskImage, UIImageView image)
        {
            var mask = new CALayer();
            mask.Contents = maskImage.CGImage;
            mask.ContentsScale = UIScreen.MainScreen.Scale;

            var capInsets = maskImage.CapInsets;
            var size = maskImage.Size;

            mask.ContentsCenter =
                new CGRect(capInsets.Left/size.Width,
                    capInsets.Top/size.Height,
                    1.0/size.Width,
                    1.0/size.Height);
            
            mask.Frame = new CGRect(0, 0, image.Bounds.Size.Width, image.Bounds.Size.Height);
            return mask;
        }

        public static UIImage CreateBubbleImage(UIImage mask)
        {
            return mask.CreateResizableImage(Cap);
        }

        public static UIImage CreateColoredImage (UIColor color, UIImage mask)
        {
            var rect = new CGRect (CGPoint.Empty, mask.Size);
            UIGraphics.BeginImageContextWithOptions (mask.Size, false, mask.CurrentScale);
            CGContext context = UIGraphics.GetCurrentContext ();
            mask.DrawAsPatternInRect (rect);
            context.SetFillColor (color.CGColor);
            context.SetBlendMode (CGBlendMode.SourceAtop);
            context.FillRect (rect);
            UIImage result = UIGraphics.GetImageFromCurrentImageContext ();
            UIGraphics.EndImageContext ();
            return result;
        }
    }
}

