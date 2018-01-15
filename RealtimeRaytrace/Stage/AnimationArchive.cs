using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeRaytrace
{
    public class AnimationArchive
    {
        Dictionary<int, Animation> _animationDictonary;

        public AnimationArchive(Dictionary<int, Animation> animationDictonary)
        {
            _animationDictonary = animationDictonary;
        }
    }
}
