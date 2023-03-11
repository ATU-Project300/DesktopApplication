using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey
{
    public class Find
    {
        // Generic function to search a directory for a file
        public static string FindFile(string? directory, string? fileName)
        {
            if (directory != null && directory.Length > 0)
            {
                var directoryInfo = new DirectoryInfo(directory);
                var files = directoryInfo.GetFiles();

                // If the file name is short, reduce the expected likeness such
                // that we are more likely to get a match. (See "Halo 3")
                if (fileName != null)
                {
                    double expectedLikeness = fileName.Length switch
                    {
                        < 3 => 55,
                        < 4 => 60,
                        < 7 => 65,
                        > 12 => 70,
                        _ => 60
                    };

                    foreach (var file in files)
                    {
                        if (CompareStrings(file.Name, fileName) > expectedLikeness)
                            return file.FullName;
                    }
                }
            }

            return "Invalid";
        }

        // Generic function to search a directory for another directory
        public static string FindFolder(string? directory, string? folderName)
        {
            if (directory != null && directory.Length > 0)
            {
                var folderInfo = new DirectoryInfo(directory);
                var folder = folderInfo.GetDirectories();

                // If the file name is short, reduce the expected likeness such
                // that we are more likely to get a match. (See "Halo 3")
                if (folderName != null)
                {
                    double expectedLikeness = folderName.Length switch
                    {
                        < 4 => 20,
                        < 7 => 30,
                        > 12 => 80,
                        _ => 60
                    };

                    foreach (var dir in folder)
                        if (CompareStrings(dir.Name, folderName) > expectedLikeness)
                            return dir.FullName;
                }
            }

            return "Invalid";
        }

        // Generic function to compare two strings and return a likeness percentage
        public static double CompareStrings(string str1, string? str2)
        {
            // Split the strings into words
            var words1 = str1.Split(' ', '-', '_', '.');
            var words2 = str2?.Split(' ', '-', '_', '.');

            // Create a list to store the matching words
            var matchingWords = new List<string>();

            // Loop through the words and compare them
            foreach (var word1 in words1)
                foreach (var word2 in words2)
                    // If the words match, add them to the list, the check also ignores case
                    if (string.Equals(word1, word2, StringComparison.OrdinalIgnoreCase))
                        matchingWords.Add(word1);

            // Set a max length for the strings
            var maxLength = Math.Min(words1.Length, words2.Length);

            // Calculate the likeness percentage
            var likeness = (double)matchingWords.Count / maxLength * 100;

            if (likeness > 0)
                Trace.WriteLine($"[INFO]: Likeness: {likeness}. {str1} VS {str2}.");

            return likeness;
        }
    }
}
