using System;
using UIKit;
using CoreGraphics;
using System.IO;

namespace knock.iOS
{
    public class ImageScrollView : UIScrollView
    {
        CGSize _imageSize;
        // if tiling this contains a very low-res placeholder image. otherwise it contains the full image.
        UIImageView zoomView;
        CGPoint _pointToCenterAfterResize;
        nfloat _scaleToRestoreAfterResize;

        public override CGRect Frame {
            get {
                return base.Frame;
            }
            set {
                bool sizeChanging = Frame.Size != value.Size;
                if (sizeChanging)
                    PrepareToResize ();

                base.Frame = value;

                if (sizeChanging)
                    RecoverFromResizing ();
            }
        }

        public ImageScrollView ()
        {
            BackgroundColor = UIColor.Clear;
            ShowsVerticalScrollIndicator = false;
            ShowsHorizontalScrollIndicator = false;
            BouncesZoom = true;
            DecelerationRate = UIScrollView.DecelerationRateFast;

            // Return the view to use when zooming
            ViewForZoomingInScrollView = (sv) => zoomView;
        }

        public override void LayoutSubviews ()
        {
            base.LayoutSubviews ();

            if (zoomView == null)
                return;

            //center the zoom view as it becomes smaller than the size of the screen
            var boundsSize = this.Bounds.Size;
            var frameToCenter = zoomView.Frame;

            //center horizontally
            if (frameToCenter.Size.Width < boundsSize.Width)
                frameToCenter.X = (boundsSize.Width - frameToCenter.Size.Width) / 2;
            else
                frameToCenter.X = 0;

            //center vertically
            if (frameToCenter.Size.Height < boundsSize.Height)
                frameToCenter.Y = (boundsSize.Height - frameToCenter.Size.Height) / 2;
            else
                frameToCenter.Y = 0;

            zoomView.Frame = frameToCenter;
        }

        public void DisplayImage (UIImage image)
        {
            if (image == null)
                throw new ArgumentNullException ("image");

            if (zoomView != null) {
                zoomView.RemoveFromSuperview ();
                zoomView = null;
                ZoomScale = 1.0f;
            }

            zoomView = new UIImageView (image)
                {
                    BackgroundColor = UIColor.Clear
                };
            AddSubview (zoomView);
            ConfigureForImageSize (image.Size);
            this.SetNeedsLayout();
        }

        public void ConfigureForImageSize (CGSize imageSize)
        {
            _imageSize = imageSize;
            ContentSize = imageSize;
            SetMaxMinZoomScalesForCurrentBounds ();
            ZoomScale = MinimumZoomScale;
        }

        public void SetMaxMinZoomScalesForCurrentBounds ()
        {
            CGSize boundsSize = Bounds.Size;

            //calculate min/max zoomscale
            nfloat xScale = boundsSize.Width / _imageSize.Width; //scale needed to perfectly fit the image width-wise
            nfloat yScale = boundsSize.Height / _imageSize.Height; //scale needed to perfectly fit the image height-wise

            //fill width if the image and phone are both portrait or both landscape; otherwise take smaller scale
            bool imagePortrait = _imageSize.Height > _imageSize.Width;
            bool phonePortrait = boundsSize.Height > boundsSize.Width;
            var minScale = (nfloat)(imagePortrait == phonePortrait ? xScale : NMath.Min (xScale, yScale));

            //on high resolution screens we have double the pixel density, so we will be seeing every pixel if we limit the maximum zoom scale to 0.5
            nfloat maxScale = 1 / UIScreen.MainScreen.Scale;

            if (minScale > maxScale)
                minScale = maxScale;

            // don't let minScale exceed maxScale. (If the image is smaller than the screen, we don't want to force it to be zoomed.)
            MaximumZoomScale = maxScale;
            MinimumZoomScale = minScale;
        }

        // Methods called during rotation to preserve the zoomScale and the visible portion of the image

        // Rotation support
        public void PrepareToResize ()
        {
            var boundsCenter = new CGPoint (Bounds.GetMidX(), Bounds.GetMidY());
            _pointToCenterAfterResize = ConvertPointToView (boundsCenter, zoomView);
            _scaleToRestoreAfterResize = ZoomScale;
            // If we're at the minimum zoom scale, preserve that by returning 0, which will be converted to the minimum
            // allowable scale when the scale is restored.
            if (_scaleToRestoreAfterResize <= this.MinimumZoomScale + float.Epsilon)
                _scaleToRestoreAfterResize = 0;
        }

        public void RecoverFromResizing ()
        {
            SetMaxMinZoomScalesForCurrentBounds ();

            //Step 1: restore zoom scale, first making sure it is within the allowable range;
            ZoomScale = NMath.Min (MaximumZoomScale, NMath.Max (MinimumZoomScale, _scaleToRestoreAfterResize));

            // Step 2: restore center point, first making sure it is within the allowable range.
            // 2a: convert our desired center point back to our own coordinate space
            var boundsCenter = this.ConvertPointFromView (_pointToCenterAfterResize, zoomView);
            // 2b: calculate the content offset that would yield that center point
            CGPoint offset = new CGPoint (boundsCenter.X - Bounds.Size.Width / 2.0f, boundsCenter.Y - Bounds.Size.Height / 2.0f);
            // 2c: restore offset, adjusted to be within the allowable range
            CGPoint maxOffset = MaximumContentOffset ();
            CGPoint minOffset = MinimumContentOffset ();
            offset.X = NMath.Max (minOffset.X, NMath.Min (maxOffset.X, offset.X));
            offset.Y = NMath.Max (minOffset.Y, NMath.Min (maxOffset.Y, offset.Y));
            ContentOffset = offset;
        }

        public CGPoint MaximumContentOffset ()
        {
            CGSize contentSize = ContentSize;
            CGSize boundsSize = Bounds.Size;
            return new CGPoint (contentSize.Width - boundsSize.Width, contentSize.Height - boundsSize.Height);
        }

        public CGPoint MinimumContentOffset ()
        {
            return CGPoint.Empty;
        }
            
    }
}

