using System;
using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiffMatchPatch;

#if UNITY_UWP
using SpellLibrary;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Security;
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
    Windows.Storage.StorageFolder storageFolder;
    Windows.Storage.StorageFolder picturesFolder;
    Windows.Storage.StorageFile sampleFile;
#endif

    public void Start()
    {

         
        //Diff diff = new Diff();
        //DiffMatchPatch.Diff diff = new Diff();

#if UNITY_UWP
        storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        Storage();
#endif
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

#if UNITY_UWP
    public async void Storage()
    {
        storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        picturesFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        sampleFile = await storageFolder.CreateFileAsync("Test.txt", Windows.Storage.CreationCollisionOption.GenerateUniqueName);
    }

    public async void WritePhotoToFile(IBuffer buffer, IStorageFile file) {
         FileIO.WriteBufferAsync(file, buffer);
    }
#endif

    public Texture2D TakePhoto()
    {
        Texture2D webcamImage = new Texture2D(webcam.width, webcam.height);
        audioSource.Play();
        Debug.Log("Take Photo");
        webcamImage.SetPixels(webcam.GetPixels());
        /*
        #if UNITY_UWP
                byte[] buffer = ImageConversion.EncodeToJPG(webcamImage);
                IBuffer temp = Windows.Security.Cryptography.CryptographicBuffer.copyToByteArray(buffer);
                WritePhotoToFile(temp, picturesFolder);
        #endif
        */
        return webcamImage;
    }

    private async void RecognizeText(Texture2D tex)
    {
        

        // string recognized = "Platform must be UWP.";
        Debug.Log("Recognize Text");

        diff_match_patch dmp = new diff_match_patch();
        // Fill in buffer with JPG data from image
        //List<byte> buffer = new List<byte>();
        //tex.GetRawTextureData();
        byte[] buffer = ImageConversion.EncodeToJPG(tex);
        String outputText = "";

#if UNITY_UWP
        try

            {
                var visionResult = await _cognitiveHelper.start(buffer);
                var description = _cognitiveHelper.ExtractOcr(visionResult);

                var incorrectCorrectedPairs = SpellCheckerClient.SpellCheck(description); // This is the "diff"
                var correctText = SpellCheckerClient.CorrectString(description,incorrectCorrectedPairs.Result);
                
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, DateTime.Now.ToString() + " | " + description + " | " + correctText +"\n");
                Debug.Log("og txt: " + description);
                Debug.Log("correct text: " + correctText);
                if (description.Equals(correctText))
                {
                    outputText = "<color=lime>" + correctText + "</color>";
                }
                else
                {

                    List<Diff> diffs = dmp.diff_main(correctText, description, false);
                    /*
                    for (int i = 0; i < diffs.Count; i++)
                    {
                        Debug.Log(i + " " + diffs[i]);
                    }
                    dmp.diff_cleanupSemantic(diffs);
                    for (int i = 0; i < diffs.Count; i++)
                    {
                        Debug.Log(i + " " + diffs[i]);
                    }

    */
                foreach (var d in diffs)
                {
                    
                    Debug.Log(d.ToString());

                    if (d.operation.Equals(Operation.INSERT))//equals insertion
                    {
                        outputText += "<color=red>" + d.text + "</color>";
                    }else if (d.operation.Equals(Operation.EQUAL))//equals equality
                    {
                        outputText += d.text;
                    }
                }
            }

            twoMesh.text = outputText;

            textMesh.text = description;
                audioSource.Play();
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