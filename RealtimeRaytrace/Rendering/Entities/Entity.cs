using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Entity
    {
        protected Vector3 _position = Vector3.Zero;
        protected float _yaw = 0;
        protected float _pitch = 0;
        protected float _roll = 0;
        protected Quaternion _rotation;
        protected Quaternion _textureRotation;
        protected List<Entity> _entityList;

        public Entity(Vector3 position, float yaw, float pitch, float roll)
        {
            _position = position;
            _yaw = yaw;
            _pitch = pitch;
            _roll = roll;
            _rotation = Quaternion.CreateFromYawPitchRoll(yaw,pitch,roll);
            _textureRotation = _rotation;
            _entityList = new List<Entity>(0);
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

            ChangePosition(newPosition - _position);
        }

        public void ChangePosition(Vector3 changePosition) 
        {
            _position += changePosition;
            foreach (Entity entity in _entityList)
            {
                entity.ChangePosition(changePosition);
            }
        }

        public void RotateYaw(float radiansYaw)  // rotate around Y axis
        {
            _yaw += radiansYaw;
            //Quaternion rotationThisFrame = Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.Up,_rotation), radiansYaw);
            //_rotation = rotationThisFrame * _rotation;
            //_textureRotation = rotationThisFrame * _textureRotation;
            RotateQuaternion(Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.Up, _rotation), radiansYaw), _position);
        }

        public void RotateCameraYaw(float radiansYaw)  // rotate around Y axis
        {
            _yaw += radiansYaw;
            //Quaternion rotationThisFrame = Quaternion.CreateFromAxisAngle(Vector3.Up, radiansYaw);
            //_rotation = rotationThisFrame * _rotation;
            //_textureRotation = rotationThisFrame * _textureRotation;
            RotateQuaternion(Quaternion.CreateFromAxisAngle(Vector3.Up, radiansYaw), _position);
        }

        public void RotatePitch(float radiansPitch) // rotate around X axis
        {
            _pitch += radiansPitch;
            //Quaternion rotationThisFrame = Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.Right, _rotation), radiansPitch);
            //_rotation = rotationThisFrame * _rotation;
            //_textureRotation = rotationThisFrame * _textureRotation;
            RotateQuaternion(Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.Right, _rotation), radiansPitch), _position);
        }

        public void RotateQuaternion(Quaternion rotationThisFrame, Vector3 centerPosition)
        {           
            _rotation = rotationThisFrame * _rotation;
            //if(this is AntiSphere)
            //    _textureRotation = rotationThisFrame * _textureRotation;
            //else
                _textureRotation = Quaternion.Inverse(rotationThisFrame) * _textureRotation;
            if (centerPosition != _position)
            {
                Vector3 offsetPosition = _position - centerPosition;
                _position = Vector3.Transform(offsetPosition, rotationThisFrame) + centerPosition;
            }
            foreach (Entity entity in _entityList)
            {
                entity.RotateQuaternion(rotationThisFrame, centerPosition);
            }
        }

        public void MoveVector(Vector3 moveVector) //Move in the local space 
        {
            _position += Vector3.Transform(moveVector, GetOrientation());
            foreach (Entity entity in _entityList)
            {
                entity.MoveVector(moveVector);
            }
        }

        public void SetTextureRotation(float yaw, float pitch, float roll)
        {
            _textureRotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
        }

        public Matrix GetOrientation()
        {
            return Matrix.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
        }

        public Quaternion GetRotation()
        {
            return _rotation;
        }

        public Quaternion GetTextureRotation()
        {
            return _textureRotation;
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

        public bool IsRotated()
        {
            return !(_yaw == 0 && _pitch == 0 && _roll == 0);
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
            return string.Format("pos: {0}", _position.ToString());
        }
    }
}
