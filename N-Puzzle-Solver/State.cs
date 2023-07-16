using System;
using System.Collections.Generic;

namespace N_Puzzle_Solver
{
    public class State
    {
        public static HashSet<int> visitedNodes = new HashSet<int>();

        private HashSet<Direction> _possibleDirections;
        private Direction _resultingMove;

        private State _parent = null;

        public int[] Content { get; set; }
        public int RowsCount;
        public int BlankIndex { get; set; }
        public int Size { get; private set; }
        public HeuristicFunction DistanceFunction { get; private set; }
        public int HScore { get; private set; }
        public int GScore { get; private set; }
        public int FScore { get; private set; }

        private State(int rowsCount, int blankIndex, int[] content, Direction resultingMove, HeuristicFunction distanceFunction)
        {
            RowsCount = rowsCount;
            Size = rowsCount * rowsCount;
            BlankIndex = blankIndex;
            Content = content;
            _resultingMove = resultingMove;
            DistanceFunction = distanceFunction;
        }

        public State(int rowsCount, int blankIndex, int[] content, HeuristicFunction distanceFunction)
            : this(rowsCount, blankIndex, content, Direction.None, distanceFunction)
        {
            DistanceFunction = distanceFunction;

            if(DistanceFunction == HeuristicFunction.ManhattenDistance || DistanceFunction == HeuristicFunction.BFS)
                HScore = HscoreForManhattan();
            else if(DistanceFunction == HeuristicFunction.HammingDistance)
                HScore = HscoreForHamming();

            GScore = 0;
            FScore = HScore;

            _possibleDirections = new HashSet<Direction>();
            _getPossibleDirections();

        }

        private State(int rowsCount, int blankIndex, int[] content, Direction resultingMove, 
            HeuristicFunction distanceFunction, int LastGScore, int lastHScore, int tileMovedIndex)
            : this(rowsCount, blankIndex, content, resultingMove, distanceFunction)
        {


            DistanceFunction = distanceFunction;

            if (DistanceFunction == HeuristicFunction.ManhattenDistance || DistanceFunction == HeuristicFunction.BFS)
            {
                var tileMovedGoal = reshape(Content[tileMovedIndex] - 1);

                var tileOldManhatten = tileManhatten(reshape(blankIndex), tileMovedGoal);
                var tileNewManhatten = tileManhatten(reshape(tileMovedIndex), tileMovedGoal);

                // Only one tile changed position
                if (tileNewManhatten > tileOldManhatten)
                    HScore = lastHScore + 1;
                else if (tileNewManhatten < tileOldManhatten)
                    HScore = lastHScore - 1;
            }
            else if (DistanceFunction == HeuristicFunction.HammingDistance)
            {
                
                if (content[tileMovedIndex] == tileMovedIndex+1)
                    lastHScore = lastHScore - 1;
                if (blankIndex+1 == content[tileMovedIndex])
                    lastHScore = lastHScore + 1;
                if (blankIndex == Size-1)
                    lastHScore = lastHScore - 1;
                if (tileMovedIndex == Size - 1)
                    lastHScore = lastHScore + 1;
                HScore = lastHScore;
                

            }

            GScore = LastGScore + 1;
            FScore = HScore + GScore;


            _possibleDirections = new HashSet<Direction>();
            _getPossibleDirections();
        }


        private void _getPossibleDirections()
        {
            // Checking for top
            int landingIndex = BlankIndex - RowsCount;
            if (!(landingIndex < 0)) _possibleDirections.Add(Direction.Top);

            // Checking for bottom
            landingIndex = BlankIndex + RowsCount;
            if (!(landingIndex >= RowsCount * RowsCount)) _possibleDirections.Add(Direction.Bottom);

            // Checking for right
            if ((BlankIndex + 1) % RowsCount != 0) _possibleDirections.Add(Direction.Right);

            // Checking for left
            if (BlankIndex % RowsCount != 0) _possibleDirections.Add(Direction.Left);


            //Preventing going back to the previous state
            switch (_resultingMove)
            {
                case Direction.Top:
                    _possibleDirections.Remove(Direction.Bottom);
                    break;
                case Direction.Bottom:
                    _possibleDirections.Remove(Direction.Top);
                    break;
                case Direction.Right:
                    _possibleDirections.Remove(Direction.Left);
                    break;
                case Direction.Left:
                    _possibleDirections.Remove(Direction.Right);
                    break;
                case Direction.None:
                    break;
                default:
                    break;
            }
        }

