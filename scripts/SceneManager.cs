using Godot;
using System.Threading.Tasks;

/// <summary>
/// Global singleton managing persistent game score and smooth fade transitions between scenes.
/// </summary>
public partial class SceneManager : Node
{
    public static SceneManager Instance { get; private set; }

    [Export] public int Score { get; private set; } = 0;
    [Export] private AnimationPlayer animationPlayer; // Reference to the screen-fade AnimationPlayer

    public override void EnterTree()
    {
        // Enforce Singleton pattern
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Increments the player's global score.
    /// </summary>
    public void AddScore(int amount)
    {
        Score += amount;
        GD.Print($"Score updated: {Score}");
    }

    /// <summary>
    /// Asynchronously transitions to a target scene using fade-to-black and fade-to-normal animations.
    /// </summary>
    public async void ChangeScene(string scenePath)
    {
        if (animationPlayer != null)
        {
            // 1. Fade screen to black and await animation completion
            animationPlayer.Play("fade_to_black");
            await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        }

        // 2. Switch the active scene tree to the target scene file
        Error result = GetTree().ChangeSceneToFile(scenePath);
        if (result != Error.Ok)
        {
            GD.PrintErr($"Failed to load scene: {scenePath}");
            return;
        }

        // Wait one idle frame to ensure the new scene nodes are initialized
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        if (animationPlayer != null)
        {
            // 3. Fade screen back in to reveal the new level
            animationPlayer.Play("fade_to_normal");
            await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        }
    }
}