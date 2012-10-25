﻿// WPF MapControl - http://wpfmapcontrol.codeplex.com/
// Copyright © 2012 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System.Windows;
using System.Windows.Controls;

namespace MapControl
{
    /// <summary>
    /// Displays a pushpin at the geographic location provided by the Location property.
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    public class Pushpin : ContentControl
    {
        public static readonly DependencyProperty LocationProperty = MapPanel.LocationProperty.AddOwner(typeof(Pushpin));

        static Pushpin()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(Pushpin), new FrameworkPropertyMetadata(typeof(Pushpin)));
        }

        public MapBase ParentMap
        {
            get { return MapPanel.GetParentMap(this); }
        }

        public Location Location
        {
            get { return (Location)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }
    }
}