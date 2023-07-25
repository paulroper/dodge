using System;
using Dodge.Entities;
using Dodge.Lib.PowerUps;
using Godot;

namespace Dodge;

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

public partial class Main : Node
{
	[Export]
	public PackedScene Mob;

	[Export]
	public PackedScene PowerUp;

	[Signal]
	public delegate void SlowdownPowerUpActiveEventHandler(SlowdownEffect effect);

	[Signal]
	public delegate void SlowdownPowerUpEndedEventHandler();

	private int _score;
	private int _hiScore;
	private bool _slowdownPowerUpSpawned;
	private readonly ActivePowerUpEffects _activePowerUpEffects = new();

	private static readonly Random _rng = new();

	public override void _Ready()
	{
		var hud = GetHud();
		hud.StartGame += NewGame;

		var player = GetPlayer();
		player.Hit += GameOver;

		var backgroundMusic = GetBackgroundMusic();
		SlowdownPowerUpActive += backgroundMusic.OnSlowdownPowerUpActive;
		SlowdownPowerUpEnded += backgroundMusic.OnSlowdownPowerUpEnded;
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

		GetTree().CallGroup(MobNodes.Group, "queue_free");

		if (_activePowerUpEffects.Slowdown is not null)
		{
			EmitSignal(SlowdownPowerUpSignals.Ended);
		}

		_activePowerUpEffects.Clear();

		var hud = GetHud();
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");

		GetBackgroundMusic().Play();
	}

	public void OnSlowdownPowerUpCollected(SlowdownEffect effect)
	{
		GD.Print("Slowdown power-up signal triggered");

		_slowdownPowerUpSpawned = false;

		// Only one slowdown effect can be active at a time
		_activePowerUpEffects.Slowdown = effect;

		GetSlowdownPowerUpTimer().Start();
		EmitSignal(SlowdownPowerUpSignals.Active, effect);
	}

	public void OnSlowdownPowerUpTimerTimeout()
	{
		GD.Print("Slowdown power-up effect over");

		GetSlowdownPowerUpTimer().Stop();

		_activePowerUpEffects.Slowdown = null;

		EmitSignal(SlowdownPowerUpSignals.Ended);
	}

	public void OnMobTimerTimeout()
	{
		// Choose a random location on Path2D.
		var mobSpawnLocation = GetMobSpawnLocation();
		mobSpawnLocation.Progress = _rng.Next();

		// Create a Mob instance and add it to the scene.
		var mobInstance = Mob.Instantiate<Mob>();
		AddChild(mobInstance);

		SlowdownPowerUpActive += mobInstance.OnSlowdownPowerUpActive;
		SlowdownPowerUpEnded += mobInstance.OnSlowdownPowerUpEnded;

		// Set the mob's direction perpendicular to the path direction.
		var direction = (mobSpawnLocation.Rotation + Mathf.Pi) / 2;

		// Set the mob's position to a random location.
		mobInstance.Position = mobSpawnLocation.Position;

		// Add some randomness to the direction.
		direction += RandfRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mobInstance.Rotation = direction;

		// Choose the velocity.
		var velocity = RandfRange(150f, 250f);
		mobInstance.LinearVelocity = new Vector2(velocity, 0).Rotated(direction);

		if (_activePowerUpEffects.Slowdown is not null)
		{
			mobInstance.OnSlowdownPowerUpActive(_activePowerUpEffects.Slowdown);
		}
	}

	public void OnPowerUpSpawnTimerTimeout()
	{
		if (_slowdownPowerUpSpawned || _activePowerUpEffects.Slowdown is not null)
		{
			return;
		}

		var powerUpInstance = PowerUp.Instantiate<SlowdownPowerUp>();
		AddChild(powerUpInstance);

		powerUpInstance.SlowdownPowerUpCollected += () => OnSlowdownPowerUpCollected(powerUpInstance.Effect);

		var screenSize = GetViewport().GetVisibleRect().Size;

		var powerUpX = _rng.Next(0, (int)screenSize.X);
		var powerUpY = _rng.Next(0, (int)screenSize.Y);

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

	private BackgroundMusic GetBackgroundMusic() =>
	  GetNode<BackgroundMusic>(MainNodes.BackgroundMusic);

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

	private Marker2D GetStartPosition() =>
	  GetNode<Marker2D>(MainNodes.StartPosition);

	private Timer GetMobTimer() =>
	  GetNode<Timer>(MainNodes.MobTimer);

	private Timer GetScoreTimer() =>
	  GetNode<Timer>(MainNodes.ScoreTimer);

	private Timer GetStartTimer() =>
	  GetNode<Timer>(MainNodes.StartTimer);

	private Timer GetSlowdownPowerUpTimer() =>
	  GetNode<Timer>(MainNodes.SlowdownPowerUpTimer);

	private static float RandfRange(float min, float max)
	{
		return (float)(_rng.NextDouble() * (max - min)) + min;
	}
}
