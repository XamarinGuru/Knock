using knock;
using Xamarin.Forms;
using knock.iOS;
using DevExpress.Mobile.DataGrid.Internal;
using DevExpress.Mobile.DataGrid.Android;
using Android.Widget;
using Android.Graphics.Drawables;
using Xamarin.Forms.Platform.Android;
using knock.Droid;

[assembly:ExportRenderer(typeof(RowContainer), typeof(KnockRowContainerRenderer))]
namespace knock.iOS
{
    public class KnockRowContainerRenderer : RowContainerRenderer
    {
        private int _spacesSize = 10;
        private Android.Graphics.Color _spacerColor = Tema.coloreSfondoScuro.ToAndroid();

        private Android.Views.View _bottomCellSpacer;
        private Android.Views.View BottomCellSpacer
        {
            get
            {
                if (this._bottomCellSpacer != null)
                    return this._bottomCellSpacer;

                this._bottomCellSpacer = new Android.Views.View(this.Context);
                this._bottomCellSpacer.SetBackgroundColor(this._spacerColor);
                this._bottomCellSpacer.LayoutParameters = 
                    new FrameLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                return this._bottomCellSpacer;
            }
        }

        private Android.Views.View _topCellSpacer;
        private Android.Views.View TopCellSpacer
        {
            get
            {
                if (this._topCellSpacer != null)
                    return this._topCellSpacer;

                this._topCellSpacer = new Android.Views.View(this.Context);
                this._topCellSpacer.SetBackgroundColor(this._spacerColor);
                this._topCellSpacer.LayoutParameters = 
                    new FrameLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                return this._topCellSpacer;
            }
        }

        private void UpdateSpacers(int t, int r, int b)
        {
            this.BottomCellSpacer.RemoveFromParent();
            this.TopCellSpacer.RemoveFromParent();
            var height = (int)(this._spacesSize / 2 * this.Context.Scale());
            this.BottomCellSpacer.Layout(0, b - height - t, r, b - t);
            this.AddView(this.BottomCellSpacer);
            this.TopCellSpacer.Layout(0, 0, r, height);
            this.AddView(this.TopCellSpacer);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            this.UpdateSpacers(t, r, b);
        }
    }
}

