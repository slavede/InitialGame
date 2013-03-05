using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XnaTest.Texture;

namespace XnaTest.Character.Characters
{
    class VodafoneMascot : ICharacterSprite
    {
        private const double MAX_ANGLE = 1.15;
        private Texture2D texture;
        private TextureAtlas textureAtlas;
        private TextureRegion textureRegion;
        private int currentFrame;
        private int frameMedian;
        private int frameOfBalance;
        private double angleScaleFactor;

        public VodafoneMascot(ContentManager content)
        {
            texture = content.Load<Texture2D>("CharacterSprites/VodafoneMascot");
            textureAtlas = content.Load<TextureAtlas>("CharacterSprites/VodafoneMascotXML");
            frameMedian = (textureAtlas.GetNumberOfFrames() - 1) / 2;
            angleScaleFactor = MAX_ANGLE / ((textureAtlas.GetNumberOfFrames()-1) / 2);
            frameOfBalance = frameMedian + frameMedian % 2;
        }

        public void Update(float angle)
        {
            int frame = Convert.ToInt32(angle / angleScaleFactor) * -1;
            frame = frame > frameMedian ? frameMedian : frame;
            frame = frame < -frameMedian ? -frameMedian : frame;
            currentFrame = frame + frameOfBalance;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            textureRegion = textureAtlas.GetRegion(currentFrame.ToString("0000"));
            spriteBatch.Draw(texture, location, textureRegion.Bounds, Color.White, 0f, textureRegion.OriginCenter, 1f, SpriteEffects.None, 0f);    
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
