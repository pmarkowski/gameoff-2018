using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Killowatt
{
    public class GameMessageLogger
    {
        public void LogMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
