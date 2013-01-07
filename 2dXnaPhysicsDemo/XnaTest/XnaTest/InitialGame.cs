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
using XnaTest.Controller;
using FarseerPhysics.Dynamics.Joints;

namespace XnaTest
{
    internal class InitialGame : PhysicsGameScreen, IDemoScreen
    {
        private List<Sprite> presentsSprites;
        private List<Texture2D> presentTextures;
        private List<Body> presentBodies;
        Random random;
        private Texture2D background;

        Stopwatch stopwatch;
        private int generatePresentsInterval = 4; //time in seconds
        private AnimatedSprite characterSprite;
        private Body character;
        private Sprite groundBodySprite;
        private Body ground;
        private KeyboardController charcterPosition;
        private FixedMouseJoint charcterJoint;
        private Texture2D circleTexture;

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

            background = ScreenManager.Content.Load<Texture2D>("background");

            presentsSprites = new List<Sprite>();
            presentBodies = new List<Body>();

            presentTextures = new List<Texture2D>();
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_1_transparent"));
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_2_transparent"));
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_3_transparent"));
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_4_transparent"));

            characterSprite = new AnimatedSprite(ScreenManager.Content.Load<Texture2D>("character"), 4, 4);
            character = BodyFactory.CreateRectangle(World, characterSprite.Width, characterSprite.Height, 10f);
            //character.Position = new Vector2(0, -50);
            character.BodyType = BodyType.Dynamic;
            character.Restitution = 2.0f;

            charcterPosition = new KeyboardController();
            charcterJoint = new FixedMouseJoint(character, charcterPosition.getLeftHandPosition());
            //charcterJoint.JointType = JointType.
            World.AddJoint(charcterJoint);
            character.Awake = true;
            charcterJoint.WorldAnchorB = charcterPosition.getLeftHandPosition();

            ground = BodyFactory.CreateRectangle(World,
                  ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width * 2f),
                  20, 1f, ConvertUnits.ToSimUnits(new Vector2(-ScreenManager.GraphicsDevice.Viewport.Width / 2f, ScreenManager.GraphicsDevice.Viewport.Height / 2f)));
            ground.Restitution = 0.8f;
            ground.IsStatic = true;

            groundBodySprite = new Sprite(ScreenManager.Assets.TextureFromShape(ground.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.Orange, 1f));
            circleTexture = CreateCircle(10);
            random = new Random();

            World.Gravity = new Vector2(0, ScreenManager.GraphicsDevice.Viewport.Height);
            base.EnableCameraControl = false;
        }


        public override void Draw(GameTime gameTime)
        {

            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            ScreenManager.SpriteBatch.Draw(background, new Rectangle(-ScreenManager.GraphicsDevice.Viewport.Width / 2, -ScreenManager.GraphicsDevice.Viewport.Height / 2, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), Color.White);
            
            characterSprite.Draw(ScreenManager.SpriteBatch, new Vector2(character.Position.X - characterSprite.Width / 2, character.Position.Y - characterSprite.Height / 2));

            ScreenManager.SpriteBatch.Draw(groundBodySprite.Texture, ConvertUnits.ToDisplayUnits(ground.Position), null, Color.White, 0f,
               groundBodySprite.Origin, 1f, SpriteEffects.None, 0f);


            ScreenManager.SpriteBatch.Draw(circleTexture, charcterPosition.getLeftHandPosition(), Color.Black);

            for (int i = 0; i < presentBodies.Count; ++i)
            {
                ScreenManager.SpriteBatch.Draw(presentsSprites[i].Texture, ConvertUnits.ToDisplayUnits(presentBodies[i].Position),
                                   null,
                                   Color.White, presentBodies[i].Rotation, presentsSprites[i].Origin, 1f,
                                   SpriteEffects.None, 0f);
            }

            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            characterSprite.Update();
            populatePresent();
            charcterPosition.HandleInput(gameTime);
            charcterJoint.WorldAnchorB = charcterPosition.getLeftHandPosition();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            
        }

        private void populatePresent()
        {
            if (stopwatch == null)
            {
                createPresent();
            }
            else
            {
                double elapsedTime = stopwatch.ElapsedMilliseconds;
                // if 10 seconds passed created new present
                if (elapsedTime / 1000 > generatePresentsInterval)
                {
                    createPresent();
                }
            }

        }

        private void createPresent()
        {
            int textureIndex = random.Next(0, 3);
            Texture2D presentTexture = presentTextures[textureIndex];
            Body presentBody = BodyFactory.CreateRectangle(World, presentTexture.Width, presentTexture.Height, 10f);
            presentBody.Position = new Vector2(random.Next(-ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Width / 2), -ScreenManager.GraphicsDevice.Viewport.Height / 2);
            presentBody.BodyType = BodyType.Dynamic;

            // create sprite based on body
            presentsSprites.Add(new Sprite(presentTextures[textureIndex]));
            presentBodies.Add(presentBody);

            stopwatch = Stopwatch.StartNew();
        }

        public Texture2D CreateCircle(int radius)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(ScreenManager.GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }
    }
}
