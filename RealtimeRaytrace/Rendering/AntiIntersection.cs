using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public struct AntiIntersection
    {
        private
        readonly bool _isNull;
        //readonly Vector3 _positionNear;
        readonly Vector3 _positionFar;
        //readonly Vector3 _normalNear;
        readonly Vector3 _normalFar;
        readonly Vector3 _normalFarTexture;
        readonly float _tNear;//Avstånd till träffen
        readonly float _tFar;//Avstånd till träffen
        readonly SphereBase _sphereBase;

        public AntiIntersection(bool isNull)
        {
            _isNull = true;
            //_positionNear = Vector3.Zero;
            _positionFar = Vector3.Zero;
            //_normalNear = Vector3.Zero;
            _normalFar = Vector3.Zero;
            _normalFarTexture = Vector3.Zero;
            _tNear = float.MaxValue;
            _tFar = float.MaxValue;
            _sphereBase = null;
        }

        public AntiIntersection(Vector3 positionFar, Vector3 normalFar, Vector3 normalFarTexture, float tNear, float tFar, SphereBase sphereBase)
        {
            _isNull = false;
            //_positionNear = positionNear;
            _positionFar = positionFar;
            //_normalNear = normalNear;
            _normalFar = normalFar;
            _normalFarTexture = normalFarTexture;
            _tNear = tNear;
            _tFar = tFar;
            _sphereBase = sphereBase;
        }

        public Intersection CreateIntersection(Vector3 parentNormalTexture)
        {
            //return new Intersection(_positionFar, _normalFar, _normalFarTexture, _tFar, _tFar, _tNear, _sphereBase);
            return new Intersection(_positionFar, _normalFar, parentNormalTexture, _tFar, _tFar, _tNear, _sphereBase);
        }

        public float GetTNear()
        {
            return _tNear;
        }

        public float GetTFar()
        {
            return _tFar;
        }

        //public Vector3 GetPositionNear()
        //{
        //    return _positionNear;
        //}

        public Vector3 GetPositionFar()
        {
            return _positionFar;
        }

        //public Vector3 GetNormalNear()
        //{
        //    return _normalNear;
        //}

        public Vector3 GetNormalFar()
        {
            return _normalFar;
        }

        public Vector3 GetNormalFarTexture()
        {
            return _normalFarTexture;
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
            return _isNull;
        }

        public override string ToString()
        {
            return string.Format("pos: {0}, normal: {1}, distance: {2}, sphere: {3}", _tNear.ToString(), (_sphereBase != null) ? _sphereBase.ToString() : "null");
        }

    }
}
