using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaTest.Controller
{
    interface CharacterController
    {
        float getX();

        /**
         * if 0, plank will be paralel with the ground
         * 
         */
        float getDeltaY();

        void HandleInput(GameTime gameTime);
    }
}
