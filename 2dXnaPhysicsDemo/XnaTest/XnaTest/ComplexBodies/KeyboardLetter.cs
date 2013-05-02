using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.SamplesFramework;
using FarseerPhysics.Factories;
using XnaTest.Utils;
using System.ComponentModel;

namespace XnaTest.ComplexBodies
{
    public class KeyboardLetter
    {
        private Texture2D Texture {get; set; }
        public Vector2 Position { get; set; }
        public String Letter { get; set; }
        private Sprite LetterSprite { get; set; }
        private Body LetterBody { get; set; }

        public float Radius { get; set; }

        private float PercentageHovered { get; set; }

        private Texture2D circleFillTexture { get; set; }

        private SpriteFont letterFont;
        private Color color;

        private Random randomGenerator;

        private Boolean isHovered;
        public Boolean IsHovered {
            get 
            {
                return isHovered;
            }
            set
            {
                isHovered = value;
                if (value == false)
                {
                    PercentageHovered = 0f;
                }
            }
        }

        public event EventHandler ActivationChanged;

        private static float hoverStep = 0.01f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Where it will be drawn</param>
        /// <param name="texture">Texture</param>
        /// <param name="world">World where it will appear</param>
        /// <param name="letter">Letter which will display</param>
        /// <param name="screenManager">Screen manager that will draw this object</param>
        public KeyboardLetter(Vector2 position, Texture2D texture, Texture2D fillTexture, World world, String letter, ScreenManager screenManager, SpriteFont letterFont, Color color)
        {
            this.Position = position;
            this.Texture = texture;
            this.Letter = letter;

            this.LetterBody = BodyFactory.CreateCircle(world, Texture.Width/2, 1);
            this.circleFillTexture = fillTexture;
            this.PercentageHovered = 0.0f;
            this.Radius = texture.Width/2;
            this.LetterSprite = new Sprite(screenManager.Assets.TextureFromShape(LetterBody.FixtureList[0].Shape, MaterialType.Squares, Color.Orange, 1f));

            this.letterFont = letterFont;
            this.color = color;

            randomGenerator = new Random(DateTime.Now.Millisecond);
            this.isHovered = false;

        }

        public void Draw(ScreenManager screenManager)
        {
            Vector2 positionToDraw = new Vector2(Position.X + randomGenerator.Next(-1, 1), Position.Y + randomGenerator.Next(-1, 1));
            screenManager.SpriteBatch.Draw(Texture, ConvertUnits.ToDisplayUnits(positionToDraw),
                null,
                Color.White, LetterBody.Rotation, ConvertUnits.ToDisplayUnits(LetterSprite.Origin), 1f,
                SpriteEffects.None, 0f);

            if (isHovered)
            {
                screenManager.SpriteBatch.Draw(circleFillTexture, ConvertUnits.ToDisplayUnits(positionToDraw),
                    null,
                    Color.White, LetterBody.Rotation, ConvertUnits.ToDisplayUnits(LetterSprite.Origin), PercentageHovered,
                    SpriteEffects.None, 0f);
            }


            screenManager.SpriteBatch.DrawString(letterFont, Letter, new Vector2(positionToDraw.X - letterFont.MeasureString(Letter).X/2, positionToDraw.Y - letterFont.MeasureString(Letter).Y/2), color);
        }

        public void Update()
        {
            if (isHovered && (PercentageHovered + hoverStep) <= 1f)
            {
                PercentageHovered += hoverStep;
            }
            else if (isHovered && (PercentageHovered + hoverStep) >= 1f)
            {
                PercentageHovered = 0f;
                ActivationChanged(this, null);
            }
            else
            {
                PercentageHovered = 0;
            }

        }
    }
}
