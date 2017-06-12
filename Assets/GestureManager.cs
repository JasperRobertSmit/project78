using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureManager : MonoBehaviour
{
    private Game game;
    public static bool gamePlaced = false;

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
        Debug.Log("addnew");

        if (gamePlaced)
            return;

        if (game == null)
        {
            game = new Game(transform.position, transform.forward);
            gamePlaced = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (game != null)
        {
            Debug.Log("update2");
            game.Chessboard.GameObjectChessboard.transform.position = game.Chessboard.GameObjectChessboard.transform.position + (game.Chessboard.GameObjectChessboard.transform.forward * 30);
        }
    }
}
