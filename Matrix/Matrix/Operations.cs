namespace Matrix;

public static class Operations
{
    public static int[,] MultiplySequential(string path1, string path2, string outputPath)
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

        WriteMatrixToFile(outputPath, resultMatrix);
        return resultMatrix;
    }

    public static int[,] MultiplyParallel(string path1, string path2, string outputPath)
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
            maxRows[i] = Math.Clamp(i * lineWidth + lineWidth, 0, matrix1.GetLength(0));
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

        WriteMatrixToFile(outputPath, resultMatrix);
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

    private static void WriteMatrixToFile(string path, int[,] matrix)
    {
        using var writer = new StreamWriter(path);
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                writer.Write(matrix[i, j]);
                if (j < matrix.GetLength(1) - 1)
                {
                    writer.Write(" ");
                }
            }

            writer.Write("\n");
        }
    }
}
