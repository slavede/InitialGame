using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizbin.Kinect.Gestures;
using Microsoft.Kinect;

namespace XnaTest.CustomHandGestures
{
    /// <summary>
    /// Class is built upon on JoinedHandsSegment, but it allows hands to be joined anywhere (mentioned class requires that hands must be below hips and shoulders)
    /// </summary>
    public class JoinedHandsAnywhere : IRelativeGestureSegment
    {
                /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // Hands very close
            if (skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.HandLeft].Position.X < 0)
            {
                return GesturePartResult.Succeed;
            }

            return GesturePartResult.Fail;
        
        }
    }
}
