using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public struct BoundingBoxIntersection
    {
        private
        readonly bool _hasValue;
        readonly float _tFirstHit;//Distance to the first hit

        public BoundingBoxIntersection(bool isNull)
        {
            _hasValue = !isNull;
            _tFirstHit = float.MaxValue;
        }

        public BoundingBoxIntersection(float tFirstHit)
        {
            _hasValue = true;
            _tFirstHit = tFirstHit;
        }

        //No intersection
        public static BoundingBoxIntersection CreateNullBoundingBoxIntersection()
        {
            return new BoundingBoxIntersection(true);
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
