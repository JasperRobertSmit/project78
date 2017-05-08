using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureManager : MonoBehaviour
{
    public static GameObject gameObjectChessboard;
    public static bool gameObjectChessboardPlaced = false;

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

        if (gameObjectChessboardPlaced)
            return;

        if (gameObjectChessboard == null)
        {
            Debug.Log("add1");
            Object itemPrefab = Resources.Load("Cube");
            gameObjectChessboard = (GameObject)Instantiate(itemPrefab, transform.position + (transform.forward * 5), new Quaternion(0, 0, 0, 0));
        }
        else
        {
            // black
            Object itemPrefabBlackBishop = Resources.Load("Black Bishop");
            Object itemPrefabBlackKing = Resources.Load("Black King");
            Object itemPrefabBlackKnight = Resources.Load("Black Knight");
            Object itemPrefabBlackPawn = Resources.Load("Black Pawn");
            Object itemPrefabBlackQueen = Resources.Load("Black Queen");
            Object itemPrefabBlackRook = Resources.Load("Black Rook");

            List<Object> chessItemsBlack = new List<Object>()
            {
                itemPrefabBlackRook,
                itemPrefabBlackKnight,
                itemPrefabBlackBishop,
                itemPrefabBlackKing,
                itemPrefabBlackQueen,
                itemPrefabBlackBishop,
                itemPrefabBlackKnight,
                itemPrefabBlackRook
            };
            
            Vector3 bounds = gameObjectChessboard.GetComponent<Renderer>().bounds.size;
            float squareSize = bounds.x / 8;
            float itemPrefabBlackX = 0;
            float itemPrefabBlackZ = 0;

            Vector3 start = gameObjectChessboard.transform.position;
            float x = (bounds.x / 2) - (float)(0.5 * squareSize);
            float z = (bounds.z / 2) - (float)(0.5 * squareSize);
            start = start - new Vector3(x, 0, z);

            foreach (Object itemPrefabBlack in chessItemsBlack)
            {
                GameObject instantiatedObject = (GameObject)Instantiate(itemPrefabBlack, start + new Vector3(itemPrefabBlackX, 0, itemPrefabBlackZ), new Quaternion(0, 90, 0, 0));
                itemPrefabBlackX += squareSize;
            }

            itemPrefabBlackX = 0;
            itemPrefabBlackZ += squareSize;
            for (int i = 0; i < 8; i++)
            {
                Instantiate(itemPrefabBlackPawn, start + new Vector3(itemPrefabBlackX, 0, itemPrefabBlackZ), new Quaternion(0, 90, 0, 0));
                itemPrefabBlackX += squareSize;
            }
            
            // White
            Object itemPrefabWhiteBishop = Resources.Load("White Bishop");
            Object itemPrefabWhiteKing = Resources.Load("White King");
            Object itemPrefabWhiteKnight = Resources.Load("White Knight");
            Object itemPrefabWhitePawn = Resources.Load("White Pawn");
            Object itemPrefabWhiteQueen = Resources.Load("White Queen");
            Object itemPrefabWhiteRook = Resources.Load("White Rook");

            List<Object> chessItemsWhite = new List<Object>()
            {
                itemPrefabWhiteRook,
                itemPrefabWhiteKnight,
                itemPrefabWhiteBishop,
                itemPrefabWhiteKing,
                itemPrefabWhiteQueen,
                itemPrefabWhiteBishop,
                itemPrefabWhiteKnight,
                itemPrefabWhiteRook
            };

            Vector3 boundsWhite = gameObjectChessboard.GetComponent<Renderer>().bounds.size;
            float squareSizeWhite = boundsWhite.x / 8;
            float itemPrefabWhiteX = 0;
            float itemPrefabWhiteZ = 0;

            Vector3 whiteStart = gameObjectChessboard.transform.position;
            float whiteX = (boundsWhite.x / 2) - (float)(0.5 * squareSizeWhite);
            float whiteZ = (boundsWhite.z / 2) + (float)(5.5 * squareSizeWhite);
            whiteStart = whiteStart + new Vector3(whiteX, 0, whiteZ);
            
            for (int i = 0; i < 8; i++)
            {
                Instantiate(itemPrefabWhitePawn, whiteStart + new Vector3(itemPrefabWhiteX, 0, itemPrefabWhiteZ), new Quaternion(0, 180, 0, 0));
                itemPrefabWhiteX -= squareSizeWhite;
            }

            itemPrefabWhiteX = 0;
            itemPrefabWhiteZ += squareSizeWhite;
            foreach (Object itemPrefabBlack in chessItemsWhite)
            {
                Instantiate(itemPrefabBlack, whiteStart + new Vector3(itemPrefabWhiteX, 0, itemPrefabWhiteZ), new Quaternion(0, 180, 0, 0));
                itemPrefabWhiteX -= squareSizeWhite;
            }



            Debug.Log("add2");
            gameObjectChessboard = null;
            gameObjectChessboardPlaced = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObjectChessboard != null)
        {
            Debug.Log("update2");
            gameObjectChessboard.transform.position = transform.position + (transform.forward * 30);
        }
    }
}
