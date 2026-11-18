using Godot;

/// <summary>
/// Displays animated floating damage numbers that rise and fade out using parallel Tweens.
/// </summary>
public partial class FloatingText : Node2D
{
    private Label label;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
    }

    /// <summary>
    /// Configures the damage value and color, then runs upward movement and fade-out animations.
    /// </summary>
    public void Initialize(int damageAmount, Color? textColor = null)
    {
        if (label == null)
            label = GetNode<Label>("Label");

        label.Text = damageAmount.ToString();

        if (textColor.HasValue)
        {
            label.Modulate = textColor.Value;
        }

        // Create a parallel Tween so movement and opacity fade run simultaneously
        var tween = CreateTween();
        tween.SetParallel(true);

        // 1. Float upward by 40 pixels over 0.6 seconds with smooth deceleration
        tween.TweenProperty(this, "position", Position + new Vector2(0, -40), 0.6f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);

        // 2. Fade alpha transparency to 0 after a brief 0.2s delay
        tween.TweenProperty(this, "modulate:a", 0f, 0.6f)
            .SetDelay(0.2f);

        // 3. Automatically free the node from memory once all animations complete
        tween.Chain().TweenCallback(Callable.From(QueueFree));
    }
}