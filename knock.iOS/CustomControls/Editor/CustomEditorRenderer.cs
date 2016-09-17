using System;
using Xamarin.Forms;
using knock;
using knock.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace knock.iOS
{
    public class CustomEditorRenderer : FixedExtendedEditorRenderer
    {
		
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            var element = this.Element as CustomEditor;
            if (element == null)
                return;
			element.FocusedEvent+= delegate {
				this.Control.BecomeFirstResponder();
			};
			this.Control.Started += (object sender, EventArgs e1) => {
				element.OnWritingStarted(new EventArgs());
			};
			if (element.BorderWidth > 0) 
            {
                this.Control.Layer.BorderWidth = element.BorderWidth;
                this.Control.Layer.BorderColor = element.BorderColor.ToCGColor();
            }

            if (element.CornerRadius > 0)
            {
                this.Control.Layer.CornerRadius = element.CornerRadius;
            }
			this.Control.InputAccessoryView = null;
        }
    }
}

