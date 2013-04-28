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
    class Majlo : ICharacterSprite
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int totalFrames;
        private int currentFrame;
        private Dictionary<int, float> majloAngles = new Dictionary<int, float>();

        public Majlo(ContentManager content)
        {
            Texture = content.Load<Texture2D>("CharacterSprites/majlo-with-plank");
            Rows = 1;
            Columns = 5;
            totalFrames = Rows * Columns;
            currentFrame = 0;
        }

        public virtual void Update(float angle, float positionX)
        {
            currentFrame = getFrameIndex(angle);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Vector2 updatedLocation = new Vector2(location.X - 70, location.Y - 153);
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
         }

        //Sredit ovo SKROZ
        protected int getFrameIndex(float angle)
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

        public int Width
        {
            get { return Texture.Width / Columns; }
        }

        public float Height
        {
            get { return Texture.Height / Rows; }
        }

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
