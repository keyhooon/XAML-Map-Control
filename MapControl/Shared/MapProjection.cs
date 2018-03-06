﻿// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// © 2018 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Globalization;
#if WINDOWS_UWP
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace MapControl
{
    /// <summary>
    /// Defines a map projection between geographic coordinates, cartesian map coordinates and viewport coordinates.
    /// </summary>
    public abstract class MapProjection
    {
        public const int TileSize = 256;
        public const double PixelPerDegree = TileSize / 360d;

        public const double Wgs84EquatorialRadius = 6378137d;
        public const double MetersPerDegree = Wgs84EquatorialRadius * Math.PI / 180d;

        private Matrix inverseViewportTransformMatrix;

        /// <summary>
        /// Gets or sets the WMS 1.3.0 CRS Identifier.
        /// </summary>
        public string CrsId { get; set; }

        /// <summary>
        /// Indicates if the map can be moved infinitely in longitudinal direction.
        /// </summary>
        public bool IsContinuous { get; protected set; } = true;

        /// <summary>
        /// Indicates if this is an azimuthal projection.
        /// </summary>
        public bool IsAzimuthal { get; protected set; } = false;

        /// <summary>
        /// Indicates if this is a web mercator projection, i.e. compatible with MapTileLayer.
        /// </summary>
        public bool IsWebMercator { get; protected set; } = false;

        /// <summary>
        /// Gets the scale factor from geographic to cartesian coordinates, on the line of true scale
        /// of a cylindrical projection, or at the projection center of an azimuthal projection.
        /// </summary>
        public double TrueScale { get; protected set; } = MetersPerDegree;

        /// <summary>
        /// Gets the absolute value of the minimum and maximum latitude that can be transformed.
        /// </summary>
        public double MaxLatitude { get; protected set; } = 90d;

        /// <summary>
        /// Gets the transformation matrix from cartesian map coordinates to viewport coordinates (pixels).
        /// </summary>
        public Matrix ViewportTransformMatrix { get; private set; }

        /// <summary>
        /// Gets the transformation from cartesian map coordinates to viewport coordinates (pixels).
        /// </summary>
        public MatrixTransform ViewportTransform { get; } = new MatrixTransform();

        /// <summary>
        /// Gets the scaling factor from cartesian map coordinates to viewport coordinates.
        /// </summary>
        public double ViewportScale { get; private set; }

        /// <summary>
        /// Gets the map scale at the specified Location as viewport coordinate units per meter (px/m).
        /// </summary>
        public abstract Point GetMapScale(Location location);

        /// <summary>
        /// Transforms a Location in geographic coordinates to a Point in cartesian map coordinates.
        /// </summary>
        public abstract Point LocationToPoint(Location location);

        /// <summary>
        /// Transforms a Point in cartesian map coordinates to a Location in geographic coordinates.
        /// </summary>
        public abstract Location PointToLocation(Point point);

        /// <summary>
        /// Transforms a BoundingBox in geographic coordinates to a Rect in cartesian map coordinates.
        /// </summary>
        public virtual Rect BoundingBoxToRect(BoundingBox boundingBox)
        {
            return new Rect(
                LocationToPoint(new Location(boundingBox.South, boundingBox.West)),
                LocationToPoint(new Location(boundingBox.North, boundingBox.East)));
        }

        /// <summary>
        /// Transforms a Rect in cartesian map coordinates to a BoundingBox in geographic coordinates.
        /// </summary>
        public virtual BoundingBox RectToBoundingBox(Rect rect)
        {
            var sw = PointToLocation(new Point(rect.X, rect.Y));
            var ne = PointToLocation(new Point(rect.X + rect.Width, rect.Y + rect.Height));

            return new BoundingBox(sw.Latitude, sw.Longitude, ne.Latitude, ne.Longitude);
        }

        /// <summary>
        /// Transforms a Location in geographic coordinates to a Point in viewport coordinates.
        /// </summary>
        public Point LocationToViewportPoint(Location location)
        {
            return ViewportTransformMatrix.Transform(LocationToPoint(location));
        }

        /// <summary>
        /// Transforms a Point in viewport coordinates to a Location in geographic coordinates.
        /// </summary>
        public Location ViewportPointToLocation(Point point)
        {
            return PointToLocation(inverseViewportTransformMatrix.Transform(point));
        }

        /// <summary>
        /// Transforms a Rect in viewport coordinates to a BoundingBox in geographic coordinates.
        /// </summary>
        public BoundingBox ViewportRectToBoundingBox(Rect rect)
        {
            var transform = new MatrixTransform { Matrix = inverseViewportTransformMatrix };

            return RectToBoundingBox(transform.TransformBounds(rect));
        }

        /// <summary>
        /// Sets ViewportScale and ViewportTransform values.
        /// </summary>
        public virtual void SetViewportTransform(Location projectionCenter, Location mapCenter, Point viewportCenter, double zoomLevel, double heading)
        {
            ViewportScale = Math.Pow(2d, zoomLevel) * PixelPerDegree / TrueScale;

            var center = LocationToPoint(mapCenter);
            var transformMatrix = CreateTransformMatrix(
                -center.X, -center.Y, ViewportScale, -ViewportScale, heading, viewportCenter.X, viewportCenter.Y);

            ViewportTransformMatrix = transformMatrix;
            ViewportTransform.Matrix = transformMatrix;

            transformMatrix.Invert();
            inverseViewportTransformMatrix = transformMatrix;
        }

        /// <summary>
        /// Gets a WMS 1.3.0 query parameter string from the specified bounding box,
        /// e.g. "CRS=...&BBOX=...&WIDTH=...&HEIGHT=..."
        /// </summary>
        public virtual string WmsQueryParameters(BoundingBox boundingBox, string version = "1.3.0")
        {
            if (string.IsNullOrEmpty(CrsId) || !boundingBox.HasValidBounds)
            {
                return null;
            }

            var format = "CRS={0}&BBOX={1},{2},{3},{4}&WIDTH={5}&HEIGHT={6}";

            if (version.StartsWith("1.1."))
            {
                format = "SRS={0}&BBOX={1},{2},{3},{4}&WIDTH={5}&HEIGHT={6}";
            }
            else if (CrsId == "EPSG:4326")
            {
                format = "CRS={0}&BBOX={2},{1},{4},{3}&WIDTH={5}&HEIGHT={6}";
            }

            var rect = BoundingBoxToRect(boundingBox);
            var width = (int)Math.Round(ViewportScale * rect.Width);
            var height = (int)Math.Round(ViewportScale * rect.Height);

            return string.Format(CultureInfo.InvariantCulture, format, CrsId,
                rect.X, rect.Y, (rect.X + rect.Width), (rect.Y + rect.Height), width, height);
        }

        public static Matrix CreateTransformMatrix(
            double translation1X, double translation1Y,
            double scaleX, double scaleY, double rotationAngle,
            double translation2X, double translation2Y)
        {
            var matrix = new Matrix(1d, 0d, 0d, 1d, translation1X, translation1Y);
            matrix.Scale(scaleX, scaleY);
            matrix.Rotate(rotationAngle);
            matrix.Translate(translation2X, translation2Y);
            return matrix;
        }
    }
}
