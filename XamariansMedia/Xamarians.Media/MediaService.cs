namespace Xamarians.Media
{
    public static class MediaService
    {
        static IMediaService _instance;
        public static IMediaService Instance
        {
            get
            {
                return _instance;
            }
        }

        internal static void Init(IMediaService media)
        {
            _instance = media;
        }

    }

    public class MediaResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string FilePath { get; set; }

        public MediaResult(bool success)
        {
            IsSuccess = success;
        }
    }

    public class CameraOption
    {
        /// <summary>
        /// Resize image to given width. Default is 0 for original width.
        /// </summary>
        public int MaxWidth { get; set; }
        /// <summary>
        /// Resize image to given height. Default is 0 for original height.
        /// </summary>
        public int MaxHeight { get; set; }

        /// <summary>
        /// output file path to save captured image.
        /// </summary>
        public string FilePath { get; set; }

    }

    public enum MediaType
    {
        Image, Audio, Video 
    }

}
