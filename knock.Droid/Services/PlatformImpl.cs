using System;
using knock.Droid;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency (typeof (PlatformImpl))]
namespace knock.Droid
{
    public class PlatformImpl : CurrentPlatform
    {
        public override float PixelDensity
        {
            get
            {
                return Forms.Context.Resources.DisplayMetrics.Density;
            }
        }
    }
}

