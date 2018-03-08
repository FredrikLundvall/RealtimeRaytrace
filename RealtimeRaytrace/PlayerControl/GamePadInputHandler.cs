using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RealtimeRaytrace
{
    public class GamePadInputHandler: IInputHandler
    {
        PlayerIndex _playerIndex = PlayerIndex.One;
        public GamePadInputHandler(PlayerIndex playerIndex)
        {
            _playerIndex = playerIndex;
        }

        public void InitiateInput()
        { }

        public void HandleInput(GameTime gameTime, IMessageSender messageSender)
        {
            GamePadState gamepadState = GamePad.GetState(_playerIndex);
            float speedStep = 1.0f;
            if (gamepadState.Buttons.LeftStick == ButtonState.Pressed)
                speedStep = 1.5f;

            messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime, "gamepad", "player1", EventMessageType.MoveDepth, gamepadState.ThumbSticks.Left.Y * -speedStep));
            messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime, "gamepad", "player1", EventMessageType.MoveSide, gamepadState.ThumbSticks.Left.X * speedStep));
            //messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime,"gamepad","player1",EventMessageType.MoveHeight, upDistance));
            messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime, "gamepad", "player1", EventMessageType.RotateYaw, gamepadState.ThumbSticks.Right.X * -0.2f));
            messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime, "gamepad", "player1", EventMessageType.RotatePitch, gamepadState.ThumbSticks.Right.Y * 0.2f));
            if (GamePad.GetState(0).IsButtonDown(Buttons.Start))
                messageSender.SendMessage(new EventMessage(gameTime.TotalGameTime, "gamepad", "player1", EventMessageType.DoQuit));
        }
    }
}
