using System;
using Dodge.PowerUps;
using Godot;

namespace Dodge.Entities
{
    public static class MobNodes
    {
        public const string PhysicsBody = "Mob";
        public const string Sprite = "AnimatedSprite";
    }

    public class Mob : RigidBody2D
    {
        private static readonly Random _rng = new Random();
        private Vector2 _originalVector;

        [Export]
        public int MinSpeed = 150;

        [Export]
        public int MaxSpeed = 250;

        public override void _Ready()
        {
            var sprite = GetSprite();

            var mobTypes = sprite.Frames.GetAnimationNames();
            sprite.Animation = mobTypes[_rng.Next(0, mobTypes.Length)];
        }

        public void OnScreenExited()
        {
            QueueFree();
        }

        public void OnSlowdownPowerUpActive(SlowdownEffect effect)
        {
            _originalVector = LinearVelocity;
            LinearVelocity = new Vector2(
                LinearVelocity.x * effect.VelocityModifier,
                LinearVelocity.y * effect.VelocityModifier
            );
        }

        public void OnSlowdownPowerUpEnded()
        {
            LinearVelocity = _originalVector;
        }

        private AnimatedSprite GetSprite() =>
          GetNode<AnimatedSprite>(MobNodes.Sprite);
    }
}
