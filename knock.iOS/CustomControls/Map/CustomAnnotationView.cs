using System;
using MapKit;
using System.Linq;
using UIKit;

namespace knock.iOS
{
    public class CustomAnnotationView : MKAnnotationView
    {
        public CustomAnnotationView(IMKAnnotation annotation, string reuseIdentifier)
            : base(annotation, reuseIdentifier)
        {
        }

        public override void SubviewAdded(UIView uiview)
        {
            base.SubviewAdded(uiview);
            this.SetNeedsLayout();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            foreach (var label in this.AllSubviews().OfType<UILabel>())
                label.Font = UIFont.FromName(Styles.FontFamily, label.Font.PointSize - 1);
        }
    }
}

