using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Detection
{
    public class FaceDetection: MonoBehaviour
    {
        int counter = 0;
        void Update()
        {
            if(counter == 6)
            {
                counter = 0;
                Debug.Log("Face detection çalıştı.");
                StartCoroutine(TakeEpicScreenshot());
            }
            counter++;
        }

        IEnumerator TakeEpicScreenshot()
        {
            yield return new WaitForEndOfFrame();

            // create a texture to pass to encoding
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            // put buffer into texture
            texture.ReadPixels(new UnityEngine.Rect(0, 0, Screen.width, Screen.height), 0, 0);

            yield return 0;

            texture.Apply();
            //Debug.Log("texture: " + texture.EncodeToPNG().Length);
            yield return texture;
        }

        Texture2D getScreenshoot()
        {
            // create a texture to pass to encoding
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            // put buffer into texture
            texture.ReadPixels(new UnityEngine.Rect(0, 0, Screen.width, Screen.height), 0, 0);

            texture.Apply();
            return texture;
        }

        public void DetectFace(Texture2D texture)
        {
            var data = texture.EncodeToPNG();
            Debug.Log("data: " + data.Length);
            try
            {
                var haarCascade = new CascadeClassifier("haarcascade_frontalface_alt.xml");
                // Load target image
                var gray = new Mat(0, 0, texture.width, texture.height);
                gray.SetArray(0, 0, data);

                // Detect faces
                OpenCvSharp.Rect[] faces = haarCascade.DetectMultiScale(
                                    gray, 1.08, 2, HaarDetectionType.ScaleImage, new Size(30, 30));

                Debug.Log("faces: " + faces.ToString());
            } catch(Exception ex)
            {
                Debug.LogError("haar exception: ex: " + ex.ToString());
            }
        }
    }
}
