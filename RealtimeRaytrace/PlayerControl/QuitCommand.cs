using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeRaytrace
{
    public struct QuitCommand : IPlayerCommand
    {
        public void Execute(Player player, float elapsedTotalSeconds)
        {
            player.DoQuit();
        }
    }
}