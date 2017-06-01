using System;

using Foundation;
using UIKit;
using MediaPlayer;

namespace Xamarians.Media.iOS
{
    class AudioPickerController : MPMediaPickerControllerDelegate
    {
        MPMediaPickerController picker;
        Action<NSUrl> _callback;

        public AudioPickerController()
        {
            picker = new MPMediaPickerController(MPMediaType.Music);
            picker.Delegate = this;
            picker.AllowsPickingMultipleItems = false;
        }

        public void OpenAudioPickerAsync(UIViewController parent, Action<NSUrl> completed)
        {
            _callback = completed;
            parent.PresentModalViewController(picker, true);
        }


        #region Media Picker Delegate

        public override void MediaItemsPicked(MPMediaPickerController sender, MPMediaItemCollection mediaItemCollection)
        {
            var item = mediaItemCollection.RepresentativeItem;
            picker.DismissModalViewController(false);
            _callback?.Invoke(item?.AssetURL);
        }

        public override void MediaPickerDidCancel(MPMediaPickerController sender)
        {
            picker.DismissModalViewController(false);
            _callback?.Invoke(null);
        }

        #endregion
    }

}