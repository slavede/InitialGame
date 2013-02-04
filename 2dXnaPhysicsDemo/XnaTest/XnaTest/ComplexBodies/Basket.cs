using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using XnaTest.Utils;
using FarseerPhysics.Common;
using FarseerPhysics.SamplesFramework;

namespace XnaTest.ComplexBodies
{
    public class Basket
    {
        public Texture2D basketTexture {get; set;}
        public Body basketBody { get; set; }

        public Texture2D basketCoverTexture { get; set; }
        public Body basketCoverBody {get; set;}

        private int publicColissionId;

        public Basket(Vector2 position, Texture2D basketTexture, Texture2D basketCoverTexture, World world, Category collidesWith, Category myCategory)
        {
            Vector2 basketPolygon = new Vector2(basketTexture.Width / 2, basketTexture.Height / 2);
            List<Vertices> list = TextureParser.ImageToPolygonBody(basketTexture, 1, ref basketPolygon);
            basketBody = BodyFactory.CreateCompoundPolygon(world, list, 1f);
            basketBody.Mass = 0;
            basketBody.Position = position;
            basketBody.CollidesWith = collidesWith;
            basketBody.CollisionCategories = myCategory;
            basketBody.BodyType = BodyType.Static;



            Vector2 basketCoverPolygon = new Vector2(basketCoverTexture.Width / 2, basketCoverTexture.Height / 2);
            basketCoverBody = BodyFactory.CreateRectangle(world, basketCoverTexture.Width, basketCoverTexture.Height, 1);
            basketCoverBody.Mass = 0;
            basketCoverBody.Position = new Vector2(basketBody.Position.X, basketBody.Position.Y - basketTexture.Height / 2);
            basketCoverBody.BodyType = BodyType.Static;
            basketCoverBody.CollisionCategories = Category.Cat5;
            basketCoverBody.CollidesWith = Category.Cat1;
        }
    }
}
