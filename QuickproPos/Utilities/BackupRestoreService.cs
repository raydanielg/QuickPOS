namespace QuickproPos.Utilities
{
    public class BackupRestoreService
    {
        public async Task BackupDatabase(string sourcePath)
        {
            try
            {
                // Open a File Save Picker for the user to choose the backup destination
                string destinationPath = await PickSaveFilePathAsync("Backup.db3");

                if (!string.IsNullOrEmpty(destinationPath))
                {
                    File.Copy(sourcePath, destinationPath, overwrite: true);
                    await App.Current.MainPage.DisplayAlert("Backup", "Database backup successful!", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Backup failed: {ex.Message}", "OK");
            }
        }

        public async Task RestoreDatabase(string destinationPath)
        {
            try
            {
                // Open a File Open Picker for the user to select a backup file
                string sourcePath = await PickOpenFilePathAsync();

                if (!string.IsNullOrEmpty(sourcePath))
                {
                    File.Copy(sourcePath, destinationPath, overwrite: true);
                    await App.Current.MainPage.DisplayAlert("Restore", "Database restore successful!", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Restore failed: {ex.Message}", "OK");
            }
        }

        // Windows-specific file save picker
        private async Task<string> PickSaveFilePathAsync(string suggestedFileName)
        {
#if WINDOWS
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            var hwnd = ((MauiWinUIWindow)Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            savePicker.SuggestedFileName = suggestedFileName;
            savePicker.FileTypeChoices.Add("SQLite Database", new List<string> { ".db3" });

            var file = await savePicker.PickSaveFileAsync();
            return file?.Path;
#else
        throw new PlatformNotSupportedException("Save picker is only implemented for Windows.");
#endif
        }

        // Windows-specific file open picker
        private async Task<string> PickOpenFilePathAsync()
        {
#if WINDOWS
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            var hwnd = ((MauiWinUIWindow)Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

            openPicker.FileTypeFilter.Add(".db3");

            var file = await openPicker.PickSingleFileAsync();
            return file?.Path;
#else
        throw new PlatformNotSupportedException("Open picker is only implemented for Windows.");
#endif
        }
    }
}