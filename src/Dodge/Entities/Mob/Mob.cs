using System;
using Dodge.PowerUps;
using Godot;

namespace Dodge.Entities
{
    public static class MobNodes
    {
        public const string Group = "Mobs";
        public const string PhysicsBody3D = "Mob";
        public const string Sprite2D = "AnimatedSprite2D";
    }

    public partial class Mob : RigidBody2D
    {
        [Signal]
        public delegate void SlowdownPowerUpActiveEventHandler(SlowdownEffect effect);

        [Signal]
        public delegate void SlowdownPowerUpEndedEventHandler();

        private static readonly Random _rng = new();
        private Vector2 _originalVector;

        [Export]
        public int MinSpeed = 150;

        [Export]
        public int MaxSpeed = 250;

        public override void _Ready()
        {
            var sprite = GetSprite();

            var mobTypes = sprite.SpriteFrames.GetAnimationNames();
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
                LinearVelocity.X * effect.VelocityModifier,
                LinearVelocity.Y * effect.VelocityModifier
            );
        }

        public void OnSlowdownPowerUpEnded()
        {
            LinearVelocity = _originalVector;
        }

        private AnimatedSprite2D GetSprite() =>
          GetNode<AnimatedSprite2D>(MobNodes.Sprite2D);
    }
}
