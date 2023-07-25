using Dodge.PowerUps;
using Godot;

namespace Dodge.Entities
{
    public partial class MobTimer : Timer
    {
        private double _originalWaitTime;

        public void OnSlowdownPowerUpActive(SlowdownEffect effect)
        {
            _originalWaitTime = WaitTime;
            WaitTime *= effect.TimerModifier;
        }

        public void OnSlowdownPowerUpEnded()
        {
            WaitTime = _originalWaitTime;
        }
    }
}
