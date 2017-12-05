using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public struct IntersectionBox
    {
        private
        readonly bool _hasValue;
        readonly float _tFirstHit;//Distance to the first hit

        public IntersectionBox(bool isNull)
        {
            _hasValue = !isNull;
            _tFirstHit = float.MaxValue;
        }

        public IntersectionBox(float tFirstHit)
        {
            _hasValue = true;
            _tFirstHit = tFirstHit;
        }

        public float GetTFirstHit()
        {
            return _tFirstHit;
        }

        public bool IsHit()
        {
            return _hasValue;
        }

        public override string ToString()
        {
            return string.Format("distance to hit: {0}, hit: {1}", _tFirstHit.ToString(), (_hasValue) ? "Hit" : "No hit");
        }

    }
}
