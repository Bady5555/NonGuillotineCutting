using NonGuillotineCutting.Manager;
using NonGuillotineCutting.Model;

public class Program
{
    private static int START_COUNT = 1;
    private static int ITERRATION = 100;
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
            //Ищем 4 лучших решений - это будет 4 лучших решений от "Муравьев иссдедователей"
            Solution bestSolution = hillClimbing.Solve(4);
            var antExplorers = hillClimbing.AntExplorers;

            // К каждому из "Хороших" решений будем добавлять новые прямоугольники для раскроя
            // это будут "муравьи последователи" которые будет пытаться улучшить существующие лучшие решения
            for (int ii = 0; ii < antExplorers.Count; ii++)
            {
                //Берем начальный набор данных
                Job newJob = (Job)job.Clone();
                var rects = newJob.Rects.ToList();

                //Это массив обьектов которые еще не учавствуют в решении - это и есть муравьи последователи
                var rectsForAdding = new List<Rect>();

                //Из этого набора данных убираем те прямоугольники которые уже задействованны в решении
                foreach (var item in rects)
                {
                    bool found = false;
                    foreach (var itemInt in antExplorers[ii])
                    {
                        if (item.H == itemInt.Height && item.W == itemInt.Width)
                        { 
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        rectsForAdding.Add(item);
					}
                }

				//Пытаемся улучшить текущее решение и сравниваем с уже имеющимся лучшим решением
				//Для этого создадим обьект Solution в который поместим текущее хорошее решение
				Solution newSolution = new Solution();
                foreach (var item in antExplorers[ii])
                {
					newSolution.ReservRectangles.Add(item);
				}

                for (int j = 0; j < rectsForAdding.Count; j++)
                {
                    Solution processingSolution = (Solution)newSolution.Clone();

					//Вконец добавим еще один прямоугольник из последователей
					processingSolution.ReservRectangles.Add(new Rectangle(rectsForAdding[j].W, rectsForAdding[j].H));

                    for (int jj = 0; jj < ITERRATION; jj++)
                    {
                        //Перемешаем новый исходный набор
                        processingSolution.MixSourceRect();

						// проверим, получиться ли разложить все текущие исходные данные
						var nSol = hillClimbing.GenerateNeighborSolution(processingSolution);
					    if (nSol.Score > bestSolution.Score)
					    {
						    bestSolution = nSol;
						    Console.WriteLine($"Improved Best score: {bestSolution.Score}");
					    }
					}
				}
			}

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
        //Console.WriteLine($"Worst result: {worstResult}");
        //Console.WriteLine($"Difference result: {bestResult - worstResult}");

        pltMain.AddScatter(xs, ys);
        pltMain.SaveFig("AllSolution.png");

        var val = sol.Select(x => (double)x.Value).ToArray();
        var pos = sol.Select(x => (double)x.Key).ToArray();
        var bar = pltCountSolution.AddBar(val, pos);
        bar.BarWidth = pos.Length * 10;
        pltCountSolution.SaveFig("CountSolution.png");
    }
}
