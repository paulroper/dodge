using Dodge.PowerUps;
using Godot;

namespace Dodge.Entities
{
    public static class SlowdownPowerUpNodes
    {
        public const string PhysicsBody = "PowerUp";
    }

    public static class SlowdownPowerUpSignals
    {
        public const string Collected = "Collected";
    }

    public class SlowdownPowerUp : Area2D
    {
        [Signal]
        public delegate void Collected(SlowdownEffect effect);

        public void OnPowerUpAreaEntered(Area2D _)
        {
            GD.Print("Slowdown power-up collected");

            var mainNode = GetParent<Main>();

            var effect = new SlowdownEffect(2.00f, 0.25f);
            Connect(SlowdownPowerUpSignals.Collected, mainNode, nameof(Main.SlowdownPowerUpCollected), new Godot.Collections.Array() { effect });

            EmitSignal(SlowdownPowerUpSignals.Collected);
            QueueFree();
        }
    }
}
