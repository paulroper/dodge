using Godot;
using System;

namespace Dodge
{
    public static class MainNodes
    {
        public static string Hud = "HUD";
        public static string MobSpawnLocation = "MobPath/MobSpawnLocation";
        public static string MobTimer = "MobTimer";
        public static string Player = "Player";
        public static string ScoreTimer = "ScoreTimer";
        public static string StartPosition = "StartPosition";
        public static string StartTimer = "StartTimer";
    }

    public class Main : Node
    {
        [Export]
        public PackedScene Mob;

        private int _score;
        private int _hiScore;

        private static readonly Random _rng = new Random();

        public override void _Ready()
        {
        }

        public void GameOver()
        {
            GetMobTimer().Stop();
            GetScoreTimer().Stop();

            _hiScore = Math.Max(_score, _hiScore);

            var hud = GetHud();
            hud.UpdateHiScore(_hiScore);
            hud.ShowGameOver();
        }

        public void NewGame()
        {
            _score = 0;

            var player = GetPlayer();
            var startPosition = GetStartPosition();

            player.Start(startPosition.Position);

            GetStartTimer().Start();

            var hud = GetHud();
            hud.UpdateScore(_score);
            hud.ShowMessage("Get Ready!");
        }

        public void OnMobTimerTimeout()
        {
            // Choose a random location on Path2D.
            var mobSpawnLocation = GetMobSpawnLocation();
            mobSpawnLocation.Offset = _rng.Next();

            // Create a Mob instance and add it to the scene.
            var mobInstance = Mob.Instance<RigidBody2D>();
            AddChild(mobInstance);

            // Set the mob's direction perpendicular to the path direction.
            var direction = (mobSpawnLocation.Rotation + Mathf.Pi) / 2;

            // Set the mob's position to a random location.
            mobInstance.Position = mobSpawnLocation.Position;

            // Add some randomness to the direction.
            direction += RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
            mobInstance.Rotation = direction;

            // Choose the velocity.
            mobInstance.LinearVelocity = new Vector2(RandRange(150f, 250f), 0).Rotated(direction);
        }

        public void OnScoreTimerTimeout()
        {
            _score++;
            GetHud().UpdateScore(_score);
        }

        public void OnStartTimerTimeout()
        {
            GetMobTimer().Start();
            GetScoreTimer().Start();
        }

        private Hud GetHud() =>
          GetNode<Hud>(MainNodes.Hud);

        private PathFollow2D GetMobSpawnLocation() =>
           GetNode<PathFollow2D>(MainNodes.MobSpawnLocation);

        private Player GetPlayer() =>
          GetNode<Player>(MainNodes.Player);

        private Position2D GetStartPosition() =>
          GetNode<Position2D>(MainNodes.StartPosition);

        private Timer GetMobTimer() =>
          GetNode<Timer>(MainNodes.MobTimer);

        private Timer GetScoreTimer() =>
          GetNode<Timer>(MainNodes.ScoreTimer);

        private Timer GetStartTimer() =>
          GetNode<Timer>(MainNodes.StartTimer);

        private float RandRange(float min, float max)
        {
            return (float)(_rng.NextDouble() * (max - min)) + min;
        }
    }
}

