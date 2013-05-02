using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaTest.Menu.Buttons;
using KinectButton;
using FarseerPhysics.SamplesFramework;

namespace XnaTest.Menu
{
    class SubMenu
    {
        List<AbstractHoverButton> buttons;
        List<String> itemNames;
        private Sprite background;
        public string name { get; set; }
        private const int offsetBetweenItems = 50;
        private Vector2 itemsPosition = new Vector2(300, 100);
        SpriteFont font;           

        public SubMenu(Dictionary<String, AbstractHoverButton.ButtonClickedHandler> buttonHandlers, ScreenManager screenManager, String name)
        {
            background = new Sprite(screenManager.Content.Load<Texture2D>("notebook"));
            Texture2D animationTexture = screenManager.Content.Load<Texture2D>("Common/AnimationStrip");
            SelectAnimation selectAnimation = new SelectAnimation(1500, new Vector2(), animationTexture, 0.3f);
            font = screenManager.Content.Load<SpriteFont>("Font");

            int i = 0;
            this.buttons = new List<AbstractHoverButton>();
            this.itemNames = new List<string>();
            foreach (var pair in buttonHandlers)
            {
                itemNames.Add(pair.Key);
                AbstractHoverButton button = new AbstractHoverButton(screenManager.Content.Load<Texture2D>("Common/stick"), itemsPosition + new Vector2(30, -50 + i* offsetBetweenItems), 0.5f, new SelectAnimation(1500, new Vector2(), animationTexture, 0.3f));
                button.buttonClicked += new AbstractHoverButton.ButtonClickedHandler(pair.Value);
                buttons.Add(button);
                i++;
            }
            this.name = name;

        }

        public void Update(GameTime gameTime, Vector2 point)
        {
            foreach (AbstractHoverButton button in buttons)
            {
                button.Update(gameTime, point);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background.Texture, ConvertUnits.ToDisplayUnits(itemsPosition),
                  null,
                  Color.White, 0f, background.Origin, 1.4f * 150 / background.Texture.Bounds.Height,
                  SpriteEffects.None, 0f);
            int i = 0;
            foreach (AbstractHoverButton button in buttons)
            {
                spriteBatch.DrawString(font, itemNames[i], ConvertUnits.ToDisplayUnits(itemsPosition + new Vector2(-65, -50 + i * offsetBetweenItems)), Color.Black);
                button.Draw(spriteBatch);
                i++;
            }
        }
    }
}
