using Godot;
using System;
using System.IO;
using System.Text.Json;

/// <summary>
/// Singleton manager responsible for serializing, encrypting, saving, and loading game state on disk.
/// </summary>
public partial class SaveManager : Node
{
    public static SaveManager Instance { get; private set; }

    // Virtual user path mapped to the OS-specific AppData/Application Support directory
    private readonly string saveFilePath = "user://savegame.json";

    public override void EnterTree()
    {
        // Enforce Singleton pattern across scene transitions
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Serializes SaveData to JSON, encrypts it with AES, and writes it to disk.
    /// </summary>
    public void SaveGame(SaveData data)
    {
        string globalPath = ProjectSettings.GlobalizePath(saveFilePath);

        try
        {
            // 1. Serialize SaveData object into a compact JSON string
            var options = new JsonSerializerOptions { WriteIndented = false };
            string jsonString = JsonSerializer.Serialize(data, options);

            // 2. Encrypt the JSON payload using AES-256
            string encryptedData = EncryptionHelper.Encrypt(jsonString);

            // 3. Write the encrypted string to disk
            File.WriteAllText(globalPath, encryptedData);
            GD.Print($"Game saved and encrypted successfully at: {globalPath}");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to save game: {e.Message}");
        }
    }

    /// <summary>
    /// Reads and decrypts the save file from disk, or returns default SaveData if none exists.
    /// </summary>
    public SaveData LoadGame()
    {
        string globalPath = ProjectSettings.GlobalizePath(saveFilePath);

        if (!File.Exists(globalPath))
        {
            GD.Print("No save file found. Returning default SaveData.");
            return new SaveData();
        }

        try
        {
            // 1. Read encrypted Base64 content from disk
            string encryptedData = File.ReadAllText(globalPath);

            // 2. Decrypt back into raw JSON
            string jsonString = EncryptionHelper.Decrypt(encryptedData);

            // 3. Deserialize JSON back into a SaveData object
            SaveData data = JsonSerializer.Deserialize<SaveData>(jsonString);

            GD.Print("Game decrypted and loaded successfully!");
            return data ?? new SaveData();
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to decrypt or load save file (corrupted or tampered): {e.Message}");
            return new SaveData();
        }
    }
}