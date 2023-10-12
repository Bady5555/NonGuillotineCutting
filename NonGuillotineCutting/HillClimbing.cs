using NonGuillotineCutting.Model;

public class HillClimbing
{
    public Job Job { get; set; }

    public HillClimbing(Job job) 
    {
        Job = job; 
    }
    public Solution Solve(int iterations)
    {
        Solution startSolution = GenerateInitialSolution();
        Solution bestSolution = startSolution;
        //int startScore = startSolution.Score;
        Console.WriteLine($"Score: {startSolution.Score}");
        for (int i = 0; i < iterations; i++)
        {
            bestSolution.MixSourceRect();
            Solution neighbor = GenerateNeighborSolution(bestSolution);
            //Console.WriteLine($"Score: {neighbor.Score}");
            if (neighbor.Score > bestSolution.Score)
            {
                bestSolution = neighbor;
                Console.WriteLine($"Best score: {bestSolution.Score}");
            }
        }
        float perc = 100 * (float)bestSolution.Score / (float)startSolution.Score;
        Console.WriteLine($"Percent remainder: {String.Format("{0:N2}", perc)}");
        return bestSolution;
    }

    private Solution GenerateInitialSolution()
    {
        Solution initialSolution = new Solution();
        for (int i = 0; i < Job.Rects.Length; i++) 
        {
            int width = Job.Rects[i].W; 
            int height = Job.Rects[i].H;
            int x = 0;// random.Next(0, maxWidth - width); 
            int y = 0;//random.Next(0, maxHeight - height);

            Rectangle rectangle = new Rectangle(width, height);
            rectangle.X = x;
            rectangle.Y = y;

            initialSolution.ReservRectangles.Add(rectangle);
        }

        initialSolution.Score = -Job.Bin.W * Job.Bin.H;

        return initialSolution;
    }


    private Solution GenerateNeighborSolution(Solution currentSolution)
    {
        Solution neighborSolution = new Solution();
        //int i = 0;

        //while (neighborSolution.ReservRectangles.Count > 0 && i < neighborSolution.ReservRectangles.Count)
        //{
        //    var newRect = neighborSolution.ReservRectangles[i];
        //    //Rectangle newRect = new Rectangle(rect.Width, rect.Height);

        //    int xOffset = random.Next(-Job.Bin.W - newRect.Width, Job.Bin.W - newRect.Width);
        //    int yOffset = random.Next(-Job.Bin.H - newRect.Height, Job.Bin.H - newRect.Height);

        //    //Console.WriteLine($"XOffset:{xOffset}  YOffset:{yOffset}");

        //    newRect.X = Math.Max(0, Math.Min(newRect.X + xOffset, Job.Bin.W - newRect.Width));
        //    newRect.Y = Math.Max(0, Math.Min(newRect.Y + yOffset, Job.Bin.H - newRect.Height));

        //    if (i == 0) Console.WriteLine($"Item 0: W:{newRect.Width} H:{newRect.Height} X:{newRect.X} Y:{newRect.Y}");

        //    // Check for intersection with existing rectangles
        //    bool intersects = neighborSolution.Rectangles.Any(existingRect =>
        //        !(newRect.X + newRect.Width <= existingRect.X || existingRect.X + existingRect.Width <= newRect.X ||
        //          newRect.Y + newRect.Height <= existingRect.Y || existingRect.Y + existingRect.Height <= newRect.Y));

        //    if (!intersects)
        //    {
        //        neighborSolution.Rectangles.Add(newRect);
        //        neighborSolution.ReservRectangles.Remove(newRect);
        //        continue;
        //    }
        //    i++;
        //}

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
