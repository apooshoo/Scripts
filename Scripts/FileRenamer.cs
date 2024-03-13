using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;

namespace Scripts
{
    public static class FileRenamer
    {
        public static void KeepFirstXAndLastYCharacters(string folderPath, int x, int y, 
            IProducerConsumerCollection<string> log)
        {
            var fullFileNames = FileSystem.GetFiles(folderPath);
            var files = fullFileNames.Select(f => FileSystem.GetFileInfo(f));
            List<FileInfo> filesToProcess = FilterOutInvalidFiles(files, x, y, log);
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

        private static void FormatFileNames(List<FileInfo> filesToProcess, int x, int y)
        {
            foreach (var file in filesToProcess)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Name);

                //store until x
                var firstX = string.Empty;
                if (x > 0)
                {
                    var rangeX = new Range(0, x);
                    firstX = fileName[rangeX];
                }

                //If y, store backwards until y
                var lastY = string.Empty;
                if (y > 0)
                {
                    var rangeY = new Range(^y, ^0);
                    lastY = fileName[rangeY];
                }

                var newFileName = Path.Combine(file.DirectoryName, firstX + lastY + file.Extension);
                if (!file.FullName.Equals(newFileName))
                {
                    file.MoveTo(newFileName);
                }
            }
        }

        private static List<FileInfo> FilterOutInvalidFiles(IEnumerable<FileInfo> files, int x, int y, 
            IProducerConsumerCollection<string> log)
        {
            var filesToProcess = new List<FileInfo>();

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Name);

                if (fileName.Length < x)
                {
                    continue;
                }
                else if (fileName.Length < x + y)
                {
                    log.TryAdd("Error: Invalid parameters for: " + fileName);
                    throw new ArgumentOutOfRangeException(nameof(y));
                }
                else if (x == 0 && y == 0)
                {
                    log.TryAdd("Error: Invalid parameters for: " + fileName);
                    throw new InvalidOperationException("Nothing to do.");
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
