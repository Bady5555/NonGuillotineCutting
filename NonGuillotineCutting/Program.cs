using NonGuillotineCutting.Manager;

public class Program
{
    private static int START_COUNT = 50;
    public static void Main()
    {
        double[] xs = new double[START_COUNT];
        double[] ys = new double[START_COUNT];
        int bestResult = -10000000;
        int worstResult = 0;
        Dictionary<double, double> sol = new Dictionary<double, double>();

        var pltMain = new ScottPlot.Plot(600, 600);
        var pltCountSolution = new ScottPlot.Plot(600, 600);
        for (int i = 0; i < START_COUNT; i++)
        {
            // run algoritm
            var job = DataParser.BuildJob("gcut2.txt");
            HillClimbing hillClimbing = new HillClimbing(job);
            Solution bestSolution = hillClimbing.Solve(iterations: 1000);

            // shown result
            Console.WriteLine("Best Solution Score: " + bestSolution.Score);
            foreach (Rectangle rectangle in bestSolution.Rectangles)
            {
                Console.WriteLine($"Rectangle: Width={rectangle.Width}, Height={rectangle.Height}, X={rectangle.X}, Y={rectangle.Y}");
            }

            // builr graph of count best solution
            if (!sol.Any(s => s.Key.Equals((double)bestSolution.Score)))
            {
                sol.Add(bestSolution.Score, 1);
            }
            else
            {
                sol[(double)bestSolution.Score] += 1;
            }

            // save best and worst result
            bestResult = Math.Max(bestResult, bestSolution.Score);
            worstResult = Math.Min(worstResult, bestSolution.Score);

            // build graph of each best solution
            xs[i] = i;
            ys[i] = bestSolution.Score;
        }
        Console.WriteLine($"The Best result: {bestResult}");
        Console.WriteLine($"Worst result: {worstResult}");
        Console.WriteLine($"Difference result: {bestResult - worstResult}");

        pltMain.AddScatter(xs, ys);
        pltMain.SaveFig("AllSolution.png");

        var val = sol.Select(x => (double)x.Value).ToArray();
        var pos = sol.Select(x => (double)x.Key).ToArray();
        var bar = pltCountSolution.AddBar(val, pos);
        bar.BarWidth = pos.Length * 10;
        pltCountSolution.SaveFig("CountSolution.png");
    }
}
