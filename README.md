# XAML Map Control

A set of controls for WPF, Silverlight and Windows Runtime for rendering digital maps from different providers and various types of map overlays.

Map providers can easily be added by specifying an URL template for their map tile bitmaps. 
Map overlay layers allow to draw and interact with graphical objects and pushpins on the map.
The Map Control API is similar to that of the Microsoft Bing Maps Control for WPF, except that no API key is needed.

The project includes sample applications for WPF, Silverlight and Windows Runtime, which demonstrate the features of XAML Map Control.

**Version 3.0** now adds support for multiple map projections. However, the MapTileLayer class only works with WebMercatorProjection.
For other projections, an appropriate WmsImageLayer could be used.

---

Main classes are

- **MapBase**: The core map control. Provides properties like Center, ZoomLevel and Heading, which
define the currently displayed map viewport.

- **Map**: MapBase with basic mouse and touch input handling for zoom, pan, and rotation.

- **MapTileLayer**: Provides tiled map content (e.g. from OpenStreetMap) by means of a **TileSource**.

- **MapImageLayer**: Provides map content that covers the entire viewport (e.g. from a Web Map Service).

- **MapItemsControl**: Displays a collection of **MapItem** objects (with a geographic **Location**).

---

The WPF version allows to use a System.Runtime.Caching.ObjectCache instance for caching map tile bitmaps.
The cache may be set to an instance of System.Runtime.Caching.MemoryCache (e.g. MemoryCache.Default),
but caching can also be done persistently by some specialized ObjectCache implementation.
Map Control comes with two such implementations:
* FileDbCache, an ObjectCache implementation based on EzTools FileDb, a simple, file based No-SQL database.
* ImageFileCache, an ObjectCache implementation that stores each cached map tile as a single image file,
in the original file format delivered by the map provider (typically PNG or JPG).
ImageFileCache does not support expiration, which means that cached tile image files will not be deleted automatically.
The cache may hence consume a considerable amount of disk space.

If you want to try the sample application with persistent caching, uncomment the appropriate TileImageLoader.Cache
setting in the sample application's MainWindow.xaml.cs file. Please note that some map providers may not allow
persistent caching of their map data.

Since version 2.4.0 the TileImageLoader of the Windows Runtime version also supports caching of map tiles to local image
files and to a FileDb file. The cache functionality is defined by the interface IImageCache (in namespace MapControl.Caching)
and implemented by the classes ImageFileCache and FileDbCache in libraries ImageFileCache.WinRT and FileDbCache.WinRT.
Local image files are written to the  ApplicationData.Current.TemporaryFolder by default.

XAML Map Control is also available on NuGet, with Package Id XAML.MapControl. The NuGet package contains MapControl libraries
for WPF, Silverlight and Windows Runtime.

---

The project is not open for contributions. Pull requests will not be accepted.
