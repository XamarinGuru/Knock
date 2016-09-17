using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;
using knock;
using knock.Droid;
using Android.Widget;

[assembly: ExportRenderer(typeof(CustomMap), typeof(GoogleMapsRenderer))]
namespace knock.Droid
{
    /// <summary>
    /// Android Renderer of <see cref="TK.CustomMap.CustomMap"/>
    /// </summary>
    public class GoogleMapsRenderer : MapRenderer, 
    IRendererFunctions, 
    IOnMapReadyCallback, 
    GoogleMap.ISnapshotReadyCallback, 
    Android.Gms.Maps.GoogleMap.IInfoWindowAdapter
    {
        private bool _init = true;

        private readonly Dictionary<CustomMapPin, Marker> _markers = new Dictionary<CustomMapPin, Marker>();

        private Marker _selectedMarker;
        private bool _isDragging;
        private byte[] _snapShot;

        private GoogleMap _googleMap;

        private CustomMap FormsMap
        {
            get { return this.Element as CustomMap; }
        }

        /// <inheritdoc />
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            MapView mapView = this.Control as MapView;
            if (mapView == null) return;

            if(e.OldElement != null && this._googleMap != null)
            {
                e.OldElement.PropertyChanged -= FormsMapPropertyChanged;

                this._googleMap.MarkerClick -= OnMarkerClick;
                this._googleMap.MapClick -= OnMapClick;
                this._googleMap.MapLongClick -= OnMapLongClick;
                this._googleMap.MarkerDragEnd -= OnMarkerDragEnd;
                this._googleMap.MarkerDrag -= OnMarkerDrag;
                this._googleMap.CameraChange -= OnCameraChange;
                this._googleMap.MarkerDragStart -= OnMarkerDragStart;
                this._googleMap.InfoWindowClick -= OnInfoWindowClick;
                this._googleMap.MyLocationChange -= OnUserLocationChange;
                this._googleMap.SetInfoWindowAdapter(null);
            }

            if (e.NewElement != null)
            {
                ((IMapFunctions)this.FormsMap).SetRenderer(this);

                mapView.GetMapAsync(this);
                this.FormsMap.PropertyChanged += FormsMapPropertyChanged;
            }
        }
        ///<inheritdoc/>
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
			try{
            base.OnLayout(changed, l, t, r, b);

            if (this._init)
            {
                this.MoveToCenter();
                this._init = false;
            }
			}
			catch(Exception ex) {
				//maybe Google play services not installed
			}
        }
        /// <summary>
        /// When a property of the Forms map changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void FormsMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(this._googleMap == null) return;

            if (e.PropertyName == CustomMap.ItemsSourceProperty.PropertyName)
            {
                this.UpdatePins();
            }
            else if (e.PropertyName == CustomMap.SelectedItemProperty.PropertyName)
            {
                this.SetSelectedItem();
            }
            else if (e.PropertyName == CustomMap.MapCenterProperty.PropertyName)
            {
                this.MoveToCenter();
            }
            else if(e.PropertyName == CustomMap.CurrentRegionProperty.PropertyName)
            {
                this.UpdateMapRegion();
            }
        }
        /// <summary>
        /// When the map is ready to use
        /// </summary>
        /// <param name="googleMap">The map instance</param>
        public virtual void OnMapReady(GoogleMap googleMap)
        {
            this._googleMap = googleMap;

            this._googleMap.MarkerClick += OnMarkerClick;
            this._googleMap.MapClick += OnMapClick;
            this._googleMap.MapLongClick += OnMapLongClick;
            this._googleMap.MarkerDragEnd += OnMarkerDragEnd;
            this._googleMap.MarkerDrag += OnMarkerDrag;
            this._googleMap.CameraChange += OnCameraChange;
            this._googleMap.MarkerDragStart += OnMarkerDragStart;
            this._googleMap.InfoWindowClick += OnInfoWindowClick;
            this._googleMap.MyLocationChange += OnUserLocationChange;
            this._googleMap.SetInfoWindowAdapter(this);

            this.MoveToCenter();
            this.UpdatePins();
        }
        /// <summary>
        /// When the location of the user changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnUserLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            if (e.Location == null || this.FormsMap == null || this.FormsMap.UserLocationChangedCommand == null) return;

            var newPosition = new Position(e.Location.Latitude, e.Location.Longitude);

            if(this.FormsMap.UserLocationChangedCommand.CanExecute(newPosition))
            {
                this.FormsMap.UserLocationChangedCommand.Execute(newPosition);
            }
        }
        /// <summary>
        /// When the info window gets clicked
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            if (this.FormsMap.CalloutTappedCommand != null && this.FormsMap.CalloutTappedCommand.CanExecute(null))
            {
                this.FormsMap.CalloutTappedCommand.Execute(null);
            }
        }
        /// <summary>
        /// Dragging process
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnMarkerDrag(object sender, GoogleMap.MarkerDragEventArgs e)
        {
            var item = this._markers.SingleOrDefault(i => i.Value.Id.Equals(e.Marker.Id));
            if (item.Key == null) return;

            item.Key.Position = e.Marker.Position.ToPosition();
        }
        /// <summary>
        /// When a dragging starts
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnMarkerDragStart(object sender, GoogleMap.MarkerDragStartEventArgs e)
        {
            this._isDragging = true;
        }
        /// <summary>
        /// When the camera position changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnCameraChange(object sender, GoogleMap.CameraChangeEventArgs e)
        {
            if(this.FormsMap == null) return;

            this.FormsMap.MapCenter = e.Position.Target.ToPosition();
            base.OnCameraChange(e.Position);
        }
        /// <summary>
        /// When a pin gets clicked
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            if (this.FormsMap == null) return;
            var item = this._markers.SingleOrDefault(i => i.Value.Id.Equals(e.Marker.Id));
            if (item.Key == null) return;

            this._selectedMarker = e.Marker;
            this.FormsMap.SelectedItem = item.Key;
            if (item.Key.ShowCallout)
            {
                item.Value.ShowInfoWindow();
            }
        }
        /// <summary>
        /// When a drag of a marker ends
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnMarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            this._isDragging = false;

            if (this.FormsMap == null) return;

            var pin = this._markers.SingleOrDefault(i => i.Value.Id.Equals(e.Marker.Id));
            if (pin.Key == null) return;

            if (this.FormsMap.ItemDragEndCommand != null && this.FormsMap.ItemDragEndCommand.CanExecute(pin.Key))
            {
                this.FormsMap.ItemDragEndCommand.Execute(pin.Key);
            }
        }
        /// <summary>
        /// When a long click was performed on the map
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnMapLongClick(object sender, GoogleMap.MapLongClickEventArgs e)
        {
            if (this.FormsMap == null || this.FormsMap.MapLongPressedCommand == null) return;

            var position = e.Point.ToPosition();

            if (this.FormsMap.MapLongPressedCommand.CanExecute(position))
            {
                this.FormsMap.MapLongPressedCommand.Execute(position);
            }
        }
        /// <summary>
        /// When the map got tapped
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnMapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            if (this.FormsMap == null || this.FormsMap.MapTappedCommand == null) return;

            var position = e.Point.ToPosition();

            if (this.FormsMap.MapTappedCommand.CanExecute(position))
            {
                this.FormsMap.MapTappedCommand.Execute(position);
            }
        }
        /// <summary>
        /// Updates the markers when a pin gets added or removed in the collection
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (CustomMapPin pin in e.NewItems)
                {
                    this.AddPin(pin);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (CustomMapPin pin in e.OldItems)
                {
                    if (!this.FormsMap.ItemsSource.Contains(pin))
                    {
                        this.RemovePin(pin);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.UpdatePins(false);
            }
        }
        /// <summary>
        /// When a property of a pin changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private async void OnPinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var pin = sender as CustomMapPin;
            if (pin == null) return;

            var marker = this._markers[pin];
            if (marker == null) return;

            switch (e.PropertyName)
            {
                case nameof(CustomMapPin.Title):
                    marker.Title = pin.Title;
                    break;
                case nameof(CustomMapPin.Subtitle):
                    marker.Snippet = pin.Subtitle;
                    break;
                case nameof(CustomMapPin.Image):
                    await this.UpdateImage(pin, marker);
                    break;
                case nameof(CustomMapPin.DefaultColor):
                    await this.UpdateImage(pin, marker);
                    break;
                case nameof(CustomMapPin.Position):
                    if (!this._isDragging)
                    {
                        marker.Position = new LatLng(pin.Position.Latitude, pin.Position.Longitude);
                    }
                    break;
                case nameof(CustomMapPin.IsVisible):
                    marker.Visible = pin.IsVisible;
                    break;
            }
        }
        /// <summary>
        /// Creates all Markers on the map
        /// </summary>
        private void UpdatePins(bool firstUpdate = true)
        {
            if (this._googleMap == null) return;

            foreach (var i in this._markers)
            {
                this.RemovePin(i.Key, false);
            }
            this._markers.Clear();
            if (this.FormsMap.ItemsSource != null)
            {
                foreach (var pin in this.FormsMap.ItemsSource)
                {
                    this.AddPin(pin);
                }
                if (firstUpdate)
                {
                    var observAble = this.FormsMap.ItemsSource as INotifyCollectionChanged;
                    if (observAble != null)
                    {
                        observAble.CollectionChanged += OnItemsSourceCollectionChanged;
                    }
                }
                if (this.FormsMap.ItemsReadyCommand != null && this.FormsMap.ItemsReadyCommand.CanExecute(this.FormsMap))
                {
                    this.FormsMap.ItemsReadyCommand.Execute(this.FormsMap);
                }
            }
        }
        /// <summary>
        /// Adds a marker to the map
        /// </summary>
        /// <param name="pin">The Forms Pin</param>
        private async void AddPin(CustomMapPin pin)
        {
            pin.PropertyChanged += OnPinPropertyChanged;

            var markerWithIcon = new MarkerOptions();
            markerWithIcon.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));

            if (!string.IsNullOrWhiteSpace(pin.Title))
                markerWithIcon.SetTitle(pin.Title);
            if (!string.IsNullOrWhiteSpace(pin.Subtitle))
                markerWithIcon.SetSnippet(pin.Subtitle);

            await this.UpdateImage(pin, markerWithIcon);
            markerWithIcon.Draggable(pin.IsDraggable);
            markerWithIcon.Visible(pin.IsVisible);

            this._markers.Add(pin, this._googleMap.AddMarker(markerWithIcon));
        }
        /// <summary>
        /// Remove a pin from the map and the internal dictionary
        /// </summary>
        /// <param name="pin">The pin to remove</param>
        private void RemovePin(CustomMapPin pin, bool removeMarker = true)
        {
            var item = this._markers[pin];
            if(item == null) return;

            if (this._selectedMarker != null)
            {
                if (item.Id.Equals(this._selectedMarker.Id))
                {
                    this.FormsMap.SelectedItem = null;
                }
            }

            item.Remove();
            pin.PropertyChanged -= OnPinPropertyChanged;

            if (removeMarker)
            {
                this._markers.Remove(pin);
            }
        }
        /// <summary>
        /// Set the selected item on the map
        /// </summary>
        private void SetSelectedItem()
        {
            if (this._selectedMarker != null)
            {
                this._selectedMarker.HideInfoWindow();
                this._selectedMarker = null;
            }
            if (this.FormsMap.SelectedItem != null)
            {
                if (!this._markers.ContainsKey(this.FormsMap.SelectedItem)) return;

                var selectedPin = this._markers[this.FormsMap.SelectedItem];
                this._selectedMarker = selectedPin;
                if (this.FormsMap.SelectedItem.ShowCallout)
                {
                    selectedPin.ShowInfoWindow();
                }
                if (this.FormsMap.ItemSelectedCommand != null && this.FormsMap.ItemSelectedCommand.CanExecute(null))
                {
                    this.FormsMap.ItemSelectedCommand.Execute(null);
                }
            }
        }
        /// <summary>
        /// Move the google map to the map center
        /// </summary>
        private void MoveToCenter()
        {
            if (this._googleMap == null) return;

            if (!this.FormsMap.MapCenter.Equals(this._googleMap.CameraPosition.Target.ToPosition()))
            {
                var cameraUpdate = CameraUpdateFactory.NewLatLng(this.FormsMap.MapCenter.ToLatLng());

                if (this.FormsMap.IsRegionChangeAnimated && !this._init)
                {
                    this._googleMap.AnimateCamera(cameraUpdate);
                }
                else
                {
                    this._googleMap.MoveCamera(cameraUpdate);
                }
            }
        }


        /// <summary>
        /// Updates the image of a pin
        /// </summary>
        /// <param name="pin">The forms pin</param>
        /// <param name="markerOptions">The native marker options</param>
        private async Task UpdateImage(CustomMapPin pin, MarkerOptions markerOptions)
        {
            BitmapDescriptor bitmap;
            try
            {
                if (pin.Image != null)
                {
                    bitmap = BitmapDescriptorFactory.FromBitmap(await pin.Image.ToBitmap(this.Context));
                }
                else
                {
                    if (pin.DefaultColor != Color.Default)
                    {
                        var hue = pin.DefaultColor.ToAndroid().GetHue();
                        bitmap = BitmapDescriptorFactory.DefaultMarker(Math.Min(hue, 359.99f));
                    }
                    else
                    {
                        bitmap = BitmapDescriptorFactory.DefaultMarker();
                    }
                }
            }
            catch (Exception)
            {
                bitmap = BitmapDescriptorFactory.DefaultMarker();
            }
            markerOptions.SetIcon(bitmap);
        }
        /// <summary>
        /// Updates the image on a marker
        /// </summary>
        /// <param name="pin">The forms pin</param>
        /// <param name="marker">The native marker</param>
        private async Task UpdateImage(CustomMapPin pin, Marker marker)
        {
            BitmapDescriptor bitmap;
            try
            {
                if (pin.Image != null)
                {
                    bitmap = BitmapDescriptorFactory.FromBitmap(await pin.Image.ToBitmap(this.Context));
                }
                else
                {
                    if (pin.DefaultColor != Color.Default)
                    {
                        var hue = pin.DefaultColor.ToAndroid().GetHue();
                        bitmap = BitmapDescriptorFactory.DefaultMarker(hue);
                    }
                    else
                    {
                        bitmap = BitmapDescriptorFactory.DefaultMarker();
                    }
                }
            }
            catch (Exception)
            {
                bitmap = BitmapDescriptorFactory.DefaultMarker();
            }
            marker.SetIcon(bitmap);
        }
       
        /// <summary>
        /// Updates the visible map region
        /// </summary>
        private void UpdateMapRegion()
        {
            if (this.FormsMap == null) return;

            if(this.FormsMap.VisibleRegion != this.FormsMap.CurrentRegion)
            {
                this.MoveToMapRegion(this.FormsMap.CurrentRegion, this.FormsMap.IsRegionChangeAnimated);
            }
        }

        /// <summary>
        /// Creates a <see cref="LatLngBounds"/> from a collection of <see cref="MapSpan"/>
        /// </summary>
        /// <param name="spans">The spans to get calculate the bounds from</param>
        /// <returns>The bounds</returns>
        private LatLngBounds BoundsFromMapSpans(params MapSpan[] spans)
        {
            LatLngBounds.Builder builder = new LatLngBounds.Builder();

            foreach (var region in spans)
            {
                builder
                    .Include(GmsSphericalUtil.ComputeOffset(region.Center, region.Radius.Meters, 0).ToLatLng())
                    .Include(GmsSphericalUtil.ComputeOffset(region.Center, region.Radius.Meters, 90).ToLatLng())
                    .Include(GmsSphericalUtil.ComputeOffset(region.Center, region.Radius.Meters, 180).ToLatLng())
                    .Include(GmsSphericalUtil.ComputeOffset(region.Center, region.Radius.Meters, 270).ToLatLng());
            }
            return builder.Build();
        }
        /// <inheritdoc/>
        public async Task<byte[]> GetSnapshot()
        {
            if (this._googleMap == null) return null;

            this._snapShot = null;
            this._googleMap.Snapshot(this);

            while (_snapShot == null) await Task.Delay(10);

            return this._snapShot;
        }
        ///<inheritdoc/>
        public void OnSnapshotReady(Bitmap snapshot)
        {
            using(var strm = new MemoryStream())
            {
                snapshot.Compress(Bitmap.CompressFormat.Png, 100, strm);
                this._snapShot = strm.ToArray();   
            }
        }
        ///<inheritdoc/>
        public void FitMapRegionToPositions(IEnumerable<Position> positions, bool animate = false)
        {
            if (this._googleMap == null) throw new InvalidOperationException("Map not ready");
            if (positions == null) throw new InvalidOperationException("positions can't be null");

            LatLngBounds.Builder builder = new LatLngBounds.Builder();

            positions.ToList().ForEach(i => builder.Include(i.ToLatLng()));

            if(animate)
                this._googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(builder.Build(), 30));
            else
                this._googleMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(builder.Build(), 30));
        }
        ///<inheritdoc/>
        public void MoveToMapRegion(MapSpan region, bool animate)
        {
            if (this._googleMap == null) return;

            if (animate)
                this._googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(this.BoundsFromMapSpans(region), 0));
            else
                this._googleMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(this.BoundsFromMapSpans(region), 0));
        }
        ///<inheritdoc/>
        public void FitToMapRegions(IEnumerable<MapSpan> regions, bool animate)
        {
            if (this._googleMap == null) return;

            if (animate)
                this._googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(this.BoundsFromMapSpans(regions.ToArray()), 0));
            else
                this._googleMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(this.BoundsFromMapSpans(regions.ToArray()), 0));
        }
        /// <summary>
        /// Gets the <see cref="CustomMapPin"/> by the native <see cref="Marker"/>
        /// </summary>
        /// <param name="marker">The marker to search the pin for</param>
        /// <returns>The forms pin</returns>
        protected CustomMapPin GetPinByMarker(Marker marker)
        {
            return this._markers.SingleOrDefault(i => i.Value.Id == marker.Id).Key;
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            throw new NotImplementedException();
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            var inflater = (Android.Views.LayoutInflater)Context.GetSystemService(Android.Content.Context.LayoutInflaterService);
            var window = inflater.Inflate(Resource.Layout.info_window, null);

            var titleView = window.FindViewById<TextView>(Resource.Id.title);
            var snippetView = window.FindViewById<TextView>(Resource.Id.snippet);

            if (string.IsNullOrWhiteSpace(marker.Snippet))
                snippetView.Visibility = Android.Views.ViewStates.Gone;

            titleView.Text = marker.Title;
            snippetView.Text = marker.Snippet;
            return window;
        }
    }
}