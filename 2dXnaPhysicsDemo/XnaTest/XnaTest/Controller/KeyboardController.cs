using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaTest.Controller
{
    class KeyboardController : CharacterController
    {
        public KeyboardController()
        {
            leftHandPosition = initLeft;
            rightHandPosition = initRight;
        }

        private Vector2 initLeft = new Vector2(-20, 0);
        private Vector2 initRight = new Vector2(20, 0);
        private Vector2 leftHandPosition;
        private Vector2 rightHandPosition;
        private float changeFactor = 0.1f;

        public Vector2 getLeftHandPosition()
        {
            return leftHandPosition;
        }
        public Vector2 getRightHandPosition()
        {
            return rightHandPosition;
        }


        

        //public KeyboardController(KeyboardState keyboardState)
        //{
        //    this.currentKeyboardState = keyboardState;
        //}
        /// <summary>
        /// Handles input for quitting the game.
        /// </summary>
        public void HandleInput(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Check for input to rotate the camera up and down around the model.
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                rightHandPosition.Y += time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                rightHandPosition.Y -= time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                rightHandPosition.X += time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                rightHandPosition.X -= time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                leftHandPosition.Y -= time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                leftHandPosition.Y += time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                leftHandPosition.X += time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
                currentKeyboardState.IsKeyDown(Keys.A))
            {
                leftHandPosition.X -= time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                leftHandPosition = initLeft;
                rightHandPosition = initRight;
            }

            Console.Out.WriteLine(leftHandPosition + " " + rightHandPosition);
        }
    }
}
