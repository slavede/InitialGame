using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;

namespace XnaTest.Character.Controller
{
    interface CharacterController
    {
        float getX();

        /**
         * if 0, plank will be paralel with the ground
         * 
         */
        float getDeltaY();

        void HandleInput(GameTime gameTime, Joint leftHandJointPosition, Joint rightHandJointPosition, Joint headJoint, Joint centerShoulderJoint, Vector2 resolution);
    }
}
