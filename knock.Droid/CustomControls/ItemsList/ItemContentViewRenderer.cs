using System;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using knock;

[assembly:ExportRenderer(typeof(ItemContentView), typeof(ItemContentViewRenderer))]
namespace knock
{
    public class ItemContentViewRenderer : ViewRenderer<ItemContentView, Android.Views.View>
    {
        private class GestureListener : GestureDetector.SimpleOnGestureListener
        {
            private readonly ItemContentView _contentView;

            public GestureListener(ItemContentView contentView)
            {
                this._contentView = contentView;
            }

            public override void OnLongPress (MotionEvent e)
            {
                Console.WriteLine ("OnLongPress");
                base.OnLongPress (e);
                this._contentView.SendLongPressed();
            }

            public override bool OnDoubleTap (MotionEvent e)
            {
                Console.WriteLine ("OnDoubleTap");
                return base.OnDoubleTap (e);
            }

            public override bool OnDoubleTapEvent (MotionEvent e)
            {
                Console.WriteLine ("OnDoubleTapEvent");
                return base.OnDoubleTapEvent (e);
            }

            public override bool OnSingleTapUp (MotionEvent e)
            {
                Console.WriteLine ("OnSingleTapUp");
                var res = base.OnSingleTapUp (e);
                this._contentView.SendTapped();
                return res;
            }

            public override bool OnDown (MotionEvent e)
            {
                Console.WriteLine ("OnDown");
                return base.OnDown (e);
            }

            public override bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                Console.WriteLine ("OnFling");
                return base.OnFling (e1, e2, velocityX, velocityY);
            }

            public override bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                Console.WriteLine ("OnScroll");
                return base.OnScroll (e1, e2, distanceX, distanceY);
            }

            public override void OnShowPress (MotionEvent e)
            {
                Console.WriteLine ("OnShowPress");
                base.OnShowPress (e);
            }

            public override bool OnSingleTapConfirmed (MotionEvent e)
            {
                Console.WriteLine ("OnSingleTapConfirmed");
                return base.OnSingleTapConfirmed (e);
            }
        }

        private GestureListener _listener;
        private GestureDetector _detector;

        protected override void OnElementChanged(ElementChangedEventArgs<ItemContentView> e)
        {
            base.OnElementChanged (e);

            if (e.NewElement == null) {
                this.GenericMotion -= HandleGenericMotion;
                this.Touch -= HandleTouch;
                this._detector.Dispose();
                this._listener.Dispose();
            }

            if (this.Element != null) {
                this.GenericMotion += HandleGenericMotion;
                this.Touch += HandleTouch;
                _listener = new GestureListener (this.Element);
                _detector = new GestureDetector (_listener);
            }
        }

        private void HandleTouch (object sender, TouchEventArgs e)
        {
            _detector.OnTouchEvent (e.Event);
        }

        private void HandleGenericMotion (object sender, GenericMotionEventArgs e)
        {
            _detector.OnTouchEvent (e.Event);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.GenericMotion -= HandleGenericMotion;
                this.Touch -= HandleTouch;
                this._detector.Dispose();
                this._listener.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

