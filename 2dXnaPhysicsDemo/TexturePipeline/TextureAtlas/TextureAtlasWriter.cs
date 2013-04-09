using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TexturePipeline.TextureAtlas
{
	[ContentTypeWriter]
	public class TextureAtlasWriter : ContentTypeWriter<TextureAtlasContent>
	{
		protected override void Write(ContentWriter output, TextureAtlasContent value)
		{
			output.WriteObject(value.Regions);
			// todo: do it
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
            return "XnaTest.Texture.TextureAtlasReader, XnaTest";
		}
	}
}
