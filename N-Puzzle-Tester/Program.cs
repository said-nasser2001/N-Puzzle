using N_Puzzle_Solver;
using System.Diagnostics;

const string TEST_CASES_DIRECTORY = "../../../Testcases",
            SAMPLE_SOLVABLES = TEST_CASES_DIRECTORY + "/Sample/Solvable puzzles/",
            SAMPLE_UNSOLVABLES = TEST_CASES_DIRECTORY + "/Sample/Unsolvable Puzzles/",

            COMPLETE_SOLVABLES_MH = TEST_CASES_DIRECTORY + "/Complete/Solvable Puzzles/Manhattan & Hamming/",
            COMPLETE_SOLVABLES_M = TEST_CASES_DIRECTORY + "/Complete/Solvable Puzzles/Manhattan Only/",
            COMPLETE_UNSOLVABLES = TEST_CASES_DIRECTORY + "/Complete/Unsolvable Puzzles/",

            V_LARGE = TEST_CASES_DIRECTORY + "/Complete/V. Large test case/TEST.txt";


entry:

bool is_vl = false;
string path;


HeuristicFunction function = HeuristicFunction.ManhattenDistance;
int test_choice = 0;

do
{
    test_choice = 0;

    Console.Clear();

    Console.WriteLine("Choose a test-set to run");
    Console.WriteLine("[1]: Sample");
    Console.WriteLine("[2]  Complete");

    try { test_choice = int.Parse(Console.ReadLine()); }
    catch (Exception) { }

} while (!new int[] { 1, 2 }.Contains(test_choice));

if (test_choice == 1)
{
    path = SAMPLE_SOLVABLES;

    getHeuristicFunction(ref function);
}
else
{
    do
    {
        Console.Clear();

        Console.WriteLine("Choose a sub-test-set to run");
        Console.WriteLine("[1]: Manhatten & Hamming");
        Console.WriteLine("[2]  Manhaten only");
        Console.WriteLine("[3]  V_Large");
        Console.WriteLine("\n[4]  < Back");

        try { test_choice = int.Parse(Console.ReadLine()); }
        catch (Exception) { }

    } while (!new int[] { 1, 2, 3, 4 }.Contains(test_choice));

    if (test_choice == 1)
    {
        path = COMPLETE_SOLVABLES_MH;

        getHeuristicFunction(ref function);
    }
    else if (test_choice == 2)
    {
        path = COMPLETE_SOLVABLES_M;
    }
    else if (test_choice == 3)
    {
        is_vl = true;
        path = V_LARGE;
    }
    else goto entry;
    
}


Console.Clear();

Console.WriteLine($"======== {path.Replace(TEST_CASES_DIRECTORY, "")} ========\n");

if (is_vl)
{
    getSolveTime(path, function);
}
else
{
    var files = Directory.GetFiles(path);

    foreach (var file in files)
        getSolveTime(file, function);
}

Console.Write("\nPress [y] if you want to continue: ");

try { if (Console.ReadLine().ToLower() == "y") goto entry; }
catch(Exception) { }




void getHeuristicFunction(ref HeuristicFunction function)
{
    int heuristicChoice = 0;
    do
    {
        Console.Clear();

        Console.WriteLine("Choose a Heuristic function");
        Console.WriteLine("[1]: Manhatten distance");
        Console.WriteLine("[2]  Hamming distance");
        Console.WriteLine("[3]  BFS");


        try { heuristicChoice = int.Parse(Console.ReadLine()); }
        catch (Exception) { }

    } while (!new int[] { 1, 2,3 }.Contains(heuristicChoice));

    if (heuristicChoice == 1) function = HeuristicFunction.ManhattenDistance;
    else if (heuristicChoice == 2) function = HeuristicFunction.HammingDistance;
    else if (heuristicChoice == 3) function = HeuristicFunction.BFS;
}


void getSolveTime(string fileName, HeuristicFunction fn)
{
    Console.WriteLine(fileName);
    Stopwatch watch = new Stopwatch();
    watch.Start();

    var state = Utils.FetchPuzzle(fileName, fn);
    if (fn == HeuristicFunction.BFS)
        Solver.BFsSolve(ref state);
    else
        Solver.PrioritySolve(ref state);
    watch.Stop();

    State.visitedNodes.Clear();

    Console.WriteLine($"#Levels: {state.GScore}");
    Console.WriteLine($"Elapsed time: {Math.Ceiling((double)watch.ElapsedMilliseconds / 1000)}sec");
    Console.WriteLine("-------------");
}