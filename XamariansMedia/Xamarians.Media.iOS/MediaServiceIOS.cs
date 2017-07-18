using Foundation;
using System;
using System.Threading.Tasks;
using UIKit;
using Xamarians.Media.iOS;
using System.Linq;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(MediaServiceIOS))]
namespace Xamarians.Media.iOS
{
    public class MediaServiceIOS : IMediaService
    {

        public MediaServiceIOS()
        {

        }

        public static void Initialize()
        {
            MediaService.Init(new MediaServiceIOS());
        }

        #region Private Methods


        private static UIViewController GetController()
        {
            var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (vc.PresentedViewController != null && vc.PresentedViewController.ToString().Contains("Xamarin_Forms_Platform_iOS_ModalWrapper"))
                vc = vc.PresentedViewController;
            return vc;
        }

        #endregion


        #region IMediaService

        public Task<MediaResult> TakePhotoAsync(CameraOption option)
        {
			if (string.IsNullOrWhiteSpace(option.FilePath))
			{
				option.FilePath = System.IO.Path.Combine(GetPublicDirectoryPath(), GenerateUniqueFileName("jpeg"));
			}

            var task = new TaskCompletionSource<MediaResult>();
            try
            {

                var picker = new ImagePickerController();
                picker.TakePhotoAsync(GetController(), (nsDict) =>
                {
                    if (nsDict == null)
                    {
                        task.SetResult(new MediaResult(false) { Message = "Cancelled." });
                        return;
                    }
                    var photo = nsDict.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
                    if (option.MaxHeight > 0 && option.MaxWidth > 0)
                        photo.Scale(new CoreGraphics.CGSize(option.MaxWidth, option.MaxHeight));

                    try
                    {
						NSData imgData = photo.AsJPEG();
						NSError err = null;
						if (imgData.Save(option.FilePath, false, out err))
						{
							task.SetResult(new MediaResult(true) {FilePath = option.FilePath});
						}
						else
						{
							task.SetResult(null);
						}
                    }
                    catch (Exception ex)
                    {
                        task.SetResult(new MediaResult(false) { Message = ex.Message });
                    }

                });

            }
            catch (Exception ex)
            {
                task.SetResult(new MediaResult(false) { Message = ex.Message });
            }
            return task.Task;
        }

        public Task<MediaResult> OpenMediaPickerAsync(MediaType fileType)
        {

            var task = new TaskCompletionSource<MediaResult>();
            try
            {
                if (fileType == MediaType.Image)
                {
                    var picker = new ImagePickerController();
                    picker.OpenImagePickerAsync(GetController(), (nsdict) =>
                    {
                        if (nsdict == null)
                        {
                            task.SetResult(new MediaResult(false) { Message = "Cancelled." });
                            return;
                        }
                        try
                        {
                            var photoUrl = nsdict.ValueForKey(new NSString("UIImagePickerControllerReferenceURL")) as NSUrl;
                            var imageName = photoUrl.LastPathComponent;
                            var dir = NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User, true).FirstOrDefault();
                            var picUrl = new NSUrl(dir, true);
                            var localPath = picUrl.Append(imageName, false);
                            var localPath1 = picUrl.Append(imageName, true);
                            //task.SetResult(new MediaResult(true) { FilePath = localPath.ToString()});
                            var photo = nsdict.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
                            if (photo != null)
                            {
                                string fileName = System.IO.Path.Combine(GetPublicDirectoryPath(), imageName);
                                NSError err = null;
                                if (photo.AsJPEG().Save(fileName, false, out err))
                                {
                                    task.SetResult(new MediaResult(true) { FilePath = fileName });
                                }
                                else
                                    task.SetResult(null);
                            }
                            else
                                task.SetResult(null);

                        }

                        catch (Exception ex)
                        {
                            task.SetResult(new MediaResult(false) { FilePath = ex.Message });
                        }
                    });
                }
                else if (fileType == MediaType.Video)
                {
                    var picker = new ImagePickerController();
                    picker.OpenVideoPickerAsync(GetController(), (nsdict) =>
                    {
                        if (nsdict == null)
                        {
                            task.SetResult(new MediaResult(false) { Message = "Cancelled." });
                            return;
                        }
                        try
                        {
                            var mediaUrl = nsdict.ValueForKey(new NSString("UIImagePickerControllerMediaURL")) as NSUrl;
                            task.SetResult(new MediaResult(true) { FilePath = mediaUrl.ToString() });
                        }
                        catch (Exception ex)
                        {
                            task.SetResult(new MediaResult(false) { FilePath = ex.Message });
                        }
                    });
                }
                else if (fileType == MediaType.Documents)
                {
                    var picker = new ImagePickerController();
                    picker.OpenDoc(GetController(), (obj) =>
                    {
                        if (obj == null)
                        {
                            task.SetResult(new MediaResult(false) { Message = "Cancelled." });
                            return;
                        }
                        try
                        {
                            var aa = obj.AbsoluteUrl;
                            var isExist = System.IO.File.Exists(aa.AbsoluteString);
                            task.SetResult(new MediaResult(true) { FilePath = aa.Path });
                        }
                        catch (Exception ex)
                        {
                            task.SetResult(new MediaResult(false) { FilePath = ex.Message });
                        }
                    });
                }
                                   
                else
                {
                    var picker = new AudioPickerController();
                    picker.OpenAudioPickerAsync(GetController(), (nsurl) =>
                    {
                        if (nsurl == null)
                        {
                            task.SetResult(new MediaResult(false) { Message = "Cancelled." });
                            return;
                        }
                        task.SetResult(new MediaResult(true) { FilePath = nsurl.ToString() });
                    });
                }
            }
            catch (Exception ex)
            {
                task.SetResult(new MediaResult(false) { Message = ex.Message });
            }
            return task.Task;
        }

