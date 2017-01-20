using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealtimeRaytrace
{
    public struct MoveDepthCommand : IPlayerCommand
    {
        float _forwardAmount;
        public MoveDepthCommand(float forwardAmount)
        {
            _forwardAmount = forwardAmount;
        }
        public void Execute(Player player,float elapsedTotalSeconds)
        {
            player.MoveDepth(_forwardAmount, elapsedTotalSeconds);
        }
    }
}
