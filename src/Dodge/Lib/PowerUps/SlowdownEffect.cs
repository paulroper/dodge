namespace Dodge.PowerUps
{
    public class SlowdownEffect : Godot.Object, IPowerUpEffect
    {
        public float OriginalTimerWait { get; set; }
        public float TimerModifier { get; }
        public float VelocityModifier { get; }

        public SlowdownEffect(float timerModifier, float velocityModifier)
        {
            TimerModifier = timerModifier;
            VelocityModifier = velocityModifier;
        }
    }
}
