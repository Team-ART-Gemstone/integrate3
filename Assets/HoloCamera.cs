﻿using System;
using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
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
    public TextMesh textMesh; //(Incorrect Text)
    public GameObject myQuad;
    public TextMesh twoMesh;
    public AudioSource audioSource;

    private bool myAnchor = false; //Variable to flip to set or destroy anchor

#if UNITY_UWP
    private CognitiveServisesVisionLibrary.CognitiveVisionHelper _cognitiveHelper;
    Windows.Storage.StorageFolder storageFolder;
    Windows.Storage.StorageFolder picturesFolder;
    Windows.Storage.StorageFile sampleFile;
#endif

    string[] secondGradeList = {"accident", "bright", "center", "enemy", "field", "greedy", "lonely", "universe", "planet", "worry"};
    string[] thirdGradeList = {"absorb",  "brief",  "decay", "disease", "flutter", "intelligent",
         "nursery",  "respect", "solution", "value"};
    string[] fourthGradeList = {"accurate", "awkward", "blossom", "concern", "descend",  "frantic", "frontier",
        "modest", "prefer", "request"};
    string[] fifthGradeList = {"absurd", "character", "decline", "escalate", "feeble",  "immense", "major",
        "pasture",  "strategy",  "visible"};
    string[] sixthGradeList = {"allegiance", "convenient", "definitely", "eclipse",
         "hazardous", "leisure", "loathe", "parody", "quest", "retrieve"};

    string[] wordList; // use setGradeLevel to set
    Dictionary<int, string> placesToWords;
    string wordString;
    int searchLoc;

    public void Start()
    {
        setGradeLevel(0); // default, uses all words

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
         Windows.Storage.FileIO.WriteTextAsync(sampleFile, "This is a Debug Statement to fill in the top of the file.\n");
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
                description = description.ToLower().Trim();
                
            //string twond =  "accident beach bright center discard enemy field greedy idea lonely ocean planet stream tower worry";

            //var incorrectCorrectedPairs = SpellCheckerClient.SpellCheck(description); // This is the "diff"
            //var correctText = SpellCheckerClient.CorrectString(description,incorrectCorrectedPairs.Result);

            //correctText = correctText.ToUpper();


            int correctPosition = dmp.match_main(wordString, description, wordString.Length/2);
            if(correctPosition != -1) // find location in wordString corresponding to start of desired word
            {
                int prev = 0;
                foreach(int n in placesToWords.Keys)
                {
                    if (n <= correctPosition)
                    {
                        prev = n;
                    }
                    else
                    {
                        correctPosition = prev;
                        break;
                    }
                }
                //searchLoc = correctPosition; // next words searched for are probably going to be near here
            }
            Debug.Log("og txt: " + description);
            Debug.Log(correctPosition);
            string correctText;
            try
            {
                correctText = placesToWords[correctPosition];
            } catch
            {
                correctText = "Error, try again.\n" + description;
            }
            Debug.Log("correct text: " + correctText);
                await Windows.Storage.FileIO.AppendTextAsync(sampleFile, DateTime.Now.ToString() + " | " + description + " | " + correctText +"\n");
                
                
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

                    if (d.operation.Equals(Operation.DELETE))//equals insertion
                    {
                        outputText += "<color=red>" + d.text + "</color>";
                    }else if (d.operation.Equals(Operation.EQUAL))//equals equality
                    {
                        outputText += d.text;
                    }
                }
            }

            twoMesh.text = outputText + "\n" + description;

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
    }

    public void InstantiatePhoto(GameObject prefab)
    {
        Debug.Log("InstantiatePhoto");
        GameObject go = GameObject.Instantiate(prefab, Camera.main.transform.position + Camera.main.transform.forward * 0.5f, Camera.main.transform.rotation);
        TakePhotoToPreview(go.transform.GetChild(0).GetComponent<Renderer>());
    }

    /************* ANCHORING THE QUAD INTO SPACE *********************/
    public void anchor()
    {
        audioSource.Play();
        myAnchor = !myAnchor;
        Debug.Log(myAnchor.ToString());
        if (myAnchor)
        {
            Debug.Log("cannot move");
            DestroyImmediate(myQuad.GetComponent<WorldAnchor>());
            myQuad.AddComponent<WorldAnchor>();
        } else {
            Debug.Log("Can Move");
            DestroyImmediate(myQuad.GetComponent<WorldAnchor>());
        }
    }

    public void setGradeLevel(int gradeLevel)
    {
        audioSource.Play();
        switch (gradeLevel)
        {
            case 2:
                wordList = secondGradeList;
                break;
            case 3:
                wordList = thirdGradeList;
                break;
            case 4:
                wordList = fourthGradeList;
                break;
            case 5:
                wordList = fifthGradeList;
                break;
            case 6:
                wordList = sixthGradeList;  
                break;
            default:
                wordList = new string[secondGradeList.Length + thirdGradeList.Length + fourthGradeList.Length + fifthGradeList.Length + sixthGradeList.Length];
                secondGradeList.CopyTo(wordList, 0);
                thirdGradeList.CopyTo(wordList, secondGradeList.Length);
                fourthGradeList.CopyTo(wordList, secondGradeList.Length + thirdGradeList.Length);
                fifthGradeList.CopyTo(wordList, secondGradeList.Length + thirdGradeList.Length + fourthGradeList.Length);
                sixthGradeList.CopyTo(wordList, secondGradeList.Length + thirdGradeList.Length + fourthGradeList.Length + fifthGradeList.Length);
                break;
        }

        placesToWords = new Dictionary<int, string>();
        int placeCounter = 0;
        wordString = "";
        foreach (string word in wordList)
        {
            wordString += word + " ";
            placesToWords[placeCounter] = word;
            placeCounter += word.Length + 1;
        }
        searchLoc = 0;
    }

}