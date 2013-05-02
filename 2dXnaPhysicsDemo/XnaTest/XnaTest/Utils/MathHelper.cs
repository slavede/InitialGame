using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaTest.Utils
{
    public class MathHelperMethods
    {
        public static double DistanceBetweenTwoVector2(Vector2 vector1, Vector2 vector2)
        {
            float xDistance = vector2.X - vector1.X;
            float yDistance = vector2.Y - vector1.Y;

            return Math.Sqrt(xDistance*xDistance + yDistance*yDistance);
        }
    }
}
