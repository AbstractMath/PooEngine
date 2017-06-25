using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PooEngine
{
    /*
     * Notes about the chunk class (what should change and what works well):
     * I need to add a way of changing to the chunks object space, so that way I'm not accidentally throwing errors everytime I want to change something. 
     * I also need to think of a way to access data outside of the chunk bounds. That's a bit tricky.
     * I need to think of a way to do voxel raycasting. 
     * I need to think of a way to do physics(Not an immediate thing, just something I should think about)
     */

   public class Chunk : meshObject
    {
        private byte[,,] _chunkData;
        public const int height = 128;
        public const int width = 32;
        public const int depth = 32;

        public byte this[int x, int y, int z]
        {
            //get { return (IsInRange(x, y, z)) ? _chunkData[x, y, z] : (byte)0; }
            get
            {
                if (IsInRange(x, y, z))
                {
                    return _chunkData[x, y, z];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (IsInRange(x, y, z))
                {
                    _chunkData[x, y, z] = value;
                }
                else
                {
                    throw new Exception("Could not set block ID, because the index is out of range.");
                }
            }
        }

        public bool IsInRange(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < width && y < height && z < depth;
        }

        public void updateVBO()
        {
            LinkedList<Vector3> Verticies = new LinkedList<Vector3>();
            LinkedList<int> Tris = new LinkedList<int>();
            LinkedList<Vector3> Norms = new LinkedList<Vector3>();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        byte blockID = this[x, y, z];

                        if (blockID != 0)
                        {
                            int upID = this[x, y + 1, z];
                            int downID = this[x, y - 1, z];
                            int rightID = this[x + 1, y, z];
                            int leftID = this[x - 1, y, z];
                            int frontID = this[x, y, z - 1];
                            int backID = this[x, y, z + 1];

                            Vector3 voxelPosition = new Vector3(x, y, z);

                            if (upID == 0)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, -0.5f));

                                Tris.AddLast(3 + Verticies.Count - 4);//2
                                Tris.AddLast(Verticies.Count - 4);//0
                                Tris.AddLast(2 + Verticies.Count - 4);//3

                                Tris.AddLast(Verticies.Count - 4);//2
                                Tris.AddLast(1 + Verticies.Count - 4);//1
                                Tris.AddLast(2 + Verticies.Count - 4);//0

                                //This is temporary, pretty much. Will add for loop.
                                Norms.AddLast(new Vector3(0, 1, 0));
                                Norms.AddLast(new Vector3(0, 1, 0));
                            }

                            if (downID == 0)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, -0.5f));

                                Tris.AddLast(2 + Verticies.Count - 4);//3
                                Tris.AddLast(Verticies.Count - 4);//0
                                Tris.AddLast(3 + Verticies.Count - 4);//2

                                Tris.AddLast(2 + Verticies.Count - 4);//0
                                Tris.AddLast(1 + Verticies.Count - 4);//1
                                Tris.AddLast(Verticies.Count - 4);//2

                                //This is temporary, pretty much. Will add for loop.
                                Norms.AddLast(new Vector3(0, -1, 0));
                                Norms.AddLast(new Vector3(0, -1, 0));
                            }

                            if (frontID == 0)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, -0.5f));

                                Tris.AddLast(3 + Verticies.Count - 4);//2
                                Tris.AddLast(Verticies.Count - 4);//0
                                Tris.AddLast(2 + Verticies.Count - 4);//3

                                Tris.AddLast(Verticies.Count - 4);//2
                                Tris.AddLast(1 + Verticies.Count - 4);//1
                                Tris.AddLast(2 + Verticies.Count - 4);//0


                                Norms.AddLast(new Vector3(0, 0, -1));
                                Norms.AddLast(new Vector3(0, 0, -1));
                            }

                            if (backID == 0)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, 0.5f));

                                Tris.AddLast(3 + Verticies.Count - 4);//2
                                Tris.AddLast(Verticies.Count - 4);//0
                                Tris.AddLast(2 + Verticies.Count - 4);//3

                                Tris.AddLast(1 + Verticies.Count - 4);//0
                                Tris.AddLast(0 + Verticies.Count - 4);//1
                                Tris.AddLast(3 + Verticies.Count - 4);//2


                                Norms.AddLast(new Vector3(0, 0, -1));
                                Norms.AddLast(new Vector3(0, 0, -1));
                            }

                            if (rightID == 0)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, -0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(0.5f, 0.5f, -0.5f));

                                Tris.AddLast(2 + Verticies.Count - 4);//3
                                Tris.AddLast(Verticies.Count - 4);//0
                                Tris.AddLast(3 + Verticies.Count - 4);//2

                                Tris.AddLast(2 + Verticies.Count - 4);//0
                                Tris.AddLast(1 + Verticies.Count - 4);//1
                                Tris.AddLast(Verticies.Count - 4);//2


                                Norms.AddLast(new Vector3(1, 0, 0));
                                Norms.AddLast(new Vector3(1, 0, 0));
                            }

                            if (leftID == 0)
                            {
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, 0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, -0.5f, -0.5f));
                                Verticies.AddLast(voxelPosition + new Vector3(-0.5f, 0.5f, -0.5f));

                                Tris.AddLast(3 + Verticies.Count - 4);//2
                                Tris.AddLast(Verticies.Count - 4);//0
                                Tris.AddLast(2 + Verticies.Count - 4);//3

                                Tris.AddLast(Verticies.Count - 4);//2
                                Tris.AddLast(1 + Verticies.Count - 4);//1
                                Tris.AddLast(2 + Verticies.Count - 4);//0


                                Norms.AddLast(new Vector3(-1, 0, 0));
                                Norms.AddLast(new Vector3(-1, 0, 0));
                            }
                        }
                    }
                }
            }

            Vector3[] verts = new Vector3[Verticies.Count];
            Vector3[] norms = new Vector3[Norms.Count];
            int[] tris = new int[Tris.Count];

            LinkedListNode<Vector3> nodeInVerticies = Verticies.First;

            for (int v = 0; v < verts.Length; v++)
            {
                verts[v] = nodeInVerticies.Value;
                if (nodeInVerticies != null)
                {
                    nodeInVerticies = nodeInVerticies.Next;
                }
            }

            LinkedListNode<int> nodeInTris = Tris.First;

            for (int t = 0; t < tris.Length; t++)
            {
                tris[t] = nodeInTris.Value;
                if (nodeInTris != null)
                {
                    nodeInTris = nodeInTris.Next;
                }
            }

            LinkedListNode<Vector3> nodeInNormals = Norms.First;

            for (int n = 0; n < norms.Length; n++)
            {
                norms[n] = nodeInNormals.Value;
                if (nodeInNormals != null)
                {
                    nodeInNormals = nodeInNormals.Next;
                }
            }

            this.verticies = verts;
            this.triangles = tris;
            this.normals = norms;
        }

        public void initializeBuffer(GraphicsDevice device)
        {
            base.getObjectBuffer(device);
        }

        public void populateChunk()
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        float noise = Noise.GetOctaveNoise((double)x/10, (double)y/10, (double)z/10, 1);

                        if (10 * noise - 75 + y < 6)
                        {
                            this[x, y, z] = 1;
                        }
                        else
                        {
                            this[x, y, z] = 0;
                        }
                    }
                }
            }
        }

        public Chunk(Effect shader, GraphicsDevice device)//There will be stuff here at some point
        {
            this.shader = shader;
            _chunkData = new byte[width, height, depth];
            populateChunk();
            updateVBO();
            base.objBuffer = base.getObjectBuffer(device);
        }
    }
}
