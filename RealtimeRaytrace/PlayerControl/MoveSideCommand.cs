using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealtimeRaytrace
{
    public struct MoveSideCommand : IPlayerCommand
    {
        float _sideAmount;
        public MoveSideCommand(float sideAmount)
        {
            _sideAmount = sideAmount;
        }

        public void Execute(Player player, float elapsedTotalSeconds)
        {
            player.MoveSide(_sideAmount, elapsedTotalSeconds);
        }
    }
}
