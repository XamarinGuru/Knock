using System;
using UIKit;
using CoreGraphics;

namespace Xamarin.Forms.Chat.iOS
{
	public class ImageTableViewCell : BubbleTableViewCell
	{
		public ImageTableViewCell(MessageCell cell) : base(cell)
		{
		}

		private UIImageView _imageView;
		public static readonly int ImageHeight = 150;
		private static readonly UIImage LoadingImage;
		private static readonly int LoadingImageWidth;
		private static readonly UIImage IncomingBubbleImageStatic;
		private static readonly UIImage OutgoingBubbleImageStatic;

		// Rener addedd
		private UIImageView loadingView = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };
		private UIActivityIndicatorView indicator = new UIActivityIndicatorView { TranslatesAutoresizingMaskIntoConstraints = false };
		public UILabel MessageView = new UILabel
			{
				TranslatesAutoresizingMaskIntoConstraints = false,
				//TextAlignment=UITextAlignment.Left, //this is useless if the view is centered
				Lines = 0,
				PreferredMaxLayoutWidth = 220f,
				TextColor = UIColor.White,
				BackgroundColor=UIColor.DarkGray,
				Font = UIFont.FromName("Avenir Next Condensed", 15),
				//Text = viewModel.Content
	};

		private void addLoadingView()
		{
			loadingView.BackgroundColor = UIColor.Black;
			loadingView.Alpha = 0.5f;
			loadingView.AddSubview(indicator);
		}
		// ended

		static ImageTableViewCell ()
		{
			var mask = UIImage.FromBundle ("BubbleIncoming");
			IncomingBubbleImageStatic = ImageHelper.CreateBubbleImage(mask);

			mask = UIImage.FromBundle ("BubbleOutgoing");
			OutgoingBubbleImageStatic = ImageHelper.CreateBubbleImage(mask);

			LoadingImage = UIImage.FromBundle("chatBianca.png");
			LoadingImageWidth = (int) (LoadingImage.Size.Width * ImageHeight / LoadingImage.Size.Height);
		}

		private MessageViewModel _viewModel;

		#region IBubbleCellContentDrawer implementation
		protected override UIKit.UIView CreateSubview(MessageViewModel viewModel)
		{
			this._viewModel = viewModel;
			return new UIView();
		}

		protected override UIImage IncomingBubbleImage { get { return null; } }
		protected override UIImage OutgoingBubbleImage { get { return null; } }
		protected override void UpdateConstraintsForIncomingCell(UIKit.UIImageView bubbleImageView) 
		{
			this.DateView.BackgroundColor = UIColor.FromWhiteAlpha(1, .7f);
			this.AuthorView.BackgroundColor = UIColor.FromWhiteAlpha(1, .7f);
			//this.MessageView.BackgroundColor = UIColor.Black;

			this.ContentView.AddConstraints(new []{
				NSLayoutConstraint.Create (this.DateView, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 16),
				NSLayoutConstraint.Create (this.DateView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Bottom, 1, -6),

				NSLayoutConstraint.Create (this.AuthorView, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 16),
				NSLayoutConstraint.Create (this.AuthorView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Top, 1, 6),
				
				                  NSLayoutConstraint.Create (this.MessageView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Bottom, 1, -20),
				                  NSLayoutConstraint.Create (this.MessageView, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 16),
				                  NSLayoutConstraint.Create (this.MessageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.CenterX, 1, 3),

			});

			// Rener addedd
			this._imageView.AddConstraints(new[]{
				NSLayoutConstraint.Create(loadingView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this._imageView, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create(loadingView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this._imageView, NSLayoutAttribute.CenterY, 1, 0),
				NSLayoutConstraint.Create(loadingView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, this._imageView, NSLayoutAttribute.Width, 1, 0),
				NSLayoutConstraint.Create(loadingView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, this._imageView, NSLayoutAttribute.Height, 1, 0)
			});
			loadingView.AddConstraints(new[]{
				NSLayoutConstraint.Create(indicator, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, loadingView, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create(indicator, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, loadingView, NSLayoutAttribute.CenterY, 1, 0)
			});
 			// end

			this.UpdateSizeConstraints();
		}

		private void UpdateSizeConstraints()
		{
			this._imageView.AddConstraints(NSLayoutConstraint.FromVisualFormat(string.Format("H:[bubble(=={0})]", this._imageView.Bounds.Width), (NSLayoutFormatOptions)0, "bubble", this._imageView));
			this._imageView.AddConstraints(NSLayoutConstraint.FromVisualFormat(string.Format("V:[bubble(=={0})]", this._imageView.Bounds.Height), (NSLayoutFormatOptions)0, "bubble", this._imageView));
		}

		protected override void UpdateConstraintsForOutgoingCell(UIKit.UIImageView bubbleImageView) 
		{
			this.DateView.BackgroundColor = UIColor.FromWhiteAlpha(1, .7f);
			this.AuthorView.BackgroundColor = UIColor.FromWhiteAlpha(1, .7f);

			this.ContentView.AddConstraints(new []{
				NSLayoutConstraint.Create (this.DateView, NSLayoutAttribute.Leading, NSLayoutRelation.LessThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 10),
				NSLayoutConstraint.Create (this.DateView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Bottom, 1, -6),

				NSLayoutConstraint.Create (this.AuthorView, NSLayoutAttribute.Leading, NSLayoutRelation.LessThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 10),
				NSLayoutConstraint.Create (this.AuthorView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Top, 1, 6),

				                  NSLayoutConstraint.Create (this.MessageView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Bottom, 1, -20),
								  NSLayoutConstraint.Create (this.MessageView, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 16),
								  NSLayoutConstraint.Create (this.MessageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.CenterX, 1, 3),
			});

			this.UpdateSizeConstraints();
		}

		protected override UIImageView IncomingBubbleImageView
		{
			get
			{
				this._imageView = CreateImageView(IncomingBubbleImageStatic);
				return this._imageView;
			}
		}

		protected override UIImageView OutgoingBubbleImageView
		{
			get
			{
				this._imageView = CreateImageView(OutgoingBubbleImageStatic);
				return this._imageView;
			}
		}

		public void SetImage(UIImage image)
		{
			this._imageView.Image = image;
		}

		// Rener added
		public void setUploadState(bool isUploaded)
		{
			if (isUploaded)
			{
				indicator.StopAnimating();
				loadingView.Hidden = true;
			}
			else {
				indicator.StartAnimating();
				loadingView.Hidden = false;
			}
		}
		// end

		#endregion

		private UIImageView CreateImageView(UIImage bubbleImage)
		{

			var res = new UIImageView(new CGRect(0, 0, LoadingImageWidth, ImageHeight)) {
				Image = LoadingImage,
				ContentMode = UIViewContentMode.ScaleAspectFill,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			var mask = ImageHelper.Mask(bubbleImage, res);
			res.Layer.Mask = mask;
			addLoadingView(); // Rener addedd
			res.AddSubview(loadingView); // Rener addede
			res.AddSubview(this.MessageView);
			return res;
		}
	}
}

