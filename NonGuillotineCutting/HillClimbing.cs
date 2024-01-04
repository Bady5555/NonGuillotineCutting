using NonGuillotineCutting.Model;

public class HillClimbing
{

    internal List<List<Rectangle>> AntExplorers = new List<List<Rectangle>>();

	public Job Job { get; set; }

    public HillClimbing(Job job) 
    {
        Job = job; 
    }
    public Solution Solve(int antExplorersLimit)
    {
        var plt = new ScottPlot.Plot(600, 600);
        double[] xs = new double[antExplorersLimit];
        double[] ys = new double[antExplorersLimit];

        Solution startSolution = GenerateInitialSolution();
        Solution bestSolution = startSolution;

        Console.WriteLine($"Score: {startSolution.Score}");
        while (AntExplorers.Count <= antExplorersLimit)
        {
            bestSolution.MixSourceRect();
            Solution neighbor = GenerateNeighborSolution(bestSolution);
            //Console.WriteLine($"Score: {neighbor.Score}");
            if (neighbor.Score > bestSolution.Score)
            {
                bestSolution = neighbor;
                Console.WriteLine($"AntExplorers Best score: {bestSolution.Score}");
                //ys[i] = bestSolution.Score;
                AntExplorers.Add(bestSolution.Rectangles);
            }

            //xs[i] = i; 
        }
        float perc = 100 * (float)bestSolution.Score / (float)startSolution.Score;
        Console.WriteLine($"Percent remainder: {String.Format("{0:N2}", perc)}");
        
        plt.AddScatter(xs, ys);
        plt.SaveFig("Solution.png");
        
        return bestSolution;
    }

	private Solution GenerateInitialSolution()
    {
        Solution initialSolution = new Solution();
        for (int i = 0; i < Job.Rects.Length; i++) 
        {
            int width = Job.Rects[i].W; 
            int height = Job.Rects[i].H;
            int x = 0;
            int y = 0;

            Rectangle rectangle = new Rectangle(width, height);
            rectangle.X = x;
            rectangle.Y = y;

            initialSolution.ReservRectangles.Add(rectangle);
        }

        initialSolution.Score = -Job.Bin.W * Job.Bin.H;

        return initialSolution;
    }


    public Solution GenerateNeighborSolution(Solution currentSolution)
    {
        Solution neighborSolution = new Solution();

        int currentRowHeight = 0;
        int currentY = 0;
        foreach (var rect in currentSolution.ReservRectangles)
        {
            neighborSolution.ReservRectangles.Add(rect);

            Rectangle newRect = new Rectangle(rect.Width, rect.Height);
            if (neighborSolution.Rectangles.Count == 0)
            {
                newRect.X = 0;
                newRect.Y = 0;
                currentRowHeight = newRect.Height;
                neighborSolution.Rectangles.Add(newRect);
                //Console.WriteLine($"Item: W:{newRect.Width} H:{newRect.Height} X:{newRect.X} Y:{newRect.Y}");
            }
            else
            {
                var currentRow = neighborSolution.Rectangles.Where(x => x.Y == currentY);
                int currentX = currentRow.Sum(x => x.X) + currentRow.Last().Width;
                if (rect.Width <= Job.Bin.W - currentX 
                    && Job.Bin.H - Math.Max(newRect.Height, currentRowHeight) - currentY >= 0) // поместился в строку и в столбец
                {
                    newRect.X = currentX;
                    newRect.Y = currentY;
                    currentRowHeight = Math.Max(newRect.Height, currentRowHeight);
                    neighborSolution.Rectangles.Add(newRect);
                    //Console.WriteLine($"Item: W:{newRect.Width} H:{newRect.Height} X:{newRect.X} Y:{newRect.Y}");
                }
                else // не поместился в строку
                {
                    int currentY_Temp = neighborSolution.Rectangles.Where(x => x.X == 0).Sum(x => x.Y) + currentRowHeight;
                    if (newRect.Height + currentY_Temp <= Job.Bin.H && currentX != 0) // первый элемент в новом столбце и поместился в столбец
                    {
                        currentY = currentY_Temp;
                        currentX = 0;
                        newRect.X = currentX;
                        newRect.Y = currentY;
                        neighborSolution.Rectangles.Add(newRect);
                        currentRowHeight = newRect.Height;
                        //Console.WriteLine($"Item: W:{newRect.Width} H:{newRect.Height} X:{newRect.X} Y:{newRect.Y}");
                    }
                }
            }

        }

        neighborSolution.Score = CalculateScore(neighborSolution, Job.Bin.W, Job.Bin.H);

        return neighborSolution;
    }

    private int CalculateScore(Solution solution, int maxWidth, int maxHeight)
    {
        int cuttingArea = maxWidth * maxHeight;
        int usedArea = solution.Rectangles.Sum(rect => rect.Width * rect.Height);
        int totalArea = cuttingArea - usedArea;

        return -totalArea;
    }
}
