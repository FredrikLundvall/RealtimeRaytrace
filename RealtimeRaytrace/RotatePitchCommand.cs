using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeRaytrace
{
    public struct RotatePitchCommand : IPlayerCommand
    {
        float _pitchAmount;
        public RotatePitchCommand(float pitchAmount)
        {
            _pitchAmount = pitchAmount;
        }
        public void Execute(Player player, float elapsedTotalSeconds)
        {
            player.RotatePitch(_pitchAmount, elapsedTotalSeconds);
        }
    }
}