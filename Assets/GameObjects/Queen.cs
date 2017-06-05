using System;
using UnityEngine;

namespace GameObjects
{
    class Queen : AbstractPiece
    {
        public Queen(string color) : base(color)
        {
            this.type = "Queen";
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
