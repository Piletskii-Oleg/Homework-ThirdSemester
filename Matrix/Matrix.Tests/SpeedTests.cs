namespace Matrix.Tests;

using System.Diagnostics;

public class SpeedTests
{
    [Test]
    public void CompareTimes()
    {
        long parallelTime = 0;
        for (int i = 0; i < 10; i++)
        {
            GenerateMatrix(35, 30, "hello");
            GenerateMatrix(30, 40, "goodbye");
            string path1 = "../../../TestFiles/hello.txt";
            string path2 = "../../../TestFiles/goodbye.txt";

            var watch = new Stopwatch();
            watch.Start();
            Operations.MultiplyParallel(path1, path2);
            watch.Stop();
            parallelTime += watch.ElapsedMilliseconds;
        }

        long consecutiveTime = 0;
        for (int i = 0; i < 10; i++)
        {
            GenerateMatrix(120, 150, "hello");
            GenerateMatrix(150, 120, "goodbye");
            string path1 = "../../../TestFiles/hello.txt";
            string path2 = "../../../TestFiles/goodbye.txt";

            var watch = new Stopwatch();
            watch.Start();
            Operations.MultiplyConsecutive(path1, path2);
            watch.Stop();
            consecutiveTime += watch.ElapsedMilliseconds;
        }
    }

    private void GenerateMatrix(int height, int width, string fileName)
    {
        using var writer = new StreamWriter($"../../../TestFiles/{fileName}.txt");
        var random = new Random();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                writer.Write(random.Next(500));
                if (j < width - 1)
                {
                    writer.Write(" ");
                }
            }
            
            writer.Write("\n");
        }
    }
}
