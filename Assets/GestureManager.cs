using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;

public class GestureManager : MonoBehaviour
{
    public static GameObject gameObjectChessboard;
    public static bool gameObjectChessboardPlaced = false;

    public static GameObject gameObjectChessPiece;
    public static GameObject gameObjectVoiceChessPiece;
    public static bool gameObjectChessPieceSelected = false;

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

    private GameObject textPrefab;
    public Vector3 headOffset = Vector3.zero;
    private TextMesh textMesh;

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
        InitText();
        InitGesture();
        InitSpeechRecognition();
        InitGameObjects();
    }

    void Update()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        if (textPrefab != null)
        {
            // move the textprefab
            textPrefab.transform.position = headPosition + (gazeDirection * 180) + new Vector3(0,5,0);
            textPrefab.transform.LookAt(Camera.main.transform);
            textPrefab.transform.Rotate(0, 180, 0);
        }

        if (!gameObjectChessboardPlaced && gameObjectChessboard != null)
        {
            // if chessboard is selected move chessboard
            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, PhysicsRaycastMask))
            {
                gameObjectChessboard.transform.position = hitInfo.point;
                //Quaternion toQuat = Camera.main.transform.localRotation;
                //toQuat.x = 0;
                //toQuat.z = 0;
                //gameObjectChessboard.transform.rotation = toQuat;
            }
        }
        else if (gameObjectChessPiece != null)
        {
            ExecuteManualMove(true);
        }
    }

    #region Init

    private void InitText()
    {
        // set the text object
        UnityEngine.Object textResource = Resources.Load("Text");
        textPrefab = (GameObject)Instantiate(textResource, transform.position + (transform.forward * 2) + new Vector3(0, 5, 0), new Quaternion(0, 0, 0, 0));
        textMesh = textPrefab.GetComponent<TextMesh>();
        textMesh.text = "Say \"load\" load the chessboard.";
    }

    private void InitGesture()
    {
        // add the gesturerecognizer
        GestureRecognizer recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap);
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            GestureManager_TapEvent();
        };
        recognizer.StartCapturingGestures();
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
                        keywords.Add(from + " to " + to);
                    }

        // add the load commands
        keywords.Add("load");
        keywords.Add("place");

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

    #region Events

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (args.text == "load")
        {
            LoadChessboard();
        }
        else if (args.text == "place")
        {
            PlaceChessboard();
        }
        else if (args.text.Contains("to"))
        {
            ExecuteVoiceMove(args.text);
        }
    }

    public void GestureManager_TapEvent()
    {
        if (!gameObjectChessPieceSelected)
        {
            LoadChessPiece();
        }
        else
        {
            PlaceChessPiece();
        }
    }

    #endregion

    #region Load / Place

    public void LoadChessPiece()
    {
        // check if chesspiece is selected and start movement
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo) && (hitInfo.collider.gameObject.name.ToLower().Contains("white") || hitInfo.collider.gameObject.name.ToLower().Contains("black")))
        {
            // temp
            if (hitInfo.collider.gameObject.name.ToLower().Contains("pawn"))
            {
                UnityEngine.Debug.Log("Chesspiece selected!");
                gameObjectChessPiece = hitInfo.collider.gameObject;
                gameObjectChessPieceSelected = true;
            }
        }
    }

    public void PlaceChessPiece()
    {
        // stop chesspiece movement
        ExecuteManualMove(false);
        UnityEngine.Debug.Log("Chesspiece deselected!");
    }

    private void LoadChessboard()
    {
        textMesh.text = "Say \"place\" to place the chessboard.";
        UnityEngine.Debug.Log("Chessboard selected!");
        UnityEngine.Object itemPrefab = Resources.Load("Cube");
        gameObjectChessboard = (GameObject)Instantiate(itemPrefab, transform.position + (transform.forward * 5), new Quaternion(0, 0, 0, 0));

        // start the spatial mapping
        spatialMappingCollider = gameObjectChessboard.GetComponent<SpatialMappingCollider>();
        spatialMappingCollider.surfaceParent = gameObjectChessboard;
        spatialMappingCollider.freezeUpdates = false;
        spatialMappingCollider.layer = physicsLayer;
        PhysicsRaycastMask = 1 << physicsLayer;
        gameObjectChessboard.SetActive(true);
    }

    private void PlaceChessboard()
    {
        // get chessboard data and stop movement
        Vector3 bounds = gameObjectChessboard.GetComponent<Renderer>().bounds.size;
        squareSize = bounds.x / 8;
        startBlack = gameObjectChessboard.transform.position;
        whiteStart = gameObjectChessboard.transform.position;
        gameObjectChessboard = null;
        gameObjectChessboardPlaced = true;
        UnityEngine.Debug.Log("Chessboard deselected!");
        textMesh.text = "";

        // black
        List<UnityEngine.Object> chessItemsBlack = new List<UnityEngine.Object>() { itemPrefabBlackRook, itemPrefabBlackKnight, itemPrefabBlackBishop, itemPrefabBlackKing, itemPrefabBlackQueen, itemPrefabBlackBishop, itemPrefabBlackKnight, itemPrefabBlackRook };
        Quaternion rotationBlack = new Quaternion(0, 90, 0, 0);
        float itemPrefabBlackX = 0;
        float x = (bounds.x / 2) - (float)(0.5 * squareSize);
        float blackZ = (bounds.z / 2) - (float)(0.5 * squareSize);
        startBlack = startBlack - new Vector3(x, 0, blackZ) + new Vector3(0, bounds.y, 0);

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
        whiteStart = whiteStart + new Vector3(x, 0, whiteZ) + new Vector3(0, bounds.y, 0);

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
            }
        }
    }

    private void ExecuteManualMove(bool temporary)
    {
        GameObject gameObjectCurrent = gameObjectVoiceChessPiece != null ? gameObjectVoiceChessPiece : gameObjectChessPiece;

        // get the current position of the chesspiece
        Vector3 chessPieceCurrentVector = new Vector3(gameObjectCurrent.transform.position.x, gameObjectCurrent.transform.position.y, gameObjectCurrent.transform.position.z);
        int verticalFrom = (int)Math.Round((chessPieceCurrentVector.x - startBlack.x) / squareSize, 0);
        int horizontalFrom = (int)Math.Round((chessPieceCurrentVector.z - startBlack.z) / squareSize, 0);

        // get the new position of the chesspiece
        Vector3 chessPieceNewVector = new Vector3(gameObjectCurrent.transform.position.x + (transform.forward * 2).x, gameObjectCurrent.transform.position.y + (transform.forward * 2).y, gameObjectCurrent.transform.position.z + (transform.forward * 2).z);
        int verticalTo = (int)Math.Round((chessPieceNewVector.x - startBlack.x) / squareSize, 0);
        int horizontalTo = (int)Math.Round((chessPieceNewVector.z - startBlack.z) / squareSize, 0);

        // check limits of chesspiece
        horizontalTo = Math.Max(horizontalTo, 0);
        horizontalTo = Math.Min(horizontalTo, 7);
        verticalTo = Math.Max(verticalTo, 0);
        verticalTo = Math.Min(verticalTo, 7);
        horizontalFrom = Math.Max(horizontalFrom, 0);
        horizontalFrom = Math.Min(horizontalFrom, 7);
        verticalFrom = Math.Max(verticalFrom, 0);
        verticalFrom = Math.Min(verticalFrom, 7);

        // get the closest available option
        ChessPiece chessPiece = MapChessPiece(gameObjectCurrent.name);
        ChessPieceColor chessPieceColor = MapChessPieceColor(gameObjectCurrent.name);
        int[] closestMove = GetClosestMove(chessPiece, chessPieceColor, horizontalFrom, verticalFrom, horizontalTo, verticalTo);
        if (closestMove != null)
        {
            // check if temporarymove of permanent
            if (temporary)
            {
                ExecuteTempMove(horizontalFrom, verticalFrom, horizontalTo, verticalTo);
            }
            else
            {
                ExecuteMove(horizontalFrom, verticalFrom, horizontalTo, verticalTo);
            }
        }
    }

    private void ExecuteTempMove(int horizontalFrom, int verticalFrom, int horizontalTo, int verticalTo)
    {
        // get the closest square
        float horizontalCoordinates = (horizontalTo) * squareSize;
        float verticalCoordinates = (verticalTo) * squareSize;

        // move the piece
        gameObjectChessPiece.transform.position = startBlack + new Vector3(horizontalCoordinates, 0, verticalCoordinates);
    }

    private void ExecuteMove(int horizontalFrom, int verticalFrom, int horizontalTo, int verticalTo)
    {
        ChessPieceColor chessPieceColor = MapChessPieceColor(gameObjectChessPiece.name);
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
        gameObjects[horizontalTo][verticalTo] = gameObjectChessPiece;
        gameObjectChessPiece.transform.position = startBlack + new Vector3(horizontalCoordinates, 0, verticalCoordinates);
        gameObjectChessPiece = null;
        gameObjectVoiceChessPiece = null;
        gameObjectChessPieceSelected = false;
    }
    #endregion
}
