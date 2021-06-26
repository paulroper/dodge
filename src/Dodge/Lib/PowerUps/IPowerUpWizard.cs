using Godot;

namespace Dodge.PowerUps
{
    public interface IPowerUpWizard
    {
        bool Apply(Node node, ActivePowerUpEffects effects);
    }
}
