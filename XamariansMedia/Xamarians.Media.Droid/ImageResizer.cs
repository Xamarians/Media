using Android.OS;
using Android.Content;
using Android.Graphics;
using Android.Media;

namespace Xamarians.Media.Droid
{
    internal class ImageResizer
    {
        #region Private Methods

        private static Bitmap ScaleImageToBitmap(string filePath, int reqWidth, int reqHeight)
        {
            // Get the dimensions of the View

            // Get the dimensions of the bitmap
            BitmapFactory.Options bmOptions = new BitmapFactory.Options();
            bmOptions.InJustDecodeBounds = true;
            BitmapFactory.DecodeFile(filePath, bmOptions);
            int photoW = bmOptions.OutWidth;
            int photoH = bmOptions.OutHeight;

            // Determine how much to scale down the image
            // int scaleFactor = Java.Lang.Math.Min(photoW / reqWidth, photoH / reqHeight);

            int inSampleSize = 1;
            if (reqHeight < photoH || reqWidth < photoW)
            {
                inSampleSize = reqWidth > reqHeight
                                   ? photoH / reqHeight
                                   : photoW / reqWidth;
            }
            else
            {
                return null;
            }
            // Decode the image file into a Bitmap sized to fill the View
            bmOptions.InJustDecodeBounds = false;
            bmOptions.InSampleSize = inSampleSize;
            bmOptions.InPurgeable = true;

            // resize image in memory to nearby width and height
            var bitmap = BitmapFactory.DecodeFile(filePath, bmOptions);


            try
            {
                // try to resize in exact pixels
                bitmap = Bitmap.CreateScaledBitmap(bitmap, reqWidth, reqHeight, false);
            }
            catch
            {

            }
            return bitmap;
        }

        private static void CopyAttributes(Context context, string sourcePath, string destPath, int reqWidth, int reqHeight)
        {

            ExifInterface oldexif = new ExifInterface(sourcePath);
            ExifInterface newexif = new ExifInterface(destPath);

            int build = (int)Build.VERSION.SdkInt;

            if (oldexif.GetAttribute("FNumber") != null)
            {
                newexif.SetAttribute("FNumber",
                        oldexif.GetAttribute("FNumber"));
            }
            if (oldexif.GetAttribute("ExposureTime") != null)
            {
                newexif.SetAttribute("ExposureTime",
                        oldexif.GetAttribute("ExposureTime"));
            }
            if (oldexif.GetAttribute("ISOSpeedRatings") != null)
            {
                newexif.SetAttribute("ISOSpeedRatings",
                        oldexif.GetAttribute("ISOSpeedRatings"));
            }

            if (oldexif.GetAttribute("DateTime") != null)
            {
                newexif.SetAttribute("DateTime",
                        oldexif.GetAttribute("DateTime"));
            }
            if (oldexif.GetAttribute("Flash") != null)
            {
                newexif.SetAttribute("Flash",
                        oldexif.GetAttribute("Flash"));
            }
            if (oldexif.GetAttribute("GPSLatitude") != null)
            {
                newexif.SetAttribute("GPSLatitude",
                        oldexif.GetAttribute("GPSLatitude"));
            }
            if (oldexif.GetAttribute("GPSLatitudeRef") != null)
            {
                newexif.SetAttribute("GPSLatitudeRef",
                        oldexif.GetAttribute("GPSLatitudeRef"));
            }
            if (oldexif.GetAttribute("GPSLongitude") != null)
            {
                newexif.SetAttribute("GPSLongitude",
                        oldexif.GetAttribute("GPSLongitude"));
            }
            if (oldexif.GetAttribute("GPSLatitudeRef") != null)
            {
                newexif.SetAttribute("GPSLongitudeRef",
                        oldexif.GetAttribute("GPSLongitudeRef"));
            }
            //Need to update it, with your new height width
            newexif.SetAttribute("ImageLength", reqHeight.ToString());
            newexif.SetAttribute("ImageWidth", reqWidth.ToString());

            if (oldexif.GetAttribute("Make") != null)
            {
                newexif.SetAttribute("Make",
                        oldexif.GetAttribute("Make"));
            }
            if (oldexif.GetAttribute("Model") != null)
            {
                newexif.SetAttribute("Model",
                        oldexif.GetAttribute("Model"));
            }
            if (oldexif.GetAttribute("Orientation") != null)
            {
                newexif.SetAttribute("Orientation",
                        oldexif.GetAttribute("Orientation"));
            }
            if (oldexif.GetAttribute("WhiteBalance") != null)
            {
                newexif.SetAttribute("WhiteBalance",
                        oldexif.GetAttribute("WhiteBalance"));
            }
            newexif.SaveAttributes();

            Android.Provider.MediaStore.Images.Media.InsertImage(context.ContentResolver, destPath, "", "");
        }

        private static void SaveBitmap(Bitmap bitmap, string outputFilePath, int quality = 80)
        {
            using (var fs = new System.IO.FileStream(outputFilePath, System.IO.FileMode.Create))
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, quality, fs);
                bitmap.Recycle();
                bitmap.Dispose();
            }
        }

        #endregion

        #region Public Methods

        public static void ResizeImage(Context context, string sourceFilePath, string outputFilePath, int reqWidth, int reqHeight)
        {
            var bitmap = ScaleImageToBitmap(sourceFilePath, reqWidth, reqHeight);
            if (bitmap != null)
                SaveBitmap(bitmap, outputFilePath);

            //if (sourceFilePath != outputFilePath)
            //    CopyAttributes(context, sourceFilePath, outputFilePath, actualWidth, actualHeight);

        }

        #endregion

    }
}