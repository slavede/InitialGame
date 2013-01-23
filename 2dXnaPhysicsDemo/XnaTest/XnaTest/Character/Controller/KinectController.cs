using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace XnaTest.Character.Controller
{
    public class KinectController : CharacterController
    {
        private float x;
        private float yLeftArm;
        private float yRightArm;

        private float yChangeFactor = 0.5f;
        private float xChangeFactor = 2f;

        public KinectController(float x, float yLeftArm, float yRightArm)
        {
            this.x = x;
            this.yLeftArm = yLeftArm;
            this.yRightArm = yRightArm;
        }

        public void HandleInput(GameTime gameTime, Joint leftHandJointPosition, Joint rightHandJointPosition, Joint headJoint, Joint centerShoulderJoint, Vector2 resolution)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            yLeftArm = (((-0.5f * leftHandJointPosition.Position.Y) + 0.5f) * (resolution.Y)) - resolution.Y / 2;
            yRightArm = (((-0.5f * rightHandJointPosition.Position.Y) + 0.5f) * (resolution.Y)) - resolution.Y / 2;


            x += ((headJoint.Position.X - centerShoulderJoint.Position.X) * time * xChangeFactor);
        }

        public float getX()
        {
            return this.x;
        }

        public float getDeltaY()
        {
            return (yLeftArm - yRightArm) * yChangeFactor;
        }
    }
}

