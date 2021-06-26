using Dodge.Entities;
using Godot;

namespace Dodge.PowerUps
{
    public class PowerUpWizard : IPowerUpWizard
    {
        public bool Apply(Node node, ActivePowerUpEffects effects)
        {
            return node switch
            {
                Mob mob => ApplyMobEffects(mob, effects),
                Timer timer => ApplyTimerEffects(timer, effects),
                _ => false
            };
        }

        private bool ApplyMobEffects(Mob mob, ActivePowerUpEffects effects)
        {
            var slowdownEffect = effects.Slowdown;
            if (slowdownEffect is null)
            {
                return false;
            }


            mob.LinearVelocity = new Vector2(
                mob.LinearVelocity.x * slowdownEffect.VelocityModifier,
                mob.LinearVelocity.y * slowdownEffect.VelocityModifier
            );

            return true;
        }

        private bool ApplyTimerEffects(Timer timer, ActivePowerUpEffects effects)
        {
            var slowdownEffect = effects.Slowdown;
            if (slowdownEffect is null)
            {
                return false;
            }

            timer.WaitTime *= slowdownEffect.TimerModifier;

            return true;
        }
    }
}
