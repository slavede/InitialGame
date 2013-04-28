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
        private const double MAX_ANGLE = 1.55;
        private const int POSITION_DEVIATION = 10;

        private Texture2D textureDown;
        private TextureAtlas textureAtlasDown;
        private TextureRegion textureRegionDown;
        private Texture2D textureUp;
        private TextureAtlas textureAtlasUp;
        private TextureRegion textureRegionUp;
        private int currentFrameUp;
        private int currentFrameDown;

        private int frameMedian;
        private int frameOfBalance;
        private double angleScaleFactor;

        float lastPosition = Int32.MaxValue;

        public VodafoneMascot(ContentManager content)
        {
            textureDown = content.Load<Texture2D>("CharacterSprites/VodafoneDown");
            textureAtlasDown = content.Load<TextureAtlas>("CharacterSprites/VodafoneDownXML");
            textureUp = content.Load<Texture2D>("CharacterSprites/VodafoneUp");
            textureAtlasUp = content.Load<TextureAtlas>("CharacterSprites/VodafoneUpXML");
            frameMedian = (textureAtlasUp.GetNumberOfFrames() - 1) / 2;
            angleScaleFactor = MAX_ANGLE / ((textureAtlasUp.GetNumberOfFrames()-1) / 2);
            frameOfBalance = frameMedian + frameMedian % 2;
        }

        public void Update(float angle, float positionX)
        {
            int frame = Convert.ToInt32(angle / angleScaleFactor) * -1;
            frame = frame > frameMedian ? frameMedian : frame;
            frame = frame < -frameMedian ? -frameMedian : frame;
            currentFrameUp = frame + frameOfBalance;

            if (Math.Abs(positionX-lastPosition) > POSITION_DEVIATION){
                if (positionX > lastPosition)
                    currentFrameDown--;
                else
                    currentFrameDown++;

                if (currentFrameDown < 0)
                    currentFrameDown = textureAtlasDown.GetNumberOfFrames() - 1;
                else
                    currentFrameDown = currentFrameDown % textureAtlasDown.GetNumberOfFrames();
                
                lastPosition = positionX;
            }
            
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            textureRegionDown = textureAtlasDown.GetRegion(currentFrameDown.ToString("0000"));
            Vector2 locationDown = new Vector2(location.X, location.Y + textureRegionDown.Bounds.Height - 160);
            spriteBatch.Draw(textureDown, locationDown, textureRegionDown.Bounds, Color.White, 0f, textureRegionDown.OriginCenter, 0.6f, SpriteEffects.None, 0f);    

            textureRegionUp = textureAtlasUp.GetRegion(currentFrameUp.ToString("0000"));
            Vector2 locationUp = new Vector2(location.X, location.Y - textureRegionUp.Bounds.Height + 210);
            spriteBatch.Draw(textureUp, locationUp, textureRegionUp.Bounds, Color.White, 0f, textureRegionUp.OriginCenter, 0.6f, SpriteEffects.None, 1f);    
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
