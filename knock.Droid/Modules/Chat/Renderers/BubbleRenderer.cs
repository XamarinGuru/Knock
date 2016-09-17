
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Chat;
using Xamarin.Forms.Chat.Droid;
using Android.App;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Content;
using Android.Graphics;
using System.Reflection;
using System.IO;
using System.Diagnostics;

[assembly: ExportRenderer (typeof(MessageCell), typeof(BubbleRenderer))]
namespace Xamarin.Forms.Chat.Droid
{
	public class BubbleRenderer : ViewCellRenderer
	{
		private enum ViewType
		{
			MsgOutgoing,
			MsgIncoming,
			ImgOutgoing,
			ImgIncoming,
			ThumbOutgoing,
			ThumbIncoming,
			AudioIncoming,
			AudioOutgoing,
			ServiceIncoming,
			ServiceOutgoing
		}

		protected override global::Android.Views.View GetCellCore (Xamarin.Forms.Cell item, global::Android.Views.View convertView, ViewGroup parent, Context context)
		{
			var msgVm = ((MessageCell)item).ViewModel;

			var view = convertView as HolderView;

			if (view != null)
			{
				switch (this.GetViewType(msgVm))
				{
					case ViewType.AudioIncoming:
						view = view as IncomingAudioHolderView;
						break;
					case ViewType.AudioOutgoing:
						view =view as OutgoingAudioHolderView;
						break;
					case ViewType.ServiceIncoming:
						view = view as IncomingServiceHolderView;
						break;
					case ViewType.ServiceOutgoing:
						view = view as OutgoingServiceHolderView;
						break;
				case ViewType.ThumbIncoming:
					view = view as IncomingThumbHolderView;
					break;
				case ViewType.ThumbOutgoing:
					view = view as OutgoingThumbHolderView;
					break;
				case ViewType.ImgIncoming:
					view = view as IncomingImageHolderView;
					break;
				case ViewType.ImgOutgoing:
					view = view as OutgoingImageHolderView;
					break;
				case ViewType.MsgIncoming:
					view = view as IncomingTextHolderView;
					break;
				case ViewType.MsgOutgoing:
					view = view as OutgoingTextHolderView;
					break;
				}
			}

			if (view == null)
			{
				switch (this.GetViewType(msgVm))
				{
					case ViewType.AudioIncoming:
						view = new IncomingAudioHolderView();
						break;
					case ViewType.AudioOutgoing:
						view = new OutgoingAudioHolderView();
						break;
					case ViewType.ServiceIncoming:
						view = new IncomingServiceHolderView();
						break;
					case ViewType.ServiceOutgoing:
						view = new OutgoingServiceHolderView();
						break;
				case ViewType.ThumbIncoming:
					view = new IncomingThumbHolderView();
					break;
				case ViewType.ThumbOutgoing:
					view = new OutgoingThumbHolderView();
					break;
				case ViewType.ImgIncoming:
					view = new IncomingImageHolderView();
					break;
				case ViewType.ImgOutgoing:
					view = new OutgoingImageHolderView();
					break;
				case ViewType.MsgIncoming:
					view = new IncomingTextHolderView();
					break;
				case ViewType.MsgOutgoing:
					view = new OutgoingTextHolderView();
					break;
				}
			}
			if (view == null)
			{
				return null;
			}
			view.Bind(msgVm);

			return view;
		}

		private ViewType GetViewType(MessageViewModel msg)
		{
			if (msg.Type == MessageType.Service)
			{
				return msg.IsOutgoing ? ViewType.ServiceOutgoing: ViewType.ServiceIncoming;
			}
			if (msg.Type == MessageType.Audio)
			{
				return msg.IsOutgoing ? ViewType.AudioOutgoing : ViewType.AudioIncoming;
			}
			if (msg.Type == MessageType.Location)
			{
				return msg.IsOutgoing ? ViewType.ImgOutgoing : ViewType.ImgIncoming;
			}
			if (msg.Type == MessageType.Text)
			{
				return msg.IsOutgoing ? ViewType.MsgOutgoing : ViewType.MsgIncoming;
			}
			return msg.IsOutgoing ? ViewType.ImgOutgoing : ViewType.ImgIncoming;
		}
	}
}
