using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaTest.Character
{
    class Vector
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2 convertToVector2()
        {
            return new Vector2(X, Y);
        }
    }
}
