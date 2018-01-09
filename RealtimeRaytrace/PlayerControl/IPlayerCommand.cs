using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealtimeRaytrace
{
    public interface IPlayerCommand
    {
        void Execute(Player player, float elapsedTotalSeconds);
    }
}
