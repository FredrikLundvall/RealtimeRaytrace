using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RealtimeRaytrace
{
    public class KeybordInputHandler : IInputHandler
    {
        public KeybordInputHandler()
        {
        }

        public void InitiateInput()
        { }

        public void HandleInput(GameTime gameTime, IMessageSender messageSender)
        {
            float forwardAmount = 0, rightDistance = 0, upDistance = 0, yawTurned = 0, pitchTurned = 0;

            KeyboardState keyboardState = Keyboard.GetState();

            float speedStep;
            if (keyboardState.IsKeyDown(Keys.LeftShift))
                speedStep = 5.4f;
            else
                speedStep = 1.0f;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                forwardAmount = -speedStep;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                forwardAmount = speedStep;
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

            if (forwardAmount != 0)
                messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime,"keyboard","player1",EventMessageType.MoveDepth,forwardAmount));
            if (rightDistance != 0)
                messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime, "keyboard", "player1", EventMessageType.MoveSide, rightDistance));
            if (upDistance != 0)
                messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime, "keyboard", "player1", EventMessageType.MoveHeight, upDistance));
            if (yawTurned != 0)
                messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime, "keyboard", "player1", EventMessageType.RotateYaw, -yawTurned));
            if (pitchTurned != 0)
                messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime, "keyboard", "player1", EventMessageType.RotatePitch, -pitchTurned));

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || (Keyboard.GetState().IsKeyDown(Keys.F4) && (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.RightAlt))))
                messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime, "keyboard", "player1", EventMessageType.DoQuit));
        }

    }
}

