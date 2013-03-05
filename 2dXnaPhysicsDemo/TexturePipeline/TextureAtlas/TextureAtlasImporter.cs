using Microsoft.Xna.Framework.Content.Pipeline;

namespace TexturePipeline.TextureAtlas
{
	[ContentImporter(".json", ".xml", DisplayName = "Texture Atlas Importer", DefaultProcessor = "TextureAtlasProcessor")]
	public class TextureAtlasImporter : ContentImporter<TextureAtlasContent>
	{
		public override TextureAtlasContent Import(string fileName, ContentImporterContext context)
		{
			return new TextureAtlasContent(fileName);
		}
	}
}
