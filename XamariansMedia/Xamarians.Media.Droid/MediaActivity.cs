using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;

[assembly: UsesPermission("android.permission.CAMERA")]
namespace Xamarians.Media.Droid
{
    [Activity(Label = "Media Activity")]
    internal class MediaActivity : Activity
    {
        const int RequestCodeCamera = 10001;
        const int RequestCodeGallery = 10002;

        string _filePath;
        string _activityType;
        int _maxWidth;
        int _maxHeight;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            _activityType = Intent.GetStringExtra("ActivityType");
            _maxHeight = Intent.GetIntExtra("MaxWidth", 0);
            _maxWidth = Intent.GetIntExtra("MaxHeight", 0);
        }

        protected override void OnStart()
        {
            base.OnStart();
            Init();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch (requestCode)
            {
                case RequestCodeCamera:
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        OpenCameraToTakePhoto();
                        return;
                    }
                    break;
            }
            Finish();
        }

        private void Init()
        {
            switch (_activityType)
            {
                case ActivityType.TakePhoto:
                    _filePath = Intent.GetStringExtra("FilePath");
                    // Check Permission MarshMelow and higer
                    if (Build.VERSION.SdkInt > BuildVersionCodes.LollipopMr1 && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Denied)
                    {
                        RequestPermissions(new string[] { Manifest.Permission.Camera }, RequestCodeCamera);
                        return;
                    }
                    OpenCameraToTakePhoto();
                    break;

                case ActivityType.MediaPicker:
                    var fileType = Intent.GetStringExtra("FileType");
                    if (fileType == MediaType.Image.ToString())
                        OpenGallery("image/*", "Choose Image");
                    else if (fileType == MediaType.Audio.ToString())
                        OpenGallery("audio/*", "Choose Audio");
                    else if (fileType == MediaType.Video.ToString())
                        OpenGallery("video/*", "Choose Video");
                    //else if (fileType == MediaType.Document.ToString())
                    //    OpenGallery("document/*", "Choose File");
                    break;
            }
        }

        private void OpenCameraToTakePhoto()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(new Java.IO.File(_filePath)));
            StartActivityForResult(intent, RequestCodeCamera);
        }

        private void OpenGallery(string fileType, string title)
        {
            var intent = new Intent(Intent.ActionGetContent);
            intent.SetType(fileType);
            StartActivityForResult(Intent.CreateChooser(intent, title), RequestCodeGallery);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != Result.Ok)
            {
                MediaServiceAndroid.SetResult(new MediaResult(false) { Message = resultCode.ToString() });
                Finish();
                return;
            }

            switch (requestCode)
            {
                case RequestCodeCamera:
                    if (_maxWidth > 0 && _maxHeight > 0)
                        ImageResizer.ResizeImage(this, _filePath, _filePath, _maxWidth, _maxHeight);
                    MediaServiceAndroid.SetResult(new MediaResult(true) { FilePath = _filePath });
                    break;
                case RequestCodeGallery:
                    MediaServiceAndroid.SetResult(new MediaResult(true) { FilePath = RealPathHelper.GetPath(this, data.Data) });
                    break;
            }
            Finish();
        }

    }


}