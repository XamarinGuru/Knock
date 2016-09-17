
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using knock;


[assembly: ExportRenderer (typeof(BubbleCell), typeof(knock.Droid.BubbleCellRenderer))]
namespace knock.Droid
{	
	public class BubbleCellRenderer : ViewCellRenderer
	{
		
		protected override Android.Views.View GetCellCore (Xamarin.Forms.Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
		{
			var x = (BubbleCell)item;

			var view = convertView;

			if (view == null) {
				// no view to re-use, create new
				view = (context as Activity).LayoutInflater.Inflate (Resource.Layout.BubbleCell, null);
			}

			view.FindViewById<TextView> (Resource.Id.txtInfo).Text = x.DateTime;
			view.FindViewById<TextView> (Resource.Id.txtMessage).Text = x.Text;

			// grab the old image and dispose of it
		/*
			if (view.FindViewById<ImageView> (Resource.Id.Image).Drawable != null) {
				using (var image = view.FindViewById<ImageView> (Resource.Id.Image).Drawable as BitmapDrawable) {
					if (image != null) {
						if (image.Bitmap != null) {
							image.Bitmap.Dispose ();
						}
					}
				}
			}

			// If a new image is required, display it
			if (!String.IsNullOrWhiteSpace (x.ImageFilename)) {
				context.Resources.GetBitmapAsync (x.ImageFilename).ContinueWith ((t) => {
					var bitmap = t.Result;
					if (bitmap != null) {
						view.FindViewById<ImageView> (Resource.Id.Image).SetImageBitmap (bitmap);
						bitmap.Dispose ();
					}
				}, TaskScheduler.FromCurrentSynchronizationContext ());

			} else {
				// clear the image
				view.FindViewById<ImageView> (Resource.Id.Image).SetImageBitmap (null);
			}
		*/
			return view;
		}
	}
}


