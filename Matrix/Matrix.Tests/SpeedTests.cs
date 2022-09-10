namespace Matrix.Tests;

using System.Diagnostics;

public class SpeedTests
{
    [Test]
    public void CompareTimes()
    {
        int width = 600;
        int height = 610;
        long[] parallelTimes = new long[10];
        for (int i = 0; i < 10; i++)
        {
            GenerateMatrix(height, width, "hello");
            GenerateMatrix(width, height, "goodbye");
            string path1 = "../../../TestFiles/hello.txt";
            string path2 = "../../../TestFiles/goodbye.txt";

            var watch = new Stopwatch();
            watch.Start();
            Operations.MultiplyParallel3(path1, path2);
            watch.Stop();
            parallelTimes[i] = watch.ElapsedMilliseconds;
        }

        long[] consecutiveTimes = new long[10];
        for (int i = 0; i < 10; i++)
        {
            GenerateMatrix(height, width, "hello");
            GenerateMatrix(width, height, "goodbye");
            string path1 = "../../../TestFiles/hello.txt";
            string path2 = "../../../TestFiles/goodbye.txt";

            var watch = new Stopwatch();
            watch.Start();
            Operations.MultiplyConsecutive(path1, path2);
            watch.Stop();
            consecutiveTimes[i] = watch.ElapsedMilliseconds;
        }

        //long[] parallel2Times = new long[10];
        //for (int i = 0; i < 10; i++)
        //{
        //    GenerateMatrix(height, width, "hello");
        //    GenerateMatrix(width, height, "goodbye");
        //    string path1 = "../../../TestFiles/hello.txt";
        //    string path2 = "../../../TestFiles/goodbye.txt";

        //    var watch = new Stopwatch();
        //    watch.Start();
        //    Operations.MultiplyParallel2(path1, path2);
        //    watch.Stop();
        //    parallel2Times[i] = watch.ElapsedMilliseconds;
        //}
        long expectationParallel = 0;
        long expectationConsecutive = 0;
        for (int i = 0; i < 10; i++)
        {
            expectationParallel += parallelTimes[i];
            expectationConsecutive += consecutiveTimes[i];
        }

        expectationParallel /= 10;
        expectationConsecutive /= 10;
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
