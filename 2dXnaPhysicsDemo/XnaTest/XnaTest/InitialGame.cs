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
using SkinnedModel;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;

namespace XnaTest
{
    /// <summary>
    /// presents - category1
    /// planks - category2
    /// ground - category3
    /// walls - category4
    /// </summary>
    internal class InitialGame : PhysicsGameScreen, IDemoScreen
    {
        private int result = 0;
        private const int generatePresentsInterval = 4; //time in seconds
        private const float plankHeightPosition = 100f;
        private const int plankLength = 150;
        private const int borderSize = 10;
        private const int explosionStays = 50; // time in miliseconds
        //TODO remove after development
        private Microsoft.Kinect.Joint emptyJoint = new Microsoft.Kinect.Joint();
        private Vector2 emptyVector = new Vector2();

        private Dictionary<int, Sprite> presentSpriteBodyMapping;
        private List<Texture2D> presentTextures;
        private List<Body> presentBodies;
        Random random;
        private Texture2D background;
        Stopwatch presentsStopwatch;

        private Sprite groundBodySprite;
        private Body ground;
        private Sprite wallSprite;
        private Body wallL;
        private Body wallR;

        private Dictionary<int, Player> players;
        private Player initialPlayer; //because if no skeletons, one should be present

        private Sprite plankBodySprite;
        private Texture2D circleTexture;

        private Dictionary<double, Vector2> explosionTimesLocationsMapping;
        private Stopwatch explosionsStopwatch;

        private Texture2D explosionTexture;

        private Texture2D basketTexture;
        private Body basketBody;
        private Sprite basketSprite;
        private Texture2D basketCoverTexture;
        private Body basketCoverBody;
        private Sprite basketCoverSprite;

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
        private Matrix[] boneTransforms;
        private SkinningData skinningData;
        private AnimationPlayer animationPlayer;

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

            initialPlayer = new Player();
            players = new Dictionary<int, Player>();

            LoadAnimationContent();

            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinect = KinectSensor.KinectSensors[0];
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
                kinect.Start();

                initialPlayer.inputPosition = new KinectController(0, 0, 0);
            }
            else
            {
                initialPlayer.inputPosition = new KeyboardController(0, 0);
            }

            background = ScreenManager.Content.Load<Texture2D>("background");
            explosionTexture = ScreenManager.Content.Load<Texture2D>("star");

            
            createBasket();


            presentBodies = new List<Body>();
            presentSpriteBodyMapping = new Dictionary<int, Sprite>();
            explosionsStopwatch = Stopwatch.StartNew();
            explosionTimesLocationsMapping = new Dictionary<double, Vector2>();

            presentTextures = new List<Texture2D>();
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_1_transparent"));
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_2_transparent"));
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_3_transparent"));
            presentTextures.Add(ScreenManager.Content.Load<Texture2D>("PresentPictures/present_4_transparent"));

            initPlankBody(initialPlayer);

            initEdges();
            circleTexture = CreateCircle(5);
            random = new Random();

            World.Gravity = new Vector2(0, ScreenManager.GraphicsDevice.Viewport.Height / 15);

            jointTexture = ScreenManager.Content.Load<Texture2D>("joint");

