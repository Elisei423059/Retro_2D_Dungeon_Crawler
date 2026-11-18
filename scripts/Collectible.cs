using Godot;

/// <summary>
/// Handles pickup items (e.g., coins) that award points when touched by the player.
/// </summary>
public partial class Collectible : Area2D
{
	[Export] private int pointValue = 10; // Score awarded upon collection

    private bool _collected = false;

    public override void _Ready()
	{
		// Subscribe to body collision signal
		BodyEntered += OnCollectibleBodyEntered;
	}

	/// <summary>
	/// Checks if the colliding body is the player, updates global score, and removes the item.
	/// </summary>
	private void OnCollectibleBodyEntered(Node2D body)
	{
        // We check if the one who entered the coin is the Player
        if ((body.Name == "Player" || body.IsInGroup("Player")) && (!_collected))
        {
			_collected = true;

			// Forward points to the global SceneManager singleton if available
			SceneManager.Instance?.AddScore(pointValue);
			GD.Print($"Item collected! Points added: {pointValue}");

			// Remove the collectible from the scene tree
			QueueFree();
		}
	}
}
