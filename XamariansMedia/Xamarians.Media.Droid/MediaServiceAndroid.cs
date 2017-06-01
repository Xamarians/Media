using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Java.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Xamarians.Media.Droid
{
    public class MediaServiceAndroid : IMediaService
    {
        static TaskCompletionSource<MediaResult> _tcs;
        static readonly Context _context = Xamarin.Forms.Forms.Context;
        static File _file;

        public MediaServiceAndroid()
        {

        }

        public static void Initialize()
        {
            Media.MediaService.Init(new MediaServiceAndroid());
        }

        #region Private Methods

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = _context.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        #endregion

        internal static void SetResult(MediaResult result)
        {
            _tcs.TrySetResult(result);
        }

        #region Imedia

        public Task<MediaResult> TakePhotoAsync(CameraOption option)
        {
            if (!IsThereAnAppToTakePictures())
            {
                return Task.FromResult(new MediaResult(false) { Message = "Camera not supported." });
            }
            if (string.IsNullOrWhiteSpace(option.FilePath))
            {
                option.FilePath = string.Format("{0}/{1}", GetPublicDirectoryPath(), GenerateUniqueFileName("jpg"));
            }

            _tcs = new TaskCompletionSource<MediaResult>();
            _file = new File(option.FilePath);
            Intent intent = new Intent(_context, typeof(MediaActivity));
            intent.PutExtra("ActivityType", ActivityType.TakePhoto);
            intent.PutExtra("FilePath", _file.Path);
            intent.PutExtra("MaxWidth", option.MaxWidth);
            intent.PutExtra("MaxHeight", option.MaxHeight);
            _context.StartActivity(intent);
            return _tcs.Task;
        }

        public Task<MediaResult> OpenMediaPickerAsync(MediaType fileType)
        {
            _tcs = new TaskCompletionSource<MediaResult>();
            Intent intent = new Intent(_context, typeof(MediaActivity));
            intent.PutExtra("ActivityType", ActivityType.MediaPicker);
            intent.PutExtra("FileType", fileType.ToString());
            _context.StartActivity(intent);
            return _tcs.Task;
        }

        public Task<bool> DeleteFileAsync(string filePath)
        {
            return Task.Run(() =>
             {
                 return new Java.IO.File(filePath).Delete();
             });
        }

        public Task<bool> ResizeImageAsync(string sourceFilePath, string outputFilePath, int reqWidth, int reqHeight)
        {
            return Task.Run(() =>
            {
                try
                {
                    ImageResizer.ResizeImage(_context, sourceFilePath, outputFilePath, reqWidth, reqHeight);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        //public string CreateDirectory(string directoryName, DeviceDirectory parentDirectory)
        //{
        //    File _dir = new File(GetDirectoryPath(parentDirectory), directoryName);
        //    if (!_dir.Exists())
        //    {
        //        _dir.Mkdirs();
        //    }
        //    return _dir.Path;
        //}

        //public string GetDirectoryPath(DeviceDirectory directoryType)
        //{
        //    File _dir = Environment.ExternalStorageDirectory;
        //    switch (directoryType)
        //    {
        //        case DeviceDirectory.Documents:
        //            _dir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDocuments);
        //            break;
        //        case DeviceDirectory.Downloads:
        //            _dir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads);
        //            break;
        //        case DeviceDirectory.Pictures:
        //            _dir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures);
        //            break;
        //        case DeviceDirectory.Music:
        //            _dir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMusic);
        //            break;
        //        case DeviceDirectory.Movies:
        //            _dir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMovies);
        //            break;
        //    }
        //    return _dir.Path;
        //}

        public string GenerateUniqueFileName(string ext)
        {
            if (!ext.StartsWith("."))
                ext = "." + ext;
            return string.Format("{0}{1}{2}", System.DateTime.Now.ToString("ddMMyyyy_hhmmssfff"), new System.Random().Next(11, 99), ext);
        }

        public string GetPublicDirectoryPath()
        {
            return Android.OS.Environment.ExternalStorageDirectory.Path;
        }

        #endregion

    }

    class ActivityType
    {
        public const string TakePhoto = "TakePhoto";
        public const string MediaPicker = "MediaPicker";
    }




}
