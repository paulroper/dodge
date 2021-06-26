using Godot;
using System;
using Dodge.Entities;
using Dodge.PowerUps;
using System.Collections.Generic;
using System.Linq;

namespace Dodge
{
    public static class MainNodes
    {
        public const string BackgroundMusic = "BackgroundMusic";
        public const string GameOverSound = "GameOverSound";
        public const string Hud = "HUD";
        public const string MobSpawnLocation = "MobPath/MobSpawnLocation";
        public const string MobTimer = "MobTimer";
        public const string Player = "Player";
        public const string PowerUpSpawnTimer = "PowerUpSpawnTimer";
        public const string Root = "Main";
        public const string ScoreTimer = "ScoreTimer";
        public const string StartPosition = "StartPosition";
        public const string StartTimer = "StartTimer";
        public const string SlowdownPowerUpTimer = "SlowdownPowerUpTimer";
    }

    public class Main : Node
    {
        [Export]
        public PackedScene Mob;

        [Export]
        public PackedScene PowerUp;

        private int _score;
        private int _hiScore;
        private bool _slowdownPowerUpSpawned;
        private readonly ActivePowerUpEffects _activePowerUpEffects = new ActivePowerUpEffects();

        private static readonly Random _rng = new Random();
        private static readonly IPowerUpWizard _powerUpWizard = new PowerUpWizard();

        public override void _Ready()
        {
        }

        public void GameOver()
        {
            GetBackgroundMusic().Stop();

            GetMobTimer().Stop();
            GetScoreTimer().Stop();
            GetPowerUpSpawnTimer().Stop();
            GetSlowdownPowerUpTimer().Stop();

            _hiScore = Math.Max(_score, _hiScore);

            var hud = GetHud();
            hud.UpdateHiScore(_hiScore);
            hud.ShowGameOver();

            GetGameOverSound().Play();
        }

        public void NewGame()
        {
            _score = 0;

            var player = GetPlayer();
            var startPosition = GetStartPosition();

            player.Start(startPosition.Position);

            GetStartTimer().Start();

            foreach (var child in GetChildren())
            {
                if (child is Mob mob)
                {
                    mob.QueueFree();
                }
            }

            ResetMobSpawnTimer();
            _activePowerUpEffects.Clear();

            var hud = GetHud();
            hud.UpdateScore(_score);
            hud.ShowMessage("Get Ready!");

            GetBackgroundMusic().Play();
        }

        public void SlowdownPowerUpCollected(SlowdownEffect effect)
        {
            GD.Print("Slowdown power-up signal triggered");

            var mobTimer = GetMobTimer();
            effect.OriginalTimerWait = mobTimer.WaitTime;

            _slowdownPowerUpSpawned = false;

            // Only one slowdown effect can be active at a time
            _activePowerUpEffects.Slowdown = effect;

            foreach (var child in GetChildren())
            {
                if (child is Mob mob)
                {
                    _powerUpWizard.Apply(mob, _activePowerUpEffects);
                }
            }

            _powerUpWizard.Apply(mobTimer, _activePowerUpEffects);

            GetSlowdownPowerUpTimer().Start();
        }

        public void OnSlowdownPowerUpTimerTimeout()
        {
            GD.Print("Slowdown power-up effect over");

            GetSlowdownPowerUpTimer().Stop();
            ResetMobSpawnTimer();

            foreach (var child in GetChildren())
            {
                if (child is Mob mob)
                {
                    mob.RevertVelocity();
                }
            }

            _activePowerUpEffects.Slowdown = null;
        }

        public void OnMobTimerTimeout()
        {
            // Choose a random location on Path2D.
            var mobSpawnLocation = GetMobSpawnLocation();
            mobSpawnLocation.Offset = _rng.Next();

            // Create a Mob instance and add it to the scene.
            var mobInstance = Mob.Instance<Mob>();
            AddChild(mobInstance);

            // Set the mob's direction perpendicular to the path direction.
            var direction = (mobSpawnLocation.Rotation + Mathf.Pi) / 2;

            // Set the mob's position to a random location.
            mobInstance.Position = mobSpawnLocation.Position;

            // Add some randomness to the direction.
            direction += RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
            mobInstance.Rotation = direction;

            // Choose the velocity.
            var velocity = RandRange(150f, 250f);
            mobInstance.SetVelocity(new Vector2(velocity, 0).Rotated(direction));

            if (_activePowerUpEffects.Count > 0)
            {
                _powerUpWizard.Apply(mobInstance, _activePowerUpEffects);
            }
        }

        public void OnPowerUpSpawnTimerTimeout()
        {
            if (_slowdownPowerUpSpawned || !(_activePowerUpEffects.Slowdown is null))
            {
                return;
            }

            var powerUpInstance = PowerUp.Instance<SlowdownPowerUp>();
            AddChild(powerUpInstance);

            var screenSize = GetViewport().Size;

            var powerUpX = _rng.Next(0, (int)screenSize.x);
            var powerUpY = _rng.Next(0, (int)screenSize.y);

            GD.Print($"Spawning a {powerUpInstance.GetType()} power-up at {powerUpX}, {powerUpY}");

            powerUpInstance.Position = new Vector2(powerUpX, powerUpY);
            _slowdownPowerUpSpawned = true;
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
            GetPowerUpSpawnTimer().Start();
        }

        private void ResetMobSpawnTimer()
        {
            var mobTimer = GetMobTimer();

            var slowdownEffect = _activePowerUpEffects.Slowdown;
            if (slowdownEffect is null)
            {
                return;
            }

            mobTimer.WaitTime = slowdownEffect.OriginalTimerWait;
        }

        private AudioStreamPlayer GetBackgroundMusic() =>
          GetNode<AudioStreamPlayer>(MainNodes.BackgroundMusic);

        private AudioStreamPlayer GetGameOverSound() =>
          GetNode<AudioStreamPlayer>(MainNodes.GameOverSound);

        private Hud GetHud() =>
          GetNode<Hud>(MainNodes.Hud);

        private PathFollow2D GetMobSpawnLocation() =>
           GetNode<PathFollow2D>(MainNodes.MobSpawnLocation);

        private Player GetPlayer() =>
          GetNode<Player>(MainNodes.Player);

        private Timer GetPowerUpSpawnTimer() =>
          GetNode<Timer>(MainNodes.PowerUpSpawnTimer);

        private Position2D GetStartPosition() =>
          GetNode<Position2D>(MainNodes.StartPosition);

        private Timer GetMobTimer() =>
          GetNode<Timer>(MainNodes.MobTimer);

        private Timer GetScoreTimer() =>
          GetNode<Timer>(MainNodes.ScoreTimer);

        private Timer GetStartTimer() =>
          GetNode<Timer>(MainNodes.StartTimer);

        private Timer GetSlowdownPowerUpTimer() =>
          GetNode<Timer>(MainNodes.SlowdownPowerUpTimer);

        private float RandRange(float min, float max)
        {
            return (float)(_rng.NextDouble() * (max - min)) + min;
        }
    }
}
