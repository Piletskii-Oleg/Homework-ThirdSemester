namespace Matrix;

/// <summary>
/// Class that allows you to perform certain operations on matrices.
/// </summary>
public class Matrix
{
    private static readonly Random RandomValue = new ();
    private readonly int[,] matrix;

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// </summary>
    /// <param name="height">Amount of rows in the matrix.</param>
    /// <param name="width">Amount of columns in the matrix.</param>
    public Matrix(int height, int width)
    {
        this.matrix = new int[height, width];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// </summary>
    /// <param name="matrix">A two-dimensional array to copy values from.</param>
    public Matrix(int[,] matrix)
    {
        this.matrix = matrix;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// </summary>
    /// <param name="path">Path to the file with the matrix.</param>
    public Matrix(string path)
    {
        this.matrix = ReadFromFile(path);
    }

    /// <summary>
    /// Gets amount of rows in the matrix.
    /// </summary>
    public int Height => this.matrix.GetLength(0);

    /// <summary>
    /// Gets amount of columns in the matrix.
    /// </summary>
    public int Width => this.matrix.GetLength(1);

    /// <summary>
    /// Gets element at position (<paramref name="verticalIndex"/>, <paramref name="horizontalIndex"/>).
    /// </summary>
    /// <param name="verticalIndex">The vertical component of the element position.</param>
    /// <param name="horizontalIndex">The horizontal component of the element position.</param>
    /// <returns>Element at position (<paramref name="verticalIndex"/>, <paramref name="horizontalIndex"/>).</returns>
    public int this[int verticalIndex, int horizontalIndex] => this.matrix[verticalIndex, horizontalIndex];

    /// <summary>
    /// Method to multiply two matrices sequentially.
    /// </summary>
    /// <param name="firstMatrix">First matrix.</param>
    /// <param name="secondMatrix">Second matrix.</param>
    /// <exception cref="IncompatibleMatrixSizesException">Throws if matrices cannot be multiplied.</exception>
    /// <returns>Matrix that is the result of multiplication of two given matrices.</returns>
    public static Matrix MultiplySequential(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.Width != secondMatrix.Height)
        {
            throw new IncompatibleMatrixSizesException();
        }

        var matrix = new Matrix(firstMatrix.Height, secondMatrix.Width);
        for (int i = 0; i < firstMatrix.Height; i++)
        {
            for (int j = 0; j < secondMatrix.Width; j++)
            {
                for (int k = 0; k < firstMatrix.Width; k++)
                {
                    matrix.matrix[i, j] += firstMatrix[i, k] * secondMatrix[k, j];
                }
            }
        }

        return matrix;
    }

    /// <summary>
    /// Method to multiply two matrices using parallel algorithm.
    /// </summary>
    /// <param name="firstMatrix">First matrix.</param>
    /// <param name="secondMatrix">Second matrix.</param>
    /// <exception cref="IncompatibleMatrixSizesException">Throws if matrices cannot be multiplied.</exception>
    /// <returns>Matrix that is the result of multiplication of two given matrices.</returns>
    public static Matrix MultiplyParallel(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.Width != secondMatrix.Height)
        {
            throw new IncompatibleMatrixSizesException();
        }

        var matrix = new Matrix(firstMatrix.Height, secondMatrix.Width);
        var threads = new Thread[Environment.ProcessorCount];
        int lineWidth = (int)Math.Ceiling((decimal)firstMatrix.Height / Environment.ProcessorCount);
        var maxRows = new int[threads.Length];
        for (int i = 0; i < threads.Length; i++)
        {
            maxRows[i] = Math.Clamp((i * lineWidth) + lineWidth, 0, firstMatrix.Height);
        }

        for (int count = 0; count < threads.Length; count++)
        {
            int localCount = count;
            threads[count] = new Thread(() =>
            {
                for (int i = localCount * lineWidth; i < maxRows[localCount]; i++)
                {
                    for (int j = 0; j < secondMatrix.Width; j++)
                    {
                        for (int k = 0; k < firstMatrix.Width; k++)
                        {
                            matrix.matrix[i, j] += firstMatrix[i, k] * secondMatrix[k, j];
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

        return matrix;
    }

    /// <summary>
    /// Generates a matrix with the given height and width and puts it to the given path.
    /// </summary>
    /// <param name="height">Amount of rows in the matrix.</param>
    /// <param name="width">Amount of columns in the matrix.</param>
    /// <param name="maxNumber">The biggest possible number in the generated matrix.</param>
    /// <returns>Generated matrix.</returns>
    public static Matrix Generate(int height, int width, int maxNumber)
    {
        var matrix = new Matrix(height, width);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                matrix.matrix[i, j] = RandomValue.Next(maxNumber);
            }
        }

        return matrix;
    }

    /// <summary>
    /// Writes the matrix to the file.
    /// </summary>
    /// <param name="path">Path to the file where matrix is to be written.</param>
    /// <param name="matrix">Matrix that should be written to the file.</param>
    public static void WriteToFile(string path, Matrix matrix)
    {
        using var writer = new StreamWriter(path);
        for (int i = 0; i < matrix.Height; i++)
        {
            for (int j = 0; j < matrix.Width; j++)
            {
                writer.Write(matrix[i, j]);
                if (j < matrix.Width - 1)
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
}
