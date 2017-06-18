using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;

public class GestureManager : MonoBehaviour
{
    public static GameObject gameObjectChessboard;
    public static GameObject gameObjectChessboardLetters;
    public static GameObject gameObjectChessboardNumbers;
    public static bool gameObjectChessboardPlaced = false;

    public static GameObject gameObjectVoiceChessPiece;

    private object[][] gameObjects = new object[8][];
    private List<string> letters = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h" };

    private Vector3 startBlack;
    private Vector3 whiteStart;
    private float squareSize = 0;

    private KeywordRecognizer keywordRecognizer = null;
    private List<string> keywords = new List<string>();

    public static GestureManager Instance { private set; get; }
    [HideInInspector]
    public static int PhysicsRaycastMask;
    private int physicsLayer = 31;
    private SpatialMappingCollider spatialMappingCollider;

    private GameObject iconPrefab;
    public Vector3 headOffset = Vector3.zero;

    private UnityEngine.Object itemPrefabBlackBishop;
    private UnityEngine.Object itemPrefabBlackKing;
    private UnityEngine.Object itemPrefabBlackKnight;
    private UnityEngine.Object itemPrefabBlackPawn;
    private UnityEngine.Object itemPrefabBlackQueen;
    private UnityEngine.Object itemPrefabBlackRook;
    private UnityEngine.Object itemPrefabWhiteBishop;
    private UnityEngine.Object itemPrefabWhiteKing;
    private UnityEngine.Object itemPrefabWhiteKnight;
    private UnityEngine.Object itemPrefabWhitePawn;
    private UnityEngine.Object itemPrefabWhiteQueen;
    private UnityEngine.Object itemPrefabWhiteRook;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Init();
        InitSpeechRecognition();
        InitGameObjects();
    }

    void Update()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        if (iconPrefab != null)
        {
            iconPrefab.transform.position = headPosition + (gazeDirection * 5);
            iconPrefab.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }

        if (!gameObjectChessboardPlaced && gameObjectChessboard != null)
        {
            // if chessboard is selected move chessboard
            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, PhysicsRaycastMask))
            {
                gameObjectChessboard.transform.position = hitInfo.point;
            }
        }
    }

    #region Init

    private void Init()
    {
        // load the icon
        UnityEngine.Object iconResource = Resources.Load("icon");
        iconPrefab = (GameObject)Instantiate(iconResource, transform.position + (transform.forward * 2), transform.rotation);
        iconPrefab.AddComponent<Rigidbody>();

        // welcome text
        Speak("Welcome to holochess! To start playing say load chessboard to create a new chessboard.");
    }

    private void Speak(string text)
    {
        var soundManager = GameObject.Find("Audio Manager");
        TextToSpeechManager textToSpeech = (TextToSpeechManager)soundManager.GetComponent<TextToSpeechManager>();
        textToSpeech.Voice = TextToSpeechVoice.Zira;
        textToSpeech.SpeakText(text);
    }

    private void InitSpeechRecognition()
    {
        // add the chessmove commands
        for (int a = 1; a <= 8; a++)
            for (int b = 1; b <= 8; b++)
                for (int c = 1; c <= 8; c++)
                    for (int d = 1; d <= 8; d++)
                    {
                        string from = letters[a - 1] + b.ToString();
                        string to = letters[c - 1] + d.ToString();
                        if (letters[c - 1] != "g" && letters[c - 1] != "b")
                        {
                            keywords.Add(from + " to " + to);
                        }
                    }

        // add the load commands
        keywords.Add("load chessboard");
        keywords.Add("place chessboard");
        keywords.Add("reset chessboard");

        keywordRecognizer = new KeywordRecognizer(keywords.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void InitGameObjects()
    {
        // init the objects
        for (int i = 0; i < 8; i++)
        {
            gameObjects[i] = new object[8];
        }

        // black
        itemPrefabBlackBishop = Resources.Load("Black Bishop");
        itemPrefabBlackKing = Resources.Load("Black King");
        itemPrefabBlackKnight = Resources.Load("Black Knight");
        itemPrefabBlackPawn = Resources.Load("Black Pawn");
        itemPrefabBlackQueen = Resources.Load("Black Queen");
        itemPrefabBlackRook = Resources.Load("Black Rook");

        // white
        itemPrefabWhiteBishop = Resources.Load("White Bishop");
        itemPrefabWhiteKing = Resources.Load("White King");
        itemPrefabWhiteKnight = Resources.Load("White Knight");
        itemPrefabWhitePawn = Resources.Load("White Pawn");
        itemPrefabWhiteQueen = Resources.Load("White Queen");
        itemPrefabWhiteRook = Resources.Load("White Rook");
    }

    #endregion

    #region Reset

    private void Reset()
    {
        if (gameObjectChessboard != null)
        {
            gameObjectChessboard.SetActive(false);

            Destroy(gameObjectChessboard);
            Destroy(spatialMappingCollider);
            Destroy(gameObjectChessboardLetters);
            Destroy(gameObjectChessboardNumbers);

            gameObjectChessboardLetters = null;
            gameObjectChessboardNumbers = null;
            gameObjectChessboard = null;
            spatialMappingCollider = null;

            gameObjectChessboardPlaced = false;

            foreach (object[] items in gameObjects)
            {
                foreach (object item in items)
                {
                    Destroy((GameObject)item);
                }
            }

            // init the objects
            for (int i = 0; i < 8; i++)
            {
                gameObjects[i] = new object[8];
            }
        }
    }

    #endregion

    #region Events

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (args.text == "load chessboard")
        {
            LoadChessboard();
        }
        else if (args.text == "place chessboard")
        {
            PlaceChessboard();
        }
        else if (args.text == "reset chessboard")
        {
            Reset();
            Init();
        }
        else if (args.text.Contains("to"))
        {
            ExecuteVoiceMove(args.text);
        }
    }

    #endregion

    #region Load / Place

    private void LoadChessboard()
    {
        // welcome text
        Speak("Now find a clear space in the room and say place chessboard to position the chessboard in the room.");
        iconPrefab.SetActive(false);

        // load the chessboard
        UnityEngine.Object itemPrefab = Resources.Load("Cube");
        gameObjectChessboard = (GameObject)Instantiate(itemPrefab, transform.position + (transform.forward * 7), new Quaternion(0, 0, 0, 0));

        // start the spatial mapping
        spatialMappingCollider = gameObjectChessboard.GetComponent<SpatialMappingCollider>();
        spatialMappingCollider.surfaceParent = gameObjectChessboard;
        spatialMappingCollider.freezeUpdates = false;
        spatialMappingCollider.layer = physicsLayer;
        PhysicsRaycastMask = 1 << physicsLayer;
    }

    private void PlaceChessboard()
    {
        // welcome text
        Speak("Great! To move a chesspiece, say the current coordinates and the new coordinates of the chesspiece, for example a, 2, to b, 2. You can reset the game by saying reset chessboard. The black chesspieces are yours, your opponent has the first move, good luck!");

        // get chessboard data and stop movement
        Vector3 bounds = gameObjectChessboard.GetComponent<Renderer>().bounds.size;
        squareSize = bounds.x / 8;
        startBlack = gameObjectChessboard.transform.position;
        whiteStart = gameObjectChessboard.transform.position;

        UnityEngine.Object itemPrefabLetters = Resources.Load("LettersResource");
        gameObjectChessboardLetters = (GameObject)Instantiate(itemPrefabLetters, gameObjectChessboard.transform.position - new Vector3(0.5f, 0, 0), new Quaternion(0, 0, 0, 0));

        UnityEngine.Object itemPrefabNumbers = Resources.Load("NumbersResource");
        gameObjectChessboardNumbers = (GameObject)Instantiate(itemPrefabNumbers, gameObjectChessboard.transform.position - new Vector3(0, 0, 0.5f), new Quaternion(0, 0, 0, 0));

        gameObjectChessboardPlaced = true;
        UnityEngine.Debug.Log("Chessboard deselected!");

        // black
        List<UnityEngine.Object> chessItemsBlack = new List<UnityEngine.Object>() { itemPrefabBlackRook, itemPrefabBlackKnight, itemPrefabBlackBishop, itemPrefabBlackKing, itemPrefabBlackQueen, itemPrefabBlackBishop, itemPrefabBlackKnight, itemPrefabBlackRook };
        Quaternion rotationBlack = new Quaternion(0, 90, 0, 0);
        float itemPrefabBlackX = 0;
        float x = (bounds.x / 2) - (float)(0.5 * squareSize);
        float blackZ = (bounds.z / 2) - (float)(0.5 * squareSize);
        startBlack = startBlack - new Vector3(x, 0, blackZ) + new Vector3(0, bounds.y / 2, 0);

        // place black pieces
        int gameObjectLocation = 0;
        foreach (UnityEngine.Object itemPrefabBlack in chessItemsBlack)
        {
            gameObjects[gameObjectLocation++][0] = (GameObject)Instantiate(itemPrefabBlack, startBlack + new Vector3(itemPrefabBlackX, 0, 0), rotationBlack);
            itemPrefabBlackX += squareSize;
        }

        // place black pawns
        itemPrefabBlackX = 0;
        gameObjectLocation = 0;
        for (int i = 0; i < 8; i++)
        {
            gameObjects[gameObjectLocation++][1] = (GameObject)Instantiate(itemPrefabBlackPawn, startBlack + new Vector3(itemPrefabBlackX, 0, squareSize), rotationBlack);
            itemPrefabBlackX += squareSize;
        }

        // white
        List<UnityEngine.Object> chessItemsWhite = new List<UnityEngine.Object>() { itemPrefabWhiteRook, itemPrefabWhiteKnight, itemPrefabWhiteBishop, itemPrefabWhiteKing, itemPrefabWhiteQueen, itemPrefabWhiteBishop, itemPrefabWhiteKnight, itemPrefabWhiteRook };
        Quaternion rotationWhite = new Quaternion(0, 180, 0, 0);
        float itemPrefabWhiteX = 0;
        float whiteZ = (bounds.z / 2) - (float)(1.5 * squareSize);
        whiteStart = whiteStart + new Vector3(x, 0, whiteZ) + new Vector3(0, bounds.y / 2, 0);

        // place white pieces
        gameObjectLocation = 7;
        for (int i = 0; i < 8; i++)
        {
            gameObjects[gameObjectLocation--][6] = (GameObject)Instantiate(itemPrefabWhitePawn, whiteStart + new Vector3(itemPrefabWhiteX, 0, 0), rotationWhite);
            itemPrefabWhiteX -= squareSize;
        }

        // place white pawns
        itemPrefabWhiteX = 0;
        gameObjectLocation = 7;
        foreach (UnityEngine.Object itemPrefabBlack in chessItemsWhite)
        {
            gameObjects[gameObjectLocation--][7] = (GameObject)Instantiate(itemPrefabBlack, whiteStart + new Vector3(itemPrefabWhiteX, 0, squareSize), rotationWhite);
            itemPrefabWhiteX -= squareSize;
        }

        // first opponent move
        ExecuteOpponentMove();
    }
    #endregion

    #region Enums
    private enum ChessPiece
    {
        Bishop,
        King,
        Knight,
        Pawn,
        Queen,
        Rook
    }

    private ChessPiece MapChessPiece(string chessPieceName)
    {
        switch (chessPieceName.ToLower().Replace("black", "").Replace("white", "").Replace("(clone)", "").Trim())
        {
            case "bishop":
                return ChessPiece.Bishop;
            case "king":
                return ChessPiece.King;
            case "knight":
                return ChessPiece.Knight;
            default:
            case "pawn":
                return ChessPiece.Pawn;
            case "queen":
                return ChessPiece.Queen;
            case "rook":
                return ChessPiece.Rook;
        }
    }

    private enum ChessPieceColor
    {
        black,
        white
    }

    private ChessPieceColor MapChessPieceColor(string chessPieceName)
    {
        if (chessPieceName.ToLower().Contains("white"))
        {
            return ChessPieceColor.white;
        }
        else
        {
            return ChessPieceColor.black;
        }
    }
    #endregion

    #region Chess Piece Movement
    private int[] GetClosestMove(ChessPiece piece, ChessPieceColor color, int currentX, int currentZ, int newX, int newZ)
    {
        if (piece == ChessPiece.Pawn)
        {
            // get possible moves
            int[][] chessMoveOptions = new int[2][]
            {
                new int[2] {currentX, currentZ },
                new int[2] {currentX, Math.Min(currentZ+1, 7) }
            };

            // calculate the distance closest to a move
            int distance = int.MaxValue;
            int[] option = null;
            foreach (int[] chessMoveOption in chessMoveOptions)
            {
                // check if another piece from the movers color is at that location
                object existingObject = gameObjects[chessMoveOption[0]][chessMoveOption[1]];
                if (existingObject != null && ((GameObject)existingObject).name.ToLower().Contains(color.ToString().ToLower()))
                {
                    continue;
                }

                // check if the distance is smaller
                int chessMoveOptionDistance = Math.Abs(chessMoveOption[0] - newX) + Math.Abs(chessMoveOption[1] - newZ);
                if (chessMoveOptionDistance < distance)
                {
                    distance = chessMoveOptionDistance;
                    option = chessMoveOption;
                }
            }

            // return the move closest to the current location
            if (option != null)
                Debug.Log(option[0] + " - " + option[1]);
            else
                Debug.Log("No option!");
            return option;
        }
        return null;
    }

    private void ExecuteVoiceMove(string voiceCommand)
    {
        // get the from and to coordinates
        string[] coordinates = voiceCommand.Split(new string[] { " to " }, StringSplitOptions.RemoveEmptyEntries);
        int verticalFrom = letters.IndexOf(coordinates[0][0].ToString());
        int horizontalFrom = int.Parse(coordinates[0][1].ToString()) - 1; 
        int verticalTo = letters.IndexOf(coordinates[1][0].ToString());
        int horizontalTo = int.Parse(coordinates[1][1].ToString()) - 1;

        // check if an object exists at the requested location
        GameObject gameObject = (GameObject)gameObjects[horizontalFrom][verticalFrom];
        if (gameObject != null)
        {
            gameObjectVoiceChessPiece = gameObject;
            UnityEngine.Debug.Log(gameObject.name + " " + voiceCommand);

            // use the closestmove method to check if the move is valid
            ChessPiece chessPiece = MapChessPiece(gameObject.name);
            ChessPieceColor chessPieceColor = MapChessPieceColor(gameObject.name);
            int[] closestMove = GetClosestMove(chessPiece, chessPieceColor, horizontalFrom, verticalFrom, horizontalTo, verticalTo);
            if (closestMove != null && closestMove[0] == horizontalTo && closestMove[1] == verticalTo)
            {
                ExecuteMove(horizontalFrom, verticalFrom, horizontalTo, verticalTo);
                ExecuteOpponentMove();
            }
            else
            {
                Speak("The move " + voiceCommand + " is invalid!");
            }
        }
    }

    private void ExecuteOpponentMove()
    {
        // get a random move
        int attempts = 0;
        while (attempts < 1000)
        {
            System.Random r = new System.Random();
            int a = r.Next(0, 8);
            int b = r.Next(0, 8);
            object existingObject = gameObjects[a][b];
            if (existingObject != null && ((GameObject)existingObject).name.ToLower().Contains("white pawn"))
            {
                gameObjectVoiceChessPiece = (GameObject)existingObject;
                object existingOpponentObject = gameObjects[a][b - 1];
                if (existingOpponentObject == null || !((GameObject)existingOpponentObject).name.ToLower().Contains("black pawn"))
                {
                    ExecuteMove(a, b, a, b - 1);
                    return;
                }
            }
            attempts++;
        }

        // get the next move
        for (int i = 0; i < 8; i++)
        {
            for (int y = 0; y < 8; y++)
            {
                object existingObject = gameObjects[i][y];
                if (existingObject != null && ((GameObject)existingObject).name.ToLower().Contains("white pawn"))
                {
                    gameObjectVoiceChessPiece = (GameObject)existingObject;
                    object existingOpponentObject = gameObjects[i][y - 1];
                    if (existingOpponentObject == null || !((GameObject)existingOpponentObject).name.ToLower().Contains("black pawn"))
                    {
                        ExecuteMove(i, y, i, y - 1);
                        return; 
                    }
                }
            }
        }

        // end of demo
        Speak("You have reached the end of the demo!");
    }

    private void ExecuteMove(int horizontalFrom, int verticalFrom, int horizontalTo, int verticalTo)
    {
        ChessPieceColor chessPieceColor = MapChessPieceColor(gameObjectVoiceChessPiece.name);
        ChessPieceColor chessPieceOpponentColor = chessPieceColor == ChessPieceColor.black ? ChessPieceColor.white : ChessPieceColor.black;

        // get the closest square
        float horizontalCoordinates = (horizontalTo) * squareSize;
        float verticalCoordinates = (verticalTo) * squareSize;

        // if a piece from the opponent is at that location remove piece
        object existingObject = gameObjects[horizontalTo][verticalTo];
        if (existingObject != null && ((GameObject)existingObject).name.ToLower().Contains(chessPieceOpponentColor.ToString().ToLower()))
        {
            Destroy((GameObject)gameObjects[horizontalTo][verticalTo]);
        }

        // move the piece
        gameObjects[horizontalFrom][verticalFrom] = null;
        gameObjects[horizontalTo][verticalTo] = gameObjectVoiceChessPiece;
        gameObjectVoiceChessPiece.transform.position = startBlack + new Vector3(horizontalCoordinates, 0, verticalCoordinates);
        gameObjectVoiceChessPiece = null;
    }
    #endregion
}
