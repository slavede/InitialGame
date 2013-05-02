using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.SamplesFramework;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Kinect.Toolkit.Controls;
using System.Runtime.InteropServices;

namespace XnaTest.Menu
{
    class KinectGrabWheelController : WheelController
    {
        UserInfo[] userInfo;
        Vector2 position;
        Vector2 positionScreen;
 
        WheelDelegate wheelDelegate;
        GraphicsDevice device;

        KinectSensor kinectSensor;
        InteractionStream interactionStream;

        private class myIntClient : Microsoft.Kinect.Toolkit.Interaction.IInteractionClient
        {
            public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
            {
                return new InteractionInfo();
            }
        }

        public KinectGrabWheelController(WheelDelegate wheelDelegate, ScreenManager screenManager, KinectSensor kinect)
        {
            this.device = screenManager.GraphicsDevice;

            this.wheelDelegate = wheelDelegate;
            position = new Vector2();
            positionScreen = new Vector2();
            this.kinectSensor = kinect;

            //if (!kinect.IsRunning)
            //{
                var parameters = new TransformSmoothParameters
                {
                    Smoothing = 0.1f,
                    Correction = 0.0f,
                    Prediction = 0.0f,
                    JitterRadius = 1.0f,
                    MaxDeviationRadius = 0.5f
                };
                kinect.SkeletonStream.Enable(parameters);
                kinect.DepthStream.Enable();
                kinect.ColorStream.Enable();

                kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(ks_AllFramesReady);

                interactionStream = new Microsoft.Kinect.Toolkit.Interaction.InteractionStream(kinect, new myIntClient());
                interactionStream.InteractionFrameReady += new EventHandler<InteractionFrameReadyEventArgs>(intStream_InteractionFrameReady);
                kinect.Start();
            //}
        }

        void intStream_InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            using (InteractionFrame iFrame = e.OpenInteractionFrame())
            {
                if (iFrame != null)
                {
                    if (userInfo == null)
                    {
                        this.userInfo = new UserInfo[6];
                    }
                    iFrame.CopyInteractionDataTo(this.userInfo);
                }
            }       
        }

        void ks_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            short[] depthPix;
            using (DepthImageFrame dif = e.OpenDepthImageFrame())
            {
                if (dif == null)
                {
                    return;
                }

                depthPix = new short[dif.PixelDataLength];
                dif.CopyPixelDataTo(depthPix);

                interactionStream.ProcessDepth(dif.GetRawPixelData(), dif.Timestamp);

            }

            Skeleton[] skeletons;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    interactionStream.ProcessSkeleton(skeletons, kinectSensor.AccelerometerGetCurrentReading(), skeletonFrame.Timestamp);
                }
            }
        }

        public Boolean isGrabPerformed()
        {
            return grabPerformed;
        }

        Boolean grabPerformed = false;

        public void HandleInput(GameTime gameTime)
        {
            if (userInfo == null)
                return;

            List<UserInfo> curUsers = userInfo.Where(x => x.SkeletonTrackingId > 0).ToList<UserInfo>();

            if (curUsers.Count > 0)
            {
                UserInfo curUser = curUsers[0];

                position.X = (float)curUser.HandPointers[1].X;
                position.Y = (float)curUser.HandPointers[1].Y;
               //Console.Out.WriteLine(position);
                positionScreen.X = position.X * device.Viewport.Width / 2;
                positionScreen.Y = position.Y * device.Viewport.Height / 2;
                if (curUser.HandPointers[1].HandEventType == InteractionHandEventType.Grip)
                {
                    grabPerformed = true;
                    wheelDelegate.setLock(positionScreen);
                }
                if (curUser.HandPointers[1].HandEventType == InteractionHandEventType.GripRelease)
                {
                    grabPerformed = false;
                    wheelDelegate.clearLock();
                }
            }
            wheelDelegate.updateTracker(gameTime, positionScreen);

        }

        public void doGrabCheck() //just warning to implement this behaviour
        {
        }

        public Vector2 getPosition()
        {
            return positionScreen;
        }

    }
}
