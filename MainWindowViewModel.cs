using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MirukuSorter
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MediaMetaHolder> _mediaFiles;

        public ObservableCollection<MediaMetaHolder> MediaFiles
        {
            get => _mediaFiles;
            set
            {
                if (_mediaFiles != value)
                {
                    _mediaFiles = value;
                    OnPropertyChanged(nameof(MediaFiles));
                }
            }
        }
        private string _loadedDirectory = "Scan Directory for Photos and Videos";
        public string LoadedDirectory
        {
            get => _loadedDirectory;
            set
            {
                if (_loadedDirectory != value)
                {
                    _loadedDirectory = value;
                    OnPropertyChanged(nameof(LoadedDirectory));
                }
            }
        }

        public MainWindowViewModel()
        {
            MediaFiles = new ObservableCollection<MediaMetaHolder>();
        }
        public void CopySortedPhoto()
        {
            CopySortedMedia("Photo");
        }

        public void CopySortedVideo()
        {
            CopySortedMedia("Video");
        }
        public void CopySortedMedia(string mediaType)
        {
            // Check if MediaFiles is empty
            if (MediaFiles == null || MediaFiles.Count == 0)
            {
                System.Windows.MessageBox.Show("No media files are available to copy. Please load media files first.",
                                               "No Media Files",
                                               System.Windows.MessageBoxButton.OK,
                                               System.Windows.MessageBoxImage.Warning);
                return;
            }

            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "Sorted Files Will be Copied into the Selected Folder",
                Multiselect = false,
            };

            if (openFolderDialog.ShowDialog() == true)
            {
                int copiedFilesCount = 0;

                foreach (var mediaFile in MediaFiles)
                {
                    if (mediaFile.MediaType == mediaType)
                    {
                        MediaCopySorter.CopyMediaFiles(mediaFile.MediaPath, mediaFile.MediaDate, openFolderDialog.FolderName);
                        copiedFilesCount++;
                    }
                }

                // Show a message box with the result
                string message = copiedFilesCount > 0
                    ? $"{copiedFilesCount} {mediaType.ToLower()} file(s) have been successfully copied to {openFolderDialog.FolderName}."
                    : $"No {mediaType.ToLower()} files were found to copy.";
                System.Windows.MessageBox.Show(message, "Copy Completed", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        public void LoadMediaFiles()
        {
            try
            {
                OpenFolderDialog openFolderDialog = new OpenFolderDialog
                {
                    Title = "Select a Folder to Scan",
                    Multiselect = false,
                };

                if (openFolderDialog.ShowDialog() == true)
                {
                    string selectedFolder = openFolderDialog.FolderName;
                    LoadedDirectory = selectedFolder; // Update the directory path

                    // Supported file extensions for photos and videos
                    string[] photoExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                    string[] videoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv" };

                    // Scan the folder for media files
                    List<string> photoPaths = MediaDiscoverer.DiscoverPhotoFiles(selectedFolder, true);
                    List<string> videoPaths = MediaDiscoverer.DiscoverVideoFiles(selectedFolder, true);
                    List<MediaMetaHolder> mediaMetaHolders = new List<MediaMetaHolder>();

                    // Populate collection
                    foreach (var photoPath in photoPaths)
                    {
                        mediaMetaHolders.Add(MediaDataReader.ReadPhotoDate(photoPath));
                    }
                    foreach (var videoPath in videoPaths)
                    {
                        mediaMetaHolders.Add(MediaDataReader.ReadVideoDate(videoPath));
                    }

                    // Sort mediaMetaHolders by MediaPath (ascending)
                    var sortedMediaMetaHolders = mediaMetaHolders
                        .OrderBy(holder => holder.MediaDate)
                        .ToList();

                    // Update the MediaFiles collection with the sorted data
                    MediaFiles.Clear();
                    foreach (var mediaMetaHolder in sortedMediaMetaHolders)
                    {
                        MediaFiles.Add(mediaMetaHolder);
                    }
                }
            }
            catch (Exception ex)
            {
                LoadedDirectory = "Scan Directory for Photos and Videos";
                MediaFiles.Clear(); // Clear the existing MediaFiles collection
                                    // Handle exceptions (e.g., folder not found, access denied)
                System.Windows.MessageBox.Show($"An error occurred while loading media files: {ex.Message}",
                                               "Error",
                                               System.Windows.MessageBoxButton.OK,
                                               System.Windows.MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}