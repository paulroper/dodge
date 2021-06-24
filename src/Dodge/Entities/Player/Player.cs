using Godot;

namespace Dodge
{
    public static class PlayerSignals
    {
        public const string Hit = "Hit";
    }

    public static class PlayerNodes
    {
        public const string Hitbox = "Hitbox";
    }

    public class Player : Area2D
    {
        [Export]
        public int Speed = 500;

        public Vector2 Velocity = new Vector2();

        private Vector2 _screenSize;
        private const string _walkAnimationName = "walk";
        private const string _upAnimationName = "up";

        [Signal]
        public delegate void Hit();

        public void Start(Vector2 pos)
        {
            Position = pos;
            Show();
            GetHitbox().Disabled = false;
        }

        public override void _Ready()
        {
            _screenSize = GetViewport().Size;
            Hide();
        }

        public override void _Process(float delta)
        {
            Velocity = new Vector2();

            if (Input.IsActionPressed(InputMap.Right))
            {
                Velocity.x += 1;
            }

            if (Input.IsActionPressed(InputMap.Left))
            {
                Velocity.x -= 1;
            }

            if (Input.IsActionPressed(InputMap.Down))
            {
                Velocity.y += 1;
            }

            if (Input.IsActionPressed(InputMap.Up))
            {
                Velocity.y -= 1;
            }

            var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

            if (Velocity.Length() > 0)
            {
                Velocity = Velocity.Normalized() * Speed;
                animatedSprite.Play();
            }
            else
            {
                animatedSprite.Stop();
            }

            Position += Velocity * delta;
            Position = new Vector2(
              x: Mathf.Clamp(Position.x, 0, _screenSize.x),
              y: Mathf.Clamp(Position.y, 0, _screenSize.y)
            );

            if (Velocity.x != 0)
            {
                animatedSprite.Animation = _walkAnimationName;
                animatedSprite.FlipV = false;
                animatedSprite.FlipH = Velocity.x < 0;
            }
            else if (Velocity.y != 0)
            {
                animatedSprite.Animation = _upAnimationName;
                animatedSprite.FlipV = Velocity.y > 0;
            }
        }

        public void OnPlayerBodyEntered(PhysicsBody2D body)
        {
            Hide();
            EmitSignal(PlayerSignals.Hit);
            GetHitbox().SetDeferred("disabled", true);
        }

        private CollisionShape2D GetHitbox() =>
          GetNode<CollisionShape2D>(PlayerNodes.Hitbox);
    }
}
