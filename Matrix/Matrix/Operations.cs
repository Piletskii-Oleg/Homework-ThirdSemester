namespace Matrix;

/// <summary>
/// Class that allows you to perform certain operations on matrices.
/// </summary>
public static class Operations
{
    /// <summary>
    /// Method to multiply two matrices sequentially.
    /// </summary>
    /// <param name="path1">Path to the first matrix.</param>
    /// <param name="path2">Path to the second matrix.</param>
    /// <param name="outputPath">Path to the output file.</param>
    /// <returns>A matrix that is the product of two given matrices.</returns>
    /// <exception cref="IncompatibleMatrixSizesException">Throws if matrices cannot be multiplied.</exception>
    public static int[,] MultiplySequential(string path1, string path2, string outputPath)
    {
        var matrix1 = ReadFromFile(path1);
        var matrix2 = ReadFromFile(path2);
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

        WriteToFile(outputPath, resultMatrix);
        return resultMatrix;
    }

    /// <summary>
    /// Method to multiply two matrices using parallel algorithm.
    /// </summary>
    /// <param name="path1">Path to the first matrix.</param>
    /// <param name="path2">Path to the second matrix.</param>
    /// <param name="outputPath">Path to the output file.</param>
    /// <returns>A matrix that is the product of two given matrices.</returns>
    /// <exception cref="IncompatibleMatrixSizesException">Throws if matrices cannot be multiplied.</exception>
    public static int[,] MultiplyParallel(string path1, string path2, string outputPath)
    {
        var matrix1 = ReadFromFile(path1);
        var matrix2 = ReadFromFile(path2);
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
            maxRows[i] = Math.Clamp((i * lineWidth) + lineWidth, 0, matrix1.GetLength(0));
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

        WriteToFile(outputPath, resultMatrix);
        return resultMatrix;
    }

    /// <summary>
    /// Generates a matrix with the given height and width and puts it to the given path.
    /// </summary>
    /// <param name="height">Amount of rows in the matrix.</param>
    /// <param name="width">Amount of columns in the matrix.</param>
    /// <param name="path">Path to the output.</param>
    public static void Generate(int height, int width, string path)
    {
        using var writer = new StreamWriter(File.Create(path));
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

    private static int[,] ReadFromFile(string path)
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

    private static void WriteToFile(string path, int[,] matrix)
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
