using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TexturePipeline.TextureAtlas
{
    [ContentSerializerRuntimeType("XnaTest.Texture.TextureRegion, XnaTest")]
	public class TextureRegionContent
	{
		public Rectangle Bounds { get; set; }
		public Vector2 OriginTopLeft { get; set; }
		public Vector2 OriginCenter { get; set; }
		public Vector2 OriginBottomRight { get; set; }
		public bool Rotated { get; set; }
	}
}
