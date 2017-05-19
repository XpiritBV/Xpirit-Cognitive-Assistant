using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xpirit.Cognitive.Assistant.Services;

namespace Xpirit.Cognitive.Assistant.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DisplayRequest _displayRequest = new DisplayRequest();
        private MediaCapture _mediaCapture;
        private CaptureElement _captureElement;
        private bool _isInitialized = false;
        private bool _isRecording;

        // Information about the camera device
        private bool _mirroringPreview;
        private bool _externalCamera;

        private FaceDetectionEffect _faceDetectionEffect;

        private IFaceRecognitionService _faceRecognitionService;

        public MainViewModel(IFaceRecognitionService faceRecognitionService)
        {
            _faceRecognitionService = faceRecognitionService;

            if (_mediaCapture == null)
            {
                _mediaCapture = new MediaCapture();
                _mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
                _mediaCapture.Failed += MediaCapture_Failed;
            }
        }

        public MediaCapture MediaCapture
        {
            get
            {
                if (_mediaCapture == null) _mediaCapture = new MediaCapture();
                return _mediaCapture;
            }
            set
            {
                Set(() => MediaCapture, ref _mediaCapture, value);
            }
        }

        public CaptureElement CaptureElement
        {
            get
            {
                if (_captureElement == null) _captureElement = new CaptureElement();
                return _captureElement;
            }
            set
            {
                Set(() => CaptureElement, ref _captureElement, value);
            }
        }

        /// <summary>
        /// The <see cref="HelloText" /> property's name.
        /// </summary>
        public const string HelloTextPropertyName = "HelloText";

        private string _helloText = "";

        /// <summary>
        /// Sets and gets the HelloText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string HelloText
        {
            get
            {
                return _helloText;
            }

            set
            {
                if (_helloText == value)
                {
                    return;
                }

                _helloText = value;
                RaisePropertyChanged(HelloTextPropertyName);
            }
        }

        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            throw new NotImplementedException();
        }

        private void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
        {
            throw new NotImplementedException();
        }

        private RelayCommand _loadedCommand;

        /// <summary>
        /// Gets the LoadedCommand.
        /// </summary>
        public RelayCommand LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand(
                           async () =>
                           {
                            
                               var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                               DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                               var cameraDevice = desiredDevice ?? allVideoDevices.FirstOrDefault();

                               var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

                               if (cameraDevice == null)
                               {
                                   Debug.WriteLine("No camera device found!");
                                   return;
                               }

                               try
                               {
                                   await _mediaCapture.InitializeAsync(settings);
                                   _isInitialized = true;
                               }
                               catch (UnauthorizedAccessException)
                               {
                                   Debug.WriteLine("The app was denied access to the camera");
                               }

                               // If initialization succeeded, start the preview
                               if (_isInitialized)
                               {
                                   // Figure out where the camera is located
                                   if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                                   {
                                       // No information on the location of the camera, assume it's an external camera, not integrated on the device
                                       _externalCamera = true;
                                   }
                                   else
                                   {
                                       // Camera is fixed on the device
                                       _externalCamera = false;

                                       // Only mirror the preview if the camera is on the front panel
                                       _mirroringPreview = (cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                                   }

                                   await StartPreviewAsync();
                               }
                           }));
            }
        }

        /// <summary>
        /// Starts the preview and adjusts it for for rotation and mirroring after making a request to keep the screen on
        /// </summary>
        /// <returns></returns>
        private async Task StartPreviewAsync()
        {
            _displayRequest.RequestActive();
            CaptureElement.Source = _mediaCapture;
            CaptureElement.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            await _mediaCapture.StartPreviewAsync();

            var properties = _mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).Select(x => x as VideoEncodingProperties);
            var maxRes = properties.OrderByDescending(x => x.Height * x.Width).FirstOrDefault();
            await _mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, maxRes);

            var definition = new FaceDetectionEffectDefinition();
            // To ensure preview smoothness, do not delay incoming samples
            definition.SynchronousDetectionEnabled = false;

            // In this scenario, choose detection speed over accuracy
            definition.DetectionMode = FaceDetectionMode.HighPerformance;

            // Add the effect to the preview stream
            _faceDetectionEffect = (FaceDetectionEffect)await _mediaCapture.AddVideoEffectAsync(definition, MediaStreamType.VideoPreview);
            _faceDetectionEffect.FaceDetected += FaceDetectionEffect_FaceDetected;
            _faceDetectionEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(1000);
            _faceDetectionEffect.Enabled = true;
        }

        private async void FaceDetectionEffect_FaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {       
            
            Debug.WriteLine("face detected " + args.ResultFrame.DetectedFaces.Count().ToString());
            if (args.ResultFrame.DetectedFaces.Count() > 0)
            {
                var stream = new InMemoryRandomAccessStream();
                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

                var pictures = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                StorageFolder folder = pictures.SaveFolder;

                var guid = Guid.NewGuid();
                var file = await folder.CreateFileAsync(guid+".jpg", CreationCollisionOption.GenerateUniqueName);
                using (var inputStream = stream)
                {
                    var decoder = await BitmapDecoder.CreateAsync(inputStream);

                    using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

                        //var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(photoOrientation, PropertyType.UInt16) } };

                        //await encoder.BitmapProperties.SetPropertiesAsync(properties);
                        await encoder.FlushAsync();
                    }   
                }

                var file2 = await folder.GetFileAsync(guid + ".jpg");
                var s = await file2.OpenReadAsync();
                var persons = await _faceRecognitionService.FindPersonsInImage(s.AsStream());

                string text = "";
                if (persons.Count() > 0)
                {
                    text = "Hello ";
                }
                foreach (var p in persons)
                {
                    text += p.FirstName + " ";
                }
                Debug.WriteLine(text);
                await file2.DeleteAsync();


                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    HelloText = text;
                });


                //var persons = await _faceRecognitionService.FindPersonsInImage(stream.AsStream());
            }

        }

        /// <summary>
        /// Stops the preview and deactivates a display request, to allow the screen to go into power saving modes
        /// </summary>
        /// <returns></returns>
        private async Task StopPreviewAsync()
        {
            // Stop the preview
            //_previewProperties = null;
            await _mediaCapture.StopPreviewAsync();

            CaptureElement.Source = null;
            
            // Use the dispatcher because this method is sometimes called from non-UI threads
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    // Cleanup the UI
            //    PreviewControl.Source = null;

            //    // Allow the device screen to sleep now that the preview is stopped
            //    _displayRequest.RequestRelease();
            //});
        }
    }
}
