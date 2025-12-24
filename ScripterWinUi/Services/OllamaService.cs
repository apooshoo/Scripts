using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ScripterWinUi.Services;

/// <summary>
/// Service for communicating with a locally-running Ollama LLM instance
/// </summary>
public class OllamaService
{
    private static readonly HttpClient _httpClient = new();
    private const string DefaultBaseUrl = "http://localhost:11434";
    //private const string DefaultModel = "llama3.1:8b"; // too big
    private const string DefaultModel = "llama3.2:3b"; // better, more lightweight
    private const int MaxFilesToAnalyze = 500;

    public string BaseUrl { get; set; } = DefaultBaseUrl;
    public string Model { get; set; } = DefaultModel;

    /// <summary>
    /// Analyzes file names and suggests rename mappings for uniform-length, zero-based file naming
    /// </summary>
    public async Task<RenameSuggestion> SuggestRenamesAsync(
        IEnumerable<string> fileNames,
        CancellationToken cancellationToken = default)
    {
        var fileNameList = fileNames.ToList();
        
        if (fileNameList.Count == 0)
        {
            return new RenameSuggestion([], "No files to analyze");
        }

        var prompt = BuildRenameSuggestionPrompt(fileNameList);
        var response = await SendPromptAsync(prompt, cancellationToken);
        
        return ParseRenameSuggestionResponse(response, fileNameList);
    }

    private static string BuildRenameSuggestionPrompt(List<string> fileNames)
    {
        if (fileNames.Count > MaxFilesToAnalyze) 
            throw new ArgumentOutOfRangeException(nameof(fileNames.Count));
        var fileListText = string.Join(";", fileNames.Take(MaxFilesToAnalyze)); 
        var fileCount = fileNames.Count;
        var digitCount = fileCount.ToString().Length;
        
        return $$"""
            You are a file renamer. Your goal is to rename all files in the Input File Names section to uniform-length, zero-padded, sequential numbers.
            
            Rules:
            - Output numbers should be zero-padded to {{digitCount}} digits (based on {{fileCount}} files), eg. 0 => 000, 01 => 001, ..., 100 => 100 for 100 files
            - Start numbering from 0 by default, but also accept 1 as a starting point if existing numbering begins at 1 (eg. if files are named 1, 2, 3, use 1 as a starting point, but if files are named 2, 3, 4, use 0 as a starting point).
            - Preserve the original file extension
            - Order files logically based on any existing numbering in the names (eg. final_1.jpg, final_2.jpg, ..., final_10.jpg should be renamed in that order to 00.jpg, 01.jpg, ..., 09.jpg)
            - If no renaming is required, return an empty list to signify no changes required
            
            Respond ONLY with a JSON object in this exact format (no markdown, no extra text):
            {"renames": [{"old": "original_name.ext", "new": "00.ext"}, ...]}
            
            Include all renamed files in the renames array.

            Input File Names (format: semicolon-delimited, eg. 00.jpg;,01.jpg...):
            {{fileListText}}
            """;
    }

    private async Task<string> SendPromptAsync(string prompt, CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            model = Model,
            prompt = prompt,
            stream = false
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(
            $"{BaseUrl}/api/generate",
            content,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        
        using var doc = JsonDocument.Parse(responseJson);
        return doc.RootElement.GetProperty("response").GetString() ?? string.Empty;
    }

    private static RenameSuggestion ParseRenameSuggestionResponse(string response, List<string> originalFileNames)
    {
        try
        {
            // Try to extract JSON from the response (in case there's extra text)
            var jsonStart = response.IndexOf('{');
            var jsonEnd = response.LastIndexOf('}');
            
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonText = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
                
                using var doc = JsonDocument.Parse(jsonText);
                var root = doc.RootElement;
                
                var renames = new List<RenameMapping>();
                
                if (root.TryGetProperty("renames", out var renamesArray))
                {
                    foreach (var item in renamesArray.EnumerateArray())
                    {
                        var oldName = item.GetProperty("old").GetString() ?? string.Empty;
                        var newName = item.GetProperty("new").GetString() ?? string.Empty;
                        
                        if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
                        {
                            renames.Add(new RenameMapping(oldName, newName));
                        }
                    }
                }
                
                // Validate that we have mappings for all original files
                if (renames.Count == 0)
                {
                    return new RenameSuggestion([], $"LLM returned no rename mappings. Response: {response}");
                }
                
                return new RenameSuggestion(renames, string.Empty);
            }
        }
        catch (Exception ex)
        {
            return new RenameSuggestion([], $"Failed to parse LLM response: {ex.Message}");
        }

        return new RenameSuggestion([], "Could not parse LLM response");
    }
}

/// <summary>
/// Represents a single file rename mapping
/// </summary>
public record RenameMapping(string OldName, string NewName);

/// <summary>
/// Represents the complete rename suggestion from the LLM
/// </summary>
public record RenameSuggestion(List<RenameMapping> Renames, string Reasoning)
{
    public bool IsSuccess => Renames.Count > 0;
}
