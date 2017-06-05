using System;
using UnityEngine;

namespace GameObjects
{
    class Rook : AbstractPiece
    {
        public Rook(string color) : base(color)
        {
            this.type = "Rook";
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
