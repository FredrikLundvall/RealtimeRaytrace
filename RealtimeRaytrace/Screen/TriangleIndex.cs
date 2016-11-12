using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RealtimeRaytrace
{
    public class TriangleIndex
    {
        private struct IndexListItem
        {
            int _idx1, _idx2, _idx3;

            public IndexListItem(int idx1, int idx2, int idx3)
            {
                _idx1 = idx1;
                _idx2 = idx2;
                _idx3 = idx3;
            }

            public override bool Equals(Object obj)
            {
                return obj is IndexListItem && this == (IndexListItem)obj;
            }
            public override int GetHashCode()
            {
                return _idx1 + _idx2 + _idx3;
            }
            //The order of the vertices in the triangles are not important
            public static bool operator ==(IndexListItem x, IndexListItem y)
            {
                return (x._idx1 == y._idx1 && x._idx2 == y._idx2 && x._idx3 == y._idx3) ||
                     (x._idx1 == y._idx2 && x._idx2 == y._idx3 && x._idx3 == y._idx1) ||
                     (x._idx1 == y._idx3 && x._idx2 == y._idx1 && x._idx3 == y._idx2);
            }
            public static bool operator !=(IndexListItem x, IndexListItem y)
            {
                return !(x == y);
            }
        }

        float _minX, _maxX;
        float _minY, _maxY;
        //Using Dictionary for lookup and List for the right order 
        //TODO: Use a sorted output on the int(index) in _verticesLookup when returning array from GetVerticesPositionColor() 
        List<Vector3> _vertices;
        Dictionary<Vector3,int> _verticesLookup;
        List<int> _trianglesIndexlist;
        Dictionary<IndexListItem, int> _trianglesIndexlistDuplicateCheck;

        public TriangleIndex(float minX, float minY, float maxX, float maxY)
        {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
            _vertices = new List<Vector3>(30000);
            _verticesLookup = new Dictionary<Vector3, int>(30000);
            _trianglesIndexlist = new List<int>(90000);
            _trianglesIndexlistDuplicateCheck = new Dictionary<IndexListItem, int>(30000);
        }

        public void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {           
            //Check if the whole triangle is outside the viewport. If so, don't add it
            if (isVectorOutsideBoundaries(p1) && isVectorOutsideBoundaries(p2) && isVectorOutsideBoundaries(p3))
                return;

            int idx1 = addVector(p1);
            int idx2 = addVector(p2);
            int idx3 = addVector(p3);

            IndexListItem triangleIndexItem = new IndexListItem(idx1, idx2, idx3);

            //Check if the triangle allready exists
            if (_trianglesIndexlistDuplicateCheck.ContainsKey(triangleIndexItem))
                return;

            _trianglesIndexlist.Add(idx1);
            _trianglesIndexlist.Add(idx2);
            _trianglesIndexlist.Add(idx3);

            _trianglesIndexlistDuplicateCheck.Add(triangleIndexItem, _trianglesIndexlistDuplicateCheck.Count);

        }
        public void AddTriangle(Triangle triangle)
        {
            AddTriangle(triangle.GetP1(), triangle.GetP2(), triangle.GetP3());
        }

        public int[] GetIndices()
        {
            return _trianglesIndexlist.ToArray();
        }

        public VertexPositionColor[] GetVerticesPositionColor()
        {
            VertexPositionColor[] verticesPositionColor = new VertexPositionColor[_vertices.Count];

            for (int i = 0; i < _vertices.Count; i++)
            {
                verticesPositionColor[i] = new VertexPositionColor(_vertices[i], Color.Black);   
            }
            return verticesPositionColor;
        }

        private int addVector(Vector3 p)
        {
            int idx;
            if (!_verticesLookup.TryGetValue(p, out idx))
            {
                idx = _vertices.Count();
                _vertices.Add(p);
                _verticesLookup.Add(p, idx);
            }

            return idx;
        }

        private bool isVectorOutsideBoundaries(Vector3 p)
        {
            return p.X < _minX || p.X > _maxX || p.Y < _minY || p.Y > _maxY;
        }

    }
}
