using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace RealtimeRaytrace
{
    public class MouseKeybordInputHandler : IInputHandler
    {
        protected int _centerWidth = 320;
        protected int _centerHeight = 160;

        public MouseKeybordInputHandler(int centerWidth, int centerHeight)
        {
            _centerWidth = centerWidth;
            _centerHeight = centerHeight;
        }

        public void InitiateInput()
        {
            Mouse.SetPosition(_centerWidth, _centerHeight);
        }

        public void HandleInput(Queue<IPlayerCommand> playerCommandQueue)
        {
            //float forwardAmount = 0, rightDistance = 0, upDistance = 0, yawTurned = 0, pitchTurned = 0;

            //KeyboardState keyboardState = Keyboard.GetState();
        
            //float speedStep;
            //if (keyboardState.IsKeyDown(Keys.LeftShift))
            //    speedStep =5.4f;
            //else
            //    speedStep = 1.0f;

            //if (keyboardState.IsKeyDown(Keys.W))
            //{
            //    forwardAmount = -speedStep;
            //}
            //else if (keyboardState.IsKeyDown(Keys.S))
            //{
            //    forwardAmount = speedStep;
            //}
            //if (keyboardState.IsKeyDown(Keys.A))
            //{
            //    rightDistance = -speedStep;
            //}
            //else if (keyboardState.IsKeyDown(Keys.D))
            //{
            //   rightDistance = speedStep;
            //}
            //if (keyboardState.IsKeyDown(Keys.E))
            //{
            //    upDistance = speedStep;
            //}
            //else if (keyboardState.IsKeyDown(Keys.C))
            //{
            //    upDistance = -speedStep;
            //}

            //if (keyboardState.IsKeyDown(Keys.Left))
            //{
            //    yawTurned = -0.9f;
            //}

            //if (keyboardState.IsKeyDown(Keys.Right))
            //{
            //    yawTurned = 0.9f;
            //}

            //if (keyboardState.IsKeyDown(Keys.Up))
            //{
            //    pitchTurned = -0.9f;
            //}

            //if (keyboardState.IsKeyDown(Keys.Down))
            //{
            //    pitchTurned = 0.9f;
            //}

            //MouseState mouseState = Mouse.GetState();
           
            //yawTurned = yawTurned + ((Mouse.GetState().X - _centerWidth) / (float)_centerWidth) * 2.0f; ;
            //pitchTurned = pitchTurned + ((Mouse.GetState().Y - _centerHeight) / (float)_centerHeight) * 2.0f;

            //Mouse.SetPosition(_centerWidth, _centerHeight);

            //if (forwardAmount != 0)
            //    playerCommandQueue.Enqueue(new MoveDepthCommand(forwardAmount));
            //if (rightDistance != 0)
            //    playerCommandQueue.Enqueue(new MoveSideCommand(rightDistance));
            //if (upDistance != 0)
            //    playerCommandQueue.Enqueue(new MoveHeightCommand(upDistance));
            //if (yawTurned != 0)
            //    playerCommandQueue.Enqueue(new RotateYawCommand(-yawTurned));
            //if (pitchTurned != 0)
            //    playerCommandQueue.Enqueue(new RotatePitchCommand(-pitchTurned));

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || (Keyboard.GetState().IsKeyDown(Keys.F4) && (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.RightAlt))))
                playerCommandQueue.Enqueue(new QuitCommand());
        }

    }
}
