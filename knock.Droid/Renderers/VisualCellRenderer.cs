using System;

using Android.Views;

using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(knock.Visual1993Cell), typeof(knock.Droid.Renderers.VisualCellRenderer))]

namespace knock.Droid.Renderers
{
    internal sealed class NativeCell : ViewGroup
    {

        public NativeCell(Android.Content.Context context, knock.Visual1993Cell fastCell) : base(context)
        {
            FastCell = fastCell;
            fastCell.PrepareCell();
            var renderer = RendererFactory.GetRenderer(fastCell.View);
            this.AddView(renderer.ViewGroup);
            //			_view = renderer.NativeView;
            //			ContentView.AddSubview (_view);
        }

        public knock.Visual1993Cell FastCell
        {
            get;
            set;
        }

        Size _lastSize;

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            if (changed)
            {
                //TODO
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            //TODO update sizes of the xamarin view
            var newSize = new Size(w, h);
            if (_lastSize.Equals(Size.Zero) || !_lastSize.Equals(newSize))
            {

                //				var layout = FastCell.Content;
                var layout = FastCell.View as Layout<Xamarin.Forms.View>;
                if (layout != null)
                {
                    layout.Layout(new Rectangle(0, 0, w, h));
                    layout.ForceLayout();
                    FixChildLayouts(layout);
                }
                _lastSize = newSize;
            }

            //TODO set the frame size
        }

        void FixChildLayouts(Layout<Xamarin.Forms.View> layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is StackLayout)
                {
                    ((StackLayout)child).ForceLayout();
                    FixChildLayouts(child as Layout<Xamarin.Forms.View>);
                }
                if (child is Xamarin.Forms.AbsoluteLayout)
                {
                    ((Xamarin.Forms.AbsoluteLayout)child).ForceLayout();
                    FixChildLayouts(child as Layout<Xamarin.Forms.View>);
                }
            }
        }
    }

    public class VisualCellRenderer : ViewCellRenderer
    {
        //TODO add a lookup for the cells we piggy back of.
        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
        {
            var cellCache = knock.Droid.Renderers.FastCellCache.Instance.GetCellCache(parent);
            var fastCell = item as knock.Visual1993Cell;

            Android.Views.View cellCore = convertView;

            if (cellCore != null && cellCache.IsCached(cellCore))
            {
                cellCache.RecycleCell(cellCore, fastCell);
            }
            else
            {
                //NON c'è una recycle, creala nuova
                var newCell = (knock.Visual1993Cell)Activator.CreateInstance(item.GetType());
                newCell.BindingContext = item.BindingContext;
                newCell.Parent = item.Parent;

                if (!newCell.IsInitialized)
                {
                    newCell.PrepareCell();
                }
                try
                {
                    cellCore = base.GetCellCore(newCell, convertView, parent, context);
                }
                catch (Exception ex) { Debug.WriteLine(ex.Message + Environment.NewLine); }
                //Debug.WriteLine("arrivo dopo la GetCellCore, prima della cachecell" + Environment.NewLine);
                cellCache.CacheCell(newCell, cellCore);
            }
            return cellCore;
        }


    }
    public class FastCellCache : IFastCellCache
    {
        static FastCellCache _instance;

        FastCellCache()
        {
            _cachedDataByView = new Dictionary<Android.Views.View, CachedData>();
        }

        public static FastCellCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FastCellCache();
                }
                return _instance;
            }
        }

        Dictionary<Android.Views.View, CachedData> _cachedDataByView;

        public CachedData GetCellCache(ViewGroup parent)
        {
            if (!_cachedDataByView.ContainsKey(parent))
            {
                _cachedDataByView[parent] = new CachedData();
            }
            return _cachedDataByView[parent];
        }

        #region IFastCellCache impl

        //TODO maintain mapping of cell to original?

        public void FlushAllCaches()
        {
            foreach (var cachedData in _cachedDataByView.Values)
            {
                cachedData.Reset();
            }

            _cachedDataByView = new Dictionary<Android.Views.View, CachedData>();
        }

        #endregion

        public class CachedData
        {
            internal CachedData()
            {
                Reset();
            }

            /// <summary>
            /// Reset this instance. 
            /// </summary>
            internal void Reset()
            {
                CellItemsByCoreCells = new Dictionary<Android.Views.View, Visual1993Cell>();
                OriginalBindingContextsForReusedItems = new Dictionary<Visual1993Cell, object>();
            }

            Dictionary<Android.Views.View, Visual1993Cell> CellItemsByCoreCells { get; set; }

            Dictionary<Visual1993Cell, object> OriginalBindingContextsForReusedItems { get; set; }

            public void RecycleCell(Android.Views.View view, Visual1993Cell newCell)
            {
                if (CellItemsByCoreCells.ContainsKey(view))
                {
                    var reusedItem = CellItemsByCoreCells[view];
                    if (OriginalBindingContextsForReusedItems.ContainsKey(newCell))
                    {
                        reusedItem.BindingContext = OriginalBindingContextsForReusedItems[newCell];
                    }
                    else
                    {
                        reusedItem.BindingContext = newCell.BindingContext;
                    }
                }
            }

            public bool IsCached(Android.Views.View view)
            {
                return CellItemsByCoreCells.ContainsKey(view);
            }

            public void CacheCell(Visual1993Cell cell, Android.Views.View view)
            {
                CellItemsByCoreCells[view] = cell;
                OriginalBindingContextsForReusedItems[cell] = cell.BindingContext;
            }

            public object GetBindingContextForReusedCell(Visual1993Cell cell)
            {
                if (OriginalBindingContextsForReusedItems.ContainsKey(cell))
                {
                    return OriginalBindingContextsForReusedItems[cell];
                }
                else
                {
                    return null;
                }
            }

            void CacheBindingContextForReusedCell(Visual1993Cell cell)
            {
                OriginalBindingContextsForReusedItems[cell] = cell.BindingContext;
            }




        }
    }
}
