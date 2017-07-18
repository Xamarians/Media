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

        static Action<NSUrl> _callbackDoc;

        public  void OpenDoc(UIViewController parent, Action<NSUrl> callback)
        {
            _callbackDoc = callback;
            var allowedUTIs = new string[]
            {
            UTType.UTF8PlainText,
            UTType.PlainText,
            UTType.RTF,
            UTType.PNG,
            UTType.Text,
            UTType.PDF,
            UTType.Image,
            UTType.UTF16PlainText,
            UTType.FileURL,
            "com.microsoft.word.doc",
            "org.openxmlformats.wordprocessingml.document"

            };

            // Display the picker
            //var picker = new UIDocumentPickerViewController (allowedUTIs, UIDocumentPickerMode.Open);
            var pickerMenu = new UIDocumentMenuViewController(allowedUTIs, UIDocumentPickerMode.Import);

            pickerMenu.DidPickDocumentPicker += (sender, args) =>
            {

                // Wireup Document Picker
                args.DocumentPicker.DidPickDocument += (sndr, pArgs) =>
                {

                    // IMPORTANT! You must lock the security scope before you can
                    // access this file
                    var securityEnabled = pArgs.Url.StartAccessingSecurityScopedResource();

                    // Open the document
                    //ThisApp.OpenDocument(pArgs.Url);

                    // IMPORTANT! You must release the security lock established
                    // above.
                    pArgs.Url.StopAccessingSecurityScopedResource();

                    var cb = _callbackDoc;
                    _callbackDoc = null;
                    pickerMenu.DismissModalViewController(true);
                    cb(pArgs.Url);
                };

                // Display the document picker
                parent.PresentViewController(args.DocumentPicker, true, null);
            };

            pickerMenu.ModalPresentationStyle = UIModalPresentationStyle.Popover;
            parent.PresentViewController(pickerMenu, true, null);
            UIPopoverPresentationController presentationPopover = pickerMenu.PopoverPresentationController;
            if (presentationPopover != null)
            {
                presentationPopover.SourceView = parent.View;
                presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Down;
                //presentationPopover.SourceRect = ((UIButton)).Frame;
            }
        }
    }
}