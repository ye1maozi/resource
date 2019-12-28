using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using System;
namespace ReignFramework
{
    public class WebImage : MonoBehaviour
    {
        private static string IMGFLAG;
        void Start()
        {
            IMGFLAG = FileUtils.GetWritePath() + "/imgCache_";
        }
        public void SetWebTextureByUrl(string url, GameObject gb, bool setNative)
        {
            Image img = gb.GetComponent<Image>();
            if( img != null)
            {
                setTexture(url, img, setNative);
            }
            else
            {
                RawImage raw = gb.GetComponent<RawImage>();
                if (raw != null)
                {
                    setTexture(url, raw, setNative);
                }
                else
                {
                    Log.Instance.error("[WebImage] no image component ");
                }
            }
        }
        private void setTexture(string url, Image image, bool setNative)
        {
            if (FileUtils.IsFileExists(IMGFLAG + url.GetHashCode()))
            {
                StartCoroutine(LoadImageLocal(url,image,setNative));
            }
            else
            {
                StartCoroutine(LoadImageWeb(url, image, setNative));

            }
        }

        private void setTexture(string url, RawImage image, bool setNative)
        {
            if (FileUtils.IsFileExists(IMGFLAG + url.GetHashCode()))
            {
                StartCoroutine(LoadImageLocal(url, image, setNative));
            }
            else
            {
                StartCoroutine(LoadImageWeb(url, image, setNative));

            }
        }

        IEnumerator LoadImageLocal(string url, MaskableGraphic image, bool setNative)
        {
            string path ="file:///" +  IMGFLAG + url.GetHashCode();
            Log.Instance.info("[WebImage] LoadImageLocal start " + path);
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture( path))
            {
                yield return www.Send();

                loadImageEnd(www, "", image, setNative);
            }
        }

        IEnumerator LoadImageWeb(string url, MaskableGraphic image, bool setNative)
        {
            Log.Instance.info("[WebImage] LoadImageWeb start " + url);
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.Send();
                loadImageEnd(www, url, image, setNative);
               
            }
        }


        private void loadImageEnd(UnityWebRequest www, string saveUrl, MaskableGraphic image, bool setNative)
        {
            if (www.isError || !string.IsNullOrEmpty(www.error) || www.downloadHandler.data == null)
            {
                Log.Instance.errorFormat("[WebImage] load error ");
            }
            else
            {

                Texture2D tex = DownloadHandlerTexture.GetContent(www);

                if (image is RawImage)
                {
                    ((RawImage)image).texture = tex;
                }
                else
                {
                    Sprite spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                    ((Image)image).sprite = spr;
                }
              
                if (setNative)
                {
                    image.SetNativeSize();
                }
                if (saveUrl != "")
                {
                    byte[] buffer = www.downloadHandler.data;
                    string path = IMGFLAG + saveUrl.GetHashCode();
                    Log.Instance.infoFormat("[WebImage] save image {0}", path);
                    using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                    {
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
               
            }
        }

        //private void loadByFileStream()
        //{
        //    FileStream fs = null;
        //    string path = IMGFLAG + url.GetHashCode();
        //    Log.Instance.info("[WebImage] load local start " + path);
        //    fs = new FileStream(path, FileMode.Open, FileAccess.Read);


        //    byte[] buffer = new byte[fs.Length];
        //    //   BeginRead(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject);
        //    State state = new State(buffer, fs);
        //    IAsyncResult ar = fs.BeginRead(buffer, 0, (int)fs.Length, new AsyncCallback(endRead), state);
        //}

        //private void endRead(IAsyncResult ar)
        //{
        //    State state = ar.AsyncState as State;
        //    FileStream fs = state.fileStream;
        //    if (fs != null)
        //    {
        //        fs.EndRead(ar);
        //        fs.Close();
        //    }
        //    Texture2D tex = new Texture2D(100, 100);
        //    tex.LoadRawTextureData(state.buffer);
        //    this.texture = tex;
        //}
          
    }

    //public class State
    //{
    //    public byte[] buffer;
    //    public FileStream fileStream;
    //    public State(byte[] buffer, FileStream fileStream)
    //    {
    //        this.buffer = buffer;
    //        this.fileStream = fileStream;
    //    }
    //}
}
