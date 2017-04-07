using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class TriangleProjectionGrid2
    {
        float _minX, _maxX;
        float _minY, _maxY;
        float _z = 1;
        TriangleIndex _triangleIndex;

        public TriangleProjectionGrid2(float minX, float minY, float maxX, float maxY)
        {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
            _triangleIndex = new TriangleIndex(_minX, _minY, _maxX, _maxY);
        }

        public void MakeTriangleHexagonRing(int radius, int count)
        {
            int pointsInCircle = count * 6;
            Vector3 prevPoint,curPoint;
            Vector3 innerPoint = new Vector3(0, 0, _z);
            curPoint = CalcPoint(1, pointsInCircle, radius);
            Vector3 firstPointInRing = curPoint;
            for (int i = 2; i <= pointsInCircle; i++)
            {
                prevPoint = curPoint;
                curPoint = CalcPoint(i, pointsInCircle, radius);
                _triangleIndex.AddTriangle(new Triangle(innerPoint, prevPoint, curPoint));
            }
            _triangleIndex.AddTriangle(new Triangle(innerPoint, curPoint, firstPointInRing));

            //kolla om det går att använda _verticesLookup i _triangleIndex för att hämta
            //innerPoint i nästa varv...
        }

        protected Vector3 CalcPoint(int index, int totalPoints, int radius)
        {
            radius += CalcFibonacci(1, 1, index);
            double angleForCurrentPoint = (Math.PI * 2) * (index - 1) / (double)totalPoints;
            return new Vector3((float)Math.Cos(angleForCurrentPoint) * radius, (float)Math.Sin(angleForCurrentPoint) * radius, _z);
        }

        protected int CalcFibonacci(int start0, int start1, int index)
        {
            double fi = (1 + Math.Sqrt(5)) / 2;
            double th = - 1 / fi;
            double a = (start1 - start0 * th) / Math.Sqrt(5);
            double b = (start0 * fi - start1) / Math.Sqrt(5);
            return (int)Math.Round(a * Math.Pow(fi,index) + b * Math.Pow(th, index));
        }

        public TriangleIndex GetTriangleIndex()
        {
            return _triangleIndex;
        }

    }
}

