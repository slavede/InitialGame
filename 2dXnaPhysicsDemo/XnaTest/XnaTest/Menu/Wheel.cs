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
        World world;

        public Wheel(ScreenManager screenManager, World world, float radius, Vector2 centerPosition, int[] entries)
        {
            this.world = world;
            this.screenManager = screenManager;
            wheelBody = BodyFactory.CreateCircle(world, radius, 1f);
            wheelBody.BodyType = BodyType.Dynamic;

            FixedRevoluteJoint rollingJoint = JointFactory.CreateFixedRevoluteJoint(world, wheelBody, Vector2.Zero, Vector2.Zero);
            wheelSprite = new Sprite(screenManager.Assets.TextureFromShape(wheelBody.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.Orange, 1f));
        }

        public int GetSelectedEntryIndex()
        {
            return 0;
        }

        /// <summary>
        /// sprite batch will be open and closed afterwards.
        /// DO NOT open or close sprite batch
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw()
        {
            screenManager.SpriteBatch.Draw(wheelSprite.Texture, ConvertUnits.ToDisplayUnits(wheelBody.Position),
                                           null,
                                           Color.White, wheelBody.Rotation, wheelSprite.Origin, 1f,
                                           SpriteEffects.None, 0f);
        }

        public void setLock(Vector2 position)
        {
            if (fixedMouseJoint == null)
            {
                Fixture savedFixture = world.TestPoint(position);
                if (savedFixture != null)
                {
                    Body body = savedFixture.Body;
                    fixedMouseJoint = new FixedMouseJoint(body, position);
                    fixedMouseJoint.MaxForce = 1000.0f * body.Mass;
                    world.AddJoint(fixedMouseJoint);
                    body.Awake = true;
                }
            }  
        }

        public void updateTracker(Vector2 position)
        {
            if (fixedMouseJoint != null)
            {
                fixedMouseJoint.WorldAnchorB = position;
            }
        }

        public void clearLock()
        {
            if (fixedMouseJoint != null)
            {
                world.RemoveJoint(fixedMouseJoint);
                fixedMouseJoint = null;
            }
        }

    }
}
