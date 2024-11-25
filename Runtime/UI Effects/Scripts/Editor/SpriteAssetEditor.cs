using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;

public class SpriteAssetEditor : Editor
{
	[MenuItem("CONTEXT/SpriteAsset/Refresh Sprite Names")]
	private static void RefreshSpriteNames(MenuCommand menuCommand)
	{
		SpriteAsset spriteAsset = (SpriteAsset)menuCommand.context;
		if (spriteAsset != null)
		{
			for (var index = 0; index < spriteAsset.spriteCharacterTable.Count; index++)
			{
				var spriteCharacter = spriteAsset.spriteCharacterTable[index];
				var spriteGlyph = spriteAsset.spriteGlyphTable[(int)spriteCharacter.glyphIndex];
				if (spriteCharacter != null && spriteCharacter.name != spriteGlyph.sprite.name)
				{
					spriteCharacter.name = spriteGlyph.sprite.name;
				}
			}

			EditorUtility.SetDirty(spriteAsset);
			AssetDatabase.SaveAssets();
			Debug.Log("Sprite names refreshed successfully!");
		}
	}
}