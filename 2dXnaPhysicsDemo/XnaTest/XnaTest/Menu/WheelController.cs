using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;

namespace XnaTest.Menu
{
    /// <summary>
    /// for kinect implementation, try to give skeleton on instatiation; no multiplayer needed
    /// </summary>
    public interface WheelController
    {

        void HandleInput(GameTime gameTime);

        void doGrabCheck(); //just warning to implement this behaviour

        Vector2 getPosition();
    }
}
