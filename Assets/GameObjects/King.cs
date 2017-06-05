using System;
using UnityEngine;

namespace GameObjects
{
    class King : AbstractPiece
    {
        public King(string color) : base(color)
        {
            this.type = "King";
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
