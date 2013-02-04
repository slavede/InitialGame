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
        private int result;

        public Player(CharacterSprite characterSprite)
        {
            centralPlankPosition = new Vector();
            leftPlankPosition = new Vector();
            rightPlankPosition = new Vector();
            this.characterSprite = characterSprite;
            this.result = 0;
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

        public int basketId { get; set; }

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

        public void addPoints(int points)
        {
            result += points;
        }

        public int getPoints()
        {
            return result;
        }
    }
}
