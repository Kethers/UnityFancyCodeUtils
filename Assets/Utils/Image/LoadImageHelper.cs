using System;
using System.Collections;
using MetaFramework.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MetaMeeting
{
    public class LoadImageHelper : MonoBehaviour
    {
        /// <summary>
        /// Load url picture as RawImage's texture
        /// </summary>
        public void LoadUrlAsRawImage(RawImage rawImg, string url, bool autoSetActiveWhenLoadEnd = true)
        
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("LoadImageHelper.LoadUrlAsRawImage() url is null");
                return;
            }
            StartCoroutine(LoadTexture(url, (tex) => {
                rawImg.texture = tex;
                if (autoSetActiveWhenLoadEnd)
                    rawImg.SafeSetEnable(true);
            }));
        }
        
        /// <summary>
        /// Load url picture as Image's sprite
        /// </summary>
        public void LoadUrlAsImage(Image img, string url, bool autoSetActiveWhenLoadEnd = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("LoadImageHelper.LoadUrlAsImage() url is null");
                return;
            }
            StartCoroutine(LoadTexture(url, (tex) => {
                img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                if (autoSetActiveWhenLoadEnd)
                    img.SafeSetEnable(true);
            }));
        }

        IEnumerator LoadTexture(string url, Action<Texture2D> cb)
        {
            yield return LoadImageMgr.Instance.LoadImage(url, cb);
        }
    }

}