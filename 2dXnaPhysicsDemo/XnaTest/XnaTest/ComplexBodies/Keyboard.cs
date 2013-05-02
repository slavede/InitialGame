using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.SamplesFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Kinect;
using XnaTest.Utils;

namespace XnaTest.ComplexBodies
{
    public class KeyboardBody
    {
        private List<KeyboardLetter> letters;
        private Texture2D letterTexture;
        private Texture2D letterFillTexture;
        private float xSpace;
        private float ySpace;

        private SpriteFont scoreFont;

        public event EventHandler ActivationChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Position of first letter of the keyboard</param>
        /// <param name="world">World where keyboard will be</param>
        /// <param name="screenManager">Screen manager which will draw letters</param>
        public KeyboardBody(Vector2 position, World world, ScreenManager screenManager, SpriteFont scoreFont)
        {
            this.scoreFont = scoreFont;
            letters = new List<KeyboardLetter>();
            letterTexture = screenManager.Content.Load<Texture2D>("letter");
            letterFillTexture = screenManager.Content.Load<Texture2D>("circle_fill");

            xSpace = (float)(letterTexture.Width * 1.5);
            ySpace = (float)(letterTexture.Height * 1.5);

            float tempX = position.X;  
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "<-", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "OK", screenManager, scoreFont, Color.Yellow));
            
            position.Y += ySpace;
            position.X = tempX;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "Q", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "W", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "E", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "R", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "T", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "Y", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "U", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "I", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "O", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "P", screenManager, scoreFont, Color.Yellow));
            
            position.X = tempX;
            position.Y += ySpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "A", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "S", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "D", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "F", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "G", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "H", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "J", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "K", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "L", screenManager, scoreFont, Color.Yellow));

            position.X = tempX;
            position.Y += ySpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "Z", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "X", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "C", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "V", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "B", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "N", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, "M", screenManager, scoreFont, Color.Yellow));
            position.X += xSpace;
            letters.Add(new KeyboardLetter(position, letterTexture, letterFillTexture, world, " ", screenManager, scoreFont, Color.Yellow));

            foreach (KeyboardLetter letter in letters)
            {
                letter.ActivationChanged += new EventHandler(letter_ActivationChanged);
            }
        }

        void letter_ActivationChanged(object sender, EventArgs e)
        {
            ActivationChanged(sender, null);
        }

        public void Draw(ScreenManager screenManager)
        {
            foreach (KeyboardLetter keyboardLetter in letters)
            {
                keyboardLetter.Draw(screenManager);
            }
        }

        public void Update()
        {
            foreach (KeyboardLetter keyboardLetter in letters)
            {
                keyboardLetter.Update();
            }
        }

        public void CheckHoveredLetters(Vector2 positionOfHand)
        {
            foreach (KeyboardLetter keyboardLetter in letters)
            {
                if (MathHelperMethods.DistanceBetweenTwoVector2(keyboardLetter.Position, positionOfHand) < keyboardLetter.Radius) {
                    keyboardLetter.IsHovered = true;
                }
                else
                {
                    keyboardLetter.IsHovered = false;
                }
            }
        }
    }
}
