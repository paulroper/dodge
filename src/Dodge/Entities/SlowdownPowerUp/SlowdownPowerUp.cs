using Dodge.Lib.PowerUps;
using Godot;

namespace Dodge.Entities;

public static class SlowdownPowerUpNodes
{
    public const string PhysicsBody3D = "PowerUp";
}

public static class SlowdownPowerUpSignals
{
    public const string Active = "SlowdownPowerUpActive";
    public const string Collected = "SlowdownPowerUpCollected";
    public const string Ended = "SlowdownPowerUpEnded";
}

public partial class SlowdownPowerUp : Area2D
{
    [Signal]
    public delegate void SlowdownPowerUpCollectedEventHandler();

    public SlowdownEffect Effect { get; private set; } = new SlowdownEffect(2.0f, 0.25f);

    public void OnPowerUpAreaEntered(Area2D _)
    {
        GD.Print("Slowdown power-up collected");

        EmitSignal(SlowdownPowerUpSignals.Collected);
        QueueFree();
    }
}
