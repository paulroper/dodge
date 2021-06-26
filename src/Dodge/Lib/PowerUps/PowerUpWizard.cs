using System.Collections.Generic;
using System.Linq;
using Dodge.Entities;
using Godot;

namespace Dodge.PowerUps
{
    public class PowerUpWizard : IPowerUpWizard
    {
        public IList<IPowerUpEffect> Apply(Node node, IList<IPowerUpEffect> effects)
        {
            return node switch
            {
                Mob mob => ApplyMobEffects(mob, effects),
                Timer timer => ApplyTimerEffects(timer, effects),
                _ => effects
            };
        }

        private IList<IPowerUpEffect> ApplyMobEffects(Mob mob, IList<IPowerUpEffect> effects)
        {
            if (!(effects.LastOrDefault(effects => effects is SlowdownEffect) is SlowdownEffect slowdownEffect))
            {
                return new List<IPowerUpEffect>();
            }

            mob.LinearVelocity = new Vector2(
                mob.LinearVelocity.x * slowdownEffect.VelocityModifier,
                mob.LinearVelocity.y * slowdownEffect.VelocityModifier
            );

            return new List<IPowerUpEffect>() { slowdownEffect };
        }

        private IList<IPowerUpEffect> ApplyTimerEffects(Timer timer, IList<IPowerUpEffect> effects)
        {
            if (!(effects.LastOrDefault(effects => effects is SlowdownEffect) is SlowdownEffect slowdownEffect))
            {
                return new List<IPowerUpEffect>();
            }

            timer.WaitTime *= slowdownEffect.TimerModifier;

            return new List<IPowerUpEffect>() { slowdownEffect };
        }
    }
}
