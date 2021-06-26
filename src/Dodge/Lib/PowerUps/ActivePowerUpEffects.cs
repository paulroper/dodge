namespace Dodge.PowerUps
{
    public class ActivePowerUpEffects
    {
        public SlowdownEffect? Slowdown { get; set; }
        public int Count { get => new bool[] { Slowdown != null }.Length; }

        public void Clear()
        {
            Slowdown = null;
        }
    }
}
