﻿using Common;
using Scripter.Models;
using Scripter.Models.Ui;
using Scripts;
using System.Collections.Concurrent;

namespace Scripter.Services
{
    public static class ScriptService
    {
        public static void Trim(FolderSelection[] folders, string trimLeftStr, string trimRightStr,
            ConcurrentQueue<string> log)
        {
            if (int.TryParse(trimLeftStr, out var trimLeft)
                && int.TryParse(trimRightStr, out var trimRight)
                && trimLeft + trimRight > 0)
            {
                foreach (var folder in folders)
                {
                    FileRenamer.KeepFirstXAndLastYCharacters(folder.Name, trimLeft, trimRight, log);
                }
            }
        }

        public static void Normalise(FolderSelection[] folders)
        {
            foreach (var folder in folders)
            {
                FileRenamer.RemoveNonNumbers(folder.Name);
                FileRenamer.Fill(folder.Name);
            }
        }

        public static void Reseed(FolderSelection[] folders, string reseedValueStr, ReseedOrderSelectionEnum reseedOrder)
        {
            if (int.TryParse(reseedValueStr, out var reseedValue) && reseedValue >= 0)
            {
                foreach (var folder in folders)
                {
                    switch (reseedOrder)
                    {
                        case ReseedOrderSelectionEnum.FileName:
                            FileRenamer.ReseedFiles(folder.Name, reseedValue);
                            break;
                        case ReseedOrderSelectionEnum.CreationDate:
                            FileRenamer.ReseedFilesByCreationDate(folder.Name, reseedValue);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(reseedOrder));
                    }
                }
            }
        }

        public static void Convert(FolderSelection[] folders, IProducerConsumerCollection<string> log)
        {
            foreach (var folder in folders)
            {
                try
                {
                    log.TryAdd("Converting: " + folder.Name);
                    ImageConverter.Convert(folder.Name, ImageFormat.WEBP, ImageFormat.JPEG);
                    log.TryAdd("Done converting.");
                }
                catch (Exception e)
                {
                    log.TryAdd($"Error while converting for: {folder} : {e.Message}");
                }
            }
        }
    }
}
