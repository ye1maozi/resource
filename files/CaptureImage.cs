using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using LuaInterface;


namespace ReignFramework
{
    public class CaptureImage:MonoBehaviour
    {
        private LuaFunction imageUrlCallFun;

        public void CaptureScreen(LuaFunction urlCallFun)
        {
            this.imageUrlCallFun = urlCallFun;
            Application.CaptureScreenshot("saveImg.png");
            Debug.Log("图片地址.." + (Application.persistentDataPath + "/saveImg.png"));
            String url = (Application.dataPath + "/saveImg.png");
            if (this.imageUrlCallFun != null)
            {
                imageUrlCallFun.Call(url);
            }
        }

        public void CaptureScreenWithView(Canvas canvas, GameObject gameobj,Camera camera, LuaFunction urlCallFun)
        {
           

            Vector3[] fourCorners = new Vector3[4];
            RectTransform rectTransform = gameobj.transform as RectTransform;
            rectTransform.GetWorldCorners(fourCorners);

            Vector3 leftBottom = fourCorners[0];
            Vector3 rightTop = fourCorners[2];

            Debug.Log("最初位置.x " + leftBottom.x);
            Debug.Log("最初位置.y " + leftBottom.y);
            Debug.Log("最初位置e.x " + rightTop.x);
            Debug.Log("最初位置e.y " + rightTop.y);

            Vector3 worldleftBottom = camera.WorldToScreenPoint(leftBottom);
            float x = worldleftBottom.x;
            float y = worldleftBottom.y;

            Vector3 worldrightTop = camera.WorldToScreenPoint(rightTop);
            float w = worldrightTop.x - x;
            float h = worldrightTop.y - y;

            float maxW = Screen.width;
            float maxH = Screen.height;

            w = w > maxW ? maxW : w;
            h = h > maxH ? maxH : h;

            Debug.Log("位置1.x " + x);
            Debug.Log("位置1.y " + y);
            Debug.Log("位置1.w " + w);
            Debug.Log("位置1.h " + h);

            CaptureScreenWithRect(new Rect(x, y, w, h), urlCallFun);

        }
        public void CaptureScreenWithRect(Rect rect ,LuaFunction urlCallFun) 
        {
            this.imageUrlCallFun = urlCallFun;
            StartCoroutine(screenShotWithRect(rect));
        }
        IEnumerator screenShotWithRect(Rect rect)
        {
         
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            yield return new WaitForEndOfFrame();
            screenShot.ReadPixels(rect, 0, 0,false);
            screenShot.Apply();

            byte[] bytes = screenShot.EncodeToPNG();
            string fileName = Application.dataPath + "/saveImg.png";
            System.IO.File.WriteAllBytes(fileName, bytes);
            Debug.Log("图片地址.." + (Application.dataPath + "/saveImg.png"));
            String url = (Application.dataPath + "/saveImg.png");
            if (this.imageUrlCallFun != null)
            {
                imageUrlCallFun.Call (url);
            }
        }

         public void CaptureScreenWithCamera(Camera camera, Rect rect, LuaFunction urlCallFun)
        {
            this.imageUrlCallFun = urlCallFun;
            RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
            camera.targetTexture = rt;
            camera.Render();
            //多个相机
            //camera2.taretTexture = rt
            //camera2.Render()

            //active
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();


            //reset
            camera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);


            byte[] bytes = screenShot.EncodeToPNG();
            string fileName = Application.dataPath + "/saveImg.png";
            System.IO.File.WriteAllBytes(fileName, bytes);
            Debug.Log("图片地址.." + (Application.dataPath + "/saveImg.png"));
            String url = (Application.dataPath + "/saveImg.png");
            if (this.imageUrlCallFun != null)
            {
                imageUrlCallFun.Call(url);
            }
        }
    }
}
