using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameObjects
{
    class PieceSet
    {
        private Rook rookLeft;
        private Rook rookRight;
        private Knight knightLeft;
        private Knight knightRight;
        private Bishop bishopLeft;
        private Bishop bishopRight;
        private Queen queen;
        private King king;

        private Pawn pawnA;
        private Pawn pawnB;
        private Pawn pawnC;
        private Pawn pawnD;
        private Pawn pawnE;
        private Pawn pawnF;
        private Pawn pawnG;
        private Pawn pawnH;

        public PieceSet(String Color, Chessboard chessboard)
        {
            float squareSize = chessboard.getSquareSize();
            // Backrow        

            this.rookLeft = new Rook(Color);
            this.rookRight = new Rook(Color);
            this.knightLeft = new Knight(Color);
            this.knightRight = new Knight(Color);
            this.bishopLeft = new Bishop(Color);
            this.bishopRight = new Bishop(Color);
            this.king = new King(Color);
            this.queen = new Queen(Color);

            List<AbstractPiece> backRow = new List<AbstractPiece>()
            {
                this.rookLeft,
                this.knightLeft,
                this.bishopLeft,
                this.king,
                this.queen,
                this.bishopRight,
                this.knightRight,
                this.rookRight
            };

            float itemPrefabX = getPrefabX(Color);
            float itemPrefabZ = getPrefabZ(Color);

            foreach (AbstractPiece piece in backRow)
            {
                piece.initiateObject(chessboard.getBottomLeft() + new Vector3(itemPrefabX, 0, itemPrefabZ));
                itemPrefabX += squareSize;
            }


            this.pawnA = new Pawn(Color);
            this.pawnB = new Pawn(Color);
            this.pawnC = new Pawn(Color);
            this.pawnD = new Pawn(Color);
            this.pawnE = new Pawn(Color);
            this.pawnF = new Pawn(Color);
            this.pawnG = new Pawn(Color);
            this.pawnH = new Pawn(Color);

            // Pawns
            List<AbstractPiece> frontRow = new List<AbstractPiece>()
            {
                this.pawnA,
                this.pawnB,
                this.pawnC,
                this.pawnD,
                this.pawnE,
                this.pawnF,
                this.pawnG,
                this.pawnH
            };

            itemPrefabX = getPrefabX(Color);
            itemPrefabZ = getPrefabZ(Color) + squareSize;
            foreach (Pawn piece in frontRow)
            {
                piece.initiateObject(chessboard.getBottomLeft() + new Vector3(itemPrefabX, 0, itemPrefabZ));
                itemPrefabX += squareSize;
            }
        }

        private float getPrefabX(String Color)
        {
            if(Color == Game.color_black)
            {
                return 0;
            }
            else
            {
                // TODO invullen witte X coordinaat
                return 0;
            }
        }

        private float getPrefabZ(String Color)
        {
            if (Color == Game.color_black)
            {
                return 0;
            }
            else
            {
                // TODO invullen witte Z coordinaat
                return 0;
            }
        }
    }
}
