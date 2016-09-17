using XLabs.Forms.Controls;
using Xamarin.Forms;
using knock.iOS;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly:ExportRenderer(typeof(ExtendedEditor), typeof(FixedExtendedEditorRenderer))]
namespace knock.iOS
{
    public class FixedExtendedEditorRenderer : ExtendedEditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            var element = this.Element;
            if (element != null)
            {
                if (element.FontFamily != null)
			        this.Control.Font = UIFont.FromName(element.FontFamily, (float)element.FontSize);
            }
        }
    }
}

