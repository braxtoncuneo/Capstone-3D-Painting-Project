using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Node
{

	public const char META_PLAIN_COLOR		= (char) 0x00;
	public const char META_BLOCK_MAPPING	= (char) 0x01;
	public const char META_BLOCK_LIST		= (char) 0x01;
	public const char META_TEXTURE_MAPPING	= (char) 0x02;
	public const char META_FREE_NODE		= (char) 0xFF;
	public const uint ADR_NOT_HERE		= 0x3FFFFFFF;
	public const uint ADR_CACHED		= 0x40000000;
	public const uint ADR_COPY_WRITE	= 0x80000000;
	public const uint AttributeCount	= 3;
	public const uint ChildCount		= 8;
	public const uint NodeSize = 1 + AttributeCount + ChildCount;

	/* copyOnWrite indicates that the node referenced is the root of 
	 * its own SVO and hence should not be traversed into for the
	 * purposes of caching or saving. This will already be handled when
	 * their respective SVO is cached or saved.
	 */
	bool copyOnWrite;

	/* cached indicates that the node referenced is stored in backing
	 * memory for eventual retrieval as it becomes more relevant to 
	 * representing the Voxel State.
	 */
	bool cached;

	/* parentSVO indicates the index of the element in the headBuffer
	 * which references the root node of the SVO containing this node.
	 */
	uint parentSVO;

	/* filePosition indicates the position of this node in 
	 * it's containing cache file.
	 */
	uint filePosition;

	/* metaSurface is simply the value of a given node's meta 
	 * surface fields, stored as a 32-bit unsigned integer.
	 */
	uint metaSurface;

	/* attributes is the array of 32-bit values that make up the
	 * attribute data for this node.
	 */
	uint[] attributes;

	/* children is the array of 32-bit values that encode the
	 * position (or absence) of a given node's child nodes,
	 * as well as important data regarding if the node is
	 * stored elsewhere (in the cache) or acts as a symbolic link
	 * to the root of another SVO (in the case of a copy-on-write
	 * node).
	 */
	Node[] children;

	/* A basic Node constructor
	 */
	public Node()
	{
		metaSurface = 0;
		attributes = new uint[AttributeCount];
		children = new Node[ChildCount];
	}

	/* BufferRead fills the calling node with the node data starting
	 * at position in src. Additionally, it populates its children
	 * and fills them with data located in the buffer according to the
	 * supplied pointers in the child fields. This is done recursively
	 * for all child nodes, except those linked to with a copy-on-write
	 * addressal.
	 */
	public void BufferRead(uint[] src, uint position)
	{
		uint addr;
		metaSurface = src[position];
		for(uint i = 0; i < AttributeCount; i++)
		{
			attributes[i] = src[position + 1 + i];
		}
		for (uint i = 0; i < ChildCount; i++)
		{
			addr = src[position + 1 + AttributeCount + i];
			if ( (addr & ADR_NOT_HERE) != ADR_NOT_HERE )
			{
				children[i] = new Node();
				children[i].copyOnWrite = ((addr & ADR_COPY_WRITE) != 0);
				children[i].cached = ((addr & ADR_CACHED) != 0);
				if ( ! children[i].copyOnWrite )
				{
					children[i].BufferRead(src, src[position + 1 + AttributeCount + i]);
				}
			}
			else
			{
				children[i] = null;
			}
		}
	}

	/* BufferWrite encodes the data present in the calling node as well
	 * as its children (recursively) in the supplied buffer according to
	 * a left-order traversal and up to the depth supplied to the function.
	 * Children linked by a copy-on-write addressal are ignored. Nodes which
	 * were not encoded into the buffer as a result of not meeting the depth
	 * limit are stored in residual, so that they may be eventually placed in
	 * backing memory.
	 */
	public uint BufferWrite(uint[] dst, uint position, uint depth, List<Node> residual)
	{
		uint next = position;
		dst[position] = metaSurface;
		for (uint i = 0; i < AttributeCount; i++)
		{
			dst[position + 1 + i] = attributes[i];
		}
		for (uint i = 0; i < ChildCount; i++)
		{
			if (children[i] != null)
			{
				next += NodeSize;
				dst[position + 1 + AttributeCount + i] = next;
				next = children[i].BufferWrite(dst, next, depth - 1, residual);
			}
			else
			{
				dst[position + 1 + i] = ADR_NOT_HERE;
			}
		}
		return next;
	}

	/* GetLast returns the position of the last node that would be encoded
	 * in an array of uints if the first node was encoded at position and
	 * encoding only traverses as deep as depth.
	 * 
	 * This is mainly used as a helper funciton for GetSize
	 */
	private uint GetLast(uint position, uint depth)
	{
		uint next = position;
		for (uint i = 0; i < 8; i++)
		{
			if (children[i] != null)
			{
				next += NodeSize;
				next = children[i].GetLast(next, depth - 1);
			}
		}
		return next;
	}

	/* Returns the number of uints required to encode the node and its
	 * children (recursively), traversing up to depth layers deep.
	 */
	public uint GetSize(uint depth)
	{
		return GetLast(0, depth) + 1;
	}

}
