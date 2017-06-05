using System;
using UnityEngine;

namespace GameObjects
{
    class Knight : AbstractPiece
    {
        public Knight(string color) : base(color)
        {
            this.type = "Knight";
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
