using System.Threading.Tasks;
using CoreLocation;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.iOS;
using System.Collections.Generic;

namespace knock.iOS
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convert <see cref="Position" /> to <see cref="CLLocationCoordinate2D"/>
        /// </summary>
        /// <param name="self">Self instance</param>
        /// <returns>iOS coordinate</returns>
        public static CLLocationCoordinate2D ToLocationCoordinate(this Position self)
        {
            return new CLLocationCoordinate2D(self.Latitude, self.Longitude);
        }
        /// <summary>
        /// Convert <see cref="CLLocationCoordinate2D" /> to <see cref="Position"/>
        /// </summary>
        /// <param name="self">Self instance</param>
        /// <returns>Forms position</returns>
        public static Position ToPosition(this CLLocationCoordinate2D self)
        {
            return new Position(self.Latitude, self.Longitude);
        }
        /// <summary>
        /// Converts an <see cref="ImageSource"/> to the native iOS <see cref="UIImage"/>
        /// </summary>
        /// <param name="source">Self intance</param>
        /// <returns>The UIImage</returns>
        public static async Task<UIImage> ToImage(this ImageSource source)
        {
            if (source is FileImageSource)
            {
                return await new FileImageSourceHandler().LoadImageAsync(source);
            }
            if (source is UriImageSource)
            {
                return await new ImageLoaderSourceHandler().LoadImageAsync(source);
            }
            if (source is StreamImageSource)
            {
                return await new StreamImagesourceHandler().LoadImageAsync(source);
            }
            return null;
        }

        public static IEnumerable<UIView> AllSubviews(this UIView source)
        {
            var result = new List<UIView>();
            result.Add(source);
            foreach (var sub in source.Subviews)
            {
                result.AddRange(sub.AllSubviews());   
            }
            return result;
        }
    }
}
