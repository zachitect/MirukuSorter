using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirukuSorter
{
    static class MediaCopySorter
    {
        public static void CopyMediaFiles(string mediaFile, string creationDate, string targetDirectory)
        {
            try
            {
                // Ensure the target directory exists
                string targetFolder = Path.Combine(targetDirectory, creationDate);
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                // Construct the target file path
                string targetFilePath = Path.Combine(targetFolder, Path.GetFileName(mediaFile));

                // Copy the file to the target directory
                File.Copy(mediaFile, targetFilePath, overwrite: false);

                System.Diagnostics.Debug.WriteLine($"File copied: {mediaFile} -> {targetFilePath}");
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error copying file {mediaFile}: {ex.Message}");
            }
        }
    }
}
