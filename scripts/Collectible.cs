using Godot;

/// <summary>
/// Handles pickup items (e.g., coins) that award points when touched by the player.
/// </summary>
public partial class Collectible : Area2D
{
    [Export] private int pointValue = 10; // Score awarded upon collection

    public override void Ready()
    {
        // Subscribe to body collision signal
        BodyEntered += OnCollectibleBodyEntered;
    }

    /// <summary>
    /// Checks if the colliding body is the player, updates global score, and removes the item.
    /// </summary>
    private void OnCollectibleBodyEntered(Node2D body)
    {
        if (body.Name == "Player" || body.IsInGroup("Player"))
        {
            // Forward points to the global SceneManager singleton if available
            SceneManager.Instance?.AddScore(pointValue);
            GD.Print($"Item collected! Points added: {pointValue}");

            // Remove the collectible from the scene tree
            QueueFree();
        }
    }
}