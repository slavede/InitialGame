using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.SamplesFramework;
using Microsoft.Xna.Framework;
using XnaTest.ComplexBodies;
using Microsoft.Xna.Framework.Graphics;
using XnaTest.Utils;
using Microsoft.Kinect;
using System.Data.SQLite;

namespace XnaTest.Menu
{
    internal class EnterHighScoreScreen : PhysicsGameScreen, IDemoScreen
    {
        private String currentName;
        private Vector2 currentNamePosition;

        private KeyboardBody keyboard;
        private Vector2 keyboardPosition;
        private SpriteFont scoreFont;

        KinectSensor kinect;
        Skeleton[] skeletonData;
        private GestureControllerHandler gestureControllerHandler;

        // Slaven, just for testing
        Skeleton skeletonToDraw;
        Texture2D jointTexture;

        private long scoreToEnter;

        private SQLiteConnection dbConnection;
        private SQLiteCommand insertHighscoreCommand;
        
        #region IDemoScreen Members

        public string GetTitle()
        {
            return "New High Score";
        }

        public string GetDetails()
        {
            return "Screen that will provide interface for user to enter his name when high score needs to be entered";
        }

        #endregion

        public EnterHighScoreScreen(long scoreToEnter) : base()
        {
            this.scoreToEnter = scoreToEnter;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinect = KinectSensor.KinectSensors[0];
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
                kinect.Start();

            }

            // Slaven, just for testing
            jointTexture = ScreenManager.Content.Load<Texture2D>("joint");

            scoreFont = ScreenManager.Content.Load<SpriteFont>("Font");
            keyboardPosition = new Vector2(-ScreenManager.GraphicsDevice.Viewport.Width / 3, -ScreenManager.GraphicsDevice.Viewport.Height / 8);
            keyboard = new KeyboardBody(keyboardPosition, World, ScreenManager, scoreFont);

            currentName = "";
            currentNamePosition = new Vector2(0, keyboardPosition.Y - ScreenManager.GraphicsDevice.Viewport.Height / 8);

            gestureControllerHandler = new GestureControllerHandler();
            keyboard.ActivationChanged += new EventHandler(keyboard_ActivationChanged);

            dbConnection = new SQLiteConnection("Data Source=../../../../../db/KinectPong.sqlite;Version=3;");
            insertHighscoreCommand = new SQLiteCommand("INSERT INTO high_scores (name, result) VALUES (?, ?)", dbConnection);

        }

        void keyboard_ActivationChanged(object sender, EventArgs e)
        {
            String activatedLetter = ((KeyboardLetter)sender).Letter;
            if (activatedLetter.Equals("OK"))
            {
                insertHighscoreCommand.Parameters.Add(new SQLiteParameter("param1", currentName));
                insertHighscoreCommand.Parameters.Add(new SQLiteParameter("param2", scoreToEnter));
                dbConnection.Open();
                int insertRes = insertHighscoreCommand.ExecuteNonQuery();
                dbConnection.Close();
            }
            else if (activatedLetter.Equals("<-"))
            {
                if (currentName.Length > 0)
                {
                    currentName = currentName.Remove(currentName.Length - 1);
                }
            }
            else
            {
                currentName += ((KeyboardLetter)sender).Letter;
            }
            ((KeyboardLetter)sender).IsHovered = false;
        }

        void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    //Copy the skeleton data to our array
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // TODO
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);

            keyboard.Draw(ScreenManager);

            ScreenManager.SpriteBatch.DrawString(scoreFont, currentName, currentNamePosition, Color.Yellow);
            // Slaven, just for testing
            DrawSkeleton(ScreenManager.SpriteBatch, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), jointTexture);


            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // TODO 

            if (skeletonData != null)
            {
                for (int i = 0; i < skeletonData.Length; i++)
                {
                    Skeleton skel = skeletonData[i];
                    
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        skeletonToDraw = skel;
                        keyboard.CheckHoveredLetters(skel.Joints[JointType.HandLeft], skel.Joints[JointType.HandRight], ScreenManager);
                    }
                }
            }
            else
            {
                skeletonToDraw = null;
            }

            currentNamePosition.X = -scoreFont.MeasureString(currentName).X / 2;

            keyboard.Update();
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void DrawSkeleton(SpriteBatch spriteBatch, Vector2 resolution, Texture2D img)
        {
            if (skeletonToDraw != null)
            {
                foreach (Microsoft.Kinect.Joint joint in skeletonToDraw.Joints)
                {
                    Vector2 position = new Vector2((((0.5f * joint.Position.X) + 0.5f) * (resolution.X)) - ScreenManager.GraphicsDevice.Viewport.Width / 2, (((-0.5f * joint.Position.Y) + 0.5f) * (resolution.Y)) - ScreenManager.GraphicsDevice.Viewport.Height / 2);
                    spriteBatch.Draw(img, new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), 10, 10), Color.Red);
                }
            }
        }
    }
}
