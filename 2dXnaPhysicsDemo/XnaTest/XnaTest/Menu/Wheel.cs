using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.SamplesFramework;

namespace XnaTest.Menu
{
    class Wheel : WheelDelegate
    {
        Body wheelBody;
        Sprite wheelSprite;
        private ScreenManager screenManager;
        FixedMouseJoint fixedMouseJoint;
        FixedMouseJoint idleJoint;
        World world;

        Dictionary<int, String> entries;
        float radius;
        int selectedEntryIndex = 0;
        Vector2 selectedEntryPosition = new Vector2();
        private Sprite phone;
        private Sprite notebook;
        private Sprite background;
        public Texture2D jointTexture { get; set; }
        public SpriteFont font { get; set; }

        public Wheel(ScreenManager screenManager, World world, float radius, Vector2 centerPosition, String[] entryStrings)
        {
            this.radius = radius;
            this.entries = new Dictionary<int,String>(entryStrings.Length);
            for(int i=0;i<entryStrings.Length;i++){
                this.entries.Add(i, entryStrings[i]);
            }
            this.world = world;
            this.screenManager = screenManager;
            wheelBody = BodyFactory.CreateCircle(world, radius, 1f);
            wheelBody.BodyType = BodyType.Dynamic;
            wheelBody.Restitution = .7f;
            wheelBody.Friction = .2f;

            font = screenManager.Content.Load<SpriteFont>("Font");
            FixedRevoluteJoint rollingJoint = JointFactory.CreateFixedRevoluteJoint(world, wheelBody, Vector2.Zero, Vector2.Zero);
            wheelSprite = new Sprite(screenManager.Content.Load<Texture2D>("wheel"));
            
            jointTexture = screenManager.Content.Load<Texture2D>("joint");
            background = new Sprite(screenManager.Content.Load<Texture2D>("wheelBackground"));
            phone = new Sprite(screenManager.Content.Load<Texture2D>("phone"));
            notebook = new Sprite(screenManager.Content.Load<Texture2D>("notebook"));
        }

        public int GetSelectedEntryIndex()
        {
            return selectedEntryIndex;
        }

        /// <summary>
        /// sprite batch will be open and closed afterwards.
        /// DO NOT open or close sprite batch
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw()
        {
            screenManager.SpriteBatch.Draw(phone.Texture, ConvertUnits.ToDisplayUnits(new Vector2(-7, -32)),
                                          null,
                                          Color.White, 0f, phone.Origin, 0.25f,
                                          SpriteEffects.None, 0f);
            screenManager.SpriteBatch.Draw(background.Texture, ConvertUnits.ToDisplayUnits(wheelBody.Position),
                                          null,
                                          Color.White, 0f, background.Origin, 2.0f *radius / background.Texture.Bounds.Height,
                                          SpriteEffects.None, 0f);
            screenManager.SpriteBatch.Draw(wheelSprite.Texture, ConvertUnits.ToDisplayUnits(wheelBody.Position),
                                           null,
                                           Color.White * 0.5f, wheelBody.Rotation, wheelSprite.Origin, 2.0f * radius / wheelSprite.Texture.Bounds.Height,
                                           SpriteEffects.None, 0f);
            screenManager.SpriteBatch.Draw(notebook.Texture, ConvertUnits.ToDisplayUnits(new Vector2(300,100)),
                                          null,
                                          Color.White, 0f, notebook.Origin, 1.4f * radius / notebook.Texture.Bounds.Height,
                                          SpriteEffects.None, 0f);
           
            screenManager.SpriteBatch.DrawString(font, entries[selectedEntryIndex], ConvertUnits.ToDisplayUnits(new Vector2(250, 47)), Color.Black);
            
        }

        public void updateTracker(Vector2 position)
        {
            if (fixedMouseJoint != null)
            {
                fixedMouseJoint.WorldAnchorB = position;
            }
        }

        public void setLock(Vector2 position)
        {
            if (idleJoint != null)
            {
                world.RemoveJoint(idleJoint);
                idleJoint = null;
            }
            if (fixedMouseJoint == null)
            {
                Fixture savedFixture = world.TestPoint(position);
                if (savedFixture != null)
                {
                    Body body = savedFixture.Body;
                    fixedMouseJoint = new FixedMouseJoint(body, position);
                    fixedMouseJoint.MaxForce = 4000.0f * body.Mass;
                    world.AddJoint(fixedMouseJoint);
                    body.Awake = true;
                }
            }  
        }

        public void clearLock()
        {
            if(fixedMouseJoint != null)
            {
                world.RemoveJoint(fixedMouseJoint);
                fixedMouseJoint = null;
            }

            if (idleJoint != null)
            {
                return;
            }

            selectedEntryIndex = (int)Math.Round(entries.Count * normalizeOrientation(wheelBody.Rotation) / (2 * Math.PI), 0, MidpointRounding.AwayFromZero) % entries.Count;
            selectedEntryPosition = getSeletedEntryPosition();
            Fixture savedFixture = world.TestPoint(selectedEntryPosition);
            if (savedFixture != null)
            {
                Body body = savedFixture.Body;
                idleJoint = new FixedMouseJoint(body, new Vector2(radius, 0));
                
                idleJoint.DampingRatio = 0f;
                idleJoint.MaxForce = 3000.0f * body.Mass;
                world.AddJoint(idleJoint);
                body.Awake = true;
                idleJoint.WorldAnchorB = selectedEntryPosition;
            }
            
        }

        /// <summary>
        /// returns angle in range [0 - 2PI>
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private double normalizeOrientation(double angle)
        {
            return 2* Math.PI - angle % (2 * Math.PI) + (angle < 0 ? 2 * Math.PI : 0);
        }

        double selectedEntryAngle = 0;

        private Vector2 getSeletedEntryPosition()
        {
            selectedEntryAngle = normalizeOrientation(wheelBody.Rotation + (selectedEntryIndex - entries.Count) * 2 * Math.PI / entries.Count);

            return new Vector2((int)(Math.Cos(selectedEntryAngle) * radius), (int)(Math.Sin(selectedEntryAngle) * radius));
        }
    }
}