		public Task<bool> DeleteFileAsync(string filePath)
        {
            return Task.Run(() =>
            {
                NSFileManager nsf = new NSFileManager();
                NSError error = null;
                return nsf.Remove(filePath, out error);
            });
        }

        public Task<bool> ResizeImageAsync(string sourceFilePath, string outputFilePath, int reqWidth, int reqHeight)
        {
            return Task.Run(() =>
            {
                try
                {
					var photo = UIImage.FromFile(sourceFilePath);
					photo.Scale(new CoreGraphics.CGSize(reqWidth, reqHeight));
					return photo.AsJPEG().Save(outputFilePath, sourceFilePath == outputFilePath);
                }
                catch
                {

                    return false;
                }
            });
        }

		public string GetPublicDirectoryPath()
		{
			//string _dir = null;
			//         //string _dir = NSSearchPath.GetDirectories(NSSearchPathDirectory.SharedPublicDirectory, NSSearchPathDomain.Local, false).First();
			//         switch (directoryType)
			//         {
			//             case DeviceDirectory.Documents:
			//                 _dir = NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.Local, false).First();
			//                 break;
			//             case DeviceDirectory.Downloads:
			//                 _dir = NSSearchPath.GetDirectories(NSSearchPathDirectory.DownloadsDirectory, NSSearchPathDomain.Local, false).First();
			//                 break;
			//             case DeviceDirectory.Pictures:
			//                 _dir = NSSearchPath.GetDirectories(NSSearchPathDirectory.PicturesDirectory, NSSearchPathDomain.Local, false).First();
			//                 break;
			//             case DeviceDirectory.Music:
			//                 _dir = NSSearchPath.GetDirectories(NSSearchPathDirectory.MusicDirectory, NSSearchPathDomain.Local, false).First();
			//                 break;
			//             case DeviceDirectory.Movies:
			//                 _dir = NSSearchPath.GetDirectories(NSSearchPathDirectory.MoviesDirectory, NSSearchPathDomain.Local, false).First();
			//                 break;

			//	case DeviceDirectory.Public:
			//		_dir = NSSearchPath.GetDirectories(NSSearchPathDirectory.SharedPublicDirectory, NSSearchPathDomain.Local, false).First();
			//                 break;
			//         }
			//         return _dir;
			var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			if (!System.IO.Directory.Exists(documentsDirectory))
			{
				System.IO.Directory.CreateDirectory(documentsDirectory);
			}
			return documentsDirectory;
		}

   //     public string CreateDirectory(string directoryName, DeviceDirectory parentDirectory)
   //     {
   //         //var path = GetDirectoryPath(parentDirectory) + "/" + directoryName;
   //        // new NSFileManager().CreateDirectory(path, true, null);
			//var path = System.IO.Path.Combine(GetDirectoryPath(parentDirectory), directoryName);
   //         return path;
   //     }

        public string GenerateUniqueFileName(string ext)
        {
            if (!ext.StartsWith("."))
                ext = "." + ext;
            return string.Format("{0}{1}{2}", DateTime.Now.ToString("ddMMyyyy_hhmmssfff"), new System.Random().Next(11, 99), ext);
        }



		public string CreateDirectory(string directoryName)
		{
			throw new NotImplementedException();
		}

		#endregion


	}
}