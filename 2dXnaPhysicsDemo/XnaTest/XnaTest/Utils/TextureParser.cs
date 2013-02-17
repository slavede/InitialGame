using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.SamplesFramework;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common.Decomposition;

namespace XnaTest.Utils
{
    public static class TextureParser
    {
        // parses picture and creates polygon based on the colors
        public static List<Vertices> ImageToPolygonBody(Texture2D texture, float scale_, ref Vector2 polygonOrigin)
        {
            //Create an array to hold the data from the texture
            uint[] data = new uint[(texture.Width) * (texture.Height)];

            //Collect data from bitmap
            texture.GetData(data);

            //Create Polygon from Bitmap
            Vertices verts = PolygonTools.CreatePolygon(data, (texture.Width), true);

            //Make sure that the origin of the texture is the centroid (real center of geometry)
            Vector2 scale = new Vector2(ConvertUnits.ToSimUnits(scale_), ConvertUnits.ToSimUnits(scale_));
            verts.Scale(ref scale);

            //Make sure that the origin of the texture is the centroid (real center of geometry)
            polygonOrigin = verts.GetCentroid();

            //Translate the polygon so that it aligns properly with centroid.
            Vector2 vertsTranslate = -polygonOrigin;
            verts.Translate(ref vertsTranslate);

            //We simplify the vertices found in the texture.
            verts = SimplifyTools.ReduceByDistance(verts, 4f);

            //Decompose polygon into smaller chuncks that Farseer can process better
            List<Vertices> list;
            list = BayazitDecomposer.ConvexPartition(verts);

            return list;
        }

        // parses picture and creates vertices based on the colors
        public static Vertices GetVertices(Texture2D polygonTexture)
        {
            //Create an array to hold the data from the texture
            uint[] data = new uint[polygonTexture.Width * polygonTexture.Height];

            //Transfer the texture data to the array
            polygonTexture.GetData(data);

            //Find the vertices that makes up the outline of the shape in the texture
            Vertices verts = PolygonTools.CreatePolygon(data, polygonTexture.Width, true);// .CreatePolygon(data, polygonTexture.Width, polygonTexture.Height, true);

            //Vector2 scale = new Vector2(0.07f, 0.07f);
            //verts.Scale(ref scale);

            return verts;
        }

    }
}
