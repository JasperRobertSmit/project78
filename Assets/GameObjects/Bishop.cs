using System;
using UnityEngine;

namespace GameObjects
{
    class Bishop : AbstractPiece
    {
        public Bishop(string color) : base(color)
        {
            this.type = "Bishop";
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
