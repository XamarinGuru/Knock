using System;
using DevExpress.Mobile.DataGrid.iOS;
using System.Linq;
using UIKit;
using knock;
using Xamarin.Forms;
using knock.iOS;
using DevExpress.Mobile.DataGrid.Internal;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using System.ComponentModel;
using DevExpress.Mobile.DataGrid;
using System.Threading.Tasks;

[assembly:ExportRenderer(typeof(RowContainer), typeof(KnockRowContainerRenderer))]
namespace knock.iOS
{
    public class KnockRowContainerRenderer : RowContainerRenderer
    {
        private int _spacesSize = 10;
        private UIColor _spacerColor = Tema.coloreSfondoScuro.ToUIColor();


        #region spacer properties
        private UIView _bottomCellSpacer;
        private UIView BottomCellSpacer
        {
            get
            {
                if (this._bottomCellSpacer != null)
                    return this._bottomCellSpacer;
                
                this._bottomCellSpacer = 
                    new UIView()
                    {
                        BackgroundColor = this._spacerColor
                    };
                this.AddSubview(this._bottomCellSpacer);
                return this._bottomCellSpacer;
            }
        }

        private UIView _topCellSpacer;
        private UIView TopCellSpacer
        {
            get
            {
                if (this._topCellSpacer != null)
                    return this._topCellSpacer;

                this._topCellSpacer = 
                    new UIView()
                    {
                        BackgroundColor = this._spacerColor
                    };
                this.AddSubview(this._topCellSpacer);
                return this._topCellSpacer;
            }
        }
        #endregion

        private void UpdateSpacers()
        {
            var spHeight = this._spacesSize / 2;
            //var spHeight = this._spacesSize;
            var top = this.Frame.Top;
            var height = this.Frame.Height;
            var width = this.Frame.Width;

            this.BottomCellSpacer.Frame = new CGRect(0, height - spHeight,
                width, spHeight);
            this.TopCellSpacer.Frame = new CGRect(0, 0, width, spHeight);

            this.BringSubviewToFront(this.BottomCellSpacer);
            this.BringSubviewToFront(this.TopCellSpacer);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            UpdateSpacers();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            var grid = this.Element.AllParents().OfType<GridControl>().First();
            grid.SwipeButtonShowing += Grid_SwipeButtonShowing;

        }

        void Grid_SwipeButtonShowing (object sender, SwipeButtonShowingEventArgs e)
        {
            var element = (RowContainer)this.Element;
            if (element.RowHandle == e.RowHandle)
                this.UpdateSpacers();
        }
    }
}

