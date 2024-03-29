using System.Globalization;
using Godot;

namespace Dodge.Entities;

public static class HudNodes
{
	public const string HiScoreLabel = "HiScoreLabel";
	public const string StartButton = "StartButton";
	public const string Message = "Message";
	public const string MessageTimer = "MessageTimer";
	public const string ScoreLabel = "ScoreLabel";
}

public partial class Hud : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();

	public void ShowMessage(string text)
	{
		var message = GetMessage();
		message.Text = text;
		message.Show();

		GetMessageTimer().Start();
	}

	public async void ShowGameOver()
	{
		ShowMessage("Game over\n:(");

		var messageTimer = GetMessageTimer();
		await ToSignal(messageTimer, "timeout");

		var message = GetMessage();
		message.Text = "Dodge the\ncreeps!";
		message.Show();

		await ToSignal(GetTree().CreateTimer(1), "timeout");
		GetButton().Show();
	}

	public void UpdateHiScore(int hiScore)
	{
		GetHiScoreLabel().Text = hiScore.ToString(CultureInfo.InvariantCulture);
	}

	public void UpdateScore(int score)
	{
		GetScoreLabel().Text = score.ToString(CultureInfo.InvariantCulture);
	}

	public void OnStartButtonPressed()
	{
		GetButton().Hide();
		EmitSignal(nameof(StartGame));
	}

	public void OnMessageTimerTimeout()
	{
		GetMessage().Hide();
	}

	private Label GetHiScoreLabel() =>
	  GetNode<Label>(HudNodes.HiScoreLabel);

	private Label GetScoreLabel() =>
	  GetNode<Label>(HudNodes.ScoreLabel);

	private Button GetButton() =>
	  GetNode<Button>(HudNodes.StartButton);

	private Label GetMessage() =>
	  GetNode<Label>(HudNodes.Message);

	private Timer GetMessageTimer() =>
	  GetNode<Timer>(HudNodes.MessageTimer);
}
