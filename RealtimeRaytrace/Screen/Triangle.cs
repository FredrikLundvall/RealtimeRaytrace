using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public struct Triangle
    {
        Vector3 _p1, _p2, _p3;

        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
        }

        public Triangle(Triangle t)
            : this(t.GetP1(), t.GetP2(), t.GetP3())
        {
        }

        public Vector3 GetP1() { return _p1; }
        public Vector3 GetP2() { return _p2; }
        public Vector3 GetP3() { return _p3; }

        public float GetSizeX() 
        { 
            float xMin = Math.Min(Math.Min(_p1.X,_p2.X),_p3.X);
            float xMax = Math.Max(Math.Max(_p1.X, _p2.X), _p3.X);
            return xMax - xMin; 
        }
        public float GetSizeY()
        {
            float yMin = Math.Min(Math.Min(_p1.Y, _p2.Y), _p3.Y);
            float yMax = Math.Max(Math.Max(_p1.Y, _p2.Y), _p3.Y);
            return yMax - yMin;
        }
        public void FlipHorizontalBaselineY()
        {
            if (_p1.Y == _p2.Y)
            {
                _p1.Y = _p3.Y;
                _p3.Y = _p2.Y;
                _p2.Y = _p1.Y;
            }
            else if (_p1.Y == _p3.Y)
            {
                _p1.Y = _p2.Y;
                _p2.Y = _p3.Y;
                _p3.Y = _p1.Y;
            }
            else if (_p2.Y == _p3.Y)
            {
                _p2.Y = _p1.Y;
                _p1.Y = _p3.Y;
                _p3.Y = _p2.Y;
            }
        }
        public void Move(Vector3 distance)
        {
            _p1 += distance;
            _p2 += distance;
            _p3 += distance;
        }

        public override bool Equals(Object obj)
        {
            return obj is Triangle && this == (Triangle)obj;
        }
        public override int GetHashCode()
        {
            return _p1.GetHashCode() + _p2.GetHashCode() + _p3.GetHashCode();
        }
        //The order of the vertices in the triangles are not important
        public static bool operator ==(Triangle x, Triangle y)
        {
            return (x._p1 == y._p1 && x._p2 == y._p2 && x._p3 == y._p3) ||
                 (x._p1 == y._p2 && x._p2 == y._p3 && x._p3 == y._p1) ||
                 (x._p1 == y._p3 && x._p2 == y._p1 && x._p3 == y._p2);
        }
        public static bool operator !=(Triangle x, Triangle y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2}", _p1.ToString(), _p2.ToString(), _p3.ToString());
        }
    }
}
