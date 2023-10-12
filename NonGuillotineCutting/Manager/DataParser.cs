using NonGuillotineCutting.Model;

namespace NonGuillotineCutting.Manager
{
    public static class DataParser
    {
        public static Job BuildJob(string fileName)
        { 
            Job job = new Job();
            var text = File.ReadAllText("SourceData/" + fileName);
            var arr = text.Split('\n');
            var row1 = arr[0].Trim().Split(' ');
            job.Bin = new Bin() { H = int.Parse(row1[0]), W = int.Parse(row1[1]) };
            job.Rects = new Rect[arr.Length - 2];
            int ii = 0;
            for (int i = 1; i < arr.Length - 1; i++)
            {
                var itemRow = arr[i].Trim().Split(' ');
                job.Rects[ii] = new Rect() { H = int.Parse(itemRow[0]), W = int.Parse(itemRow[1]) };
                ii++;
            }
            return job;
        }
    }
}
