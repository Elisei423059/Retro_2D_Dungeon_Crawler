using Godot;

/// <summary>
/// Represents the melee weapon's active collision area, dealing damage and passing attacker position for knockback.
/// </summary>
public partial class SwordHitbox : Area2D
{
    [Export] private int damageAmount = 35; // Damage dealt per successful hit

    public override void _Ready()
    {
        // Listen for physics bodies entering the hitbox area
        BodyEntered += OnBodyEntered;
    }

    /// <summary>
    /// Triggered when a PhysicsBody2D enters the active sword hitbox.
    /// </summary>
    private void OnBodyEntered(Node2D body)
    {
        // Check if the collided body implements a TakeDamage method (duck typing via Godot's HasMethod)
        if (body.HasMethod("TakeDamage"))
        {
            // Pass both damage amount and attacker's position so the target can calculate knockback direction
            Vector2 attackerPosition = GlobalPosition;
            body.Call("TakeDamage", damageAmount, attackerPosition);
            
            GD.Print($"Enemy hit! Damage dealt: {damageAmount}");
        }
    }
}