﻿using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;

namespace Scripts
{
    public static class FileRenamer
    {
        public static void KeepFirstXAndLastYCharacters(string folderPath, int x, int y, 
            IProducerConsumerCollection<string> log)
        {
            try
            {
                var files = FileSystem.GetFiles(folderPath).Select(f => FileSystem.GetFileInfo(f));
                List<FileInfo> filesToProcess = FilterOutInvalidFiles(files, x, y);
                if (filesToProcess.Any())
                {
                    log.TryAdd("Trimming contents of: " + folderPath);
                    FormatFileNames(filesToProcess, x, y);
                }
                else
                {
                    log.TryAdd("Skipped trimming for: " + folderPath);
                }
            }
            catch (Exception e)
            {
                log.TryAdd($"Error while trimming for: {folderPath} : {e.Message}");
            }
        }

        private static void FormatFileNames(List<FileInfo> filesToProcess, int x, int y)
        {
            foreach (var file in filesToProcess)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Name);

                // Store until x
                var firstX = string.Empty;
                if (x > 0)
                {
                    var rangeX = new Range(0, x);
                    firstX = fileName[rangeX];
                }

                // If y, store backwards until y
                var lastY = string.Empty;
                if (y > 0)
                {
                    var rangeY = new Range(^y, ^0);
                    lastY = fileName[rangeY];
                }

                // Rename
                var newFileName = Path.Combine(file.DirectoryName, firstX + lastY + file.Extension);
                if (!file.FullName.Equals(newFileName))
                {
                    file.MoveTo(newFileName);
                }
            }
        }

        private static List<FileInfo> FilterOutInvalidFiles(IEnumerable<FileInfo> files, int x, int y)
        {
            var filesToProcess = new List<FileInfo>();

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Name);

                if (fileName.Length < x)
                {
                    continue;
                }
                else if (x == 0 && y == 0)
                {
                    continue;
                }
                else if (fileName.Length < x + y)
                {
                    throw new Exception($"Invalid parameters for: {fileName}");
                }

                filesToProcess.Add(file);
            }

            return filesToProcess;
        }

        public static void NormaliseFilenames(string path)
        {
            RemoveNonNumbers(path);
            Fill(path);
        }

        private static void RemoveNonNumbers(string path)
        {
            var fullFileNames = FileSystem.GetFiles(path);
            var files = fullFileNames.Select(f => FileSystem.GetFileInfo(f));

            //Remove all characters
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Name);
                var ignoredPrefix = fileName.Contains("-") ? fileName.Split("-").Last() : fileName;
                var newFileName = Path.Combine(file.DirectoryName, Regex.Replace(ignoredPrefix, "[^0-9]", "") + file.Extension);
                if (!file.FullName.Equals(newFileName))
                {
                    file.MoveTo(newFileName);
                }
            }
        }

        private static void Fill(string path)
        {
            //Fill
            var fullFileNames = FileSystem.GetFiles(path);
            var files = fullFileNames.Select(f => FileSystem.GetFileInfo(f));
            var maxLength = files.Any() ? files.Max(f => Path.GetFileNameWithoutExtension(f.Name).Length) : 0;
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Name);
                var digitsToFill = maxLength - fileName.Length;
                var stringToFill = "";
                while (digitsToFill > 0)
                {
                    stringToFill += "0";
                    digitsToFill--;
                }

                if (stringToFill.Length > 0)
                {
                    var newFileName = Path.Combine(file.DirectoryName, stringToFill + fileName + file.Extension);
                    if (!Path.GetFileName(fileName).Equals(newFileName))
                    {
                        file.MoveTo(newFileName);
                    }
                }
            }
        }
    }
}
