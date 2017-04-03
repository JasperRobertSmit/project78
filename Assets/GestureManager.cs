using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        // get a new gesture recognizer
        GestureRecognizer recognizer = new GestureRecognizer();
        // set up to receive both tap and double tap events
        recognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap);
        // see https://docs.unity3d.com/550/Documentation/ScriptReference/VR.WSA.Input.GestureRecognizer.TappedEventDelegate.html 
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            Add();
            Debug.Log("Tap");
        };
        recognizer.StartCapturingGestures();
    }

    public void Add()
    {
        Object itemPrefab = Resources.Load("Assets/Resources/Cube.prefab");

        GameObject itemObject = (GameObject)Instantiate(itemPrefab);
        itemObject.transform.parent = Camera.allCameras[0].gameObject.transform;
        itemObject.transform.position = Camera.allCameras[0].gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
