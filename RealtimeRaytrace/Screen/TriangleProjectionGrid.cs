using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class TriangleProjectionGrid
    {
        float _minX, _maxX;
        float _minY, _maxY;
        float _z = 1;
        TriangleIndex _triangleIndex;

        public TriangleProjectionGrid(float minX, float minY, float maxX, float maxY)
        {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
            _triangleIndex = new TriangleIndex(_minX, _minY, _maxX, _maxY);
        }
        public void CreateGrid()
        {
            int radiusDiff = 1;
            int radius = 1;
            List<Vector3> lastLayerPoints = new List<Vector3>(170 * 6);
            List<Vector3> currentLayerPoints = new List<Vector3>(170 * 6);
            List<Vector3> swapLayerPoints;
            for (int i = 1; i < 170; i++)
            {
                swapLayerPoints = lastLayerPoints;
                lastLayerPoints = currentLayerPoints;
                currentLayerPoints = swapLayerPoints;
                MakeTriangleHexagonRing(radius, i, lastLayerPoints, currentLayerPoints);
                radius += radiusDiff;

                if (i % 3 == 0 && i > 75)
                    radiusDiff += 1;
            }
        }

        protected void MakeTriangleHexagonRing(int radius, int curLayer, List<Vector3> lastLayerPoints, List<Vector3> currentLayerPoints)
        {
            currentLayerPoints.Clear();
            int prevLayer = curLayer - 1;    
            int curPointsInCircle = curLayer * 6;
            int numOfTrianglesOnSide = prevLayer * 2 + 1;

            Vector3 curPoint, nextPoint;
            Vector3 innerPoint;

            int p = 0;
            int sida = 6;
            for (int s = 0; s < sida; s++)
            {
                for (int t = 0; t < numOfTrianglesOnSide; t++)
                {
                    if ((t & 1) == 0)
                    {
                        innerPoint = GetInnerPoint(p - s, lastLayerPoints);
                        curPoint = CalcPoint(p, curPointsInCircle, radius);
                        nextPoint = CalcPoint(p + 1, curPointsInCircle, radius);
                        _triangleIndex.AddTriangle(new Triangle(innerPoint, curPoint, nextPoint));
                        p++;
                        currentLayerPoints.Add(curPoint);
                    }
                    else
                    {
                        innerPoint = GetInnerPoint(p - 1 - s, lastLayerPoints);
                        nextPoint = GetInnerPoint(p - s, lastLayerPoints);
                        curPoint = CalcPoint(p, curPointsInCircle, radius);
                        _triangleIndex.AddTriangle(new Triangle(innerPoint, curPoint, nextPoint));
                    }
                }
            }
        }

        protected Vector3 GetInnerPoint(int index, List<Vector3> lastLayerPoints)
        {
            if(lastLayerPoints.Count == 0)
                return new Vector3(0, 0, _z);
            //använd lastLayerPoints för att hämta innerPoint (från förra varvet)
            return lastLayerPoints[index % lastLayerPoints.Count ];
        }

        protected Vector3 CalcPoint(int index, int totalPoints, int radius)
        {
            //radius += CalcFibonacci(1, 1, index);
            double angleForCurrentPoint = (Math.PI * 2) * (index % totalPoints) / (double)totalPoints;
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

