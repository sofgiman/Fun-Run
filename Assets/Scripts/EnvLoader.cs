using System;
using System.IO;
using UnityEngine;

public static class EnvLoader
{
    /// <summary>
    /// Loads environment variables from a .env file.
    /// </summary>
    /// <param name="path">Path to the .env file (default: ".env" in current folder)</param>
    public static void Load(string path = ".env")
    {
        try
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($".env file not found at {path}. Using defaults.");
                return;
            }

            foreach (var raw in File.ReadAllLines(path))
            {
                var line = raw.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                // Optional: support "export KEY=VALUE"
                if (line.StartsWith("export "))
                    line = line.Substring(7);

                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim().Trim('"');

                Environment.SetEnvironmentVariable(key, value);
            }

            Debug.Log(".env file loaded successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load .env file: {e.Message}");
        }
    }
}
