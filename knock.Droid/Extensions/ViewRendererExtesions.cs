using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Widget;
using System.Reflection;
using Android.Runtime;
using Android.Content;

namespace knock.Droid
{
    public static class ViewRendererExtesions
    {
        public static float Scale(this Context context)
        {
            return context.Resources.DisplayMetrics.Density;
        }

        public static void MakeRoundCorners<TView, TNativeView>(this ViewRenderer<TView, TNativeView> renderer) 
            where TView : View where TNativeView : Android.Views.View
        {
            var element = renderer.Element as IRoundedCorners;
            if (element == null)
                throw new ArgumentException("Element must implement " + nameof(IRoundedCorners));

            var viewGroup = renderer.ViewGroup;

            var background = (ColorDrawable)viewGroup.Background;
            if (background != null)
                background.Color = Color.Transparent.ToAndroid();
            
            var drawable = new GradientDrawable();

            drawable.SetColor(renderer.Element.BackgroundColor.ToAndroid());
            drawable.SetCornerRadius(element.CornerRadius);

            var width = (int)Math.Ceiling(element.BorderWidth * renderer.Context.Scale());
            drawable.SetStroke(width, element.BorderColor.ToAndroid());

            renderer.Control.Background = drawable;

            if (renderer.Control is TextView)
            {
                IntPtr IntPtrtextViewClass = JNIEnv.FindClass(typeof(TextView));
                IntPtr mCursorDrawableResProperty = 
                    JNIEnv.GetFieldID (IntPtrtextViewClass, "mCursorDrawableRes", "I");
                JNIEnv.SetField (renderer.Control.Handle, 
                    mCursorDrawableResProperty, Resource.Drawable.cursor);
            }

        }
    }
}

