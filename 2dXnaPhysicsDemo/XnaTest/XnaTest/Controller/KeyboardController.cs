using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;

namespace XnaTest.Controller
{
    public class KeyboardController : CharacterController
    {
        public KeyboardController(float x, float y)
        {
            this.x = x;
            this.y = y;
            initX = x;
            initY = y;
        }

        private float x;
        private float y;
        private float initX;
        private float initY;
        private float changeFactor = 0.1f;

        public float getX()
        {
            return x;
        }
        public float getDeltaY()
        {
            return y;
        }

        public void HandleInput(GameTime gameTime, Joint leftHandJointPosition, Joint rightHandJointPosition, Joint headJoint, Joint centerShoulderJoint, Vector2 resolution)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Check for input to rotate the camera up and down around the model.
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                y -= time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                y += time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                x += time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                x -= time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                x = initX;
                y = initY;
            }

            //Console.Out.WriteLine(leftHandPosition + " " + rightHandPosition);
        }
    }
}
