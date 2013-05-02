using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KinectButton
{
    public enum AnimationStates
    {
        Ready,
        Active,
        Done
    }

    class SelectAnimation
    {
        private Texture2D texture;
        private List<Rectangle> sourceRects;
        private Vector2 origin;
        private int currentFrame;
        private float elapsed;
        private float frameTime;
        private int nofFrames;

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        private AnimationStates animationState;
        public AnimationStates AnimationState
        {
            get { return animationState; }
            set
            {
                animationState = value;

                if (animationState == AnimationStates.Active)
                {
                    currentFrame = 0;
                    elapsed = 0;
                }
            }
        }
        

        public SelectAnimation(int duration, Vector2 position, Texture2D texture, float scale)
        {
            this.scale = scale;
            this.texture = texture;
            //assume square frames in a single row
            nofFrames = texture.Width / texture.Height;
            int ribLength = texture.Height;
            sourceRects = new List<Rectangle>();
            for (int i = nofFrames - 1; i >= 0; i--)
                sourceRects.Add(new Rectangle(i * ribLength, 0, ribLength, texture.Height));

            frameTime = duration / nofFrames;

            origin = new Vector2(ribLength / 2, texture.Height / 2);
            this.position = position;
            animationState = AnimationStates.Ready;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (animationState == AnimationStates.Active)
            {
                elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (elapsed > frameTime)
                {
                    currentFrame++;
                    elapsed = 0;
                }

                if (currentFrame == nofFrames)
                {
                    animationState = AnimationStates.Done;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (animationState == AnimationStates.Active)
            {
                spriteBatch.Draw(texture, position, sourceRects[currentFrame], Color.White, 0.0f, origin, scale, SpriteEffects.None, 1.0f);
            }
        }

        public float scale { get; set; }
    }
}
