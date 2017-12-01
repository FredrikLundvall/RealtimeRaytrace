using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public struct Intersection
    {
        private
        readonly bool _hasValue;
        readonly Vector3 _positionFirstHit;
        readonly Vector3 _normalFirstHit;
        readonly Vector3 _normalFirstHitTexture;
        readonly float _tFirstHit;//Distance to the first hit (the nearest if not inside the sphere)
        readonly float _tNear;//Distance to the nearest hit
        readonly float _tFar;//Distance to the farthest hit
        readonly SphereBase _sphereBase;

        public Intersection(bool isNull)
        {
            _hasValue = !isNull;
            _positionFirstHit = Vector3.Zero;
            _normalFirstHit = Vector3.Zero;
            _normalFirstHitTexture = Vector3.Zero;
            _tFirstHit = float.MaxValue;
            _tNear = float.MaxValue;
            _tFar = float.MaxValue;
            _sphereBase = null;
        }

        public Intersection(Vector3 positionFirstHit, Vector3 normalFirstHit, Vector3 normalFirstHitTexture, float tFirstHit, float tNear, float tFar, SphereBase sphereBase)
        {
            _hasValue = true;
            _positionFirstHit = positionFirstHit;
            _normalFirstHit = normalFirstHit;
            _normalFirstHitTexture = normalFirstHitTexture;
            _tFirstHit = tFirstHit;
            _tNear = tNear;
            _tFar = tFar;
            _sphereBase = sphereBase;
        }

        public float GetTFirstHit()
        {
            return _tFirstHit;
        }

        public float GetTNear()
        {
            return _tNear;
        }

        public float GetTFar()
        {
            return _tFar;
        }

        public Vector3 GetPositionFirstHit()
        {
            return _positionFirstHit;
        }

        public Vector3 GetNormalFirstHit()
        {
            return _normalFirstHit;
        }

        public Vector3 GetNormalFirstHitTexture()
        {
            return _normalFirstHitTexture;
        }

        public SphereBase GetSphere()
        {
            return _sphereBase;
        }

        public bool IsHit()
        {
            return !(_sphereBase == null);
        }

        public bool IsNull()
        {
            return !_hasValue;
        }

        public override string ToString()
        {
            return string.Format("pos: {0}, normal: {1}, distance: {2}, sphere: {3}", _positionFirstHit.ToString(), _normalFirstHit.ToString(), _tFirstHit.ToString(), (_sphereBase != null) ? _sphereBase.ToString() : "null");
        }

    }
}
