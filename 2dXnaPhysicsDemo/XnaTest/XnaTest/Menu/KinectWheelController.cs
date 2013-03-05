using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.SamplesFramework;

namespace XnaTest.Menu
{
    class KinectWheelController : WheelController
    {
        Skeleton[] skeletonData;
        Skeleton skeleton;
        Vector2 position;
        Vector2 centerPosition;
        WheelDelegate wheelDelegate;
        GraphicsDevice device;
        Camera2D camera;
        int radius;
        
        public KinectWheelController(WheelDelegate wheelDelegate, GraphicsDevice device, KinectSensor kinect, int activeSkeletonIndex, int radius, Vector2 centerPosition)
        {
            this.device = device;
            this.radius = radius;
            this.centerPosition = centerPosition;
            this.wheelDelegate = wheelDelegate;
            position = new Vector2();
            camera = new Camera2D(device);

            if (!kinect.IsRunning)
            {
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSkeletonFrameReady);
                kinect.Start();
            }
            else
            {
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSkeletonFrameReady);
            }
            if(activeSkeletonIndex>=0){
                skeleton = skeletonData[activeSkeletonIndex];
            }
        }

        public void HandleInput(GameTime gameTime)
        {
            if (skeleton == null || skeleton.TrackingState != SkeletonTrackingState.Tracked)
            {
                if (skeletonData != null)
                {
                    for (int i = 0; i < skeletonData.Length; i++)
                    {
                        if (skeletonData[i].TrackingState == SkeletonTrackingState.Tracked)
                        {
                            skeleton = skeletonData[i];
                            break;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            if (skeleton == null || skeleton.TrackingState != SkeletonTrackingState.Tracked)
            {
                return;
            }
            position.X = skeleton.Joints[Microsoft.Kinect.JointType.HandRight].Position.X * device.Viewport.Width;
            position.Y = -skeleton.Joints[Microsoft.Kinect.JointType.HandRight].Position.Y * device.Viewport.Height;

            doGrabCheck();
            wheelDelegate.updateTracker(position);

        }

        public void doGrabCheck() //just warning to implement this behaviour
        {
            if (position.X < (radius + centerPosition.X) && position.Y < (radius + centerPosition.Y))
            {
                wheelDelegate.setLock(position);
            }
            else
            {
                wheelDelegate.clearLock();
            }
        }

        public Vector2 getPosition()
        {
            return position;
        }

        void kinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    //Copy the skeleton data to our array
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                }
            }
        }
    }
}
