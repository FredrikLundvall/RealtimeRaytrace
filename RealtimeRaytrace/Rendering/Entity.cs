using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Entity
    {
        protected readonly int _index;
        protected readonly bool _isIndexedByPosition;
        protected Vector3 _position = Vector3.Zero;

        public Entity(int index, bool isIndexedByPosition, Vector3 position)
        {
            _index = index;
            _isIndexedByPosition = isIndexedByPosition;
            _position = position;
        }

        public int GetIndex()
        {
            return _index;
        }

        public bool GetIsIndexedByPosition()
        {
            return _isIndexedByPosition;
        }

        public void MoveDepth(float distanceZ) // move in Z axis
        {
            MoveVector(new Vector3(0, 0, distanceZ));   
        }

        public void MoveSide(float distanceX) // move in X axis
        {
            MoveVector(new Vector3(distanceX, 0, 0));
        }

        public void MoveHeight(float distanceY) // move in Y axis
        {
            MoveVector(new Vector3(0, distanceY, 0));
        }

        public virtual void MoveVector(Vector3 moveVector) //Move in the world space 
        {
            _position += moveVector;
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        public void SetPosition(Vector3 newPosition) 
        {
            _position = newPosition;
        }

        public override string ToString()
        {
            return string.Format("id: {0}, index: {1}, pos: {2}", _index.ToString(), _isIndexedByPosition.ToString(), _position.ToString());
        }

    }
}
