using Godot;

/// <summary>
/// Environmental hazard that inflicts contact damage when an entity steps onto it.
/// </summary>
public partial class SpikeTrap : Area2D
{
    [Export] private int damageAmount = 25; // Damage inflicted upon stepping on the trap

    public override void Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    /// <summary>
    /// Deals damage to any entering body that exposes a TakeDamage method.
    /// </summary>
    private void OnBodyEntered(Node2D body)
    {
        if (body.HasMethod("TakeDamage"))
        {
            body.Call("TakeDamage", damageAmount);
            GD.Print("Entity stepped on a spike trap and took damage!");
        }
    }
}