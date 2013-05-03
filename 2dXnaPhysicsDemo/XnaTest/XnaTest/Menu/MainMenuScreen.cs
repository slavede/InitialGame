using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.SamplesFramework;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Input;
using XnaTest.Character;
using XnaTest.Character.Controller;
using XnaTest.Character.Characters;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using XnaTest.Utils;
using System.Collections.ObjectModel;
using KinectButton;

namespace XnaTest.Menu
{
    internal class MainMenuScreen : PhysicsGameScreen, IDemoScreen, MenuDelegate
    {
        private Wheel wheel;
        private WheelController wheelController;
        private Sprite cursorSprite;
        int activeSkeletonIndex = -1;
        int selectedMenuIndex;
        String[] entries = new String[] { "1 Player", "Highscores", "About", "Settings", "2 Players" };
        Dictionary<int, SubMenu> submenus;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Body with a single fixture";
        }

        public string GetDetails()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo shows a single body with one attached fixture and shape.");
            sb.AppendLine("A fixture binds a shape to a body and adds material");
            sb.AppendLine("properties such as density, friction, and restitution.");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Rotate object: left and right triggers");
            sb.AppendLine("  - Move object: right thumbstick");
            sb.AppendLine("  - Move cursor: left thumbstick");
            sb.AppendLine("  - Grab object (beneath cursor): A button");
            sb.AppendLine("  - Drag grabbed object: left thumbstick");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Rotate Object: left and right arrows");
            sb.AppendLine("  - Move Object: A,S,D,W");
            sb.AppendLine("  - Exit to menu: Escape");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Mouse / Touchscreen");
            sb.AppendLine("  - Grab object (beneath cursor): Left click");
            sb.AppendLine("  - Drag grabbed object: move mouse / finger");
            return sb.ToString();
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = Vector2.Zero;
            wheel = new Wheel(ScreenManager, this, World, 150, new Vector2(), entries, new Sprite(ScreenManager.Content.Load<Texture2D>("wheelBackgroundMain")));
            
            cursorSprite = new Sprite(ScreenManager.Content.Load<Texture2D>("Common/cursor"));
            if (wheelController != null)
            {
                return;
            }
            if (KinectSensor.KinectSensors.Count > 0)
            {
               wheelController = new KinectGrabWheelController(wheel, ScreenManager, KinectSensor.KinectSensors[0]);
            }
            else
            {
                wheelController = new MouseWheelController(wheel, ScreenManager.GraphicsDevice);
            }
            submenus = new Dictionary<int, SubMenu>();

            Dictionary<String, AbstractHoverButton.ButtonClickedHandler> singlePlayerItems = new Dictionary<string,AbstractHoverButton.ButtonClickedHandler>();
            singlePlayerItems.Add("New Game", new AbstractHoverButton.ButtonClickedHandler(onSinglePlayerNewGameClicked));
            submenus.Add(0, new SubMenu(singlePlayerItems, ScreenManager, entries[0]));

            Dictionary<String, AbstractHoverButton.ButtonClickedHandler> highscoreItems = new Dictionary<string,AbstractHoverButton.ButtonClickedHandler>();
            highscoreItems.Add("Top 10", new AbstractHoverButton.ButtonClickedHandler(onViewHighScoresClicked));
            submenus.Add(1, new SubMenu(highscoreItems, ScreenManager, entries[1]));

            Dictionary<String, AbstractHoverButton.ButtonClickedHandler> aboutItems = new Dictionary<string,AbstractHoverButton.ButtonClickedHandler>();
            aboutItems.Add("About Us", new AbstractHoverButton.ButtonClickedHandler(onAboutUsClicked));
            submenus.Add(2, new SubMenu(aboutItems, ScreenManager, entries[2]));

            Dictionary<String, AbstractHoverButton.ButtonClickedHandler> settingsItems = new Dictionary<string,AbstractHoverButton.ButtonClickedHandler>();
            settingsItems.Add("Settings", new AbstractHoverButton.ButtonClickedHandler(onSettingsClicked));
            submenus.Add(3, new SubMenu(settingsItems, ScreenManager, entries[3]));

            Dictionary<String, AbstractHoverButton.ButtonClickedHandler> multiPlayerItems = new Dictionary<string, AbstractHoverButton.ButtonClickedHandler>();
            multiPlayerItems.Add("New Game", new AbstractHoverButton.ButtonClickedHandler(onMultiPlayerNewGameClicked));
            submenus.Add(4, new SubMenu(multiPlayerItems, ScreenManager, entries[4]));
            
        }

        void onSinglePlayerNewGameClicked() {
            ScreenManager.AddScreen(new InitialGame());
        }

        void onMultiPlayerNewGameClicked() { }

        void onViewHighScoresClicked() {
            ScreenManager.AddScreen(new HighScoreScreen());
        }

        void onAboutUsClicked() { }

        void onSettingsClicked() { }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            wheel.Draw();
            submenus[selectedMenuIndex].Draw(ScreenManager.SpriteBatch);
            if (wheelController.isGrabPerformed())
            {
                ScreenManager.SpriteBatch.Draw(cursorSprite.Texture, ConvertUnits.ToDisplayUnits(wheelController.getPosition()), null, Color.Red, 0f, cursorSprite.Origin, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                ScreenManager.SpriteBatch.Draw(cursorSprite.Texture, ConvertUnits.ToDisplayUnits(wheelController.getPosition()), null, Color.White, 0f, cursorSprite.Origin, 1f, SpriteEffects.None, 0f);
            }

            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (otherScreenHasFocus)
            {
                return;
            }

            if(Keyboard.GetState().IsKeyDown(Keys.N)){
                ScreenManager.AddScreen(new InitialGame());
            }
            wheelController.HandleInput(gameTime);

            submenus[selectedMenuIndex].Update(gameTime, wheelController.getPosition());

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public void setSelectedMenuIndex(int value)
        {
            this.selectedMenuIndex = value;
        }

        
    }
}
