using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinectButton
{
    class AbstractHoverButton
    {
        private bool enteredHotArea;

        public delegate void ButtonClickedHandler();

        public event ButtonClickedHandler buttonClicked;

        protected Texture2D buttonImage;
        protected SelectAnimation selectAnimation;

        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        protected Rectangle hotArea;
        public Rectangle HotArea
        {
            get { return hotArea; }
            set { hotArea = value; }
        }

        protected Vector2 pointerPosition;

        public AbstractHoverButton(Texture2D buttonImage, Vector2 position, float scale, SelectAnimation selectAnimation)
        {
            this.scale = scale;
            this.buttonImage = buttonImage;
            this.position = position;
            this.selectAnimation = selectAnimation;

            hotArea = new Rectangle((int)position.X, (int)position.Y, buttonImage.Width, buttonImage.Height);
        }

        public virtual void Update(GameTime gameTime, Vector2 position)
        {
            this.pointerPosition = position;
            if (hotArea.Contains((int)position.X, (int)position.Y))
            {
                if (selectAnimation.AnimationState == AnimationStates.Ready && !enteredHotArea)
                {
                    selectAnimation.AnimationState = AnimationStates.Active;
                    enteredHotArea = true;
                }

                selectAnimation.Update(gameTime);                

                if (selectAnimation.AnimationState == AnimationStates.Done)
                {
                    if (buttonClicked != null)
                        buttonClicked();

                    selectAnimation.AnimationState = AnimationStates.Ready;
                }
            }
            else
            {
                if (selectAnimation.AnimationState == AnimationStates.Active)
                    selectAnimation.AnimationState = AnimationStates.Ready;

                enteredHotArea = false;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonImage, position, null, Color.White,  0f, new Vector2(), scale, SpriteEffects.None, 0f);

            if (selectAnimation.AnimationState == AnimationStates.Active)
            {
                selectAnimation.Position = pointerPosition;
                selectAnimation.Draw(spriteBatch);
            }
        }

        public float scale { get; set; }
    }
}
