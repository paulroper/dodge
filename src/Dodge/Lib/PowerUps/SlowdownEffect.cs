namespace Dodge.PowerUps
{
	public partial class SlowdownEffect : Godot.GodotObject
	{
		public float TimerModifier { get; }
		public float VelocityModifier { get; }

		public SlowdownEffect(float timerModifier, float velocityModifier)
		{
			TimerModifier = timerModifier;
			VelocityModifier = velocityModifier;
		}
	}
}
