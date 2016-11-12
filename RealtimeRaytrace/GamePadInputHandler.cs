using System;
using System.Collections.Generic;
using System.Linq;
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

        public IPlayerCommand HandleInput()
        {
            GamePadState gamepadState = GamePad.GetState(_playerIndex);
            return new MoveDepthCommand(gamepadState.ThumbSticks.Left.Y, gamepadState.ThumbSticks.Left.X, 0, gamepadState.ThumbSticks.Right.X * 0.2f, gamepadState.ThumbSticks.Right.Y * -0.2f, gamepadState.Buttons.BigButton == ButtonState.Pressed, gamepadState.Buttons.Back == ButtonState.Pressed);
        }
    }
}
