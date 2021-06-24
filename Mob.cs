using System;
using Godot;

namespace Dodge
{
    public static class MobNodes
    {
        public static string Sprite = "AnimatedSprite";
    }

    public class Mob : RigidBody2D
    {
        private static readonly Random _rng = new Random();

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

        private AnimatedSprite GetSprite() =>
          GetNode<AnimatedSprite>(MobNodes.Sprite);
    }
}
