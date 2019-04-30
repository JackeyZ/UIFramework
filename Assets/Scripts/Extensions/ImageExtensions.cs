using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

/// <summary>
/// The extensions for <see cref="Image"/>.
/// </summary>
public static class ImageExtensions
{
    /// <summary>
    /// Load the sprite into this image.
    /// </summary>
    public static void LoadSprite(this Image image, string bundleName, string spriteName, bool isCache = true, Action complete = null)
    {
#if UNITY_EDITOR
        // 是否设置了使用assetbundle资源
        if (AssetBundleFramework.DeveloperSetting.GetUseAssetBundleAsset())
        {
            LoadSpriteByAssetBundle(image, bundleName, spriteName, isCache = true, complete = null);
        }
        else
        {
            image.sprite = AssetBundleFramework.AssetLoadInEditor.LoadObject<Sprite>(bundleName, spriteName);
            if (complete != null)
            {
                complete();
            }
        }
#else
        LoadSpriteByAssetBundle(image, bundleName, spriteName, isCache = true, complete = null);
#endif
    }

    /// <summary>
    /// Load the sprite into this image.
    /// </summary>
    public static void LoadSprite(this Image image,AssetBundleFramework.ABAsset abAsset, bool isCache = true, Action complete = null)
    {
        LoadSprite(image, abAsset.ABPath, abAsset.AssetName, isCache, complete);
    }

    /// <summary>
    /// 从assetbundle中加载sprite
    /// </summary>
    /// <param name="image"></param>
    /// <param name="bundleName"></param>
    /// <param name="spriteName"></param>
    /// <param name="isCache"></param>
    /// <param name="complete"></param>
    public static void LoadSpriteByAssetBundle(this Image image, string bundleName, string spriteName, bool isCache = true, Action complete = null)
    {
        image.enabled = false;
        AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAsset(bundleName, spriteName, (obj) => {
            if (image != null && obj != null)
            {
                //如果是贴图
                Texture2D tex = obj as Texture2D;
                if (tex != null)
                {
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    image.sprite = sprite as Sprite;
                    image.enabled = true;
                    if (complete != null)
                    {
                        complete();
                    }
                    return;
                }
                //没有加载到贴图则加载图集
                AssetBundleFramework.AssetBundleMgr.Instance.LoadBundleAsset(bundleName, AssetBundleFramework.AssetBundleDefined.SPRITE_ATLAS_NAME, (atlasObj) =>
                {
                    SpriteAtlas atlas = atlasObj as SpriteAtlas;
                    if (atlas != null)
                    {
                        image.sprite = atlas.GetSprite(spriteName);
                        image.enabled = true;
                        if (complete != null)
                        {
                            complete();
                        }
                        return;
                    }
                    Debug.LogError(bundleName + ":" + spriteName + "加载失败");
                }, isCache);
            }
        }, isCache);
    }
}
