using Godot;
using InputMap = Dodge.Lib.InputMap;

namespace Dodge.Entities;

public static class PlayerSignals
{
	public const string Hit = "Hit";
}

public static class PlayerNodes
{
	public const string Hitbox = "Hitbox";
}

public partial class Player : Area2D
{
	[Export]
	public int Speed = 500;

	public Vector2 Velocity;

	private Vector2 _screenSize;
	private const string _walkAnimationName = "walk";
	private const string _upAnimationName = "up";

	[Signal]
	public delegate void HitEventHandler();

	public void Start(Vector2 pos)
	{
		Position = pos;
		Show();
		GetHitbox().Disabled = false;
	}

	public override void _Ready()
	{
		_screenSize = GetViewport().GetVisibleRect().Size;
		Hide();
	}

	public override void _Process(double delta)
	{
		Velocity = new Vector2();

		if (Input.IsActionPressed(InputMap.Right))
		{
			Velocity.X++;
		}

		if (Input.IsActionPressed(InputMap.Left))
		{
			Velocity.X--;
		}

		if (Input.IsActionPressed(InputMap.Down))
		{
			Velocity.Y++;
		}

		if (Input.IsActionPressed(InputMap.Up))
		{
			Velocity.Y--;
		}

		var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (Velocity.Length() > 0)
		{
			Velocity = Velocity.Normalized() * Speed;
			animatedSprite.Play();
		}
		else
		{
			animatedSprite.Stop();
		}

		Position += Velocity * (float)delta;
		Position = new Vector2(
		  x: Mathf.Clamp(Position.X, 0, _screenSize.X),
		  y: Mathf.Clamp(Position.Y, 0, _screenSize.Y)
		);

		if (Velocity.X != 0)
		{
			animatedSprite.Animation = _walkAnimationName;
			animatedSprite.FlipV = false;
			animatedSprite.FlipH = Velocity.X < 0;
		}
		else if (Velocity.Y != 0)
		{
			animatedSprite.Animation = _upAnimationName;
			animatedSprite.FlipV = Velocity.Y > 0;
		}
	}

	public void OnPlayerBodyEntered(PhysicsBody2D body)
	{
		if (body is not Mob)
		{
			return;
		}

		Hide();
		EmitSignal(PlayerSignals.Hit);
		GetHitbox().SetDeferred("disabled", true);
	}

	private CollisionShape2D GetHitbox() =>
	  GetNode<CollisionShape2D>(PlayerNodes.Hitbox);
}
