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

        public void HandleInput(Queue<IPlayerCommand> playerCommandQueue)
        {
            GamePadState gamepadState = GamePad.GetState(_playerIndex);
            //return new MoveDepthCommand(gamepadState.ThumbSticks.Left.Y, gamepadState.ThumbSticks.Left.X, 0, gamepadState.ThumbSticks.Right.X * 0.2f, gamepadState.ThumbSticks.Right.Y * -0.2f, gamepadState.Buttons.BigButton == ButtonState.Pressed, gamepadState.Buttons.Back == ButtonState.Pressed);
            float speedStep = 1.0f;
            if (gamepadState.Buttons.LeftStick == ButtonState.Pressed)
                speedStep = 1.5f;

            playerCommandQueue.Enqueue(new MoveDepthCommand(gamepadState.ThumbSticks.Left.Y * speedStep));
            playerCommandQueue.Enqueue(new MoveSideCommand(gamepadState.ThumbSticks.Left.X * speedStep));
            //playerCommandQueue.Enqueue(new MoveHeightCommand(upDistance));
            playerCommandQueue.Enqueue(new RotateYawCommand(gamepadState.ThumbSticks.Right.X * 0.2f));
            playerCommandQueue.Enqueue(new RotatePitchCommand(gamepadState.ThumbSticks.Right.Y * -0.2f));
        }
    }
}
