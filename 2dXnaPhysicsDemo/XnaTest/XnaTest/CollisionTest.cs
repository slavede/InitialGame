using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.SamplesFramework;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using System.Collections.Generic;
using XnaTest.Controller;
using Microsoft.Xna.Framework.Input;
using System;
using FarseerPhysics.Dynamics.Joints;

namespace XnaTest
{ 
    internal class CollisionTest : PhysicsGameScreen, IDemoScreen
    {
        private Border _border;
        private Body _rectangle;
        private Body _plankBody;
        private Sprite _rectangleSprite;
        private Texture2D background;
        private AnimatedSprite characterSprite;
        private List<Body> _bridgeBodiesL;
        private List<Body> _bridgeBodiesR;
        private Sprite _bridgeBox;

        private FixedMouseJoint _fixedMouseJoint;

        private CharacterController plankPosition;
        private PolygonShape shape;
        private Vertices box;
        private Sprite _plankBodySprite;
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
            characterSprite = new AnimatedSprite(ScreenManager.Content.Load<Texture2D>("character"), 4, 4);

            World.Gravity = new Vector2(0, 20f);

            _border = new Border(World, this, ScreenManager.GraphicsDevice.Viewport);

            _rectangle = BodyFactory.CreateRectangle(World, characterSprite.Width, characterSprite.Height, 10f);
            _rectangle.Position = new Vector2(0, -50);
            _rectangle.BodyType = BodyType.Dynamic;

            //SetUserAgent(_rectangle, 5f, 1f);

            // create sprite based on body
            _rectangleSprite = new Sprite(ScreenManager.Assets.TextureFromShape(_rectangle.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.Orange, 1f));
            
            plankPosition = new KeyboardController();
            base.EnableCameraControl = false;
         
            //LoadStaticObstacles();
            LoadDynamicObstacles();

            circleTexture = CreateCircle(10);
            
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
        
        private void LoadDynamicObstacles()
        {
            _plankBody = BodyFactory.CreateRectangle(World, 300, 10, 1f);
            _plankBody.BodyType = BodyType.Dynamic;
            _plankBody.Restitution = 1f;
           
            
            _plankBodySprite = new Sprite(ScreenManager.Assets.TextureFromShape(_plankBody.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.Orange, 1f));

            _fixedMouseJoint = new FixedMouseJoint(_plankBody, plankPosition.getLeftHandPosition());

            _fixedMouseJoint.MaxForce = 1000.0f * _plankBody.Mass;
            World.AddJoint(_fixedMouseJoint);
            _plankBody.Awake = true;

            UpdateDynamicObstacles();
        }

        private void UpdateDynamicObstacles()
        {
   
            //World.AddJoint(_fixedMouseJoint);
            _fixedMouseJoint.WorldAnchorB = plankPosition.getLeftHandPosition();
            

            //Fixture savedFixture = World.TestPoint(plankPosition.getLeftHandPosition());
            //if (savedFixture != null)
            //{
            //    Body body = savedFixture.Body;
            //    _fixedMouseJoint = new FixedMouseJoint(body, plankPosition.getLeftHandPosition());
            //    _fixedMouseJoint.MaxForce = 1000.0f * body.Mass;
            //    World.AddJoint(_fixedMouseJoint);
            //    body.Awake = true;
            //}

            //_plankBody.Position = plankPosition.getLeftHandPosition();
           // _plankBody.Rotation = (float)Math.Atan(1.0f*(plankPosition.getLeftHandPosition().Y - plankPosition.getRightHandPosition().Y) / (plankPosition.getRightHandPosition().X - plankPosition.getLeftHandPosition().X));
        }

