using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace RealtimeRaytrace
{
    public class MouseKeybordInputHandler : IInputHandler
    {
        public void HandleInput(Queue<IPlayerCommand> playerCommandQueue)
        {
            float forwardAmount = 0, rightDistance = 0, upDistance = 0, yawTurned = 0, pitchTurned = 0;

            KeyboardState keyboardState = Keyboard.GetState();
        
            float speedStep;
            if (keyboardState.IsKeyDown(Keys.LeftShift))
                speedStep = 0.9f;
            else
                speedStep = 0.4f;

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
                rightDistance = -speedStep;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
               rightDistance = speedStep;
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
                yawTurned = -0.9f;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                yawTurned = 0.9f;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                pitchTurned = -0.9f;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                pitchTurned = 0.9f;
            }

        //public void SetStateFromMouse(float ElapsedTotalSeconds, MouseState mouseState, Vector2 mouseChange)
        //{
        //    YawRotaded = mouseChange.X * 0.2f;
        //    PitchRotaded = mouseChange.Y * 0.2f;
        //}

            if (forwardAmount != 0)
                playerCommandQueue.Enqueue(new MoveDepthCommand(forwardAmount));
            if (rightDistance != 0)
                playerCommandQueue.Enqueue(new MoveSideCommand(rightDistance));
            if (upDistance != 0)
                playerCommandQueue.Enqueue(new MoveHeightCommand(upDistance));
            if (yawTurned != 0)
                playerCommandQueue.Enqueue(new RotateYawCommand(yawTurned));
            if (pitchTurned != 0)
                playerCommandQueue.Enqueue(new RotatePitchCommand(pitchTurned));

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || (Keyboard.GetState().IsKeyDown(Keys.F4) && (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.RightAlt))))
                playerCommandQueue.Enqueue(new QuitCommand());
        }

    }
}
