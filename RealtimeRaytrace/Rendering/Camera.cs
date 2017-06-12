using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Camera : Entity
    {
        //Matrix _orientation;
        readonly float _maxFov = (float)Math.PI * 2.0f;
        float _yaw = 0;
        float _pitch = 0;
        float _roll = 0;
        float _fov = (float)Math.PI/2.0f;
        float _correctionAmount = 0.5f;

        public Camera(int index, Vector3 position, float yaw, float pitch, float roll, float fov = (float)Math.PI / 2.2f)
            : base(index, false, position)
        {
            if (fov > (_maxFov + 0.01f))
                throw new ArgumentException("FOV cannot exceed: " + _maxFov.ToString());
            _yaw = yaw;
            _pitch = pitch;
            _roll = roll;
            _fov = fov;
            _correctionAmount = 1.0f;
            //_orientation = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
        }

        public void RotateYaw(float radiansYaw)  // rotate around Y axis
        {
            _yaw += radiansYaw;
            //_orientation = Matrix.Multiply(_orientation, Matrix.CreateRotationY(radiansYaw));
        }

        public void RotatePitch(float radiansPitch) // rotate around X axis
        {
            _pitch += radiansPitch;
            //_orientation = Matrix.Multiply(_orientation, Matrix.CreateRotationX(radiansPitch));
        }

        public void RotateRoll(float radiansRoll) // rotate around Z axis
        {
            _roll += radiansRoll;
            //_orientation = Matrix.Multiply(_orientation, Matrix.CreateRotationZ(radiansRoll));
        }

        public override void MoveVector(Vector3 moveVector) //Move in the local space 
        {
            _position += Vector3.Transform(moveVector, GetOrientation());
        }

        public Matrix GetOrientation()
        {
            //return _orientation;
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

        public float GetFov()
        {
            return _fov;
        }

        public float GetMaxFov()
        {
            return _maxFov;
        }

        public float GetCorrectionAmount()
        {
            return _correctionAmount;
        }

    }
}
