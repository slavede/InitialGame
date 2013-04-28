using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;
using FarseerPhysics.SamplesFramework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaTest.Menu
{
    class MouseWheelController : WheelController
    {
        WheelDelegate wheelDelegate;
        MouseState mouseState;
        Vector2 position;
        GraphicsDevice device;
        Camera2D camera;

        public MouseWheelController(WheelDelegate wheelDelegate, GraphicsDevice device)
        {
            this.device = device;
            this.wheelDelegate = wheelDelegate;
            mouseState = new MouseState();
            position = new Vector2();
            camera = new Camera2D(device);
        }

        public Boolean isGrabPerformed()
        {
            return mouseState.LeftButton == ButtonState.Pressed;
        }

        public void HandleInput(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            position.X = mouseState.X;
            position.Y = mouseState.Y;

            position = camera.ConvertScreenToWorld(position);
            doGrabCheck();

            wheelDelegate.updateTracker(position);
        }

        public void doGrabCheck()
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                wheelDelegate.setLock(position);
            }
            if (mouseState.LeftButton == ButtonState.Released)
            {
                wheelDelegate.clearLock();
            }
           
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public void Draw() { }
    }
}
