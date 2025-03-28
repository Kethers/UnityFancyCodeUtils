﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;

namespace Utils
{
    public class LoadImageMgr
    {
        /// <summary>
        /// download from web or hard disk
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loadEnd">callback</param>
        /// <returns></returns>
        public IEnumerator LoadImage(string url, Action<Texture2D> loadEnd)
        {
            Texture2D texture = null;
            //先从内存加载
            if (_imageDic.TryGetValue(url, out texture))
            {
                loadEnd.Invoke(texture);
                yield break;
            }

            string savePath = GetLocalPath();
            string filePath = string.Format("file://{0}/{1}.png", savePath, EncryptUtil.MD5Encrypt(url));
            //from hard disk
            bool hasLoad = false;
            if (Directory.Exists(filePath))
                yield return DownloadImage(filePath, (state, localTexture) =>
                {
                    hasLoad = state;
                    if (state)
                    {
                        loadEnd.Invoke(localTexture);
                        if (!_imageDic.ContainsKey(url))
                            _imageDic.Add(url, localTexture);
                    }
                });
            if (hasLoad) yield break; //loaded
            //from web
            yield return DownloadImage(url, (state, downloadTexture) =>
            {
                hasLoad = state;
                if (state)
                {
                    loadEnd.Invoke(downloadTexture);
                    if (!_imageDic.ContainsKey(url))
                        _imageDic.Add(url, downloadTexture);
                    Save2LocalPath(url, downloadTexture);
                }
            });
        }

        public IEnumerator DownloadImage(string url, Action<bool, Texture2D> downloadEnd)
        {
            using (UnityWebRequest request = new UnityWebRequest(url))
            {
                DownloadHandlerTexture downloadHandlerTexture = new DownloadHandlerTexture(true);
                request.downloadHandler = downloadHandlerTexture;
                yield return request.Send();
                if (string.IsNullOrEmpty(request.error))
                {
                    Texture2D localTexture = downloadHandlerTexture.texture;
                    downloadEnd.Invoke(true, localTexture);
                    request.Dispose();
                }
                else
                {
                    downloadEnd.Invoke(false, null);
                    Debug.Log(request.error);
                }
            }
        }

        /// <summary>
        /// save the picture
        /// </summary>
        /// <param name="url"></param>
        /// <param name="texture"></param>
        private void Save2LocalPath(string url, Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            string savePath = GetLocalPath();
            try
            {
                File.WriteAllBytes(string.Format("{0}/{1}.png", savePath, EncryptUtil.MD5Encrypt(url)), bytes);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// get which path will save
        /// </summary>
        /// <returns></returns>
        private string GetLocalPath()
        {
            string savePath = Application.persistentDataPath + "/pics";
#if UNITY_EDITOR
            savePath = Application.dataPath + "/pics";
#endif
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            return savePath;
        }

        private Dictionary<string, Texture2D> _imageDic = new Dictionary<string, Texture2D>();
        public static LoadImageMgr Instance { get; private set; } = new LoadImageMgr();
    }
}