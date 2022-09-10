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

    public static int[,] MultiplyParallel(string path1, string path2)
    {
        var matrix1 = ReadMatrixFromFile(path1);
        var matrix2 = ReadMatrixFromFile(path2);
        if (matrix1.GetLength(1) != matrix2.GetLength(0))
        {
            throw new IncompatibleMatrixSizesException();
        }

        var resultMatrix = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
        var threads = new Thread[matrix1.GetLength(0)];
        int count = 0;
        for (int i = 0; i < matrix1.GetLength(0); i++)
        {
            var localI = i;
            threads[count] = new Thread(() =>
            {
                for (int j = 0; j < matrix2.GetLength(1); j++)
                {
                    for (int k = 0; k < matrix1.GetLength(1); k++)
                    {
                        resultMatrix[localI, j] += matrix1[localI, k] * matrix2[k, j];
                    }
                }
            });
            count++;
        }

        foreach(var thread in threads)
        {
            thread.Start();
        }

        foreach(var thread in threads)
        {
            thread.Join();
        }

        return resultMatrix;
    }

    public static int[,] MultiplyParallel3(string path1, string path2)
    {
        var matrix1 = ReadMatrixFromFile(path1);
        var matrix2 = ReadMatrixFromFile(path2);
        if (matrix1.GetLength(1) != matrix2.GetLength(0))
        {
            throw new IncompatibleMatrixSizesException();
        }

        var resultMatrix = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
        var threads = new Thread[8];
        int lineWidth = (int)Math.Ceiling((decimal)matrix1.GetLength(0) / 8);
        var maxRows = new int[8];
        for (int i = 0; i < 7; i++)
        {
            maxRows[i] = i * lineWidth + lineWidth;
        }

        maxRows[7] = matrix1.GetLength(0);
        for (int count = 0; count < 8; count++)
        {
            int localCount = count;
            threads[count] = new Thread(() =>
            {
                for (int i = localCount * lineWidth; i < maxRows[localCount]; i++)
                {
                    for (int j = 0; j < matrix2.GetLength(1); j++)
                    {
                        for (int k = 0; k < matrix1.GetLength(1); k++)
                        {
                            resultMatrix[i, j] += matrix1[i, k] * matrix2[k, j];
                        }
                    }
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return resultMatrix;
    }

    public static int[,] MultiplyParallel2(string path1, string path2)
    {
        var matrix1 = ReadMatrixFromFile(path1);
        var matrix2 = ReadMatrixFromFile(path2);
        if (matrix1.GetLength(1) != matrix2.GetLength(0))
        {
            throw new IncompatibleMatrixSizesException();
        }

        var resultMatrix = new int[matrix1.GetLength(0), matrix2.GetLength(1)];
        var threads = new Thread[8];
        int count = 0;
        for (int i = 0; i < matrix1.GetLength(0); i++)
        {
            var localI = i;
            threads[count] = new Thread(() =>
            {
                for (int j = 0; j < matrix2.GetLength(1); j++)
                {
                    for (int k = 0; k < matrix1.GetLength(1); k++)
                    {
                        resultMatrix[localI, j] += matrix1[localI, k] * matrix2[k, j];
                    }
                }
            });
            count++;
            if (count % 8 == 0 && count > 0)
            {
                count = 0;
                foreach (var thread in threads)
                {
                    thread.Start();
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }
            }
        }

        for (int i = 0; i < count; i++)
        {
            threads[i].Start();
        }

        for (int i = 0; i < count; i++)
        {
            threads[i].Join();
        }

        return resultMatrix;
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
