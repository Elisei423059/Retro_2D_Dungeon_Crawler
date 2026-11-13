using System;

/// <summary>
/// Serializable data transfer object (DTO) representing the persistent state of a game save.
/// </summary>
[Serializable]
public class SaveData
{
    public int Score { get; set; } = 0;
    public int Health { get; set; } = 100;
    public float PlayerPositionX { get; set; } = 0f;
    public float PlayerPositionY { get; set; } = 0f;
    public string CurrentLevel { get; set; } = "res://scenes/Level1.tscn";
}