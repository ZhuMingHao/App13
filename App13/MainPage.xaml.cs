using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App13
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BitmapImage inkimage;
        public MainPage()
        {
            this.InitializeComponent();
            ink.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Touch;
            var attr = new InkDrawingAttributes();
            attr.Color = Colors.Red;
            attr.IgnorePressure = true;
            attr.PenTip = PenTipShape.Circle;
            attr.Size = new Size(4, 10);
            ink.InkPresenter.UpdateDefaultDrawingAttributes(attr);
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StorageFile inputFile = await StorageFile.GetFileFromApplicationUriAsync(inkimage.UriSource);

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, (int)ink.ActualWidth, (int)ink.ActualHeight, 96);
            using (var ds = renderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.White);
                var image = await CanvasBitmap.LoadAsync(device, inputFile.Path, 96);
                ds.DrawImage(image);// Draw image firstly
                ds.DrawInk(ink.InkPresenter.StrokeContainer.GetStrokes());// Draw the stokes
            }

            //Save them to the output.jpg in picture folder
            StorageFolder storageFolder = KnownFolders.PicturesLibrary;
            var file = await storageFolder.CreateFileAsync("output.jpg", CreationCollisionOption.ReplaceExisting);
            using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await renderTarget.SaveAsync(fileStream, CanvasBitmapFileFormat.Jpeg, 1f);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Get the image source that for attaching to the inkcanvas, update the inkcanvas to be same size with image. 
            inkimage = (BitmapImage)imgbackground.Source;
            var imagewidth = inkimage.PixelWidth;
            var imageheight = inkimage.PixelWidth;
            ink.Height = imageheight;
            ink.Width = imagewidth;
            ink.UpdateLayout();
        }
    }
}
