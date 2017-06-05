using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameObjects
{
    class Chessboard
    {
        private GameObject gameObjectChessboard { get; set; }
        private float SquareSize { get; set; }
        private UnityEngine.Object prefab;

        public Chessboard(Vector3 cameraPosition, Vector3 cameraForward)
        {
            this.prefab = Resources.Load("Board");
            this.gameObjectChessboard = (GameObject)GameObject.Instantiate(this.prefab, cameraPosition + (cameraForward * 5), new Quaternion(0, 0, 0, 0));
        }

        public GameObject resetBoard()
        {
            return this.gameObjectChessboard;
        }

        public Vector3 getBottomLeft()
        {
            return this.gameObjectChessboard.transform.position;
        }

        public float getSquareSize()
        {
            Vector3 bounds = this.gameObjectChessboard.GetComponent<Renderer>().bounds.size;
            return bounds.x / 10;
        }
    }
}
