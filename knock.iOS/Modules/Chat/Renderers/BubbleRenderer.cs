using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.Chat.iOS;
using Xamarin.Forms;
using Foundation;
using Xamarin.Forms.Chat;
using UIKit;
using CoreGraphics;

[assembly: ExportRenderer (typeof(MessageCell), typeof(BubbleRenderer))]
namespace Xamarin.Forms.Chat.iOS
{
    public class BubbleRenderer : ViewCellRenderer
    {

        public override UIKit.UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var c = (item as MessageCell);
            var viewModel = c.ViewModel;

            UITableViewCell cell = null;
			/*if (viewModel.Type == MessageType.Audio)
			{
				cell = new FormsViewCell(c);
				c.Height = ImageTableViewCell.ImageHeight + c.Padding.Top;
			}*/
			if (viewModel.Type == MessageType.Text)
            {
                cell = new TextTableViewCell(c);
               	c.Height = this.CalculateHeightFor(cell, tv);
            }
            else
            {
                var imageCell = new ImageTableViewCell(c);
                cell = imageCell;
                c.Height = ImageTableViewCell.ImageHeight + c.Padding.Top;
                this.LoadImage(imageCell, viewModel);

 				// Rener addedd
				imageCell.setUploadState(viewModel.isUploaded);
				imageCell.MessageView.Text = viewModel.Content;

				viewModel.UpdateCell += delegate {
					this.LoadImage(imageCell, viewModel);
					imageCell.setUploadState(viewModel.isUploaded);
					imageCell.MessageView.Text = viewModel.Content;
				};
				// end
            }
            return cell;
        }

        private async void LoadImage(ImageTableViewCell cell, MessageViewModel viewModel)
        {
            if (viewModel.ThumbnailImageSource == null)
                return;
            
			// Rener changed
			IImageSourceHandler imageSourceHandler; // var imageSourceHandler = new ImageLoaderSourceHandler();
			if (viewModel.ThumbnailImageSource is UriImageSource)
			{
				imageSourceHandler = new ImageLoaderSourceHandler();
			}
			else {
				imageSourceHandler = new FileImageSourceHandler();
			}
			// end

			try{
            var img = await imageSourceHandler.LoadImageAsync(viewModel.ThumbnailImageSource);
            cell.SetImage(img);
			}
			catch (Exception ex) {
				Insights.Report (ex);
				return;
			}
        }

        private double CalculateHeightFor (UITableViewCell cell, UITableView tableView)
        {
            cell.SetNeedsLayout ();
            cell.LayoutIfNeeded ();
            var size = cell.ContentView.SystemLayoutSizeFittingSize (UIView.UILayoutFittingCompressedSize);
            return NMath.Ceiling (size.Height) + 1;
        }
    }
}

