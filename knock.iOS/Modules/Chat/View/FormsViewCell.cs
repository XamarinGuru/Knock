using System;
using UIKit;
using Visual1993.Extensions;

namespace Xamarin.Forms.Chat.iOS
{
	public class FormsViewCell: BubbleTableViewCell
	{
		public FormsViewCell(MessageCell cell) : base(cell)
		{
		}
		private UIView _view;

		private static readonly UIImage IncomingBubbleImageStatic;
		private static readonly UIImage OutgoingBubbleImageStatic;

		static FormsViewCell()
		{
			var color = UIColor.FromRGB(65, 58, 74);
			var mask = UIImage.FromBundle("BubbleIncoming");
			IncomingBubbleImageStatic = ImageHelper.CreateBubbleImage(mask, color);

			mask = UIImage.FromBundle("BubbleOutgoing");
			color = UIColor.FromRGB(167, 34, 141);
			OutgoingBubbleImageStatic = ImageHelper.CreateBubbleImage(mask, color);
		}

		#region IBubbleCellContentDrawer implementation

		protected override UIKit.UIView CreateSubview(MessageViewModel viewModel)
		{
			//Here the cell content has to be implemented
			var label = new UILabel
			{
				TranslatesAutoresizingMaskIntoConstraints = false,
				//TextAlignment=UITextAlignment.Left, //this is useless if the view is centered
				Lines = 0,
				PreferredMaxLayoutWidth = 220f,
				TextColor = UIColor.White,
				Font = UIFont.FromName("Avenir Next Condensed", 15),
				Text = viewModel.Content
			};
			var viewForms = new knock.AudioCellView();
			viewForms.label.Text = viewModel.Content;
			var view = FormsViewToNativeiOS.ConvertFormsToNative(viewForms, new CoreGraphics.CGRect(10, 10, 300, 30));

			view.TranslatesAutoresizingMaskIntoConstraints = false;
			this._view = view;
			return this._view;
		}

		protected override UIImageView IncomingBubbleImageView { get { return null; } }
		protected override UIImageView OutgoingBubbleImageView { get { return null; } }

		protected override UIImage IncomingBubbleImage
		{
			get
			{
				return IncomingBubbleImageStatic;
			}
		}

		protected override UIImage OutgoingBubbleImage
		{
			get
			{
				return OutgoingBubbleImageStatic;
			}
		}

		protected override void UpdateConstraintsForIncomingCell(UIImageView bubbleImageView)
		{
			bubbleImageView.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:[bubble(>=48)]",
				(NSLayoutFormatOptions)0,
				"bubble", bubbleImageView
			));

			this.ContentView.AddConstraints(new[] {
				NSLayoutConstraint.Create (this._view, NSLayoutAttribute.Top, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Top, 1, 18),
				NSLayoutConstraint.Create (this._view, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Bottom, 1, -20),
				NSLayoutConstraint.Create (this._view, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 16),
				NSLayoutConstraint.Create (this._view, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.CenterX, 1, 3),

				NSLayoutConstraint.Create (this.DateView, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 16),
				NSLayoutConstraint.Create (this.DateView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Bottom, 1, -6),

				NSLayoutConstraint.Create (this.AuthorView, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 16),
				NSLayoutConstraint.Create (this.AuthorView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Top, 1, 6),

				NSLayoutConstraint.Create (bubbleImageView, NSLayoutAttribute.Trailing, NSLayoutRelation.GreaterThanOrEqual, this.AuthorView, NSLayoutAttribute.Trailing, 1, 10),
			});
		}

		protected override void UpdateConstraintsForOutgoingCell(UIImageView bubbleImageView)
		{
			bubbleImageView.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:[bubble(>=48)]",
				(NSLayoutFormatOptions)0,
				"bubble", bubbleImageView
			));
			
			this.ContentView.AddConstraints(new[] {
				NSLayoutConstraint.Create (this._view, NSLayoutAttribute.Top, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Top, 1, 30),
				NSLayoutConstraint.Create (this._view, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Bottom, 1, -20),
				NSLayoutConstraint.Create (this._view, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 16),
				NSLayoutConstraint.Create (this._view, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.CenterX, 1, 3),

				NSLayoutConstraint.Create (this.DateView, NSLayoutAttribute.Leading, NSLayoutRelation.LessThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 10),
				NSLayoutConstraint.Create (this.DateView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Bottom, 1, -6),

				NSLayoutConstraint.Create (this.AuthorView, NSLayoutAttribute.Leading, NSLayoutRelation.LessThanOrEqual, bubbleImageView, NSLayoutAttribute.Leading, 1, 10),
				NSLayoutConstraint.Create (this.AuthorView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.Top, 1, 6),
				NSLayoutConstraint.Create (this.AuthorView, NSLayoutAttribute.Trailing, NSLayoutRelation.LessThanOrEqual, bubbleImageView, NSLayoutAttribute.Trailing, 1, -16),
				NSLayoutConstraint.Create (this.AuthorView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, bubbleImageView, NSLayoutAttribute.CenterX, 1, -3)
			});
		}


		#endregion
	}
}


