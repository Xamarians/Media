using System;
using System.IO;
using Xamarians.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaTestPage : ContentPage
    {
        public MediaTestPage()
        {
            InitializeComponent();
        }

        private string GenerateFilePath()
        {
			return Path.Combine(MediaService.Instance.GetPublicDirectoryPath(), MediaService.Instance.GenerateUniqueFileName("jpg"));
        }

        private async void TakePhoto_Clicked(object sender, EventArgs e)
        {
            string filePath = GenerateFilePath();
            var result = await MediaService.Instance.TakePhotoAsync(new CameraOption()
            {
                FilePath = filePath,
                MaxWidth = 200,
                MaxHeight = 200
            });
            if (result.IsSuccess)
                image.Source = result.FilePath;
            else
                await DisplayAlert("Error", result.Message, "OK");
        }

        private async void ChooseImage_Clicked(object sender, EventArgs e)
        {
            var result = await MediaService.Instance.OpenMediaPickerAsync(MediaType.Image);
            if (result.IsSuccess)
                image.Source = result.FilePath;
            else
                await DisplayAlert("Error", result.Message, "OK");
        }

        private async void ChooseFile_Clicked(object sender,EventArgs e)
        {
            var result = await MediaService.Instance.OpenMediaPickerAsync(MediaType.Documents);
            if (result.IsSuccess)
                await DisplayAlert("Success", result.FilePath, "OK");
            else
                await DisplayAlert("Error", result.Message, "OK");
        }
        private async void ChooseVideo_Clicked(object sender, EventArgs e)
        {
            var result = await MediaService.Instance.OpenMediaPickerAsync(MediaType.Video);
            if (result.IsSuccess)
                await DisplayAlert("Success", result.FilePath, "OK");
            else
                await DisplayAlert("Error", result.Message, "OK");
        }

        
        private async void ChooseAudio_Clicked(object sender, EventArgs e)
        {
            var result = await MediaService.Instance.OpenMediaPickerAsync(MediaType.Audio);
            if (result.IsSuccess)
                await DisplayAlert("Success", result.FilePath, "OK");
            else
                await DisplayAlert("Error", result.Message, "OK");
        }

        private async void ResizeImage_Clicked(object sender, EventArgs e)
        {
            var result = await MediaService.Instance.OpenMediaPickerAsync(MediaType.Image);
            string resizeFilePath = GenerateFilePath();
            var success = await MediaService.Instance.ResizeImageAsync(result.FilePath, resizeFilePath, 100, 100);
            if (success)
            {
                image.Source = resizeFilePath;
            }
        }
    }
}