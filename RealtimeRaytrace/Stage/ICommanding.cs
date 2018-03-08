using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealtimeRaytrace
{
    public interface ICommanding
    {
        void Execute(ICommandable player, float elapsedTotalSeconds);
    }
}
