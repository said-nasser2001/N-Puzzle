using N_Puzzle_Solver.Collections;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace N_Puzzle_Solver
{

    public static class Solver
    {
        public static void PrioritySolve(ref State state)
        {
            PriorityQueue queue = new PriorityQueue();

            queue.enqueue(state);

            int dequeus = 0;
            while (state.HScore > 0)
            {

                State.visitedNodes.Add(state.Hash());
                state = queue.dequeue();
                dequeus++;

                foreach (var child in state.GenerateChildren())
                {
                    if(!State.visitedNodes.Contains(child.Hash()))
                        queue.enqueue(child);
                }
            }

            Console.WriteLine($"#Dequeues: {dequeus}");
        }

        public static void BFsSolve(ref State state)
        {
            Queue<State> queue = new Queue<State>();

            queue.Enqueue(state);

            int dequeus = 0;
            while (state.HScore > 0)
            {

                State.visitedNodes.Add(state.Hash());
                state = queue.Dequeue();
                dequeus++;

                foreach (var child in state.GenerateChildren())
                {
                    if (!State.visitedNodes.Contains(child.Hash()))
                        queue.Enqueue(child);
                }
            }

            Console.WriteLine($"#Dequeues: {dequeus}");
        }
    }
}
