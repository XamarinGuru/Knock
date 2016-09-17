using Xamarin.Forms.Platform.iOS;
using UIKit;
using Xamarin.Forms;
using knock;
using knock.iOS;

[assembly:ExportRenderer(typeof(ItemContentView), typeof(ItemContainerViewRenderer))]
namespace knock
{
    public class ItemContainerViewRenderer : ViewRenderer<ItemContentView, UIView>
    {
        private UITapGestureRecognizer _tapGesture;
        private UILongPressGestureRecognizer _longPressGesture;

        protected override void OnElementChanged(ElementChangedEventArgs<ItemContentView> e)
        {
            if (this.NativeView != null)
            {
                if (this._tapGesture != null)
                    this.NativeView.RemoveGestureRecognizer(this._tapGesture);
                if (this._longPressGesture != null)
                    this.NativeView.RemoveGestureRecognizer(this._longPressGesture);
            }

            base.OnElementChanged(e);

            if (this.Element != null)
            {
                _tapGesture = new UITapGestureRecognizer(() =>
                    {
                        this.Element.SendTapped();
                    });
                _longPressGesture = new UILongPressGestureRecognizer(() =>
                    {
                        this.Element.SendLongPressed();
                    });


                this.NativeView.AddGestureRecognizer(this._tapGesture);
                this.NativeView.AddGestureRecognizer(this._longPressGesture);
            }
        }
    }
}

