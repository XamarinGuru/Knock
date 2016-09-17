using System;
using XLabs.Forms.Controls;
using Xamarin.Forms;
using knock;
using knock.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Content.Res;
using Android.Support.V4.Content;

[assembly:ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace knock.Droid
{
    public class CustomEntryRenderer : ExtendedEntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            var element = this.Element as CustomEntry;
            if (element == null)
                return;

            this.MakeRoundCorners();

            this.Control.SetTextColor(Tema.coloreSfondoScuro.ToAndroid());
        }
    }
}

