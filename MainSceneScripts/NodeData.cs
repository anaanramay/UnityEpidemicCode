using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NodeData class to store edges and paths for the person pathfinding code
public class NodeData : IEquatable<NodeData>, IComparable<NodeData> {

    public GameObject node;   // The start node for this edge
    public float totalDist;   // The length of this edge
    public GameObject parent; // The parent of this node in the path

    // Constructor
    public NodeData(GameObject node, float totalDist, GameObject parent) {
        this.node = node;
        this.totalDist = totalDist;
        this.parent = parent;
    }

    // Determines if this NodeData and other are equal
    // Two NodeDatas are equal if they connect the same two nodes
    public bool Equals(NodeData other) {
        if (this.node == other.node && this.parent == other.parent) {
            return true;
        } else if (this.node == other.parent && this.parent == other.node) {
            return true;
        } else {
            return false;
        }
    }

    // Gets the other node in this NodeData
    // If given the GameObject that is equal to this.node, returns this.parent
    // If given the GameObject that is equal to this.parent, returns this.node
    // Otherwise the given GameObject is invalid, returns null
    public GameObject OtherNode(GameObject node) {
        if (this.node == node) {
            return this.parent;
        } else if (this.parent == node) {
            return this.node;
        } else {
            return null;
        }
    }

    // Compares two NodeDatas by their lengths
    public int CompareTo(NodeData other) {
        return this.totalDist.CompareTo(other.totalDist);
    }
}
