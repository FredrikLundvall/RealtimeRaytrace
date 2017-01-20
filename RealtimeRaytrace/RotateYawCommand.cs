using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RealtimeRaytrace
{
    public struct RotateYawCommand : IPlayerCommand
    {
        float _yawAmount;
        public RotateYawCommand(float yawAmount)
        {
            _yawAmount = yawAmount;
        }
        public void Execute(Player player, float elapsedTotalSeconds)
        {
            player.RotateYaw(_yawAmount, elapsedTotalSeconds);
        }
    }
}