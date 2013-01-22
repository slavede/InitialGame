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

namespace XnaTest
{
    internal class InitialGame : PhysicsGameScreen, IDemoScreen
    {
        private Dictionary<int, Sprite> presentSpriteBodyMapping;
        private List<Texture2D> presentTextures;
        private List<Body> presentBodies;
        Random random;
        private Texture2D background;

        Stopwatch presentsStopwatch;
        private int generatePresentsInterval = 4; //time in seconds
        private float plankHeightPosition = 0f;
        private int plankLength = 100;

        private Sprite groundBodySprite;
        private Body ground;

        private Player player;
        //private Body plankBody;
        //private CharacterController characterPosition;
        //private Vector2 centralPlankPosition;
        //private Vector2 leftPlankPosition;
        //private Vector2 rightPlankPosition;
        //private FixedMouseJoint fixedMouseJointL;
        //private FixedMouseJoint fixedMouseJointC;
        //private FixedMouseJoint fixedMouseJointR;

        private Sprite plankBodySprite;
        private Texture2D circleTexture;

        private Dictionary<double, Vector2> explosionTimesLocationsMapping;
        private Stopwatch explosionsStopwatch;
        private int explosionStays = 50; // time in miliseconds
        private Texture2D explosionTexture;

        KinectSensor kinect;
        Skeleton[] skeletonData;
        Skeleton skeleton;
        private Texture2D jointTexture;

        Model myModel;
        // Set the position of the model in world space, and set the rotation.
        Vector3 modelPosition = Vector3.Zero;
        float modelRotation = 0.0f;
        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 5000);
        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        //TODO remove after development
        Microsoft.Kinect.Joint emptyJoint = new Microsoft.Kinect.Joint();
        Vector2 emptyVector = new Vector2();

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

            player = new Player();

            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinect = KinectSensor.KinectSensors[0];
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
                kinect.Start();

