using Godot;

/// <summary>
/// Controls enemy AI chasing behavior, health management, knockback physics, visual hit feedback, and item drops.
/// </summary>
public partial class Enemy : CharacterBody2D
{
    [ExportGroup("Enemy Stats")]
    [Export] private int maxHealth = 100;
    [Export] private float speed = 40.0f;
    [Export] private float knockbackForce = 400.0f;    // Impulse strength when hit
    [Export] private float knockbackDuration = 0.2f;  // Duration of stun/knockback state in seconds

    [ExportGroup("Item Drops")]
    [Export] private PackedScene coinScene; // Scene resource to instantiate upon death
    [Export(PropertyHint.Range, "0,100,1")] private float dropChance = 50.0f; // Drop probability (0% to 100%)

    private int currentHealth;
    private Vector2 knockbackVelocity = Vector2.Zero;
    private float knockbackTimer = 0f;
    private Node2D player;
    private CanvasItem sprite;

    // Preload the floating damage number scene for instant instantiation during combat
    private readonly PackedScene floatingTextScene = GD.Load<PackedScene>("res://scenes/FloatingText.tscn");

    public override void _Ready()
    {
        currentHealth = maxHealth;
        
        // Locate the player node using the global "Player" group
        player = GetTree().GetFirstNodeInGroup("Player") as Node2D;
        
        // Cache the visual sprite node for hit-flash effects
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        Vector2 direction = Vector2.Right; // Default direction for wall collision response

        // If currently knocked back, override normal AI movement with knockback velocity
        if (knockbackTimer > 0)
        {
            knockbackTimer -= (float)delta;
            velocity = knockbackVelocity;
        }
        else
        {
            // Normal AI behavior: move directly toward the player's global position
            if (player != null)
            {
                direction = GlobalPosition.DirectionTo(player.GlobalPosition);
                velocity = direction * speed;
            }
            else
            {
                velocity = Vector2.Zero;
            }
        }

        Velocity = velocity;

        MoveAndSlide();

        // If it hits a wall, it changes direction.
        if (IsOnWall())
        {
            direction *= -1;
        }
    }

    /// <summary>
    /// Applies damage to the enemy, spawns floating damage text, triggers knockback, and plays hit flash.
    /// </summary>
    public void TakeDamage(int amount, Vector2 attackerPosition)
    {
        currentHealth -= amount;
        GD.Print($"Enemy hit! Remaining health: {currentHealth}");

        // 1. Spawn floating damage number above the enemy
        ShowFloatingDamage(amount);

        // 2. Calculate knockback vector pointing away from the attacker
        Vector2 knockbackDir = attackerPosition.DirectionTo(GlobalPosition);
        knockbackVelocity = knockbackDir * knockbackForce;
        knockbackTimer = knockbackDuration;

        // 3. Trigger visual red flash feedback
        PlayFlashEffect();

        // Check for death condition
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Instantiates a FloatingText scene in the main game tree above the enemy.
    /// </summary>
    private void ShowFloatingDamage(int damage)
    {
        if (floatingTextScene == null) return;

        var textInstance = (FloatingText)floatingTextScene.Instantiate();
        
        // Add to CurrentScene so the text persists smoothly even if the enemy dies immediately
        GetTree().CurrentScene.AddChild(textInstance);
        textInstance.GlobalPosition = GlobalPosition + new Vector2(0, -30);
        textInstance.Initialize(damage, Colors.OrangeRed);
    }

    /// <summary>
    /// Flashes the enemy sprite red and smoothly tweens back to normal color.
    /// </summary>
    private void PlayFlashEffect()
    {
        if (sprite == null) return;

        var tween = CreateTween();
        sprite.Modulate = Colors.Red;
        tween.TweenProperty(sprite, "modulate", new Color(1, 1, 1, 1), 0.15f);
    }

    /// <summary>
    /// Handles enemy defeat, rolls for loot drops, and removes the node from the scene tree.
    /// </summary>
    private void Die()
    {
        GD.Print("Enemy defeated. Rolling for item drops...");
        DropItem();
        QueueFree();
    }

    /// <summary>
    /// Evaluates RNG drop chance and spawns a collectible item at the enemy's death location.
    /// </summary>
    private void DropItem()
    {
        if (coinScene == null) return;

        // Generate a random percentage between 0.0 and 100.0
        float roll = GD.Randf() * 100.0f;

        if (roll <= dropChance)
        {
            Node2D droppedItem = (Node2D)coinScene.Instantiate();
            GetTree().CurrentScene.AddChild(droppedItem);
            droppedItem.GlobalPosition = GlobalPosition;
            GD.Print($"Item dropped! (Roll: {roll:F1} vs Chance: {dropChance}%)");
        }
    }
}