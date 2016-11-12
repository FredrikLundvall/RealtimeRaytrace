using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Camera : Entity
    {
        Vector3 _lookAt = Vector3.Backward;
        float _yaw = 0, _pitch = 0, _roll = 0;

        public Camera(int index, Vector3 position, Vector3 lookAt)
            : base(index, false, position)
        {
            _lookAt = lookAt;
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

        public override void MoveVector(Vector3 moveVector) //Move in the local space 
        {
            _position += Vector3.Transform(moveVector, GetCurrentMatrix());
        }

        public Matrix GetCurrentMatrix()
        {
            return Matrix.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
        }

        public Vector3 GetLookAt()
        {
            return _lookAt;
        }

        public Vector3 GetRotatedLookAt()
        {
            return Vector3.Transform(_lookAt, GetCurrentMatrix());
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

        //public void UpdateFromPlayerAction(float forwardDistance, float sideDistance, float upDistance, float yawTurned, float pitchTurned)
        //{
        //    MoveDepth(forwardDistance);
        //    MoveSide(sideDistance);
        //    MoveHeight(upDistance);
        //    RotateYaw(yawTurned);
        //    RotatePitch(pitchTurned);
        //}
    }
}
