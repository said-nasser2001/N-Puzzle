
namespace N_Puzzle_Solver.Collections
{
    internal class PriorityQueue
    {
        State[] States;
        int size = 0;

        public PriorityQueue()
        {
            States = new State[100000000];
        }

        public int Count()
        {
            return size;
        }

        public State extractMin()
        {
            if (size == 0)
            {
                throw new InvalidOperationException("There is no elements in the queue.");
            }

            State minState = States[1];
            States[1] = States[size];
            size -= 1;
            minHeapify(1, size);
            return minState;
        }

        public void minHeapify(int index, int size)
        {
            int leftStateIndex = 2 * index;
            int rightStateIndex = 2 * index + 1;
            int minStateIndex;

            if (leftStateIndex <= size && States[leftStateIndex].FScore < States[index].FScore)
            {
                minStateIndex = leftStateIndex;
            }
            else
            {
                minStateIndex = index;
            }

            if (rightStateIndex <= size && States[rightStateIndex].FScore < States[minStateIndex].FScore)
            {
                minStateIndex = rightStateIndex;
            }

            if (minStateIndex != index)
            {
                swap(ref States[index], ref States[minStateIndex]);
                minHeapify(minStateIndex, size);
            }
        }
        public void swap(ref State firstState, ref State secondState)
        {
            State tempState = firstState;
            firstState = secondState;
            secondState = tempState;
        }

        public State dequeue()
        {
            return extractMin();
        }

        public void enqueue(State State)
        {
            insert(State);
        }

        public void insert(State State)
        {
            size += 1;
            States[size] = null;
            increaseKey(State, size);
        }

        public void increaseKey(State State, int size)
        {
            States[size] = State;
            while (size > 1 && States[size / 2].FScore >= States[size].FScore)
            {
                swap(ref States[size / 2], ref States[size]);
                size /= 2;
            }
        }
    }
}
