using System;

using UIKit;
using CoreGraphics;
using Foundation;

namespace Xamarin.Forms.Chat.iOS
{
	public abstract class BubbleTableViewCell : UITableViewCell
	{
		private MessageCell _cell;

		public BubbleTableViewCell (MessageCell cell) : base()
		{
			this._cell = cell;
			this.Initialize();
		}

		abstract protected UIView CreateSubview(MessageViewModel viewModel);
		abstract protected UIImageView OutgoingBubbleImageView { get; }
		abstract protected UIImageView IncomingBubbleImageView { get; }
		abstract protected UIImage IncomingBubbleImage { get; }
		abstract protected UIImage OutgoingBubbleImage { get; }
		abstract protected void UpdateConstraintsForIncomingCell(UIImageView bubbleImageView);
		abstract protected void UpdateConstraintsForOutgoingCell(UIImageView bubbleImageView);

		protected UILabel AuthorView { get; private set; }
		protected UILabel DateView { get; private set; }
		protected UILabel MessageView { get; private set; }

		public virtual void Initialize()
		{
			var viewModel = this._cell.ViewModel;

			var subview = this.CreateSubview(viewModel);

			var bubbleImageView = viewModel.IsOutgoing 
			                               ? this.OutgoingBubbleImageView 
			                               : this.IncomingBubbleImageView;

			this.AuthorView = new UILabel()
			{
				TranslatesAutoresizingMaskIntoConstraints = false,
				TextColor = UIColor.LightGray,
				Font = UIFont.SystemFontOfSize(9),
				Text = viewModel.Author
			};

			this.DateView = new UILabel()
			{
				TranslatesAutoresizingMaskIntoConstraints = false,
				TextColor = UIColor.LightGray,
				Font = UIFont.SystemFontOfSize(9),
				Text = viewModel.DateStr
			};

			if (bubbleImageView == null)
			{
				bubbleImageView = new UIImageView {
					TranslatesAutoresizingMaskIntoConstraints = false
				};
			}
			/*if (viewModel.Type != MessageType.Text)
			{
				ContentView.AddSubviews(bubbleImageView, this.AuthorView, subview, this.MessageView, this.DateView);
			}
			else { 
				ContentView.AddSubviews(bubbleImageView, this.AuthorView, subview, this.MessageView, this.DateView);
			}
			*/
			ContentView.AddSubviews(bubbleImageView, this.AuthorView, subview, this.DateView);

			bubbleImageView.UserInteractionEnabled = false;

			var padding = this._cell.Padding;

			if (viewModel.IsOutgoing)
			{
				if (bubbleImageView.Image == null)
				{
					bubbleImageView.Image = this.OutgoingBubbleImage;
				}
				this.SetupConstraints(string.Format("H:|-(>={0})-[bubble]-{1}-|", padding.Right, padding.Left), bubbleImageView);
				this.UpdateConstraintsForOutgoingCell(bubbleImageView);
			}
			else
			{
				if (bubbleImageView.Image == null)
				{
					bubbleImageView.Image = this.IncomingBubbleImage;
				}
				this.SetupConstraints(string.Format("H:|-{0}-[bubble]-(>={1})-|", padding.Left, padding.Right), bubbleImageView);
				this.UpdateConstraintsForIncomingCell(bubbleImageView);
			}
		}

		private void SetupConstraints(string horizontalBubbleConstraintFormat, UIView bubbleImageView)
		{
			this.ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat (horizontalBubbleConstraintFormat,
				(NSLayoutFormatOptions)0, 
				"bubble", bubbleImageView
			));

			var padding = this._cell.Padding;

			this.ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat (string.Format("V:|-{0}-[bubble]-{1}-|", padding.Top, padding.Bottom),
				(NSLayoutFormatOptions)0,
				"bubble", bubbleImageView
			));
		}
	}
}