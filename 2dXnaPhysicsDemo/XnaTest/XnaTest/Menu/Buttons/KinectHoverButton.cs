using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KinectButton;
using Microsoft.Kinect;

namespace XnaTest.Menu.Buttons
{
    class KinectHoverButton : AbstractHoverButton
    {
        protected JointType clientJoint;
        public JointType ClientJoint
        {
            get { return clientJoint; }
            set { clientJoint = value; }
        }

        public KinectHoverButton(Texture2D buttonImage, Vector2 position, SelectAnimation selectAnimation)
            : base(buttonImage, position, 1f, selectAnimation)
        {
        }

        public virtual void Update(GameTime gameTime, Skeleton skeleton)
        {
             if (skeleton == null)
                return;
            base.Update(gameTime, new Vector2(skeleton.Joints[clientJoint].ScaleToScreen().Position.X, skeleton.Joints[clientJoint].ScaleToScreen().Position.Y));
        }
       
    }
}
