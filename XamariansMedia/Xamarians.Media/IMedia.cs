using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Xamarians.Media.Droid")]
[assembly: InternalsVisibleTo("Xamarians.Media.iOS")]
namespace Xamarians.Media
{
    public interface IMediaService
    {
        /// <summary>
        /// Open camera to click picture. Camera pictures can not be saved into personal folder.
        /// </summary>
        Task<MediaResult> TakePhotoAsync(CameraOption option);

        /// <summary>
        /// Open media picker to choose Photo or Video or Mp3 or Dcument files 
        /// </summary>
        /// <param name="fileType">Image, Audio, Video, Document</param>
        /// <returns></returns>
        Task<MediaResult> OpenMediaPickerAsync(MediaType fileType);

        /// <summary>
        /// Delete file if has sufficient permission to delete.
        /// </summary>
        /// <param name="filePath">File path to delete</param>
        Task<bool> DeleteFileAsync(string filePath);       
        /// <summary>
        /// Resize image to given width and height by maintaining aspect ratio.
        /// </summary>
        /// <param name="sourceFilePath">Source file to resize</param>
        /// <param name="outputFilePath">Output file after resize</param>
        /// <param name="reqWidth">Resize Width</param>
        /// <param name="reqHeight">Resize Height</param>
        /// <returns></returns>
        Task<bool> ResizeImageAsync(string sourceFilePath, string outputFilePath, int reqWidth, int reqHeight);

        /// <summary>
        /// Get given directory path if exists else return null;
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        string GetPublicDirectoryPath();


        ///// <summary>
        ///// Create new directory under parent directory and return full path.
        ///// </summary>
        //string CreateDirectory(string directoryName, DeviceDirectory parentDirectory = DeviceDirectory.Public);

      
        /// <summary>
        /// Generate unique file name based on timestamp.
        /// </summary>
        /// <param name="ext">e.g. png, jpeg, mp3, mp3</param>
        /// <returns></returns>
        string GenerateUniqueFileName(string ext);

    }

    //public enum DeviceDirectory
    //{
    //    Public = 0,
    //    Pictures = 1,
    //    Music = 2,
    //    Movies = 3,
    //    Documents = 4,
    //    Downloads = 5,
    //}
}
