using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace RealtimeRaytrace
{
    public class MouseKeybordInputHandler : IInputHandler
    {
        public IPlayerCommand HandleInput()
        {
            float forwardAmount = 0, sideDistance = 0, upDistance = 0, yawTurned = 0, pitchTurned = 0;

            KeyboardState keyboardState = Keyboard.GetState();
        
            float speedStep;
            if (keyboardState.IsKeyDown(Keys.LeftShift))
                speedStep = 0.7f;
            else
                speedStep = 0.3f;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                forwardAmount = speedStep;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                forwardAmount = -speedStep;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                sideDistance = -speedStep;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
               sideDistance = speedStep;
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                upDistance = speedStep;
            }
            else if (keyboardState.IsKeyDown(Keys.C))
            {
                upDistance = -speedStep;
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                yawTurned = -0.02f;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                yawTurned = 0.02f;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                pitchTurned = -0.02f;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                pitchTurned = 0.02f;
            }


        //public void SetStateFromMouse(float ElapsedTotalSeconds, MouseState mouseState, Vector2 mouseChange)
        //{
        //    YawRotaded = mouseChange.X * 0.2f;
        //    PitchRotaded = mouseChange.Y * 0.2f;
        //}
            return new MoveDepthCommand(forwardAmount);
        }

    }
}
