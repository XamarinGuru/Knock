﻿using CoreLocation;
using Foundation;
using MapKit;

namespace knock.iOS
{
    /// <summary>
    /// Custom map annotation
    /// </summary>
    [Preserve(AllMembers = true)]
    internal class CustomMapAnnotation : MKAnnotation
    {
        private readonly CustomMapPin _formsPin;

        ///<inheritdoc/>
        public override string Title
        {
            get
            {
                return this._formsPin.Title;
            }
        }
        ///<inheritdoc/>
        public override string Subtitle
        {
            get
            {
                return this._formsPin.Subtitle;
            }
        }
        ///<inheritdoc/>
        public override CLLocationCoordinate2D Coordinate
        {
            get { return this._formsPin.Position.ToLocationCoordinate(); }
        }
        /// <summary>
        /// Gets the forms pin
        /// </summary>
        public CustomMapPin CustomPin
        {
            get { return this._formsPin; }
        }
        ///<inheritdoc/>
        public override void SetCoordinate(CLLocationCoordinate2D value)
        {
            this._formsPin.Position = value.ToPosition();
        }
        /// <summary>
        /// Xamarin.iOS does (still) not export <value>_original_setCoordinate</value>
        /// </summary>
        /// <param name="value">The coordinate</param>
        [Export("_original_setCoordinate:")]
        public void SetCoordinateOriginal(CLLocationCoordinate2D value)
        {
            this.SetCoordinate(value);
        }
        /// <summary>
        /// Creates a new instance of <see cref="TKCustomMapAnnotation"/>
        /// </summary>
        /// <param name="pin">The forms pin</param>
        public CustomMapAnnotation(CustomMapPin pin)
        {
            this._formsPin = pin;
        }
    }
}
