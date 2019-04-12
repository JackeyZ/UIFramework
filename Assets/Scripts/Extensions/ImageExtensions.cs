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
                    if(complete != null)
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
        //image.sprite = AssetBundleFramework.AssetLoadInEditor.LoadObject<Sprite>(bundleName, spriteName);
        //if (complete != null)
        //{
        //    complete();
        //}
#else
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
                    if(complete != null)
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
#endif
    }
}
