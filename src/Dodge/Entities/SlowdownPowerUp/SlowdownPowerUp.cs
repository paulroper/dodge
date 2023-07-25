using Dodge.PowerUps;
using Godot;

namespace Dodge.Entities
{
	public static class SlowdownPowerUpNodes
	{
		public const string PhysicsBody3D = "PowerUp";
	}

	public static class SlowdownPowerUpSignals
	{
		public const string Active = "OnSlowdownPowerUpActive";
		public const string Collected = "OnSlowdownPowerUpCollected";
		public const string Ended = "OnSlowdownPowerUpEnded";
	}

	public partial class SlowdownPowerUp : Area2D
	{
		public void OnPowerUpAreaEntered(Area2D _)
		{
			GD.Print("Slowdown power-up collected");

			var mainNode = GetParent<Main>();

			var effect = new SlowdownEffect(2.00f, 0.25f);
			mainNode.OnSlowdownPowerUpCollected(effect);

			EmitSignal(SlowdownPowerUpSignals.Collected);
			QueueFree();
		}
	}
}
