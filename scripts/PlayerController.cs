using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	[ExportGroup("Movement Settings")]
	[Export] private float Speed { get; set; } = 300.0f;
	[Export] private float Acceleration { get; set; } = 1200.0f;
	[Export] private float Friction { get; set; } = 1000.0f;

	[ExportGroup("Jump Settings")]
	[Export] private float JumpVelocity { get; set; } = -450.0f;

	// Obținem gravitația nativă din proiectul Godot
	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// 1. Aplicarea gravitației dacă jucătorul este în aer
		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
		}

		// 2. Gestionarea săriturii
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// 3. Obținerea direcției de input (Stânga / Dreapta)
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		// 4. Aplicarea mișcării orizontale cu accelerație și frânare lină (Smooth Movement)
		if (direction.X != 0)
		{
			velocity.X = Mathf.MoveToward(Velocity.X, direction.X * Speed, Acceleration * (float)delta);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Friction * (float)delta);
		}

		// 5. Setarea vitezei finale și executarea mișcării în motorul fizic
		Velocity = velocity;
		MoveAndSlide();
	}
}
