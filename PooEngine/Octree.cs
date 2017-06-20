using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Note: The whole idea here is separate block types to make mesh generation faster. 
 * Note: Node removal or changing Node types may require a special case.
 * Rules for the leaf nodes:
 * If there is an insertion with a different type than the node type, then do an insertion up to a certain depth. (Don't actually store vector3s in this structure, that's dumb)
 * Otherwise, do nothing. 
 * If a leaf node is subdivided, it must become a regular node. 
 * Non-leaf nodes do not care about block type.
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

    }

    public class LeafNode : Node
    {

    }
}
