using Foundation;
using MediaPlayer;
using MobileCoreServices;
using System;
using UIKit;

namespace Xamarians.Media.iOS
{

    class ImagePickerController : UIImagePickerControllerDelegate
    {
        UIImagePickerController picker;
        Action<NSDictionary> _callback;

        public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
        {
            picker.DismissModalViewController(true);
            _callback?.Invoke(info);
        }

        public override void Canceled(UIImagePickerController picker)
        {
            picker.DismissModalViewController(true);
            _callback?.Invoke(null);
        }

        public ImagePickerController()
        {
            picker = new UIImagePickerController();
            picker.Delegate = this;
        }

        public void TakePhotoAsync(UIViewController parent, Action<NSDictionary> completed)
        {
            if (!UIImagePickerController.IsCameraDeviceAvailable(UIImagePickerControllerCameraDevice.Front | UIImagePickerControllerCameraDevice.Rear))
            {
                completed(null);
                return;
            }

            _callback = completed;
            picker.SourceType = UIImagePickerControllerSourceType.Camera;
            picker.MediaTypes = new string[] { UTType.Image };
            parent.PresentModalViewController(picker, true);
        }

        public void OpenImagePickerAsync(UIViewController parent, Action<NSDictionary> completed)
        {
            _callback = completed;
            picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            picker.MediaTypes = new string[] { UTType.Image };
            parent.PresentModalViewController(picker, true);
        }

        public void OpenVideoPickerAsync(UIViewController parent, Action<NSDictionary> completed)
        {
            _callback = completed;
            picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            picker.MediaTypes = new string[] { UTType.Movie };
            parent.PresentModalViewController(picker, true);
        }

    }
    
}