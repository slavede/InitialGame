using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.SamplesFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaTest.DataAccessLayer;

namespace XnaTest.Menu
{
    internal class HighScoreScreen : PhysicsGameScreen, IDemoScreen
    {

        private SpriteFont headingFont;
        private SpriteFont scoresFont;

        private String highScoreTitle = "HIGH SCORES";
        private Vector2 highScoreHeadingPosition;

        private KinectPongDAL kinectPongDAL;
        private Dictionary<String, String> highScoreDisplay;
        private float xNamePosition;
        private float xScorePosition;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "HIGH SCORES";
        }

        public string GetDetails()
        {
            return "Screen will displayed high scores";
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();

            headingFont = ScreenManager.Content.Load<SpriteFont>("HighScoreHeading");
            scoresFont = ScreenManager.Content.Load<SpriteFont>("Font");
            highScoreHeadingPosition = new Vector2(-headingFont.MeasureString(highScoreTitle).X/2, -ScreenManager.GraphicsDevice.Viewport.Height / 2);

            xNamePosition = highScoreHeadingPosition.X - 100;
            xScorePosition = highScoreHeadingPosition.X + 100;

            kinectPongDAL = new KinectPongDAL();
            Dictionary<String, long> highScores = kinectPongDAL.GetHighScores();
            highScoreDisplay = new Dictionary<String, String>();
            
            foreach (String name in highScores.Keys)
            {
                TimeSpan t = TimeSpan.FromMilliseconds(highScores[name]);
                string answer = string.Format("{0:D2}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);

                highScoreDisplay.Add(name, answer);
            }

        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);

            ScreenManager.SpriteBatch.DrawString(headingFont, highScoreTitle, highScoreHeadingPosition, Color.Yellow);

            Vector2 scoreVector = new Vector2(highScoreHeadingPosition.X, highScoreHeadingPosition.Y);

            scoreVector.Y += 60;

            foreach (String name in highScoreDisplay.Keys)
            {
                ScreenManager.SpriteBatch.DrawString(scoresFont, name, new Vector2(xNamePosition, scoreVector.Y), Color.Yellow);
                ScreenManager.SpriteBatch.DrawString(scoresFont, highScoreDisplay[name], new Vector2(xScorePosition, scoreVector.Y), Color.Yellow);

                scoreVector.Y += 30;

            }

            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {



            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

    }
}
