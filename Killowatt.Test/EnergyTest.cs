using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Killowatt.Test
{
    public class EnergyTest
    {
        [Fact]
        public void CurrentSoc_NewObject_EqualToMax()
        {
            Energy energy = new Energy(20, 3);
            Assert.Equal(energy.MaxSoc, energy.CurrentSoc);
        }

        [Fact]
        public void CurrentSoc_SetNegative_ClampsToZero()
        {
            Energy energy = new Energy(20, 3);
            energy.CurrentSoc = -10;
            Assert.Equal(0, energy.CurrentSoc);
        }

        [Fact]
        public void CurrentFuel_NewObject_EqualToMax()
        {
            Energy energy = new Energy(20, 3);
            Assert.Equal(energy.MaxFuel, energy.CurrentFuel);
        }

        [Fact]
        public void CurrentFuel_SetNegative_ClampsToZero()
        {
            Energy energy = new Energy(20, 3);
            energy.CurrentFuel = -10;
            Assert.Equal(0, energy.CurrentFuel);
        }
    }
}
