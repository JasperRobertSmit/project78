using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureManager : MonoBehaviour
{
    public static GameObject gameObject;

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
        };
        recognizer.StartCapturingGestures();
    }

    public void Add()
    {
        Debug.Log("add");

        if (gameObject == null)
        {
            Debug.Log("add1");
            Object itemPrefab = Resources.Load("Cube");
            gameObject = (GameObject)Instantiate(itemPrefab, transform.position + (transform.forward * 5), transform.rotation);
        }
        else
        {
            Debug.Log("add2");
            gameObject = null;
        }
        
        //GameObject itemObject = (GameObject)Instantiate(itemPrefab);
        //itemObject.transform.parent = Camera.allCameras[0].gameObject.transform;
        //itemObject.transform.position = Camera.allCameras[0].gameObject.transform.position;

        //Debug.Log(itemObject.transform.position.x + " " + itemObject.transform.position.y + " " + itemObject.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("update1");
        if (gameObject != null)
        {
            Debug.Log("update2");
            gameObject.transform.position = transform.position + (transform.forward * 5);
        }
    }
}
