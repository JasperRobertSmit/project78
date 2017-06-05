using System;
using UnityEngine;

namespace GameObjects
{
    class Pawn : AbstractPiece
    {
        public Pawn(string color) : base(color)
        {
            this.type = "Pawn";
        }

        public override bool isValidAttackMove(Vector3 target, bool enemyPresent)
        {
            throw new NotImplementedException();
        }

        public override bool isValidMove(Vector3 target)
        {
            throw new NotImplementedException();
        }
    }
}
