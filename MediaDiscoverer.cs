using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MirukuSorter
{
    static class MediaDiscoverer
    {
        public static List<string> DiscoverPhotoFiles(string directoryPath, bool includeSubdirectories = false)
        {
            // Search for photo files in the directory
            SearchOption searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return Directory.EnumerateFiles(directoryPath, "*.*", searchOption)
                            .Where(file => MediaDataReader.photoExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
                            .ToList();
        }
        public static List<string> DiscoverVideoFiles(string directoryPath, bool includeSubdirectories = false)
        {
            // Define common video file extensions
            // Search for video files in the directory
            SearchOption searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return Directory.EnumerateFiles(directoryPath, "*.*", searchOption)
                            .Where(file => MediaDataReader.videoExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
                            .ToList();
        }
    }
}