using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RealtimeRaytrace
{
    public class GameControl
    {
        Vector2 _center;

        public GameControl(int width, int height)
        {
            _center.X = width * 0.5f;
            _center.Y = height * 0.5f;

#if !DEBUG 
            Mouse.SetPosition((int)_center.X, (int)_center.Y);
#endif
        }

        public bool ExitIsSelectedOnControl()
        {
            return (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape));
        }

        public void UpdateCameraFromControl(GameTime gameTime,Camera camera)
        {
            float speedStep;

            camera.MoveDepth(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y);
            camera.MoveSide(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X);
            camera.RotateYaw(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * 0.2f);
            camera.RotatePitch(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * -0.2f);

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                speedStep = 0.7f;
            else
                speedStep = 0.3f;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                camera.MoveDepth(speedStep);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                camera.MoveDepth(-speedStep);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                camera.MoveSide(-speedStep);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                camera.MoveSide(speedStep);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                camera.MoveHeight(speedStep);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                camera.MoveHeight(-speedStep);
            }

#if !DEBUG 
            if (Mouse.GetState().X - (int)_center.X != 0)
            {
                camera.RotateYaw((Mouse.GetState().X - (int)_center.X) * 0.0002f);
            }
            if (Mouse.GetState().Y - (int)_center.Y != 0)
            {
                camera.RotatePitch((Mouse.GetState().Y - (int)_center.Y) * 0.0002f);
            }

            Mouse.SetPosition((int)_center.X, (int)_center.Y);
#else

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                camera.RotateYaw(-0.02f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                camera.RotateYaw(0.02f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                camera.RotatePitch(-0.02f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                camera.RotatePitch(0.02f);
            }

#endif
        }

        public bool ToggleFullscreenIsSelectedOnControl()
        {
            return (Keyboard.GetState().IsKeyDown(Keys.T));
        }
    }

}
