using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset.Editor;

// [DisplayName("打包特效纹理（自定义）")]
// public class PackEffectTexture : IPackRule
// {
// 	private const string PackDirectory = "Assets/Effect/Textures/";
//
// 	PackRuleResult IPackRule.GetPackRuleResult(PackRuleData data)
// 	{
// 		string assetPath = data.AssetPath;
// 		if (assetPath.StartsWith(PackDirectory) == false)
// 			throw new Exception($"Only support folder : {PackDirectory}");
// 	
// 		string assetName = Path.GetFileName(assetPath).ToLower();
// 		string firstChar = assetName.Substring(0, 1);
// 		string bundleName = $"{PackDirectory}effect_texture_{firstChar}";
// 		var packRuleResult = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
// 		return packRuleResult;
// 	}
//
// 	bool IPackRule.IsRawFilePackRule()
// 	{
// 		return false;
// 	}
// }
//

using System;
using System.IO;
using UnityEditor;

namespace YooAsset.Editor
{

	//自定义扩展范例
	public class CollectScene : IFilterRule
	{
		public bool IsCollectAsset(FilterRuleData data)
		{
			return Path.GetExtension(data.AssetPath) == ".unity";
		}
	}
	
	
	[DisplayName("打包视频（自定义）")]
	public class JQPackVideo : IPackRule
	{
		public const string Mp4FileExtension = "mp4";
		PackRuleResult IPackRule.GetPackRuleResult(PackRuleData data)
		{
			string bundleName = data.AssetPath;
			PackRuleResult result = new PackRuleResult(bundleName, Mp4FileExtension);
			return result;
		}

		bool IPackRule.IsRawFilePackRule()
		{
			return true;
		}
	}
}