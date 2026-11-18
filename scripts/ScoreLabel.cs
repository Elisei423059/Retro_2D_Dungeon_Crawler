using Godot;
using System;

public partial class ScoreLabel : Label
{
    public override void _Process(double delta)
    {
        // We check if SceneManager is initialized and update the text.
        if (SceneManager.Instance != null)
        {
            Text = $"Score: {SceneManager.Instance.Score}";
        }
        else
        {
            Text = "Score: 0";
        }
    }
}
