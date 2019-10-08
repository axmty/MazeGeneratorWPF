using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeWPF
{
    public class MazeEngine
    {
        private readonly MazeStepByStepDrawer _drawer;
        private readonly Random _random = new Random();
        private Maze _maze;

        public MazeEngine(MazeStepByStepDrawer drawer)
        {
            _drawer = drawer;
        }

        public void GenerateRandom(int width, int height)
        {
            _maze = new Maze(width, height);
            this.InitMazeDrawing(width, height);
            
            var visitedNodes = new bool[height, width];
            var backtrackStack = new Stack<Node>();
            var currentNode = _maze.FirstNode;

            visitedNodes[0, 0] = true;
            _drawer.ChangeNodeState(currentNode.X, currentNode.Y, NodeState.Current);

            do
            {
                var unvisitedNeighbours = this.GetUnvisitedNeighbours(currentNode, visitedNodes);
                var unvisitedNeigboursCount = unvisitedNeighbours.Count();
                Node nextNode;

                if (unvisitedNeigboursCount > 0)
                {
                    backtrackStack.Push(currentNode);
                    nextNode = this.ChooseRandomNode(unvisitedNeighbours);
                    _drawer.RemoveWallBetween(currentNode.X, currentNode.Y, nextNode.X, nextNode.Y);
                    _drawer.ChangeNodesState(
                        (currentNode.X, currentNode.Y, NodeState.Visited),
                        (nextNode.X, nextNode.Y, NodeState.Current));
                    visitedNodes[nextNode.Y, nextNode.X] = true;
                    currentNode = nextNode;
                }
                else if (backtrackStack.Count > 0)
                {
                    nextNode = backtrackStack.Pop();
                    _drawer.ChangeNodesState(
                        (currentNode.X, currentNode.Y, NodeState.Backtracked),
                        (nextNode.X, nextNode.Y, NodeState.Current));
                    currentNode = nextNode;
                }
            } while (backtrackStack.Count > 0);

            _drawer.Draw();
        }

        public void Solve()
        {
            if (_maze == null)
            {
                throw new InvalidOperationException("Maze must be generated before being solved.");
            }

            throw new NotImplementedException();
        }

        private IEnumerable<Node> GetUnvisitedNeighbours(Node node, bool[,] visitedCells)
        {
            return _maze.GetNeighbours(node).Where(n => !visitedCells[n.Y, n.X]);
        }

        private Node ChooseRandomNode(IEnumerable<Node> nodes)
        {
            var randomIndex = _random.Next(0, nodes.Count());

            return nodes.ElementAt(randomIndex);
        }

        private void InitMazeDrawing(int width, int height)
        {
            _drawer.InitMazeArea(width, height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    _drawer.InitNode(j, i);

                    if (j < width - 1)
                    {
                        _drawer.DrawWallBetween(j, i, j + 1, i);
                    }

                    if (i < height - 1)
                    {
                        _drawer.DrawWallBetween(j, i, j, i + 1);
                    }
                }
            }
        }
    }
    
}

