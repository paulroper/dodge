using System.Collections.Generic;
using Godot;

namespace Dodge.PowerUps
{
    public interface IPowerUpWizard
    {
        IList<IPowerUpEffect> Apply(Node node, IList<IPowerUpEffect> effects);
    }
}
