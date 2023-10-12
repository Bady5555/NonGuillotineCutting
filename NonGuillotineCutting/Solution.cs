using NonGuillotineCutting.Utils;
using System.Security.Cryptography;

public class Solution : ICloneable
{
    public List<Rectangle> ReservRectangles { get; set; }
    public List<Rectangle> Rectangles { get; set; }
    public int Score { get; set; }

    public Solution()
    {
        Rectangles = new List<Rectangle>();
        ReservRectangles = new List<Rectangle>();
        Score = 0;
    }

    public object Clone()
    {
        Solution newSolution = new Solution();
        newSolution.Score = Score;
        foreach (var item in Rectangles)
        {
            newSolution.Rectangles.Add(new Rectangle(item.Width, item.Height) { X = item.X, Y = item.Y });
        }
        foreach (var item in ReservRectangles)
        {
            newSolution.ReservRectangles.Add(new Rectangle(item.Width, item.Height) { X = item.X, Y = item.Y });
        }
        return newSolution;
    }

    internal void MixSourceRect()
    {
        ReservRectangles.Shuffle();
    }
}
