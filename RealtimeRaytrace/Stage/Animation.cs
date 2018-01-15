using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeRaytrace
{

    public class Animation
    {
        List<AnimationFrame> _animationFrameList;

        public Animation(List<AnimationFrame> animationFrameList)
        {
            _animationFrameList = animationFrameList;
        }
    }
}
