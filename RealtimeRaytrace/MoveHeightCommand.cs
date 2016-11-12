using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealtimeRaytrace
{
    public class MoveHeightCommand : IPlayerCommand
    {
        float _heightAmount;
        public MoveHeightCommand(float heightAmount)
        {
            _heightAmount = heightAmount;
        }

        public void Execute(Player player, float elapsedTotalSeconds)
        {
            player.MoveHeight(_heightAmount, elapsedTotalSeconds);
        }
    }
}
