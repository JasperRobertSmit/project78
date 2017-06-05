using System;
using UnityEngine;

namespace GameObjects
{
    abstract class AbstractPiece
    {
        protected String color;
        protected String type;
        protected Vector3 coordinates;
        protected UnityEngine.Object prefab;

        public AbstractPiece(String color)
        {
            this.color = color;
        }

        public void initiateObject(Vector3 coordinates)
        {
            this.coordinates = coordinates;
            String resourceName = this.getResourceName();
            Debug.Log(resourceName);
            this.prefab = Resources.Load(resourceName);
            GameObject.Instantiate(this.prefab, coordinates, new Quaternion(0, 0, 0, 0));
        }

        abstract public bool isValidMove(Vector3 target);

        abstract public bool isValidAttackMove(Vector3 target, bool enemyPresent);

        protected String getResourceName()
        {
            return this.color + " " + this.type;
        }
    }
}
