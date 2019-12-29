using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MapControl;
using ViewModel;

namespace WpfCoreApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ImageLoader.HttpClient.DefaultRequestHeaders.Add("User-Agent", "XAML Map Control Test Application");
            //ImageLoader.HttpClient.Timeout = TimeSpan.FromSeconds(10);
            TileImageLoader.Cache = new MapControl.Caching.ImageFileCache(TileImageLoader.DefaultCacheFolder) ;
            InitializeComponent();
        }

        private void MapMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            itemsControl.SelectedItem = null;
            if (e.ClickCount == 2)
            {
                map.TargetCenter = map.ViewportPointToLocation(e.GetPosition(map));
            }
        }

        private void MapMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //map.ZoomMap(e.GetPosition(map), Math.Ceiling(map.ZoomLevel - 1.5));
            }
        }

        private void MapMouseMove(object sender, MouseEventArgs e)
        {
            var location = map.ViewportPointToLocation(e.GetPosition(map));
            
            mouseLocation.Text = location.GetPrettyString();
        }

        private void MapMouseLeave(object sender, MouseEventArgs e)
        {
            mouseLocation.Text = string.Empty;
        }

        private void MapManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = .001;

        }

        private void MapItemTouchDown(object sender, TouchEventArgs e)
        {
            var mapItem = (MapItem)sender;
            mapItem.IsSelected = !mapItem.IsSelected;
            e.Handled = true;
        }

        private void SeamarksChecked(object sender, RoutedEventArgs e)
        {
            map.Children.Insert(map.Children.IndexOf(mapGraticule), ((MapViewModel)DataContext).MapLayers.SeamarksLayer);
        }

        private void SeamarksUnchecked(object sender, RoutedEventArgs e)
        {
            map.Children.Remove(((MapViewModel)DataContext).MapLayers.SeamarksLayer);
        }
    }
}
