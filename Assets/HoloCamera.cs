using System;
using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_UWP
using SpellLibrary;
#endif

public class HoloCamera : MonoBehaviour
{

    private WebCamTexture webcam;

    // Should make this into function argument later
    public TextMesh textMesh;
    public TextMesh twoMesh;
    public AudioSource audioSource;

#if UNITY_UWP
    private CognitiveServisesVisionLibrary.CognitiveVisionHelper _cognitiveHelper;
#endif

    public void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);
        webcam = new WebCamTexture();
        webcam.Play();
        Debug.LogFormat("webcam: {0} {1} x {2}", webcam.deviceName, webcam.width, webcam.height);

#if UNITY_UWP
        Debug.Log("hello");
        _cognitiveHelper = new CognitiveServisesVisionLibrary.CognitiveVisionHelper();
#endif
    }

    public Texture2D TakePhoto()
    {
        Debug.Log("Take Photo");
        Texture2D webcamImage = new Texture2D(webcam.width, webcam.height);
        webcamImage.SetPixels(webcam.GetPixels());
        audioSource.Play();
        return webcamImage;
    }

    private async void RecognizeText(Texture2D tex)
    {

        // string recognized = "Platform must be UWP.";
        Debug.Log("Recognize Text");
        // Fill in buffer with JPG data from image
        //List<byte> buffer = new List<byte>();
        //tex.GetRawTextureData();
        byte[] buffer = ImageConversion.EncodeToJPG(tex);
#if UNITY_UWP
            try 
            {
				Debug.Log("sent");
                var visionResult = await _cognitiveHelper.start(buffer);
                var description = _cognitiveHelper.ExtractOcr(visionResult);
#if NETFX_CORE
            var incorrectCorrectedPairs = SpellCheckerClient.SpellCheck(description); // This is the "diff"
            var correctText = SpellCheckerClient.CorrectString(description,incorrectCorrectedPairs);
            twoMesh.text = correctText.Result;
#endif
            textMesh.text = description;
                
            } 
            catch(Exception e)
            {
                textMesh.text = e.Message;
				Debug.Log(e.Message);
            }
#endif

    }

    public void TakePhotoToPreview(Renderer preview)
    {
        Debug.Log("Take Photo preview");
        Texture2D image = TakePhoto();
        preview.material.mainTexture = image;

        RecognizeText(image);
        Debug.Log("update text");
        // update the aspect ratio to match webcam
        float aspectRatio = (float)image.width / (float)image.height;
        Vector3 scale = preview.transform.localScale;
        scale.x = scale.y * aspectRatio;
        preview.transform.localScale = scale;
        Debug.Log("OwO");
    }

    public void InstantiatePhoto(GameObject prefab)
    {
        Debug.Log("InstantiatePhoto");
        GameObject go = GameObject.Instantiate(prefab, Camera.main.transform.position + Camera.main.transform.forward * 0.5f, Camera.main.transform.rotation);
        TakePhotoToPreview(go.transform.GetChild(0).GetComponent<Renderer>());
    }

}