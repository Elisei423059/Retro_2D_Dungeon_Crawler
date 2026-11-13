using Godot;

/// <summary>
/// Controls the main player character, handling top-down 2D movement and melee sword attacks.
/// </summary>
public partial class Player : CharacterBody2D
{
    [ExportGroup("Movement Settings")]
    [Export] private float Speed { get; set; } = 300.0f; // Movement speed in pixels per second

    private AnimationPlayer animationPlayer;
    private Area2D swordHitbox;
    private bool isAttacking = false; // Tracks whether the player is currently mid-swing

    public override void Ready()
    {
        // Cache references to child nodes
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        swordHitbox = GetNode<Area2D>("SwordHitbox");

        // Ensure the weapon hitbox is disabled at startup so it doesn't deal passive damage
        ToggleHitbox(false);

        // Subscribe to the AnimationFinished signal to reset attack state when the swing completes
        animationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public override void PhysicsProcess(double delta)
    {
        // Lock player movement while performing an attack animation
        if (isAttacking) return;

        // Check for attack input action
        if (Input.IsActionJustPressed("attack"))
        {
            StartAttack();
            return;
        }

        // Retrieve normalized 8-directional movement vector from Input Map actions
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        
        // Apply velocity and execute physics movement with collision sliding
        Velocity = direction * Speed;
        MoveAndSlide();
    }

    /// <summary>
    /// Initiates the attack state, halts movement, and plays the attack animation.
    /// </summary>
    private void StartAttack()
    {
        isAttacking = true;
        Velocity = Vector2.Zero; // Stop sliding during the attack
        animationPlayer.Play("attack");
    }

    /// <summary>
    /// Enables the sword hitbox. Designed to be called via an AnimationPlayer Method Track.
    /// </summary>
    public void EnableHitbox()
    {
        ToggleHitbox(true);
    }

    /// <summary>
    /// Disables the sword hitbox. Designed to be called via an AnimationPlayer Method Track.
    /// </summary>
    public void DisableHitbox()
    {
        ToggleHitbox(false);
    }

    /// <summary>
    /// Toggles collision monitoring on the sword's Area2D hitbox.
    /// </summary>
    private void ToggleHitbox(bool enable)
    {
        if (swordHitbox != null)
        {
            swordHitbox.Monitoring = enable;
            swordHitbox.Monitorable = enable;
        }
    }

    /// <summary>
    /// Callback triggered when any animation in AnimationPlayer finishes.
    /// </summary>
    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "attack")
        {
            isAttacking = false;
            DisableHitbox(); // Guarantee the hitbox is turned off at the end of the animation
        }
    }
}