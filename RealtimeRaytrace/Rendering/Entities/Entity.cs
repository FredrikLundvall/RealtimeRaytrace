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
        float _yaw = 0;
        float _pitch = 0;
        float _roll = 0;

        public Entity(int index, bool isIndexedByPosition, Vector3 position, float yaw, float pitch, float roll)
        {
            _index = index;
            _isIndexedByPosition = isIndexedByPosition;
            _position = position;
            _yaw = yaw;
            _pitch = pitch;
            _roll = roll;
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
            MoveVector(Vector3.Backward * distanceZ);   
        }

        public void MoveSide(float distanceX) // move in X axis
        {
            MoveVector(Vector3.Right * distanceX);
        }

        public void MoveHeight(float distanceY) // move in Y axis
        {
            MoveVector(Vector3.Up * distanceY);
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        public void SetPosition(Vector3 newPosition) 
        {
            _position = newPosition;
        }

        public void RotateYaw(float radiansYaw)  // rotate around Y axis
        {
            _yaw += radiansYaw;
        }

        public void RotatePitch(float radiansPitch) // rotate around X axis
        {
            _pitch += radiansPitch;
        }

        public void RotateRoll(float radiansRoll) // rotate around Z axis
        {
            _roll += radiansRoll;
        }

        public void MoveVector(Vector3 moveVector) //Move in the local space 
        {
            _position += Vector3.Transform(moveVector, GetOrientation());
        }

        public Matrix GetOrientation()
        {
            return Matrix.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
        }

        public float GetYaw()
        {
            return _yaw;
        }

        public float GetPitch()
        {
            return _pitch;
        }

        public float GetRoll()
        {
            return _roll;
        }

        public virtual Intersection Intersect(Ray ray)
        {
            return new Intersection(true);
        }

        public virtual bool IsPointInside(Vector3 point)
        {
            return false;
        }

        protected virtual Intersection Intersect(Vector3 start, Vector3 direction)
        {
            return new Intersection(true);
        }

        public override string ToString()
        {
            return string.Format("id: {0}, index: {1}, pos: {2}", _index.ToString(), _isIndexedByPosition.ToString(), _position.ToString());
        }

    }
}
