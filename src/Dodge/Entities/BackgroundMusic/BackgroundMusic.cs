using Dodge.Lib.PowerUps;
using Godot;

namespace Dodge.Entities;

public partial class BackgroundMusic : AudioStreamPlayer
{
	public void OnSlowdownPowerUpActive(SlowdownEffect effect)
	{
		PitchScale = effect.VelocityModifier;
	}

	public void OnSlowdownPowerUpEnded()
	{
		PitchScale = 1.0f;
	}
}
