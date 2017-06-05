using GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

class Game
{
    public const String color_black = "Black";
    public const String color_white = "White";

    private Chessboard chessboard { get; set; }
    private PieceSet blackPieces;
    private PieceSet whitePieces;

    public Game(Vector3 cameraPosition, Vector3 cameraForward)
    {
        this.chessboard = new Chessboard(cameraPosition, cameraForward);
        this.whitePieces = new PieceSet(color_white, this.chessboard);
        this.blackPieces = new PieceSet(color_black, this.chessboard);
    }
}