        private State MoveBlank(Direction direction)
        {
            if (!_possibleDirections.Contains(direction)) throw new Exception($"Going {direction} is not accessible");

            int[] NewStateContent = (int[])Content.Clone();

            int landingIndex = 0;

            switch (direction)
            {
                case Direction.Top:
                    landingIndex = BlankIndex - RowsCount;
                    Utils.Swap(ref NewStateContent[BlankIndex], ref NewStateContent[landingIndex]);
                    break;
                case Direction.Bottom:
                    landingIndex = BlankIndex + RowsCount;
                    Utils.Swap(ref NewStateContent[BlankIndex], ref NewStateContent[landingIndex]);
                    break;
                case Direction.Right:
                    landingIndex = BlankIndex + 1;
                    Utils.Swap(ref NewStateContent[BlankIndex], ref NewStateContent[landingIndex]);
                    break;
                case Direction.Left:
                    landingIndex = BlankIndex - 1;
                    Utils.Swap(ref NewStateContent[BlankIndex], ref NewStateContent[landingIndex]);
                    break;
                default:
                    break;
            }

            return new State(RowsCount, landingIndex, NewStateContent, direction, DistanceFunction,
                GScore, HScore, BlankIndex)
                { _parent  = this};
        }

        private bool validMove(Direction direction)
        {
            return direction != _resultingMove;
        }

        public IEnumerable<State> GenerateChildren()
        {
            foreach (var direction in _possibleDirections)
                yield return this.MoveBlank(direction);
        }


        private int HscoreForManhattan()
        {
            int h = 0;
            for (int i = 0; i < Size; i++)
            {
                if (Content[i] == 0)
                    continue;
                Tuple<int, int> tileIndex = reshape(i);
                Tuple<int, int> tileGoal = reshape(Content[i] - 1);

                h = h + tileManhatten(tileGoal, tileIndex);
            }

            return h;
        }

        private int tileManhatten(Tuple<int, int> CurrentPosition, Tuple<int, int> RealPosition) =>
            Math.Abs(CurrentPosition.Item1 - RealPosition.Item1) + 
            Math.Abs(CurrentPosition.Item2 - RealPosition.Item2);

        private Tuple<int, int> reshape(int index)
        {
            int x = index / RowsCount;
            int y = index % RowsCount;
            
            return new Tuple<int, int>(x, y);
        }

        private int HscoreForHamming() // O(N^2)
        {
            int h = 0;
            for (int i = 0; i < Size - 1; i++)
            {

                if (Content[i] != i + 1)
                    h++;
            }
            if (Content[Size - 1] != 0)
                h++;
            return h;
        }

        public IEnumerable<State> GetPath() //TODO
        {
            var current = this;

            while (current != null)
            {
                yield return current;
                current = current._parent;
            }
        }

        public IEnumerable<Direction> GetMoves() //TODO
        {
            var current = this;

            while (current._resultingMove != Direction.None)
            {
                yield return current._resultingMove;
                current = current._parent;
            }
        }


        private int _blankRowPosition() =>
            RowsCount - BlankIndex / RowsCount;  // O(1)

        private int _inversions()
        {
            int inrvensions = 0;

            int n = RowsCount * RowsCount;
            for (int i = 0; i < n - 1; i++)
            {
                if (i == BlankIndex) continue;
                for (int j = i + 1; j < n; j++)
                {
                    if (j == BlankIndex) continue;

                    if (Content[i] > Content[j]) inrvensions++;
                }
            }

            return inrvensions;
        }


        public bool Solvable()
        {
            if (RowsCount % 2 == 0)
            {
                if (_blankRowPosition() % 2 == 0 && _inversions() % 2 != 0)
                    return true;

                if (_blankRowPosition() % 2 != 0 && _inversions() % 2 == 0)
                    return true;
            }
            else
            {
                if (_inversions() % 2 == 0) return true;
            }


            return false;
        }

        public override string ToString()
        {
            string stateStr = "";
            for (int i = 0; i < RowsCount * RowsCount; i++)
            {
                stateStr += Content[i].ToString() + ' ';

                if ((i + 1) % RowsCount == 0)
                    stateStr += '\n';
            }
            return stateStr;

        }

        public int Hash()
        {
            int hash = 193;

            for (int i = 0; i < Size; i++)
            {
                hash = hash * 59 + (Content[i]);
            }

            return hash;
        }
    }
}
