using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

/*
 * Note: The whole idea here is separate block types to make mesh generation faster. 
 * Note: Node removal or changing Node types may require a special case.
 * Rules for the leaf nodes:
 * If there is an insertion with a different type than the node type, then do an insertion up to a certain depth. (Don't actually store vector3s in this structure, that's dumb)
 * Otherwise, do nothing. 
 * If a leaf node is subdivided, it must become a regular node. 
 * Non-leaf nodes do not care about block type.
 * 
 * directions:
 * 0: 1,1,1
 * 1: -1,1,1
 * 2: -1,-1,1
 * 3: 1,-1,1
 * 4: 1,-1,-1
 * 5: 1,1,-1
 * 6: -1,1,-1
 * 7: -1,-1,-1
 */

namespace PooEngine
{
    public enum nodeType{Air, Solid, NonLeaf};//Two main types, many types may come from Solid blocks.
    //May not even use an enum, it might just boil down to a bunch of bytes representing the block types.

    public class Octree
    {

    }

    public class Node
    {
        public Vector3 Position;
        public float Size;

        private Node child0;
        private Node child1;
        private Node child2;
        private Node child3;
        private Node child4;
        private Node child5;
        private Node child6;
        private Node child7;

        public Node this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return child0;
                    case 1:
                        return child1;
                    case 2:
                        return child2;
                    case 3:
                        return child3;
                    case 4:
                        return child4;
                    case 5:
                        return child5;
                    case 6:
                        return child6;
                    case 7:
                        return child7;
                    default:
                        throw new Exception("Index out of range");
                }
            }
        }

        public Node Parent;

        public Octree Tree;

        public void Subdivide()
        {
            float newSize = this.Size / 2;
            child0 = new Node(new Vector3(0.5f, 0.5f, 0.5f) * newSize, newSize, this);
            child1 = new Node(new Vector3(-0.5f, 0.5f, 0.5f) * newSize, newSize, this);
            child2 = new Node(new Vector3(-0.5f, -0.5f, 0.5f) * newSize, newSize, this);
            child3 = new Node(new Vector3(0.5f, -0.5f, 0.5f) * newSize, newSize, this);
            child4 = new Node(new Vector3(0.5f, -0.5f, -0.5f) * newSize, newSize, this);
            child5 = new Node(new Vector3(0.5f, 0.5f, -0.5f) * newSize, newSize, this);
            child6 = new Node(new Vector3(-0.5f, 0.5f, -0.5f) * newSize, newSize, this);
            child7 = new Node(new Vector3(-0.5f, -0.5f, -0.5f) * newSize, newSize, this);
        }

        public void Collapse()
        {
            child0 = null;
            child1 = null;
            child2 = null;
            child3 = null;
            child4 = null;
            child5 = null;
            child6 = null;
            child7 = null;
        }

        public bool isSubdivided()
        {
            if (child0 != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Node search(Vector3 p)//No need to handle edge cases
        {
            Vector3 dp = p - this.Position;
            dp.Normalize();//Because Normalize doesn't return a vector3
            return this;
        }

        public Node(Vector3 _position, float _size, Node _parent)
        {
            this.Position = _position;
            this.Size = _size;
            this.Parent = _parent;
        }
    }
}
