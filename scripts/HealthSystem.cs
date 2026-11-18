using Godot;
using System;

/// <summary>
/// Reusable, decoupled health component that manages HP, audio/visual hit feedback, and emits state signals.
/// </summary>
public partial class HealthSystem : Node
{
    [ExportGroup("Health Settings")]
    [Export] private int maxHealth = 100;
    [Export] private int currentHealth;

    [ExportGroup("Nodes Reference")]
    [Export] private NodePath animationPlayerPath;
    [Export] private NodePath audioPlayerPath;

    [ExportGroup("Audio Feedback")]
    [Export] private AudioStream hitSound;
    [Export] private AudioStream deathSound;

    private AnimationPlayer animationPlayer;
    private AudioStreamPlayer2D audioPlayer;

    // Godot Signals for decoupling UI bars and game-over logic from the entity
    [Signal] public delegate void HealthChangedEventHandler(int currentHealth, int maxHealth);
    [Signal] public delegate void DiedEventHandler();

    public override void _Ready()
    {
        currentHealth = maxHealth;

        // Resolve optional node references safely
        if (!animationPlayerPath.IsEmpty)
            animationPlayer = GetNode<AnimationPlayer>(animationPlayerPath);

        if (!audioPlayerPath.IsEmpty)
            audioPlayer = GetNode<AudioStreamPlayer2D>(audioPlayerPath);
    }

    /// <summary>
    /// Subtracts damage from current health, clamps values, plays feedback, and emits signals.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return; // Ignore damage if already dead

        damageAmount = Mathf.Max(0, damageAmount); // Prevent negative damage
        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);

        // Notify listeners (e.g., HUD health bar)
        EmitSignal(SignalName.HealthChanged, currentHealth, maxHealth);

        // Play hurt animation if available
        if (animationPlayer != null && animationPlayer.HasAnimation("hit"))
        {
            animationPlayer.Play("hit");
        }

        PlaySound(hitSound);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Restores health up to maxHealth and notifies UI listeners.
    /// </summary>
    public void Heal(int healAmount)
    {
        if (currentHealth <= 0) return; // Cannot heal a dead entity

        healAmount = Mathf.Max(0, healAmount);
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);

        EmitSignal(SignalName.HealthChanged, currentHealth, maxHealth);
    }

    /// <summary>
    /// Plays death sound and emits the Died signal for parent nodes to handle.
    /// </summary>
    private void Die()
    {
        PlaySound(deathSound);
        EmitSignal(SignalName.Died);
    }

    /// <summary>
    /// Helper method to safely play an AudioStream on the assigned AudioStreamPlayer2D.
    /// </summary>
    private void PlaySound(AudioStream sound)
    {
        if (audioPlayer != null && sound != null)
        {
            audioPlayer.Stream = sound;
            audioPlayer.Play();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}