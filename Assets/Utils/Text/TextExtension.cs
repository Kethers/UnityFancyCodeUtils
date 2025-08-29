using TMPro;
using UnityEngine;

namespace MetaFramework.Tools
{
	public static partial class TextExtension
	{
		public static void SafeSetText(this TMP_Text tmpText, string text, bool showLog = true)
		{
			if (tmpText == null)
			{
				if (showLog)
				{
					Debug.LogError($"TMP_Text NULL ERROR from SafeSetText");
				}
				return;
			}
        
			tmpText.text = text;
		}
	}
}