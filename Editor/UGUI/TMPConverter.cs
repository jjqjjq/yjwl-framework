#if TextMeshPro
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// TMP转Text静态工具类
/// </summary>
public static class TMPConverter
{
    /// <summary>
    /// 将TMP组件转换为Text组件
    /// </summary>
    /// <param name="tmpComponent">要转换的TMP组件</param>
    /// <returns>转换后的Text组件</returns>
    public static Text ToText(this TMP_Text tmpComponent, Font defaultFont)
    {
        if (tmpComponent == null)
        {
            Debug.LogError("TMP组件为空，无法转换");
            return null;
        }

        GameObject gameObject = tmpComponent.gameObject;
        
        // 在移除组件前先保存所有需要的属性
        string text = tmpComponent.text;
        float fontSize = tmpComponent.fontSize;
        FontStyles fontStyle = tmpComponent.fontStyle;
        TextAlignmentOptions alignment = tmpComponent.alignment;
        Color color = tmpComponent.color;
        bool raycastTarget = tmpComponent.raycastTarget;
        TMP_FontAsset fontAsset = tmpComponent.font;
        
        // 保存RectTransform属性
        RectTransform tmpRectTransform = tmpComponent.GetComponent<RectTransform>();
        Vector2 anchoredPosition = Vector2.zero;
        Vector2 sizeDelta = Vector2.zero;
        Vector2 anchorMin = Vector2.zero;
        Vector2 anchorMax = Vector2.zero;
        Vector2 pivot = Vector2.zero;
        
        if (tmpRectTransform != null)
        {
            anchoredPosition = tmpRectTransform.anchoredPosition;
            sizeDelta = tmpRectTransform.sizeDelta + new Vector2(0, 15);
            anchorMin = tmpRectTransform.anchorMin;
            anchorMax = tmpRectTransform.anchorMax;
            pivot = tmpRectTransform.pivot;
        }
        
        // 移除TMP组件
        Object.DestroyImmediate(tmpComponent);
        
        // 获取或创建Text组件
        Text textComponent = gameObject.GetComponent<Text>();
        if (textComponent == null)
        {
            textComponent = gameObject.AddComponent<Text>();
        }

        // 使用保存的属性值设置Text组件
        textComponent.text = text;
        textComponent.fontSize = Mathf.RoundToInt(fontSize);
        textComponent.fontStyle = ConvertFontStyle(fontStyle);
        textComponent.alignment = ConvertAlignment(alignment);
        textComponent.color = color;
        textComponent.raycastTarget = raycastTarget;
        textComponent.font = defaultFont;
        
        // 设置RectTransform属性
        RectTransform textRectTransform = textComponent.GetComponent<RectTransform>();
        if (textRectTransform != null)
        {
            textRectTransform.anchoredPosition = anchoredPosition;
            textRectTransform.sizeDelta = sizeDelta;
            textRectTransform.anchorMin = anchorMin;
            textRectTransform.anchorMax = anchorMax;
            textRectTransform.pivot = pivot;
        }

        return textComponent;
    }

    /// <summary>
    /// 转换字体样式
    /// </summary>
    private static FontStyle ConvertFontStyle(FontStyles tmpFontStyle)
    {
        return tmpFontStyle switch
        {
            FontStyles.Normal => FontStyle.Normal,
            FontStyles.Bold => FontStyle.Bold,
            FontStyles.Italic => FontStyle.Italic,
            FontStyles.Bold | FontStyles.Italic => FontStyle.BoldAndItalic,
            _ => FontStyle.Normal
        };
    }

    /// <summary>
    /// 转换对齐方式
    /// </summary>
    private static TextAnchor ConvertAlignment(TextAlignmentOptions tmpAlignment)
    {
        return tmpAlignment switch
        {
            TextAlignmentOptions.TopLeft => TextAnchor.UpperLeft,
            TextAlignmentOptions.Top => TextAnchor.UpperCenter,
            TextAlignmentOptions.TopRight => TextAnchor.UpperRight,
            TextAlignmentOptions.Left => TextAnchor.MiddleLeft,
            TextAlignmentOptions.Center => TextAnchor.MiddleCenter,
            TextAlignmentOptions.Right => TextAnchor.MiddleRight,
            TextAlignmentOptions.BottomLeft => TextAnchor.LowerLeft,
            TextAlignmentOptions.Bottom => TextAnchor.LowerCenter,
            TextAlignmentOptions.BottomRight => TextAnchor.LowerRight,
            _ => TextAnchor.MiddleCenter
        };
    }

    /// <summary>
    /// 获取普通字体
    /// </summary>
    private static Font GetRegularFont(TMP_FontAsset tmpFontAsset)
    {
        if (tmpFontAsset == null) return null;

        // 尝试获取源字体
        Font sourceFont = tmpFontAsset.sourceFontFile;
        if (sourceFont != null)
        {
            return sourceFont;
        }

        // 使用默认字体
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }


    /// <summary>
    /// 检查对象是否有TMP组件
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    /// <returns>是否有TMP组件</returns>
    public static bool HasTMPComponent(GameObject gameObject)
    {
        return gameObject != null && gameObject.GetComponent<TMP_Text>() != null;
    }

    /// <summary>
    /// 获取对象上的TMP组件数量
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    /// <returns>TMP组件数量</returns>
    public static int GetTMPComponentCount(GameObject gameObject)
    {
        if (gameObject == null) return 0;
        
        TMP_Text[] tmpComponents = gameObject.GetComponentsInChildren<TMP_Text>();
        return tmpComponents.Length;
    }
}
#endif