using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace XnaTest.Controller
{
    public class KinectController : CharacterController
    {
        private float x;
        private float yLeftArm;
        private float yRightArm;

        public KinectController(float x, float yLeftArm, float yRightArm)
        {
            this.x = x;
            this.yLeftArm = yLeftArm;
            this.yRightArm = yRightArm;
        }

        public void UpdatePositions(Joint leftHandJointPosition, Joint rightHandJointPosition, Joint headJoint, Joint centerShoulderJoint, Vector2 resolution)
        {

            yLeftArm = (((-0.5f * leftHandJointPosition.Position.Y) + 0.5f) * (resolution.Y)) - resolution.Y / 2;
            yRightArm = (((-0.5f * rightHandJointPosition.Position.Y) + 0.5f) * (resolution.Y)) - resolution.Y / 2;

            x += (((-0.5f * centerShoulderJoint.Position.X) + 0.5f) * (resolution.X)) - (((-0.5f * headJoint.Position.X) + 0.5f) * (resolution.X));
        }

        public float getX()
        {
            return this.x;
        }

        public float getDeltaY()
        {
            return yLeftArm - yRightArm;
        }

        public void HandleInput(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}

