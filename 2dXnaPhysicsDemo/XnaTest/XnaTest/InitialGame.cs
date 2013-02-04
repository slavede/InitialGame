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

        // Set the position of the model in world space, and set the rotation.
        Vector3 modelPosition = Vector3.Zero;
        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 5000);

        private SpriteFont scoreFont;
        private Color[] scoreColors;
        private int xScorePosition;
        private int yScorePosition;

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

            initialPlayer = new Player(new Majlo(ScreenManager.Content));       

            players = new Dictionary<int, Player>();

            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinect = KinectSensor.KinectSensors[0];
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSkeletonFrameReady);
                kinect.Start();

                initialPlayer.inputPosition = new KinectController(0, 0, 0);
            }
            else
            {
                initialPlayer.inputPosition = new KeyboardController(0, 0);
            }

            background = ScreenManager.Content.Load<Texture2D>("background");
            explosionTexture = ScreenManager.Content.Load<Texture2D>("star");

            xScorePosition = -ScreenManager.GraphicsDevice.Viewport.Width / 2 + 20;
            yScorePosition = -ScreenManager.GraphicsDevice.Viewport.Height / 2 + 20;
            scoreFont = ScreenManager.Content.Load<SpriteFont>("Font");
            scoreColors = new Color[2];
            scoreColors[0] = Color.Red;
            scoreColors[1] = Color.Yellow;

            initialPlayer.basketId = createBasket();


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

        void kinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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

        private void initPlankBody(Player player)
        {
            player.updatePlankPositionVectors();

            player.plankBody = BodyFactory.CreateRectangle(World, Player.plankLength, 10, 100f);
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

            int playerCounter = 0;
            if (players.Count > 0)
            {
                foreach (Player player in players.Values)
                {
                    drawPlayer(player);
                    ScreenManager.SpriteBatch.DrawString(scoreFont, "Player" + (playerCounter+1).ToString() + " score: " + initialPlayer.getPoints().ToString(), new Vector2(xScorePosition, yScorePosition), Color.Red);
                    yScorePosition = 20 + (int)scoreFont.MeasureString("Player" + (playerCounter + 1).ToString() + " score: ").Y * playerCounter;
                    playerCounter++;
                }
            }
            else
            {
                drawPlayer(initialPlayer);
                ScreenManager.SpriteBatch.DrawString(scoreFont, "Player1 score: " + initialPlayer.getPoints().ToString(), new Vector2(xScorePosition, yScorePosition), Color.Red);
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
            player.Draw(ScreenManager.SpriteBatch);

            //only for development
            ScreenManager.SpriteBatch.Draw(circleTexture, player.leftPlankPosition.convertToVector2(), Color.Black);
            ScreenManager.SpriteBatch.Draw(circleTexture, player.centralPlankPosition.convertToVector2(), Color.Black);
            ScreenManager.SpriteBatch.Draw(circleTexture, player.rightPlankPosition.convertToVector2(), Color.Black);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            populatePresent();

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
                                Player newPlayer = new Player(new Majlo(ScreenManager.Content)); //TODO sredit ovo - nekakav random ili sta vec
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
                        players[i].update();
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
                initialPlayer.update();
            }


            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
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

        private int createBasket()
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

            return basketCoverBody.BodyId;
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

            if (players.Count > 0)
            {
                foreach (Player player in players.Values)
                {
                    if (fixtureB.Body.BodyId == player.basketId)
                    {
                        bodyIdToRemove = fixtureA.Body.BodyId;
                        bodyToRemove = fixtureA.Body;
                        player.addPoints(1);
                    }
                }
            }
            else
            {
                if (fixtureB.Body.BodyId == initialPlayer.basketId)
                {
                    bodyIdToRemove = fixtureA.Body.BodyId;
                    bodyToRemove = fixtureA.Body;
                    initialPlayer.addPoints(1);
                }
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
