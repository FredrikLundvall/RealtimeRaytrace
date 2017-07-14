using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeRaytrace
{
    public class AntiSphereAntiIntersection
    {
        private readonly AntiSphere _antiSphere;
        private AntiIntersection _antiIntersection;

        public AntiSphereAntiIntersection(AntiSphere antiSphere)
        {
            _antiSphere = antiSphere;
            _antiIntersection = new AntiIntersection(true);
        }

        public AntiSphere GetAntiSphere()
        {
            return _antiSphere;
        }

        public AntiIntersection GetAntiIntersection()
        {
            return _antiIntersection;
        }

        public void SetAntiIntersection(AntiIntersection antiIntersection)
        {
            _antiIntersection = antiIntersection;
        }
    }
}
