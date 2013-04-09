using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XnaTest.Character.Characters
{
    /// <summary>
    /// Abstract sprite sheet class whose update is based on plank angle.
    /// Implementation of each character sprite must handle frame calculation.
    /// Should be returning (absolute) closest-lower available angle-frame.
    /// Angles are presented in radians.
    /// 
    /// </summary>
    interface ICharacterSprite
    {
        void Update(float angle);

        void Draw(SpriteBatch spriteBatch, Vector2 location);
    }
}
