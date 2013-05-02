using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectButton
{
    public static class ExtensionMethods
    {
        public static Joint ScaleToScreen(this Joint joint)
        {
            return Scale(joint, 1200, 800);
        }

        public static Joint Scale(this Joint joint, int width, int height)
        {
            SkeletonPoint skeletonPoint = new SkeletonPoint()
            {
                X = Scale(joint.Position.X, width),
                Y = Scale(-joint.Position.Y, height),
                Z = joint.Position.Z
            };

            Joint scaledJoint = new Joint()
            {
                TrackingState = joint.TrackingState,
                Position = skeletonPoint
            };

            return scaledJoint;
        }

        public static float Scale(float value, int max)
        {
            return (max >> 1) + (value * (max >> 1));
        }
    }
}
