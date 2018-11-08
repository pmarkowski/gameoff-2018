using System;

namespace Killowatt
{
    public class Energy
    {
        public int MaxSoc { get; private set; }
        private int currentSoc;
        public int CurrentSoc
        {
            get
            {
                return currentSoc;
            }
            set
            {
                currentSoc = Math.Max(0, value);
            }
        }

        public int MaxFuel { get; private set; }
        private int currentFuel;
        public int CurrentFuel
        {
            get
            {
                return currentFuel;
            }
            set
            {
                currentFuel = Math.Max(0, value);
            }
        }

        public Energy(int maxSoc, int maxFuel)
        {
            MaxSoc = maxSoc;
            MaxFuel = maxFuel;
            currentSoc = maxSoc;
            currentFuel = maxFuel;
        }

        public bool HasEnergy()
        {
            return (CurrentSoc != 0) || (CurrentFuel != 0);
        }

        public bool HasEnoughEnergy(int energy)
        {
            return (CurrentSoc + CurrentFuel) > energy;
        }

        public void ConsumeEnergy(int energy)
        {
            int overflow = Math.Abs(CurrentSoc - energy);
            CurrentSoc -= energy;
            if (overflow > 0)
            {
                CurrentFuel -= overflow;
            }
        }
    }
}