using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MapKit;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;
using knock;
using knock.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace knock.iOS
{
    /// <summary>
    /// iOS Renderer of <see cref="TK.CustomMap.CustomMap"/>
    /// </summary>
    [Preserve(AllMembers = true)]
    public class CustomMapRenderer : MapRenderer, IRendererFunctions
    {
        private const double MercatorRadius = 85445659.44705395;
        private const int MaxGoogleLevels = 20;

        private const string AnnotationIdentifier = "CustomAnnotation";
        private const string AnnotationIdentifierDefaultPin = "CustomAnnotationDefaultPin";

        private bool _isDragging;
        private IMKAnnotation _selectedAnnotation;
        private UIGestureRecognizer _longPressGestureRecognizer;
        private UIGestureRecognizer _tapGestureRecognizer;

        private MKMapView Map
        {
            get { return this.Control as MKMapView; }
        }
        private CustomMap FormsMap
        {
            get { return this.Element as CustomMap; }
        }
        private int ZoomLevel
        {
            get
            {

                double longitudeDelta = this.Map.Region.Span.LongitudeDelta;
                nfloat mapWidthInPixels = this.Map.Bounds.Size.Width;
                double zoomScale = longitudeDelta * MercatorRadius * Math.PI / (180.0 * mapWidthInPixels);
                double zoomer = MaxGoogleLevels - Math.Log(zoomScale);
                if (zoomer < 0) zoomer = 0;

                return (int)Math.Round(zoomer);
            }
        }
        /// <summary>
        /// Dummy function to avoid linker.
        /// </summary>
        [Preserve]
        public static void InitMapRenderer()
        { }
        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || this.FormsMap == null || this.Map == null) return;

            if(e.OldElement != null && this.Map != null)
            {
                e.OldElement.PropertyChanged -= OnMapPropertyChanged;

                this.Map.GetViewForAnnotation = null;
                this.Map.OverlayRenderer = null;
                this.Map.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                this.Map.RegionChanged -= OnMapRegionChanged;
                this.Map.DidUpdateUserLocation -= OnDidUpdateUserLocation;
                this.Map.ChangedDragState -= OnChangedDragState;
                this.Map.CalloutAccessoryControlTapped -= OnMapCalloutAccessoryControlTapped;

                this.Map.RemoveGestureRecognizer(this._longPressGestureRecognizer);
                this.Map.RemoveGestureRecognizer(this._tapGestureRecognizer);
                this._longPressGestureRecognizer.Dispose();
                this._tapGestureRecognizer.Dispose();
            }

            if (e.NewElement != null)
            {
                ((IMapFunctions)this.FormsMap).SetRenderer(this);

                this.Map.GetViewForAnnotation = this.GetViewForAnnotation;
                this.Map.DidSelectAnnotationView += OnDidSelectAnnotationView;
                this.Map.RegionChanged += OnMapRegionChanged;
                this.Map.DidUpdateUserLocation += OnDidUpdateUserLocation;
                this.Map.ChangedDragState += OnChangedDragState;
                this.Map.CalloutAccessoryControlTapped += OnMapCalloutAccessoryControlTapped;

                this.Map.AddGestureRecognizer((this._longPressGestureRecognizer = new UILongPressGestureRecognizer(this.OnMapLongPress)));

                this._tapGestureRecognizer = new UITapGestureRecognizer(this.OnMapClicked);
                this._tapGestureRecognizer.ShouldReceiveTouch = (recognizer, touch) => !(touch.View is MKAnnotationView);

                this.Map.AddGestureRecognizer(this._tapGestureRecognizer);

                this.SetMapCenter();
                this.UpdatePins();
                this.FormsMap.PropertyChanged += OnMapPropertyChanged;
            }
        }

        /// <summary>
        /// When the user location changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnDidUpdateUserLocation(object sender, MKUserLocationEventArgs e)
        {
            if (e.UserLocation == null || this.FormsMap == null || this.FormsMap.UserLocationChangedCommand == null) return;

            var newPosition = e.UserLocation.Location.Coordinate.ToPosition();

            if (this.FormsMap.UserLocationChangedCommand.CanExecute(newPosition))
            {
                this.FormsMap.UserLocationChangedCommand.Execute(newPosition);
            }
        }
        /// <summary>
        /// When a property of the forms map changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CustomMap.ItemsSourceProperty.PropertyName)
            {
                this.UpdatePins();
            }
            else if (e.PropertyName == CustomMap.SelectedItemProperty.PropertyName)
            {
                this.SetSelectedPin();
            }
            else if (e.PropertyName == CustomMap.MapCenterProperty.PropertyName)
            {
                this.SetMapCenter();
            }
            else if (e.PropertyName == CustomMap.CalloutTappedCommandProperty.PropertyName)
            {
                this.UpdatePins(false);
            }
            else if(e.PropertyName == CustomMap.CurrentRegionProperty.PropertyName)
            {
                this.UpdateMapRegion();
            }
        }
        /// <summary>
        /// When the collection of pins changed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(CustomMapPin pin in e.NewItems)
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
                        if (this.FormsMap.SelectedItem != null && this.FormsMap.SelectedItem.Equals(pin))
                        {
                            this.FormsMap.SelectedItem = null;
                        }

                        var annotation = this.Map.Annotations
                            .OfType<CustomMapAnnotation>()
                            .SingleOrDefault(i => i.CustomPin.Equals(pin));

                        if (annotation != null)
                        {
                            annotation.CustomPin.PropertyChanged -= OnPinPropertyChanged;
                            this.Map.RemoveAnnotation(annotation);
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
				if (this.Map == null)
				{
					return;
				}
                foreach (var annotation in this.Map.Annotations.OfType<CustomMapAnnotation>())
                {
                    annotation.CustomPin.PropertyChanged -= OnPinPropertyChanged;
                }
                this.UpdatePins(false);
            }
        }
        /// <summary>
        /// When the accessory control of a callout gets tapped
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnMapCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            if (this.FormsMap.CalloutTappedCommand.CanExecute(null))
            {
                this.FormsMap.CalloutTappedCommand.Execute(null);
            }
        } 
        /// <summary>
        /// When the drag state changed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnChangedDragState(object sender, MKMapViewDragStateEventArgs e)
        {
            var annotation = e.AnnotationView.Annotation as CustomMapAnnotation;
            if (annotation == null) return;

            if (e.NewState == MKAnnotationViewDragState.Starting)
            {
                this._isDragging = true;
            }
            else if (e.NewState == MKAnnotationViewDragState.Dragging)
            {
                annotation.CustomPin.Position = e.AnnotationView.Annotation.Coordinate.ToPosition();
            }
            else if (e.NewState == MKAnnotationViewDragState.Ending || e.NewState == MKAnnotationViewDragState.Canceling)
            {
                e.AnnotationView.DragState = MKAnnotationViewDragState.None;
                this._isDragging = false;
                if (this.FormsMap.ItemDragEndCommand != null && this.FormsMap.ItemDragEndCommand.CanExecute(annotation.CustomPin))
                {
                    this.FormsMap.ItemDragEndCommand.Execute(annotation.CustomPin);
                }
            }
        }
        /// <summary>
        /// When the camera region changed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnMapRegionChanged(object sender, MKMapViewChangeEventArgs e)
        {
            this.FormsMap.MapCenter = this.Map.CenterCoordinate.ToPosition();
        }
        /// <summary>
        /// When an annotation view got selected
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public virtual void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            var pin = e.View.Annotation as CustomMapAnnotation;
            if(pin == null) return;

            this._selectedAnnotation = e.View.Annotation;
            this.FormsMap.SelectedItem = pin.CustomPin;

            if (this.FormsMap.ItemSelectedCommand != null && this.FormsMap.ItemSelectedCommand.CanExecute(pin.CustomPin))
            {
                this.FormsMap.ItemSelectedCommand.Execute(pin.CustomPin);
            }
        }
        /// <summary>
        /// When a tap was perfomed on the map
        /// </summary>
        /// <param name="recognizer">The gesture recognizer</param>
        private void OnMapClicked(UITapGestureRecognizer recognizer)
        {
            if (recognizer.State != UIGestureRecognizerState.Ended) return;

            var pixelLocation = recognizer.LocationInView(this.Map);
            var coordinate = this.Map.ConvertPoint(pixelLocation, this.Map);

            if (this.FormsMap.MapTappedCommand != null && this.FormsMap.MapTappedCommand.CanExecute(coordinate.ToPosition()))
            {
                this.FormsMap.MapTappedCommand.Execute(coordinate.ToPosition());
            }

        }
        /// <summary>
        /// When a long press was performed
        /// </summary>
        /// <param name="recognizer">The gesture recognizer</param>
        private void OnMapLongPress(UILongPressGestureRecognizer recognizer)
        {
            if (recognizer.State != UIGestureRecognizerState.Began) return;

            var pixelLocation = recognizer.LocationInView(this.Map);
            var coordinate = this.Map.ConvertPoint(pixelLocation, this.Map);

            if (this.FormsMap.MapLongPressedCommand != null && this.FormsMap.MapLongPressedCommand.CanExecute(coordinate.ToPosition()))
            {
                this.FormsMap.MapLongPressedCommand.Execute(coordinate.ToPosition());
            }
        }
        /// <summary>
        /// Get the view for the annotation
        /// </summary>
        /// <param name="mapView">The map</param>
        /// <param name="annotation">The annotation</param>
        /// <returns>The annotation view</returns>
        public virtual MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            var customAnnotation = annotation as CustomMapAnnotation;

            if (customAnnotation == null) return null;

            MKAnnotationView annotationView;
            if(customAnnotation.CustomPin.Image != null)
                annotationView = mapView.DequeueReusableAnnotation(AnnotationIdentifier);
            else
                annotationView = mapView.DequeueReusableAnnotation(AnnotationIdentifierDefaultPin);

            if (annotationView == null)
            {
                if(customAnnotation.CustomPin.Image != null)
                    annotationView = new CustomAnnotationView(customAnnotation, AnnotationIdentifier);
                else
                    annotationView = new MKPinAnnotationView(customAnnotation, AnnotationIdentifierDefaultPin);
            }
            else 
            {
                annotationView.Annotation = customAnnotation;
            }
            annotationView.CanShowCallout = customAnnotation.CustomPin.ShowCallout;
            annotationView.Draggable = customAnnotation.CustomPin.IsDraggable;
            annotationView.Selected = this._selectedAnnotation != null && customAnnotation.Equals(this._selectedAnnotation);
            this.SetAnnotationViewVisibility(annotationView, customAnnotation.CustomPin);
            this.UpdateImage(annotationView, customAnnotation.CustomPin);

            if (FormsMap.CalloutTappedCommand != null)
            {
                var button = new UIButton(UIButtonType.InfoLight);
                button.Frame = new CGRect(0, 0, 23, 23);
                button.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
                button.VerticalAlignment = UIControlContentVerticalAlignment.Center;
                annotationView.RightCalloutAccessoryView = button;
                annotationView.LeftCalloutAccessoryView = new UIImageView(new CGRect(0, 0, 50, 50))
                    {
                        BackgroundColor = Tema.coloreRosa.ToUIColor(),
                        Image = ImageSource.FromFile("deskBiancoIcon").ToImage().Result
                    };
            }

            return annotationView;
        }
        /// <summary>
        /// Creates the annotations
        /// </summary>
        private void UpdatePins(bool firstUpdate = true)
        {
            this.Map.RemoveAnnotations(this.Map.Annotations);

            if (this.FormsMap.ItemsSource == null) return;

            foreach (var i in FormsMap.ItemsSource)
            {
                i.PropertyChanged -= OnPinPropertyChanged;
                this.AddPin(i);
            }

            if (firstUpdate)
            {
                var observAble = this.FormsMap.ItemsSource as INotifyCollectionChanged;
                if (observAble != null)
                {
                    observAble.CollectionChanged += OnCollectionChanged;
                }
            }

            if (this.FormsMap.ItemsReadyCommand != null && this.FormsMap.ItemsReadyCommand.CanExecute(this.FormsMap))
            {
                this.FormsMap.ItemsReadyCommand.Execute(this.FormsMap);
            }
        }
        /// <summary>
        /// Adds a pin
        /// </summary>
        /// <param name="pin">The pin to add</param>
        private void AddPin(CustomMapPin pin)
        {
            var annotation = new CustomMapAnnotation(pin);
			if (Map == null)
			{
				return;}
            this.Map.AddAnnotation(annotation);

            pin.PropertyChanged += OnPinPropertyChanged;
        }
        /// <summary>
        /// When a property of the pin changed
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnPinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CustomMapPin.Title) ||
                e.PropertyName == nameof(CustomMapPin.Subtitle) ||
                (e.PropertyName == nameof(CustomMapPin.Position) && this._isDragging))
                return;

            var formsPin = (CustomMapPin)sender;
            var annotation = this.Map.Annotations
                .OfType<CustomMapAnnotation>()
                .SingleOrDefault(i => i.CustomPin.Equals(formsPin));

            if (annotation == null) return;

            var annotationView = this.Map.ViewForAnnotation(annotation);
            if (annotationView == null) return;

            switch (e.PropertyName)
            {
                case nameof(CustomMapPin.Image):
                    this.UpdateImage(annotationView, formsPin);
                    break;
                case nameof(CustomMapPin.DefaultColor):
                    this.UpdateImage(annotationView, formsPin);
                    break;
                case nameof(CustomMapPin.IsDraggable):
                    annotationView.Draggable = formsPin.IsDraggable;
                    break;
                case nameof(CustomMapPin.IsVisible):
                    this.SetAnnotationViewVisibility(annotationView, formsPin);
                    break;
                case nameof(CustomMapPin.Position):
                    annotation.SetCoordinate(formsPin.Position.ToLocationCoordinate());
                    break;
                case nameof(CustomMapPin.ShowCallout):
                    annotationView.CanShowCallout = formsPin.ShowCallout;
                    break;
            }
        }
        /// <summary>
        /// Set the visibility of an annotation view
        /// </summary>
        /// <param name="annotationView">The annotation view</param>
        /// <param name="pin">The forms pin</param>
        private void SetAnnotationViewVisibility(MKAnnotationView annotationView, CustomMapPin pin)
        {
            annotationView.Hidden = !pin.IsVisible;
            annotationView.UserInteractionEnabled = pin.IsVisible;
            annotationView.Enabled = pin.IsVisible;
        }
        /// <summary>
        /// Set the image of the annotation view
        /// </summary>
        /// <param name="annotationView">The annotation view</param>
        /// <param name="pin">The forms pin</param>
        private async void UpdateImage(MKAnnotationView annotationView, CustomMapPin pin)
        {
            if (pin.Image != null)
            {
                // If this is the case, we need to get a whole new annotation view. 
                if (annotationView.GetType() == typeof (MKPinAnnotationView))
                {
                    this.Map.RemoveAnnotation(annotationView.Annotation);
                    this.Map.AddAnnotation(new CustomMapAnnotation(pin));
                    return;
                }
                UIImage image = await pin.Image.ToImage();
                Device.BeginInvokeOnMainThread(() =>
                    {
                        annotationView.Image = image;
                        annotationView.CenterOffset = new CGPoint(0, -image.Size.Height / 2 + 10);
                    });
            }
            else
            {
                var pinAnnotationView = annotationView as MKPinAnnotationView;
                if (pinAnnotationView != null)
                {
                    pinAnnotationView.AnimatesDrop = true;

                    var pinTintColorAvailable = pinAnnotationView.RespondsToSelector(new Selector("pinTintColor"));

                    if (!pinTintColorAvailable)
                    {
                        return;
                    }

                    if (pin.DefaultColor != Color.Default)
                    {
                        pinAnnotationView.PinTintColor = pin.DefaultColor.ToUIColor();
                    }
                    else
                    {
                        pinAnnotationView.PinTintColor = UIColor.Red;
                    }
                }
            }
        }
        /// <summary>
        /// Sets the selected pin
        /// </summary>
        private void SetSelectedPin()
        {
            var customAnnotion = this._selectedAnnotation as CustomMapAnnotation;

            if (customAnnotion != null)
            {
                if (customAnnotion.CustomPin.Equals(this.FormsMap.SelectedItem)) return;

                var annotationView = this.Map.ViewForAnnotation(customAnnotion);
                annotationView.Selected = false;

                this._selectedAnnotation = null;
            }
            if (this.FormsMap.SelectedItem != null)
            {
                var selectedAnnotation = this.Map.Annotations
                    .OfType<CustomMapAnnotation>()
                    .SingleOrDefault(i => i.CustomPin.Equals(this.FormsMap.SelectedItem));

                if (selectedAnnotation != null)
                {
                    var annotationView = this.Map.ViewForAnnotation(selectedAnnotation);
                    this._selectedAnnotation = selectedAnnotation;
                    if (annotationView != null)
                    {
                        this.Map.SelectAnnotation(selectedAnnotation, true);
                    }

                    if (this.FormsMap.ItemSelectedCommand != null && this.FormsMap.ItemSelectedCommand.CanExecute(null))
                    {
                        this.FormsMap.ItemSelectedCommand.Execute(null);
                    }
                }
            }
        }
        /// <summary>
        /// Sets the center of the map
        /// </summary>
        private void SetMapCenter()
        {
            if(this.FormsMap == null || this.Map == null) return;

            if (!this.FormsMap.MapCenter.Equals(this.Map.CenterCoordinate.ToPosition()))
            {
                this.Map.SetCenterCoordinate(this.FormsMap.MapCenter.ToLocationCoordinate(), this.FormsMap.IsRegionChangeAnimated);   
            }
        }
        /// <summary>
        /// Updates the map region when changed
        /// </summary>
        private void UpdateMapRegion()
        {
            if (this.FormsMap == null) return;

            if(this.FormsMap.CurrentRegion != this.FormsMap.VisibleRegion)
            {
                this.MoveToMapRegion(this.FormsMap.CurrentRegion, this.FormsMap.IsRegionChangeAnimated);
            }
        }
        /// <summary>
        /// Calculates the closest distance of a point to a polyline
        /// </summary>
        /// <param name="pt">The point</param>
        /// <param name="poly">The polyline</param>
        /// <returns>The closes distance</returns>
        private double DistanceOfPoint(MKMapPoint pt, MKPolyline poly)
        {
            double distance = float.MaxValue;
            for (int n = 0; n < poly.PointCount - 1; n++)
            {

                MKMapPoint ptA = poly.Points[n];
                MKMapPoint ptB = poly.Points[n + 1];

                double xDelta = ptB.X - ptA.X;
                double yDelta = ptB.Y - ptA.Y;

                if (xDelta == 0.0 && yDelta == 0.0)
                {

                    // Points must not be equal
                    continue;
                }

                double u = ((pt.X - ptA.X) * xDelta + (pt.Y - ptA.Y) * yDelta) / (xDelta * xDelta + yDelta * yDelta);
                MKMapPoint ptClosest;
                if (u < 0.0)
                {

                    ptClosest = ptA;
                }
                else if (u > 1.0)
                {

                    ptClosest = ptB;
                }
                else
                {

                    ptClosest = new MKMapPoint(ptA.X + u * xDelta, ptA.Y + u * yDelta);
                }

                distance = Math.Min(distance, MKGeometry.MetersBetweenMapPoints(ptClosest, pt));
            }

            return distance;
        }
        /// <summary>
        /// Returns the meters between two points
        /// </summary>
        /// <param name="px">X in pixels</param>
        /// <param name="pt">Position</param>
        /// <returns>Distance in meters</returns>
        private double MetersFromPixel(int px, CGPoint pt)
        {
            CGPoint ptB = new CGPoint(pt.X + px, pt.Y);

            CLLocationCoordinate2D coordA = this.Map.ConvertPoint(pt, this.Map);
            CLLocationCoordinate2D coordB = this.Map.ConvertPoint(ptB, this.Map);

            return MKGeometry.MetersBetweenMapPoints(MKMapPoint.FromCoordinate(coordA), MKMapPoint.FromCoordinate(coordB));
        }
        /// <summary>
        /// Convert a <see cref="MKCoordinateRegion"/> to <see cref="MKMapRect"/>
        /// http://stackoverflow.com/questions/9270268/convert-mkcoordinateregion-to-mkmaprect
        /// </summary>
        /// <param name="region">Region to convert</param>
        /// <returns>The map rect</returns>
        private MKMapRect RegionToRect(MKCoordinateRegion region)
        {
            MKMapPoint a = MKMapPoint.FromCoordinate(
                new CLLocationCoordinate2D(
                    region.Center.Latitude + region.Span.LatitudeDelta / 2,
                    region.Center.Longitude - region.Span.LongitudeDelta / 2));

            MKMapPoint b = MKMapPoint.FromCoordinate(
                new CLLocationCoordinate2D(
                    region.Center.Latitude - region.Span.LatitudeDelta / 2,
                    region.Center.Longitude + region.Span.LongitudeDelta / 2));

            return new MKMapRect(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
        }
        ///<inheritdoc/>
        public async Task<byte[]> GetSnapshot()
        {
            UIImage img = null;
            await Task.Factory.StartNew(() =>
                {
                    UIGraphics.BeginImageContextWithOptions(this.Frame.Size, false, 0.0f);
                    this.Layer.RenderInContext(UIGraphics.GetCurrentContext());

                    img = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();
                });
            return img.AsPNG().ToArray();
        }
        /// <inheritdoc/>
        public void FitMapRegionToPositions(IEnumerable<Position> positions, bool animate = false)
        {
            if(this.Map == null) return;

            MKMapRect zoomRect = MKMapRect.Null;

            foreach(var position in positions)
            {
                MKMapPoint point = MKMapPoint.FromCoordinate(position.ToLocationCoordinate());
                MKMapRect pointRect = new MKMapRect(point.X, point.Y, 0.1, 0.1);
                zoomRect = MKMapRect.Union(zoomRect, pointRect);
            }
            this.Map.SetVisibleMapRect(zoomRect, animate);
        }
        /// <inheritdoc/>
        public void MoveToMapRegion(MapSpan region, bool animate)
        {
            if (this.Map == null) return;

            var coordinateRegion = MKCoordinateRegion.FromDistance(
                region.Center.ToLocationCoordinate(), 
                region.Radius.Meters * 2, 
                region.Radius.Meters * 2);

            this.Map.SetRegion(coordinateRegion, animate);
        }
        /// <inheritdoc/>
        public void FitToMapRegions(IEnumerable<MapSpan> regions, bool animate)
        {
            if (this.Map == null) return;

            MKMapRect rect = MKMapRect.Null;
            foreach(var region in regions)
            {
                rect = MKMapRect.Union(
                    rect,
                    this.RegionToRect(
                        MKCoordinateRegion.FromDistance(
                            region.Center.ToLocationCoordinate(),
                            region.Radius.Meters * 2,
                            region.Radius.Meters * 2)));
            }
            this.Map.SetVisibleMapRect(rect, new UIEdgeInsets(15, 15, 15, 15), animate);
        }
        /// <summary>
        /// Returns the <see cref="CustomMapPin"/> by the native <see cref="IMKAnnotation"/>
        /// </summary>
        /// <param name="annotation">The annotation to search with</param>
        /// <returns>The forms pin</returns>
        protected CustomMapPin GetPinByAnnotation(IMKAnnotation annotation)
        {
            var customAnnotation = annotation as CustomMapAnnotation;
            if (customAnnotation == null) return null;

            return customAnnotation.CustomPin;
        }
    }
}
