﻿// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// © 2019 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System.Windows;

namespace MapControl
{
    public partial class MapPanel
    {
        public static readonly DependencyProperty LocationProperty = DependencyProperty.RegisterAttached(
            "Location", typeof(Location), typeof(MapPanel),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty BoundingBoxProperty = DependencyProperty.RegisterAttached(
            "BoundingBox", typeof(BoundingBox), typeof(MapPanel),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        private static readonly DependencyPropertyKey ParentMapPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "ParentMap", typeof(MapBase), typeof(MapPanel),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, ParentMapPropertyChanged));

        private static readonly DependencyPropertyKey ViewportPositionPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "ViewportPosition", typeof(Point?), typeof(MapPanel), new PropertyMetadata());

        public static readonly DependencyProperty ParentMapProperty = ParentMapPropertyKey.DependencyProperty;
        public static readonly DependencyProperty ViewportPositionProperty = ViewportPositionPropertyKey.DependencyProperty;

        public static MapBase GetParentMap(FrameworkElement element)
        {
            return (MapBase)element.GetValue(ParentMapProperty);
        }

        public static void InitMapElement(FrameworkElement element)
        {
            if (element is MapBase)
            {
                element.SetValue(ParentMapPropertyKey, element);
            }
        }

        private static void SetViewportPosition(FrameworkElement element, Point? viewportPosition)
        {
            element.SetValue(ViewportPositionPropertyKey, viewportPosition);
        }
    }
}
