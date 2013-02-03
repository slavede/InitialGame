using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics;
using XnaTest.Character.Controller;
using Microsoft.Xna.Framework.Graphics;
using XnaTest.Character.Characters;

namespace XnaTest.Character
{
    class Player
    {
        public const float plankHeightPosition = 100f;
        public const int plankLength = 150;

        public Player(CharacterSprite characterSprite)
        {
            centralPlankPosition = new Vector();
            leftPlankPosition = new Vector();
            rightPlankPosition = new Vector();
            this.characterSprite = characterSprite;
        }

        public CharacterSprite characterSprite { get; set; }

        public Body plankBody { get; set; }

        public CharacterController inputPosition { get; set; }

        public Vector centralPlankPosition { get; set; }

        public Vector leftPlankPosition { get; set; }

        public Vector rightPlankPosition { get; set; }

        public FixedMouseJoint fixedMouseJointL { get; set; }

        public FixedMouseJoint fixedMouseJointC { get; set; }

        public FixedMouseJoint fixedMouseJointR { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            characterSprite.Draw(spriteBatch, plankBody.Position); 
        }

        public void updatePlankPositionVectors()
        {
            //TODO get constants from plank body
            centralPlankPosition.X = inputPosition.getX();
            centralPlankPosition.Y = plankHeightPosition;
            leftPlankPosition.X = inputPosition.getX() - plankLength / 2;
            leftPlankPosition.Y = plankHeightPosition + inputPosition.getDeltaY();
            rightPlankPosition.X = inputPosition.getX() + plankLength / 2;
            rightPlankPosition.Y = plankHeightPosition - inputPosition.getDeltaY();
        }

        private void updateBodyFixedJoints()
        {
            fixedMouseJointL.WorldAnchorB = leftPlankPosition.convertToVector2();
            fixedMouseJointC.WorldAnchorB = centralPlankPosition.convertToVector2();
            fixedMouseJointR.WorldAnchorB = rightPlankPosition.convertToVector2();
        }

        public void update()
        {
            updatePlankPositionVectors();
            updateBodyFixedJoints();
            characterSprite.Update(plankBody.Rotation);
        }
    }
}
