using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;

namespace MirukuSorter
{
    static class MediaDataReader
    {
        public static readonly string[] photoExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp", ".heic", ".heif" };
        public static readonly string[] videoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".3gp", ".m4v" };

        public static MediaMetaHolder ReadPhotoDate(string filePath)
        {
            var mediaMeta = new MediaMetaHolder
            {
                MediaType = "Photo",
                MediaPath = filePath
            };

            try
            {
                // Attempt to read metadata for photo
                var directories = ImageMetadataReader.ReadMetadata(filePath);
                var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                if (subIfdDirectory != null && subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTaken))
                {
                    mediaMeta.MediaDate = dateTaken.ToString("yyyy.MM");
                    return mediaMeta;
                }
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                System.Diagnostics.Debug.WriteLine($"Error reading photo metadata: {ex.Message}");
            }

            // Fallback to file modified date
            var modifiedDate = File.GetLastWriteTime(filePath);
            mediaMeta.MediaDate = modifiedDate.ToString("yyyy.MM");
            return mediaMeta;
        }

        public static MediaMetaHolder ReadVideoDate(string filePath)
        {
            var mediaMeta = new MediaMetaHolder
            {
                MediaType = "Video",
                MediaPath = filePath
            };

            try
            {
                // Attempt to read metadata for video
                var directories = ImageMetadataReader.ReadMetadata(filePath);

                // Check for QuickTime metadata (e.g., .mov, .mp4, .m4v)
                var quickTimeDirectory = directories.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();
                if (quickTimeDirectory != null && quickTimeDirectory.TryGetDateTime(QuickTimeMovieHeaderDirectory.TagCreated, out var quickTimeDate))
                {
                    // Validate the extracted date
                    if (quickTimeDate.Year >= 1970) // Use 1970 as a reasonable threshold
                    {
                        mediaMeta.MediaDate = quickTimeDate.ToString("yyyy.MM");
                        return mediaMeta;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                System.Diagnostics.Debug.WriteLine($"Error reading video metadata: {ex.Message}");
            }

            // Fallback to file modified date
            var modifiedDate = File.GetLastWriteTime(filePath);
            mediaMeta.MediaDate = modifiedDate.ToString("yyyy.MM");
            return mediaMeta;
        }
    }
}