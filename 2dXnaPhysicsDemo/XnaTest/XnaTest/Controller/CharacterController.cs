using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaTest.Controller
{
    interface CharacterController
    {
        Vector2 getLeftHandPosition();
        Vector2 getRightHandPosition();
        void HandleInput(GameTime gameTime);
    }
}
