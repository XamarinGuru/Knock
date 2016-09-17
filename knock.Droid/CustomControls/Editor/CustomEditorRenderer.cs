using System;
using XLabs.Forms.Controls;
using Xamarin.Forms;
using knock;
using knock.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Content.Res;

[assembly:ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace knock.Droid
{
    public class CustomEditorRenderer : ExtendedEditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            var element = this.Element as CustomEditor;
            if (element == null)
                return;

            this.MakeRoundCorners();
        }
    }
}

