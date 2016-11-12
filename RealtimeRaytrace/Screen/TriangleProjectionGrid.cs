using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class TriangleProjectionGrid
    {
        public enum HexagonSide {Side0 = 0, Side1, Side2, Side3, Side4, Side5};

        float _minX, _maxX;
        float _minY, _maxY;
        float _sizeX, _sizeY;
        float _z = 1;
        TriangleIndex _triangleIndex;

        public TriangleProjectionGrid(float minX, float minY, float maxX, float maxY, float sizeX, float sizeY)
        {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
            _sizeX = sizeX;
            _sizeY = sizeY;
            _triangleIndex = new TriangleIndex(_minX, _minY, _maxX, _maxY);
        }
        public void AddTriangle(Triangle t)
        {
            _triangleIndex.AddTriangle(t);
        }

        public void MakeTriangleHexagonRing(int radiusMultiple, int triangleSizeMultiple)
        {

            int trianglesPerSide = (radiusMultiple / triangleSizeMultiple) * 2 + 1;

            if(trianglesPerSide <= 1 && radiusMultiple != 0 )
                throw new ArgumentException("The radiusMultiple and triangleSizeMultiple combination is invalid");

            trianglesPerSide = Math.Max(1, trianglesPerSide);

            Triangle t0,t;
            HexagonSide side;

            //TODO: gör alla sidor
            side = HexagonSide.Side0;
            t0 = getStartTriangle(side, radiusMultiple, triangleSizeMultiple);
            for (int i = 0; i < trianglesPerSide; i++)
            {
                t = getTriangleAtSide(side, t0, i, triangleSizeMultiple);
                _triangleIndex.AddTriangle(t);
            }

            side = HexagonSide.Side1;
            t0 = getStartTriangle(side, radiusMultiple, triangleSizeMultiple);
            for (int i = 0; i < trianglesPerSide; i++)
            {
                t = getTriangleAtSide(side, t0, i, triangleSizeMultiple);
                _triangleIndex.AddTriangle(t);
            }

            side = HexagonSide.Side2;
            t0 = getStartTriangle(side, radiusMultiple, triangleSizeMultiple);
            for (int i = 0; i < trianglesPerSide; i++)
            {
                t = getTriangleAtSide(side, t0, i, triangleSizeMultiple);
                _triangleIndex.AddTriangle(t);
            }

            side = HexagonSide.Side3;
            t0 = getStartTriangle(side, radiusMultiple, triangleSizeMultiple);
            for (int i = 0; i < trianglesPerSide; i++)
            {
                t = getTriangleAtSide(side, t0, i, triangleSizeMultiple);
                _triangleIndex.AddTriangle(t);
            }

            side = HexagonSide.Side4;
            t0 = getStartTriangle(side, radiusMultiple, triangleSizeMultiple);
            for (int i = 0; i < trianglesPerSide; i++)
            {
                t = getTriangleAtSide(side, t0, i, triangleSizeMultiple);
                _triangleIndex.AddTriangle(t);
            }

            side = HexagonSide.Side5;
            t0 = getStartTriangle(side, radiusMultiple, triangleSizeMultiple);
            for (int i = 0; i < trianglesPerSide; i++)
            {
                t = getTriangleAtSide(side, t0, i, triangleSizeMultiple);
                _triangleIndex.AddTriangle(t);
            }

        }

        private Triangle getStartTriangle(HexagonSide side, int radiusMultiple, int triangleSizeMultiple)
        {
            Triangle t;
            bool oddSide = ((int)side & 1) == 1;

            t = new Triangle(
                new Vector3(0, 0, _z),
                new Vector3(triangleSizeMultiple * -_sizeX, 0, _z),
                new Vector3(triangleSizeMultiple * -_sizeX / 2.0f, triangleSizeMultiple * -_sizeY, _z)
                );
            if (oddSide)
                t.FlipHorizontalBaselineY();

            Vector3 diff = (getStartOffset(side) * triangleSizeMultiple) + (getRadiusMovement(side) * radiusMultiple);

            t.Move(diff);
            return t;
        }

        private Vector3 getStartOffset(HexagonSide side)
        {
            float difX = 0, difY = 0;

            switch (side)
            {
                case HexagonSide.Side0:
                    difX = 0;
                    difY = 0;
                    break;
                case HexagonSide.Side1:
                    difX = _sizeX / 2.0f;
                    difY = 0;
                    break;
                case HexagonSide.Side2:
                    difX = _sizeX;
                    difY = 0;
                    break;
                case HexagonSide.Side3:
                    difX = _sizeX;
                    difY = _sizeY;
                    break;
                case HexagonSide.Side4:
                    difX = _sizeX / 2.0f;
                    difY = _sizeY;
                    break;
                case HexagonSide.Side5:
                    difX = 0;
                    difY = _sizeY;
                    break;
            }
            return new Vector3(difX, difY, 0);
        }

        private Vector3 getOddOffset(HexagonSide side)
        {
            float difX = 0, difY = 0;

            switch (side)
            {
                case HexagonSide.Side0:
                    difX = _sizeX / 2.0f;
                    difY = 0;
                    break;
                case HexagonSide.Side1:
                    difX = _sizeX / 2.0f;
                    difY = 0;
                    break;
                case HexagonSide.Side2:
                    difX = 0;
                    difY = _sizeY;
                    break;
                case HexagonSide.Side3:
                    difX = -_sizeX / 2.0f;
                    difY = 0;
                    break;
                case HexagonSide.Side4:
                    difX = -_sizeX / 2.0f;
                    difY = 0;
                    break;
                case HexagonSide.Side5:
                    difX = 0;
                    difY = -_sizeY;
                    break;
            }
            return new Vector3(difX, difY, 0);
        }

        private Vector3 getRadiusMovement(HexagonSide side)
        {
            float difX = 0, difY = 0;

            switch (side)
            {
                case HexagonSide.Side0:
                    difX = -_sizeX;
                    difY = 0;
                    break;
                case HexagonSide.Side1:
                    difX = -_sizeX / 2.0f;
                    difY = -_sizeY;
                    break;
                case HexagonSide.Side2:
                    difX = _sizeX / 2.0f;
                    difY = -_sizeY;
                    break;
                case HexagonSide.Side3:
                    difX = _sizeX;
                    difY = 0;
                    break;
                case HexagonSide.Side4:
                    difX = _sizeX / 2.0f;
                    difY = _sizeY;
                    break;
                case HexagonSide.Side5:
                    difX = -_sizeX / 2.0f;
                    difY = _sizeY;
                    break;
            }
            return new Vector3(difX, difY, 0);
        }

        private Vector3 getSideMovement(HexagonSide side)
        {
            return getRadiusMovement((HexagonSide)(((int)side + 2) % 6));
        }

        private Triangle getTriangleAtSide(HexagonSide side, Triangle startTriangle, int linePosition, int triangleSizeMultiple)
        {
            bool oddPosition = (linePosition & 1) == 1;

            //Check if the triangle should be flipped or not
            if (oddPosition)
            {
                //Its supposed to be flipped, so flipp it.. 
                startTriangle.FlipHorizontalBaselineY();
            }

            linePosition = linePosition / 2;

            Triangle triangleAtSide = new Triangle(startTriangle);

            Vector3 diff = Vector3.Zero;
            if (oddPosition)
                diff = getOddOffset(side) * triangleSizeMultiple;
            diff = diff + getSideMovement(side) * linePosition * triangleSizeMultiple;
            triangleAtSide.Move(diff);

            return triangleAtSide;
        }

        public TriangleIndex GetTriangleIndex()
        {
            return _triangleIndex;
        }

    }
}
