using System.Collections.Generic;

namespace XnaTest.Texture
{
	public class TextureAtlas
	{
		internal TextureAtlas(Dictionary<string, TextureRegion> regions)
		{
			Regions = regions;
		}

		public bool ContainsTexture(string textureName)
		{
			return Regions.ContainsKey(textureName);
		}
		
		public TextureRegion GetRegion(string textureName)
		{
			TextureRegion region;
			
			if(Regions.TryGetValue(textureName, out region))
				return region;
			return null;
		}

        public int GetNumberOfFrames()
        {
            return Regions.Count;
        }

		Dictionary<string, TextureRegion> Regions { get; set; }
	}
}
