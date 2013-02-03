using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaTest.Character.Characters
{
    class Majlo : CharacterSprite
    {
        private Dictionary<int, float> majloAngles = new Dictionary<int, float>();

        public Majlo(ContentManager content) : base(content.Load<Texture2D>("CharacterSprites/majlo-with-plank"), 1, 5){
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Vector2 updatedLocation = new Vector2(location.X - 70, location.Y - 153);
            base.Draw(spriteBatch, updatedLocation);
         }

        //Sredit ovo SKROZ
        protected override int getFrameIndex(float angle)
        {
            if (angle > Math.PI / 4)
            {
                return 2;
            }
            if (angle > Math.PI / 7)
            {
                return 3;
            }
            if (Math.Abs(angle) > Math.PI / 4)
            {
                return 0;
            }
            if (Math.Abs(angle) > Math.PI / 7)
            {
                return 1;
            }
   
            return 4;

        }
        private float changeFactor = 0.1f;
        private float x = 0;
        private float y = 0;

        public void HandleInput(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                y -= time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                y += time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                x += time * changeFactor;
            }

            if (currentKeyboardState.IsKeyDown(Keys.A))
            {
                x -= time * changeFactor;
            }

            Console.Out.WriteLine(x + " " + y);
        }
    }
}
