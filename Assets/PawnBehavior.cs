using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class PawnBehavior : MonoBehaviour {



	// Use this for initialization
	void Start () {
        Debug.Log("PawnBehavior Loaded");

        GestureRecognizer recognizer = new GestureRecognizer();
        // set up to receive both tap and double tap events
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        // see https://docs.unity3d.com/550/Documentation/ScriptReference/VR.WSA.Input.GestureRecognizer.TappedEventDelegate.html 
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            selectPiece();
        };
        recognizer.StartCapturingGestures();
    }

    void selectPiece() {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            Debug.Log("Raycast hit a hologram");
            // If the raycast hit a hologram, use that as the focused object.
            GameObject currentlyTargetedGameObject = hitInfo.collider.gameObject;
            if (gameObject.Equals(currentlyTargetedGameObject))
            {
                Debug.Log("Selected object is the same as currentObject");
            }
            else
            {
                Debug.Log("$$$");
            }

        }
        else
        {
            // If the raycast did not hit a hologram, clear the focused object.
            Debug.Log("Raycast did NOT hit a hologram");
        }
    }

    // Update is called once per frame
    void Update () {
       
    }
}
