namespace Matrix;

public static class Operations
{
    public static int[,] MultiplyConsecutive(string path1, string path2)
    {
        var matrix1 = ReadMatrixFromFile(path1);
        var matrix2 = ReadMatrixFromFile(path2);
        if (matrix1.GetLength(1) != matrix2.GetLength(0))
        {
            throw new IncompatibleMatrixSizesException();
        }

        var resultMatrix = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
        for (int i = 0; i < matrix1.GetLength(0); i++)
        {
            for (int j = 0; j < matrix2.GetLength(1); j++)
            {
                for (int k = 0; k < matrix1.GetLength(1); k++)
                {
                    resultMatrix[i, j] += matrix1[i, k] * matrix2[k, j];
                }
            }
        }

        return resultMatrix;
    }

    public static void MultiplyParallel(string path1, string path2)
    {
        var matrix1 = ReadMatrixFromFile(path1);
        var matrix2 = ReadMatrixFromFile(path2);
        if (matrix1.GetLength(1) != matrix2.GetLength(0))
        {
            throw new IncompatibleMatrixSizesException();
        }

        var resultMatrix = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
        var threads = new Thread[matrix1.GetLength(0) * matrix2.GetLength(1)];
        int count = 0;
        for (int i = 0; i < matrix1.GetLength(0); i++)
        {
            for (int j = 0; j < matrix2.GetLength(1); j++)
            {
                var (localI, localJ) = (i, j);
                threads[count] = new Thread(() =>
                {
                    for (int k = 0; k < matrix1.GetLength(1); k++)
                    {
                        resultMatrix[localI, localJ] += matrix1[localI, k] * matrix2[k, localJ];
                    }
                });
                count++;
            }
        }

        foreach(var thread in threads)
        {
            thread.Start();
        }

        foreach(var thread in threads)
        {
            thread.Join();
        }
    }

    private static int[,] ReadMatrixFromFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        int height = lines.Length;
        int width = lines[0].Split(" ").Length;
        var matrix = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            var line = lines[i].Split(" ");
            for (int j = 0; j < width; j++)
            {
                matrix[i, j] = Convert.ToInt32(line[j]);
            }
        }

        return matrix;
    }
}
