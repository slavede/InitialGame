using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XnaTest.Character.Characters
{
    /// <summary>
    /// Abstract sprite sheet class whose update is based on plank angle.
    /// Implementation of each character sprite must handle frame calculation.
    /// Should be returning (absolute) closest-lower available angle-frame.
    /// Angles are presented in radians.
    /// 
    /// </summary>
    public abstract class CharacterSprite
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int totalFrames;
        private int currentFrame;

        public CharacterSprite(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            totalFrames = Rows * Columns;
            currentFrame = 0;
        }

        public void Update(float angle)
        {
            currentFrame = getFrameIndex(angle);
        }

        protected abstract int getFrameIndex(float angle);

        public int Width
        {
            get { return Texture.Width / Columns; }
        }

        public float Height
        {
            get { return Texture.Height / Rows; }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);

        }
    }
}
