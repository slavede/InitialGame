using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaTest.Menu
{
   public interface WheelDelegate
    {
        void setLock(Vector2 position);

        void clearLock();

        //public void rotateToEntry(int entryIndex); //will be used for swiping delegate if that's the way :/

        void updateTracker(GameTime gameTime, Vector2 position);

    }
}