                player.inputPosition = new KinectController(0, 0, 0);
            }
            else
            {
                player.inputPosition = new KeyboardController(0, 0);
            }

            aspectRatio = (float)ScreenManager.GraphicsDevice.Viewport.Width /
            (float)ScreenManager.GraphicsDevice.Viewport.Height;
            myModel = ScreenManager.Content.Load<Model>("Models\\p1_wedge");

            background = ScreenManager.Content.Load<Texture2D>("background");
            explosionTexture = ScreenManager.Content.Load<Texture2D>("star");

            presentBodies = new List<Body>();
            presentSpriteBodyMapping = new Dictionary<int, Sprite>();
            explosionsStopwatch = Stopwatch.StartNew();
            explosionTimesLocationsMapping = new Dictionary<double, Vector2>();

            presentTextures = new List<Texture2D>();
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_1_transparent"));
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_2_transparent"));
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_3_transparent"));
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_4_transparent"));

            initPlankBody();

            ground = BodyFactory.CreateRectangle(World,
                  ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width * 2f),
                  20, 1f, ConvertUnits.ToSimUnits(new Vector2(-ScreenManager.GraphicsDevice.Viewport.Width / 2f, ScreenManager.GraphicsDevice.Viewport.Height / 2f)));
            ground.Restitution = 0.8f;
            ground.IsStatic = true;

            groundBodySprite = new Sprite(ScreenManager.Assets.TextureFromShape(ground.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.Orange, 1f));
            circleTexture = CreateCircle(5);
            random = new Random();

            World.Gravity = new Vector2(0, ScreenManager.GraphicsDevice.Viewport.Height);

            jointTexture = ScreenManager.Content.Load<Texture2D>("joint");

            base.EnableCameraControl = false;
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

        private void updatePlankPositionVectors()
        {
            //TODO fetch constants from plank body
            player.centralPlankPosition.X = player.inputPosition.getX();
            player.centralPlankPosition.Y = plankHeightPosition;
            player.leftPlankPosition.X = player.inputPosition.getX() - plankLength / 2;
            player.leftPlankPosition.Y = plankHeightPosition + player.inputPosition.getDeltaY();
            player.rightPlankPosition.X = player.inputPosition.getX() + plankLength / 2;
            player.rightPlankPosition.Y = plankHeightPosition - player.inputPosition.getDeltaY();
        }

        private void initPlankBody()
        {
            updatePlankPositionVectors();

            player.plankBody = BodyFactory.CreateRectangle(World, plankLength, 10, 1000f);

            player.plankBody.BodyType = BodyType.Dynamic;
            player.plankBody.Restitution = 1f;

            plankBodySprite = new Sprite(ScreenManager.Assets.TextureFromShape(player.plankBody.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.Orange, 1f));
            player.fixedMouseJointL = new FixedMouseJoint(player.plankBody, player.leftPlankPosition.convertToVector2());
            player.fixedMouseJointL.MaxForce = 1000.0f * player.plankBody.Mass;
            World.AddJoint(player.fixedMouseJointL);
            player.fixedMouseJointC = new FixedMouseJoint(player.plankBody, player.centralPlankPosition.convertToVector2());
            player.fixedMouseJointC.MaxForce = 1000.0f * player.plankBody.Mass;
            World.AddJoint(player.fixedMouseJointC);
            player.fixedMouseJointR = new FixedMouseJoint(player.plankBody, player.rightPlankPosition.convertToVector2());
            player.fixedMouseJointR.MaxForce = 1000.0f * player.plankBody.Mass;
            World.AddJoint(player.fixedMouseJointR);
            player.plankBody.Awake = true;

            player.fixedMouseJointL.DampingRatio = 1.0f;
            player.fixedMouseJointC.DampingRatio = 1.0f;
            player.fixedMouseJointR.DampingRatio = 1.0f;

            //fixedMouseJointL.WorldAnchorB = characterPosition.getLeftHandPosition();
            //fixedMouseJointR.WorldAnchorB = characterPosition.getRightHandPosition();
        }

        private void DrawSkeleton(SpriteBatch spriteBatch, Vector2 resolution, Texture2D img)
        {
            if (skeleton != null)
            {
                foreach (Microsoft.Kinect.Joint joint in skeleton.Joints)
                {
                    //float xPosition = joint.Position.X - ScreenManager.GraphicsDevice.Viewport.Width / 2;
                    //float yPosition = joint.Position.Y - ScreenManager.GraphicsDevice.Viewport.Height / 2;
                    Vector2 position = new Vector2((((0.5f * joint.Position.X) + 0.5f) * (resolution.X)) - ScreenManager.GraphicsDevice.Viewport.Width / 2, (((-0.5f * joint.Position.Y) + 0.5f) * (resolution.Y)) - ScreenManager.GraphicsDevice.Viewport.Height / 2);
                    spriteBatch.Draw(img, new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), 10, 10), Color.Red);
                }
            }
        }

        private void draw3DModel()
        {
            // Draw the model (to the rendertarget)
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(cameraPosition,
                        Vector3.Zero,
                        Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);
                    effect.EnableDefaultLighting();
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);

            ScreenManager.SpriteBatch.Draw(background, new Rectangle(-ScreenManager.GraphicsDevice.Viewport.Width / 2, -ScreenManager.GraphicsDevice.Viewport.Height / 2, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), Color.White);

            ScreenManager.SpriteBatch.Draw(groundBodySprite.Texture, ConvertUnits.ToDisplayUnits(ground.Position), null, Color.White, 0f,
               groundBodySprite.Origin, 1f, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.Draw(plankBodySprite.Texture, ConvertUnits.ToDisplayUnits(player.plankBody.Position),
                               null,
                               Color.White, player.plankBody.Rotation, plankBodySprite.Origin, 1f,
                               SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(circleTexture, player.leftPlankPosition.convertToVector2(), Color.Black);
            ScreenManager.SpriteBatch.Draw(circleTexture, player.centralPlankPosition.convertToVector2(), Color.Black);
            ScreenManager.SpriteBatch.Draw(circleTexture, player.rightPlankPosition.convertToVector2(), Color.Black);

            foreach (Body body in presentBodies) 
            {
                ScreenManager.SpriteBatch.Draw(presentSpriteBodyMapping[body.BodyId].Texture, ConvertUnits.ToDisplayUnits(body.Position),
                                   null,
                                   Color.White, body.Rotation, presentSpriteBodyMapping[body.BodyId].Origin, 1f,
                                   SpriteEffects.None, 0f);
            }



            List<double> explosionsToRemove = new List<double>();
            foreach (double timestamp in explosionTimesLocationsMapping.Keys)
            {
                ScreenManager.SpriteBatch.Draw(explosionTexture, new Rectangle((int)explosionTimesLocationsMapping[timestamp].X, (int)explosionTimesLocationsMapping[timestamp].Y, explosionTexture.Width, explosionTexture.Height), Color.White);
                if (explosionsStopwatch.Elapsed.Milliseconds - timestamp > explosionStays)
                {
                    explosionsToRemove.Add(timestamp);
                }
            }

            foreach (double timestamp in explosionsToRemove) 
            {
                explosionTimesLocationsMapping.Remove(timestamp);
            }

            // Slaven, just for testing
            DrawSkeleton(ScreenManager.SpriteBatch, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), jointTexture);
            
            ScreenManager.SpriteBatch.End();

            //draw3DModel();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            populatePresent();

            update3DModel(gameTime);
            //characterPosition.HandleInput(gameTime);
            updatePlankPositionVectors();
            player.fixedMouseJointL.WorldAnchorB = player.leftPlankPosition.convertToVector2();
            player.fixedMouseJointC.WorldAnchorB = player.centralPlankPosition.convertToVector2();
            player.fixedMouseJointR.WorldAnchorB = player.rightPlankPosition.convertToVector2();

            if (skeletonData != null)
            {
                foreach (Skeleton skel in skeletonData)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        skeleton = skel;
                        player.inputPosition.HandleInput(
                            gameTime,
                            skel.Joints[Microsoft.Kinect.JointType.HandLeft], skel.Joints[Microsoft.Kinect.JointType.HandRight],
                            skel.Joints[Microsoft.Kinect.JointType.Head], skel.Joints[Microsoft.Kinect.JointType.ShoulderCenter],
                            new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height));
                    }
                }
            }
            else
            {
                player.inputPosition.HandleInput(
                            gameTime, emptyJoint, emptyJoint, emptyJoint, emptyJoint, emptyVector);
            }


            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void update3DModel(GameTime gameTime)
        {
            modelPosition = new Vector3(player.plankBody.Position.X, player.plankBody.Position.Y, 0);
            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds *
                MathHelper.ToRadians(0.1f);
        }

        private void populatePresent()
        {
            if (presentsStopwatch == null)
            {
                createPresent();
            }
            else
            {
                double elapsedTime = presentsStopwatch.ElapsedMilliseconds;
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
            Body presentBody = BodyFactory.CreateRectangle(World, presentTexture.Width, presentTexture.Height, 100f);
            presentBody.Position = new Vector2(random.Next(-ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Width / 2), -ScreenManager.GraphicsDevice.Viewport.Height / 2);
            presentBody.BodyType = BodyType.Dynamic;
            presentBody.OnCollision += new OnCollisionEventHandler(presentBody_OnCollision);
            presentBody.Restitution = 2.0f;

            // create sprite based on body
            presentSpriteBodyMapping.Add(presentBody.BodyId, new Sprite(presentTextures[textureIndex]));
            presentBodies.Add(presentBody);

            presentsStopwatch = Stopwatch.StartNew();
        }

        bool presentBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            int bodyIdToRemove = -1;
            Body bodyToRemove = null;
            if (fixtureA.Body.BodyId == ground.BodyId)
            {
                // it has hit the floor
                bodyIdToRemove = fixtureB.Body.BodyId;
                bodyToRemove = fixtureB.Body;
            }
            else if (fixtureB.Body.BodyId == ground.BodyId) 
            {
                // it has hit the floor
                bodyIdToRemove = fixtureA.Body.BodyId;
                bodyToRemove = fixtureA.Body;
            }

            if (bodyIdToRemove != -1)
            {
                int explosionTime = presentsStopwatch.Elapsed.Milliseconds;

                do
                {
                    if (!explosionTimesLocationsMapping.ContainsKey(explosionTime))
                    {
                        explosionTimesLocationsMapping.Add(explosionTime, new Vector2(bodyToRemove.Position.X - presentSpriteBodyMapping[bodyIdToRemove].Texture.Width / 2, bodyToRemove.Position.Y - presentSpriteBodyMapping[bodyIdToRemove].Texture.Height / 2));
                    }
                    else
                    {
                        explosionTime++;
                    }
                }
                while (explosionTimesLocationsMapping.ContainsKey(explosionTime));

                int removed_index = presentBodies.RemoveAll(body => body.BodyId == bodyIdToRemove) - 1; // remove by condition 
                
                if (removed_index != -1)
                {
                    presentSpriteBodyMapping.Remove(removed_index);
                    bodyToRemove.Dispose();
                    return false;
                }
                return true;
                
            }
            else
            {
                return true;
            }
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
