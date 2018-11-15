using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Killowatt
{
    public class GameMessageLogger
    {
        SadConsole.Console messageConsole;

        public GameMessageLogger(SadConsole.Console messageConsole)
        {
            this.messageConsole = messageConsole;
        }
        public void LogMessage(string message)
        {
            messageConsole.ShiftUp();
            messageConsole.Fill(
                new Microsoft.Xna.Framework.Rectangle(0, 0, messageConsole.Width, 1),
                Microsoft.Xna.Framework.Color.DimGray,
                null,
                null);
            messageConsole.Print(0, 1, $">{message}");
        }
    }
}