            base.EnableCameraControl = false;
        }

        private void initEdges()
        {
            ground = BodyFactory.CreateRectangle(World,
                  ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width),
                  borderSize, 1f, ConvertUnits.ToSimUnits(new Vector2(0, ScreenManager.GraphicsDevice.Viewport.Height / 2f)));
            ground.IsStatic = true;
            ground.CollisionCategories = Category.Cat3;
            groundBodySprite = new Sprite(ScreenManager.Assets.TextureFromShape(ground.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.Orange, 1f));
            wallL = BodyFactory.CreateRectangle(World, borderSize,
                  ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f, 0)));
            wallR = BodyFactory.CreateRectangle(World, borderSize,
                  ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(-ScreenManager.GraphicsDevice.Viewport.Width / 2f, 0)));
            wallL.IsStatic = true;
            wallR.IsStatic = true;
            wallL.CollisionCategories = Category.Cat4;
            wallR.CollisionCategories = Category.Cat4;
            wallSprite = new Sprite(ScreenManager.Assets.TextureFromShape(wallL.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.Orange, 1f));
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

        private void updatePlankPositionVectors(Player player)
        {
            //TODO fetch constants from plank body
            player.centralPlankPosition.X = player.inputPosition.getX();
            player.centralPlankPosition.Y = plankHeightPosition;
            player.leftPlankPosition.X = player.inputPosition.getX() - plankLength / 2;
            player.leftPlankPosition.Y = plankHeightPosition + player.inputPosition.getDeltaY();
            player.rightPlankPosition.X = player.inputPosition.getX() + plankLength / 2;
            player.rightPlankPosition.Y = plankHeightPosition - player.inputPosition.getDeltaY();
        }

        private void initPlankBody(Player player)
        {
            updatePlankPositionVectors(player);

            player.plankBody = BodyFactory.CreateRectangle(World, plankLength, 10, 100f);
            player.plankBody.CollisionCategories = Category.Cat2;
            player.plankBody.CollidesWith = Category.Cat1 | Category.Cat4;
            player.plankBody.Position = player.centralPlankPosition.convertToVector2();

            player.plankBody.BodyType = BodyType.Dynamic;
            player.plankBody.Restitution = 0f;

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

        }

        private void DrawSkeleton(SpriteBatch spriteBatch, Vector2 resolution, Texture2D img)
        {
            if (skeleton != null)
            {
                foreach (Microsoft.Kinect.Joint joint in skeleton.Joints)
                {
                    Vector2 position = new Vector2((((0.5f * joint.Position.X) + 0.5f) * (resolution.X)) - ScreenManager.GraphicsDevice.Viewport.Width / 2, (((-0.5f * joint.Position.Y) + 0.5f) * (resolution.Y)) - ScreenManager.GraphicsDevice.Viewport.Height / 2);
                    spriteBatch.Draw(img, new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), 10, 10), Color.Red);
                }
            }
        }


        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);

            drawBackground();

            if (players.Count > 0)
            {
                foreach (Player player in players.Values)
                {
                    drawPlayer(player);
                }
            }
            else
            {
                drawPlayer(initialPlayer);
            }

            foreach (Body body in presentBodies)
            {
                ScreenManager.SpriteBatch.Draw(presentSpriteBodyMapping[body.BodyId].Texture, ConvertUnits.ToDisplayUnits(body.Position),
                                   null,
                                   Color.White, body.Rotation, presentSpriteBodyMapping[body.BodyId].Origin, 1f,
                                   SpriteEffects.None, 0f);
            }

            // draw basket and cover
            ScreenManager.SpriteBatch.Draw(basketTexture, ConvertUnits.ToDisplayUnits(basketBody.Position),
                    null,
                    Color.White, basketBody.Rotation, ConvertUnits.ToDisplayUnits(basketSprite.Origin), 1f,
                    SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(basketCoverTexture, ConvertUnits.ToDisplayUnits(basketCoverBody.Position),
                    null,
                    Color.White, basketCoverBody.Rotation, ConvertUnits.ToDisplayUnits(basketCoverSprite.Origin), 1f,
                    SpriteEffects.None, 0f);

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

            drawAnimationModel();

            base.Draw(gameTime);
        }

        private void drawBackground()
        {
            ScreenManager.SpriteBatch.Draw(background, new Rectangle(-ScreenManager.GraphicsDevice.Viewport.Width / 2, -ScreenManager.GraphicsDevice.Viewport.Height / 2, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), Color.White);
            ScreenManager.SpriteBatch.Draw(groundBodySprite.Texture, ConvertUnits.ToDisplayUnits(ground.Position), null, Color.White, 0f,
               groundBodySprite.Origin, 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(wallSprite.Texture, ConvertUnits.ToDisplayUnits(wallL.Position), null, Color.White, 0f,
               wallSprite.Origin, 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(wallSprite.Texture, ConvertUnits.ToDisplayUnits(wallR.Position), null, Color.White, 0f,
               wallSprite.Origin, 1f, SpriteEffects.None, 0f);
        }


        private void drawPlayer(Player player)
        {
            ScreenManager.SpriteBatch.Draw(plankBodySprite.Texture, ConvertUnits.ToDisplayUnits(player.plankBody.Position),
                               null,
                               Color.White, player.plankBody.Rotation, plankBodySprite.Origin, 1f,
                               SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(circleTexture, player.leftPlankPosition.convertToVector2(), Color.Black);
            ScreenManager.SpriteBatch.Draw(circleTexture, player.centralPlankPosition.convertToVector2(), Color.Black);
            ScreenManager.SpriteBatch.Draw(circleTexture, player.rightPlankPosition.convertToVector2(), Color.Black);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            populatePresent();

            updateAnimationModel(gameTime);

            if (skeletonData != null)
            {
                for (int i = 0; i < skeletonData.Length; i++)
                {
                    Skeleton skel = skeletonData[i];
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (players.Count == 0)
                        {
                            players.Add(i, initialPlayer);
                        }
                        else
                        {
                            if (!players.ContainsKey(i))
                            {
                                Player newPlayer = new Player();
                                newPlayer.inputPosition = new KinectController(0, 0, 0);
                                initPlankBody(newPlayer);
                                players.Add(i, newPlayer);
                            }
                        }
                        skeleton = skel;
                        players[i].inputPosition.HandleInput(
                            gameTime,
                            skel.Joints[Microsoft.Kinect.JointType.HandLeft], skel.Joints[Microsoft.Kinect.JointType.HandRight],
                            skel.Joints[Microsoft.Kinect.JointType.Head], skel.Joints[Microsoft.Kinect.JointType.ShoulderCenter],
                            new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height));
                        updatePlankPositionVectors(players[i]);
                        updateBodyFixedJoints(players[i]);
                    }
                    else
                    {
                        if (players.ContainsKey(i))
                        {
                            Player p = players[i];
                            if (p != initialPlayer)
                            {
                                p.plankBody.Dispose();
                            }
                            players.Remove(i);
                        }
                    }
                }
            }
            else
            {
                initialPlayer.inputPosition.HandleInput(
                            gameTime, emptyJoint, emptyJoint, emptyJoint, emptyJoint, emptyVector);
                updatePlankPositionVectors(initialPlayer);
                updateBodyFixedJoints(initialPlayer);
            }


            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private static void updateBodyFixedJoints(Player player)
        {
            player.fixedMouseJointL.WorldAnchorB = player.leftPlankPosition.convertToVector2();
            player.fixedMouseJointC.WorldAnchorB = player.centralPlankPosition.convertToVector2();
            player.fixedMouseJointR.WorldAnchorB = player.rightPlankPosition.convertToVector2();
        }

        private void update3DModel(GameTime gameTime)//TODO update this after fixing model animation
        {
            modelPosition = new Vector3(initialPlayer.plankBody.Position.X, initialPlayer.plankBody.Position.Y, 0);
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
            presentBody.Restitution = 1.0f;
            presentBody.Mass = 1;
            presentBody.CollisionCategories = Category.Cat1;
            presentBody.CollidesWith = Category.Cat1 | Category.Cat2 | Category.Cat3 | Category.Cat4 | Category.Cat5;
            // create sprite based on body
            presentSpriteBodyMapping.Add(presentBody.BodyId, new Sprite(presentTextures[textureIndex]));
            presentBodies.Add(presentBody);

            presentsStopwatch = Stopwatch.StartNew();
        }


        // Slaven, it requires more cleaning
        // parses picture and creates vertices based on the colors
        private Vertices getVertices (Texture2D polygonTexture) 
        {
            //Create an array to hold the data from the texture
            uint[] data = new uint[polygonTexture.Width * polygonTexture.Height];

            //Transfer the texture data to the array
            polygonTexture.GetData(data);

            //Find the vertices that makes up the outline of the shape in the texture
            Vertices verts = PolygonTools.CreatePolygon(data, polygonTexture.Width, true);// .CreatePolygon(data, polygonTexture.Width, polygonTexture.Height, true);

            //Vector2 scale = new Vector2(0.07f, 0.07f);
            //verts.Scale(ref scale);

            return verts;
        }

        // Slaven, it requires more cleaning
        // parses picture and creates polygon based on the colors
        public static Body ImageToPolygonBody(Texture2D texture, World world, float density, float scale_, ref Vector2 polygonOrigin)
        {
            //Create an array to hold the data from the texture
            uint[] data = new uint[(texture.Width) * (texture.Height)];

            //Collect data from bitmap
            texture.GetData(data);

            //Create Polygon from Bitmap
            Vertices verts = PolygonTools.CreatePolygon(data, (texture.Width), false);

            //Make sure that the origin of the texture is the centroid (real center of geometry)
            Vector2 scale = new Vector2(ConvertUnits.ToSimUnits(scale_), ConvertUnits.ToSimUnits(scale_));
            verts.Scale(ref scale);

            //Make sure that the origin of the texture is the centroid (real center of geometry)
            polygonOrigin = verts.GetCentroid();

            //Translate the polygon so that it aligns properly with centroid.
            Vector2 vertsTranslate = -polygonOrigin;
            verts.Translate(ref vertsTranslate);

            //We simplify the vertices found in the texture.
            verts = SimplifyTools.ReduceByDistance(verts, 4f);

            //Decompose polygon into smaller chuncks that Farseer can process better
            List<Vertices> list;
            list = BayazitDecomposer.ConvexPartition(verts);

            //Create a body
            return BodyFactory.CreateCompoundPolygon(world, list, density);


        }

        private void createBasket()
        {
            basketTexture = ScreenManager.Content.Load<Texture2D>("wired_basket");

            Vector2 basketPolygon = new Vector2(basketTexture.Width/2, basketTexture.Height/2);
            basketBody = ImageToPolygonBody(basketTexture, World, 1f, 1, ref basketPolygon);
            basketBody.Mass = 0;
            basketBody.Position = new Vector2((int)(-ScreenManager.GraphicsDevice.Viewport.Width / 2.5), ScreenManager.GraphicsDevice.Viewport.Height / 3);
            basketBody.CollidesWith = Category.Cat1;
            basketBody.CollisionCategories = Category.Cat5;
            basketBody.BodyType = BodyType.Static;

            basketSprite = new Sprite(ScreenManager.Assets.TextureFromVertices(getVertices(basketTexture), MaterialType.Squares, Color.Orange, 1f));

            // create cover which will colide with presents
            basketCoverTexture = ScreenManager.Content.Load<Texture2D>("wired_basket_cover");

            Vector2 basketCoverPolygon = new Vector2(basketCoverTexture.Width / 2, basketCoverTexture.Height / 2);
            basketCoverBody = BodyFactory.CreateRectangle(World, basketCoverTexture.Width, basketCoverTexture.Height, 1);// ImageToPolygonBody(basketCoverTexture, World, 1f, 1, ref basketCoverPolygon);
            basketCoverBody.Mass = 0;
            basketCoverBody.Position = new Vector2(basketBody.Position.X, basketBody.Position.Y - basketTexture.Height / 2);
            basketCoverBody.BodyType = BodyType.Static;
            basketCoverBody.CollisionCategories = Category.Cat5;
            basketCoverBody.CollidesWith = Category.Cat1;

            basketCoverSprite = new Sprite(ScreenManager.Assets.TextureFromShape(basketCoverBody.FixtureList[0].Shape, MaterialType.Squares, Color.Orange, 1f));
        }

        bool presentBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            int bodyIdToRemove = -1;
            Body bodyToRemove = null;

            if (fixtureB.Body.BodyId == ground.BodyId)
            {
                // it has hit the floor
                bodyIdToRemove = fixtureA.Body.BodyId;
                bodyToRemove = fixtureA.Body;
            }

            if (fixtureB.Body.BodyId == basketCoverBody.BodyId)
            {
                // it has hit basket on top
                bodyIdToRemove = fixtureA.Body.BodyId;
                bodyToRemove = fixtureA.Body;
                result++;
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

        #region Animation

        /// <summary>
        /// Loads content necessary for Animation
        /// </summary>
        private void LoadAnimationContent()
        {
            //TODO: Obrisi ovo
            aspectRatio = (float)ScreenManager.GraphicsDevice.Viewport.Width /
           (float)ScreenManager.GraphicsDevice.Viewport.Height;

            //Initialize model
            myModel = ScreenManager.Content.Load<Model>("Models/DudeWithAnimations");

            skinningData = myModel.Tag as SkinningData;
            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");
            boneTransforms = new Matrix[skinningData.BindPose.Count];

            // Create an animation player, and start decoding an animation clip.
            animationPlayer = new AnimationPlayer(skinningData);
            AnimationClip clip = skinningData.AnimationClips["Aim2"];
            animationPlayer.StartClip(clip);

        }

        /// <summary>
        /// Updates Animation Model
        /// </summary>
        private void updateAnimationModel(GameTime gameTime)
        {
            // Read gamepad inputs.
            float headRotation = 0f; // currentGamePadState.ThumbSticks.Left.X;
            float armRotation = 1f; // Math.Max(currentGamePadState.ThumbSticks.Left.Y, 0);

            // Create rotation matrices for the head and arm bones.
            Matrix headTransform = Matrix.CreateRotationX(headRotation);
            Matrix armTransform = Matrix.CreateFromYawPitchRoll(5f, 0f, armRotation);

            // Tell the animation player to compute the latest bone transform matrices.
            animationPlayer.UpdateBoneTransforms(gameTime.ElapsedGameTime, true);

            // Copy the transforms into our own array, so we can safely modify the values.
            animationPlayer.GetBoneTransforms().CopyTo(boneTransforms, 0);

            // Modify the transform matrices for the head and upper-left arm bones.
            int headIndex = skinningData.BoneIndices["Head"];
            int armIndex = skinningData.BoneIndices["L-Hand"];

            boneTransforms[headIndex] = headTransform * boneTransforms[headIndex];
            boneTransforms[armIndex] = armTransform * boneTransforms[armIndex];

            // Tell the animation player to recompute the world and skin matrices.
            animationPlayer.UpdateWorldTransforms(Matrix.Identity, boneTransforms);
            animationPlayer.UpdateSkinTransforms();

            //TODO obriši ovo
            modelPosition = new Vector3(initialPlayer.plankBody.Position.X / 120, initialPlayer.plankBody.Position.Y / 120, 0);
            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds *
                MathHelper.ToRadians(0.1f);
        }

        /// <summary>
        /// Draw Animation Model
        /// </summary>
        private void drawAnimationModel()
        {
            Matrix[] bones = animationPlayer.GetSkinTransforms();

            Matrix world = Matrix.CreateRotationX(MathHelper.ToRadians(-90f))
                         * Matrix.CreateTranslation(new Vector3(0, 1, 10))
                         * Matrix.CreateTranslation(modelPosition);

            // Compute camera matrices.
            Matrix view = Matrix.CreateTranslation(0, -40, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(0)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(0)) *
                          Matrix.CreateLookAt(new Vector3(0, 0, -100),
                                              new Vector3(0, 0, 0), Vector3.Up) * Matrix.CreateTranslation(modelPosition);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    ScreenManager.GraphicsDevice.Viewport.AspectRatio,
                                                                    1,
                                                                    10000);

            // Render the skinned mesh.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.World = Matrix.CreateRotationX(MathHelper.ToRadians(-90f))
                                   * Matrix.CreateTranslation(new Vector3(0, -3f, -1))
                                   * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                    ScreenManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);

                    //effect.World = world;
                    //effect.View = view;
                    //effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }

            // TODO Obriši ovo
            /*
            // Draw the model (to the rendertarget)
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.Bones["Neck"].Transform = Matrix.CreateTranslation(new Vector3(0, 20, 0)) * myModel.Bones["Neck"].Transform;
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationX(MathHelper.ToRadians(-90f))
                         * Matrix.CreateTranslation(new Vector3(0, -1, 0))
                         * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                    (float)ScreenManager.GraphicsDevice.Viewport.Width / (float)ScreenManager.GraphicsDevice.Viewport.Height, 1.0f, 100.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            */

        }


        #endregion
    }
}
