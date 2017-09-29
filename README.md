# ThresholdPhotoTask

Library to call native Windows 10 Photo crop task if you are using a Windows Phone 8.1 Runtime / Silverlight app

## Requirements

You can include this project in applications based on:

- Windows Phone 8.1 Silverlight;
- Windows Phone 8.1 Runtime;

- Soon: Windows 8.1 and Windows Phone Silverlight 7.x/8.0; 

## How to use

There is an unique namespace to use:

```csharp
using ThresholdPhotoTask;
```

Then, to check if your app is running on a Windows 10 Mobile device, just call the async static method

```csharp
await PhotoTask.IsWindows10();
```

This photo task has a simple constructor, which require only the destination file where to write the cropped image, for example

```csharp
var destFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("test.jpg");

var task = new PhotoTask(destFile);

//Select cropped width
task.CropWidthPixels = 400; 

//Select cropped height
task.CropHeightPixels = 400; 

//Choose if you want to show an elliptical crop guide
task.EllipticalCrop = true; 

//Choose if you want to enable the option to take a photo as image to pick
task.ShowCamera = true; 

```

Finally, just use the async method to pick an image an crop it

```csharp
var file = await task.LaunchAsync();
```

If the returned StorageFile variable is not null, it means that your image is cropped successfully!

## Nuget

This library is available also on [Nuget](https://www.nuget.org/packages/ThresholdPhotoTask/), you can also install it using the command line

```
Install-Package ThresholdPhotoTask
```

## Credits

Author: Luca Montanari

Special thanks: The project is based on excellent guide of the Italian developer [Fela Ameghino](https://github.com/FrayxRulez/), to use the [crop task](http://blogs.msdn.com/b/italy/archive/2015/07/21/guest-post-fare-il-crop-delle-immagini-usando-l-app-foto-di-windows-10.aspx) integrated in [Microsoft Photo](http://www.microsoft.com/it-it/store/apps/microsoft-foto/9wzdncrfjbh4) official app for Windows 10.
