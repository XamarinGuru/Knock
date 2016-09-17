using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Chat.iOS;
using System.Diagnostics;

[assembly: ExportRenderer (typeof(Page), typeof(KeyboardOverlapRenderer))]
namespace Xamarin.Forms.Chat.iOS
{
    public class KeyboardOverlapRenderer : PageRenderer
    {
        public static void Init ()
        {
            var now = DateTime.Now;
            Debug.WriteLine ("Keyboard Overlap plugin initialized {0}", now);
        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);

            AddKeyboardObserving (this.View);
        }

        public override void ViewWillDisappear (bool animated)
        {
            base.ViewWillDisappear (animated);

            RemoveKeyboardObserving(this.View);
        }

        private static Dictionary<UIView, IEnumerable<IDisposable>> KeyboardObservers = new Dictionary<UIView, IEnumerable<IDisposable>>();

        public static void RemoveKeyboardObserving(UIView view)
        {
            if (!KeyboardObservers.ContainsKey(view))
                return;

            foreach (var observer in KeyboardObservers[view])
                observer.Dispose();
            KeyboardObservers.Remove(view);
        }

        public static void AddKeyboardObserving(UIView view, Action onShow = null, Action onHide = null)
        {
            if (KeyboardObservers.ContainsKey(view))
                return;

            CGRect initialFrame = view.Frame;
            Action<bool, UIKeyboardEventArgs> scrollTheView = (bool show, UIKeyboardEventArgs e) =>
                {
                    var animationDuration = e.AnimationDuration;
                    var animationCurve = e.AnimationCurve;
                    if (show)
                    {
                        initialFrame = view.Frame;
                        UIView.BeginAnimations(string.Empty, IntPtr.Zero);
                        UIView.SetAnimationDuration(animationDuration);
                        UIView.SetAnimationCurve(animationCurve);
                    }

                    var frame = show ? view.Frame : initialFrame;
                    var height = show ? (e.FrameEnd.Y - frame.Y) : frame.Height;
                    view.Frame = new CoreGraphics.CGRect(frame.X, frame.Y, frame.Width, height);

                    if (show)
                        UIView.CommitAnimations();
                };


            var willShow = UIKeyboard.Notifications.ObserveWillShow((object sender, UIKeyboardEventArgs e) =>
                {
                    scrollTheView(true, e);
                    if (onShow != null)
                        onShow();
                });
            var willHide = UIKeyboard.Notifications.ObserveWillHide((object sender, UIKeyboardEventArgs e) =>
                {
                    scrollTheView(false, e);
                    if (onHide != null)
                        onHide();
                });
            KeyboardObservers[view] = new [] { willShow, willHide };
        }
    }
}

