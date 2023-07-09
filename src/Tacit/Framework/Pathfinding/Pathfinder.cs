using System;
using System.Collections.Generic;
using C5;
using Tacit.Primer;

namespace Tacit.Framework.Pathfinding;

public class Pathfinder {
    private readonly Point _goal;
    private readonly Node[,] _nodeGrid;
    private readonly IntervalHeap<Node> _openList = new();

    private readonly Predicate<Point> _passable;
    private readonly Point _size;
    private readonly Point _start;

    public Pathfinder(Point size, Point start, Point goal, Predicate<Point> passable) {
        _size = size;
        _start = start;
        _goal = goal;
        _passable = passable;
        _nodeGrid = new Node[_size.x, _size.y];
    }

    public List<Point>? FindPath() {
        if (_start == _goal) return new List<Point>();

        // add the start node to the open list
        _openList.Add(_nodeGrid[_start.x, _start.y] =
            new Node(_start.x, _start.y, g: 0, Point.MhDist(_goal, _start), null!));

        var curNode = default(Node);
        while (!_openList.IsEmpty) {
            curNode = _openList.DeleteMin();// pop the next node off the open list
            curNode.open = false;

            // check if we've reached the goal
            var dx = _goal.x - curNode.x;
            var dy = _goal.y - curNode.y;
            if (Math.Abs(dx) + Math.Abs(dy) <= 0) {
                // return the found path
                var path = new List<Point>();
                while (curNode.parent != null) {
                    path.Add(new Point(curNode.x, curNode.y));
                    curNode = curNode.parent;
                }

                path.Reverse();
                return path;
            }

            // add valid neighbors to the open list
            var x = curNode.x;
            var y = curNode.y;
            var g = curNode.g + 1;
            if (x > 0) TryOpenNode(x - 1, y, g, curNode);
            if (x < _size.x - 1) TryOpenNode(x + 1, y, g, curNode);
            if (y > 0) TryOpenNode(x, y - 1, g, curNode);
            if (y < _size.y - 1) TryOpenNode(x, y + 1, g, curNode);
        }

        // there isn't a path to the goal
        return null;
    }

    private void TryOpenNode(int x, int y, int g, Node parent) {
        var node = _nodeGrid[x, y];
        if (node == null) {
            if (_passable(new Point(x, y))) {
                // unvisited node; add to open list
                node = _nodeGrid[x, y] = new Node(x, y, g,
                    Point.MhDist(_goal, new Point(x, y)), parent);
                _openList.Add(ref node.pqHandle, node);
            }
        } else if (node.open) {
            if (g < node.g) {
                // this route is better
                node.g = g;
                node.parent = parent;
                _openList.Replace(node.pqHandle, node);// update in the priority queue
            }
        }
    }

    private class Node : IComparable<Node> {
        public readonly int h;
        public readonly int x;
        public readonly int y;
        public int g;
        public bool open = true;
        public Node parent;

        public IPriorityQueueHandle<Node>? pqHandle;

        public Node(int x, int y, int g, int h, Node parent) {
            this.x = x;
            this.y = y;
            this.g = g;
            this.h = h;
            this.parent = parent;
        }

        public int F => g + h;

        public int CompareTo(Node other) {
            return F - other.F;
        }
    }
}