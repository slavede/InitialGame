using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics;
using XnaTest.Character.Controller;

namespace XnaTest.Character
{
    class Player
    {
        public Player()
        {
            centralPlankPosition = new Vector();
            leftPlankPosition = new Vector();
            rightPlankPosition = new Vector();
        }

        public Body plankBody { get; set; }

        public CharacterController inputPosition { get; set; }

        public Vector centralPlankPosition { get; set; }

        public Vector leftPlankPosition { get; set; }

        public Vector rightPlankPosition { get; set; }

        public FixedMouseJoint fixedMouseJointL { get; set; }

        public FixedMouseJoint fixedMouseJointC { get; set; }

        public FixedMouseJoint fixedMouseJointR { get; set; }

    }
}