        private void LoadStaticObstacles()
        {
            box = PolygonTools.CreateRectangle(1f, 10f);
            shape = new PolygonShape(box, 30);
            _bridgeBox = new Sprite(ScreenManager.Assets.TextureFromShape(shape, MaterialType.Dots, Color.SandyBrown, 1f));

            Path bridgePathL = new Path();
            bridgePathL.Add(new Vector2(-400, -50));
            bridgePathL.Add(new Vector2(0, 0));
            bridgePathL.Closed = false;

            _bridgeBodiesL = PathManager.EvenlyDistributeShapesAlongPath(World, bridgePathL, shape,
                                                                        BodyType.Dynamic, 30);
           

            //Attach the first and last fixtures to the world
            JointFactory.CreateFixedRevoluteJoint(World, _bridgeBodiesL[0], new Vector2(0f, -0.5f),
                                                  _bridgeBodiesL[0].Position);
            JointFactory.CreateFixedRevoluteJoint(World, _bridgeBodiesL[_bridgeBodiesL.Count - 1], new Vector2(0, 0.5f),
                                                  _bridgeBodiesL[_bridgeBodiesL.Count - 1].Position);

            PathManager.AttachBodiesWithRevoluteJoint(World, _bridgeBodiesL, new Vector2(0f, -0.5f),
                                                      new Vector2(0f, 0.5f),
                                                      false, true);

            Path bridgePathR = new Path();
            bridgePathR.Add(new Vector2(350, -50));
            bridgePathR.Add(new Vector2(0, 0));
            bridgePathR.Closed = false;

            _bridgeBodiesR = PathManager.EvenlyDistributeShapesAlongPath(World, bridgePathR, shape,
                                                                        BodyType.Dynamic, 30);
          
            //Attach the first and last fixtures to the world
            JointFactory.CreateFixedRevoluteJoint(World, _bridgeBodiesR[0], new Vector2(0f, -0.5f),
                                                  _bridgeBodiesR[0].Position);
            JointFactory.CreateFixedRevoluteJoint(World, _bridgeBodiesR[_bridgeBodiesR.Count - 1], new Vector2(0, 0.5f),
                                                  _bridgeBodiesR[_bridgeBodiesR.Count - 1].Position);

            PathManager.AttachBodiesWithRevoluteJoint(World, _bridgeBodiesR, new Vector2(0f, -0.5f),
                                                      new Vector2(0f, 0.5f),
                                                      false, true);


        }

        public override void Draw(GameTime gameTime)
        {
            
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            ScreenManager.SpriteBatch.Draw(background, new Rectangle(-ScreenManager.GraphicsDevice.Viewport.Width / 2, -ScreenManager.GraphicsDevice.Viewport.Height / 2, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), Color.Red);
            // otkomentiraj ovo za gledat kako izgleda model
            ScreenManager.SpriteBatch.Draw(_rectangleSprite.Texture, ConvertUnits.ToDisplayUnits(_rectangle.Position),
                               null,
                               Color.White, _rectangle.Rotation, _rectangleSprite.Origin, 1f,
                               SpriteEffects.None, 0f);

            
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Content.Load<SpriteFont>("Font"), "width, height: " + _rectangle.Position.X +" "+ _rectangle.Position.Y, new Vector2(0, 130), Color.Black);

            ScreenManager.SpriteBatch.Draw(circleTexture, plankPosition.getLeftHandPosition(), Color.Black);


            // otkomentiraj ovo za gledat kako izgleda path sisa
            //for (int i = 0; i < _bridgeBodiesL.Count; ++i)
            //{
            //    ScreenManager.SpriteBatch.Draw(_bridgeBox.Texture,
            //                                   ConvertUnits.ToDisplayUnits(_bridgeBodiesL[i].Position), null,
            //                                   Color.White, _bridgeBodiesL[i].Rotation, _bridgeBox.Origin, 1f,
            //                                   SpriteEffects.None, 0f);
            //}
            //for (int i = 0; i < _bridgeBodiesR.Count; ++i)
            //{
            //    ScreenManager.SpriteBatch.Draw(_bridgeBox.Texture,
            //                                   ConvertUnits.ToDisplayUnits(_bridgeBodiesR[i].Position), null,
            //                                   Color.White, _bridgeBodiesR[i].Rotation, _bridgeBox.Origin, 1f,
            //                                   SpriteEffects.None, 0f);
            //}

            ScreenManager.SpriteBatch.Draw(_plankBodySprite.Texture, ConvertUnits.ToDisplayUnits(_plankBody.Position),
                               null,
                               Color.White, _plankBody.Rotation, _plankBodySprite.Origin, 1f,
                               SpriteEffects.None, 0f);
         

            ScreenManager.SpriteBatch.End();
            _border.Draw();

            base.Draw(gameTime);
        }

         public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            characterSprite.Update();
            plankPosition.HandleInput(gameTime);
            UpdateDynamicObstacles();
            //updateDynamicPathPosition();
            
        }
    }
}