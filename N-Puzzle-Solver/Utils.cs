using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace N_Puzzle_Solver
{
    public static class Utils
    {   
        public static void Swap(ref int x, ref int y)
        {
            int temp = x;
            x = y;
            y = temp;
        }

        public static State FetchPuzzle(string path, HeuristicFunction function)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                int currTileIndex = 0;

                int rowsCount = int.Parse(sr.ReadLine());
                int[] puzzle = new int[rowsCount * rowsCount];
                int blankIndex = 0;

                _ = sr.ReadLine(); //Blank line

                
                for (int i = 0; i < rowsCount; i++)
                {
                    string rowStr = sr.ReadLine().Trim();
                    foreach (var tileStr in rowStr.Split(' '))
                    {
                        int tile = int.Parse(tileStr);
                        if (tile == 0)
                        {
                            blankIndex = currTileIndex;
                        }
                        puzzle[currTileIndex++] = tile;
                    }
                }

                return new State(rowsCount,blankIndex,puzzle, function);
            }

        }
    }
}
