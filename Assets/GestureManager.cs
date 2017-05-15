using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;

public class GestureManager : MonoBehaviour
{
    public static GameObject gameObjectChessboard;
    public static bool gameObjectChessboardPlaced = false;


    private object[][] gameObjects = new object[8][];
    private List<string> letters = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h" };

    private Vector3 startBlack;
    private Vector3 whiteStart;

    float squareSize = 0;

    KeywordRecognizer keywordRecognizer = null;
    List<string> keywords = new List<string>();


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

        for (int i = 0; i < 8; i++)
        {
            gameObjects[i] = new object[8];
        }

        // add the voice commands
        for (int a = 1; a <= 8; a++)
            for (int b = 1; b <= 8; b++)
                for (int c = 1; c <= 8; c++)
                    for (int d = 1; d <= 8; d++)
                    {
                        string from = letters[a - 1] + b.ToString();
                        string to = letters[c - 1] + d.ToString();
                        keywords.Add(from + " to " + to);
                    }

        keywordRecognizer = new KeywordRecognizer(keywords.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (args.text.Contains("to"))
        {
            string[] coordinates = args.text.Split(new string[] { " to " }, StringSplitOptions.RemoveEmptyEntries);
            int horizontalFrom = letters.IndexOf(coordinates[0][0].ToString());
            int verticalFrom = int.Parse(coordinates[0][1].ToString()) - 1;

            int horizontalTo = letters.IndexOf(coordinates[1][0].ToString());
            int verticalTo = int.Parse(coordinates[1][1].ToString()) - 1;

            UnityEngine.Object gameObject = (UnityEngine.Object)gameObjects[verticalFrom][horizontalFrom];
            if (gameObject != null)
            {
                Debug.Log(gameObject.name);
                float horizontalCoordinates = (horizontalTo) * squareSize;
                float verticalCoordinates = (verticalTo) * squareSize;
                ((GameObject)gameObject).transform.position = startBlack + new Vector3(horizontalCoordinates, 0, verticalCoordinates);
                gameObjects[verticalTo][horizontalTo] = gameObject;
                gameObjects[verticalFrom][horizontalFrom] = null;
            }
            Debug.Log(args.text);
        }
    }

    public void Add()
    {
        Debug.Log("addnew");

        if (gameObjectChessboardPlaced)
            return;

        if (gameObjectChessboard == null)
        {
            Debug.Log("add1");
            UnityEngine.Object itemPrefab = Resources.Load("Cube");
            gameObjectChessboard = (GameObject)Instantiate(itemPrefab, transform.position + (transform.forward * 5), new Quaternion(0, 0, 0, 0));
        }
        else
        {
            // black
            UnityEngine.Object itemPrefabBlackBishop = Resources.Load("Black Bishop");
            UnityEngine.Object itemPrefabBlackKing = Resources.Load("Black King");
            UnityEngine.Object itemPrefabBlackKnight = Resources.Load("Black Knight");
            UnityEngine.Object itemPrefabBlackPawn = Resources.Load("Black Pawn");
            UnityEngine.Object itemPrefabBlackQueen = Resources.Load("Black Queen");
            UnityEngine.Object itemPrefabBlackRook = Resources.Load("Black Rook");

            List<UnityEngine.Object> chessItemsBlack = new List<UnityEngine.Object>()
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
            squareSize = bounds.x / 8;
            float itemPrefabBlackX = 0;
            float itemPrefabBlackZ = 0;

            startBlack = gameObjectChessboard.transform.position;
            float x = (bounds.x / 2) - (float)(0.5 * squareSize);
            float z = (bounds.z / 2) - (float)(0.5 * squareSize);
            startBlack = startBlack - new Vector3(x, 0, z);

            int gameObjectLocation = 0;
            foreach (UnityEngine.Object itemPrefabBlack in chessItemsBlack)
            {
                GameObject instantiatedObject = (GameObject)Instantiate(itemPrefabBlack, startBlack + new Vector3(itemPrefabBlackX, 0, itemPrefabBlackZ), new Quaternion(0, 90, 0, 0));
                itemPrefabBlackX += squareSize;
                gameObjects[gameObjectLocation][0] = instantiatedObject;
                gameObjectLocation++;
            }

            itemPrefabBlackX = 0;
            itemPrefabBlackZ += squareSize;
            gameObjectLocation = 0;
            for (int i = 0; i < 8; i++)
            {
                GameObject instantiatedObject = (GameObject)Instantiate(itemPrefabBlackPawn, startBlack + new Vector3(itemPrefabBlackX, 0, itemPrefabBlackZ), new Quaternion(0, 90, 0, 0));
                itemPrefabBlackX += squareSize;
                gameObjects[gameObjectLocation][1] = instantiatedObject;
                gameObjectLocation++;
            }

            // White
            UnityEngine.Object itemPrefabWhiteBishop = Resources.Load("White Bishop");
            UnityEngine.Object itemPrefabWhiteKing = Resources.Load("White King");
            UnityEngine.Object itemPrefabWhiteKnight = Resources.Load("White Knight");
            UnityEngine.Object itemPrefabWhitePawn = Resources.Load("White Pawn");
            UnityEngine.Object itemPrefabWhiteQueen = Resources.Load("White Queen");
            UnityEngine.Object itemPrefabWhiteRook = Resources.Load("White Rook");

            List<UnityEngine.Object> chessItemsWhite = new List<UnityEngine.Object>()
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

            whiteStart = gameObjectChessboard.transform.position;
            float whiteX = (boundsWhite.x / 2) - (float)(0.5 * squareSizeWhite);
            float whiteZ = (boundsWhite.z / 2) - (float)(1.5 * squareSizeWhite);
            whiteStart = whiteStart + new Vector3(whiteX, 0, whiteZ);
            gameObjectLocation = 7;

            for (int i = 0; i < 8; i++)
            {
                GameObject instantiatedObject = (GameObject)Instantiate(itemPrefabWhitePawn, whiteStart + new Vector3(itemPrefabWhiteX, 0, itemPrefabWhiteZ), new Quaternion(0, 180, 0, 0));
                itemPrefabWhiteX -= squareSizeWhite;

                gameObjects[gameObjectLocation][6] = instantiatedObject;
                gameObjectLocation--;
            }

            itemPrefabWhiteX = 0;
            itemPrefabWhiteZ += squareSizeWhite;
            gameObjectLocation = 7;
            foreach (UnityEngine.Object itemPrefabBlack in chessItemsWhite)
            {
                GameObject instantiatedObject = (GameObject)Instantiate(itemPrefabBlack, whiteStart + new Vector3(itemPrefabWhiteX, 0, itemPrefabWhiteZ), new Quaternion(0, 180, 0, 0));
                itemPrefabWhiteX -= squareSizeWhite;

                gameObjects[gameObjectLocation][7] = instantiatedObject;
                gameObjectLocation--;
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
