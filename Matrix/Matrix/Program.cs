Console.WriteLine("Программа: умножение матриц.");
int heightFirst = 0;
Console.Write("Введите количество строк в первой матрице: ");
while (!int.TryParse(Console.ReadLine(), out heightFirst))
{
    Console.Write("Введите количество строк в первой матрице: ");
}

int widthFirst = 0;
Console.Write("Введите количество столбцов в первой матрице: ");
while (!int.TryParse(Console.ReadLine(), out widthFirst))
{
    Console.Write("Введите колиество столбцов в первой матрице: ");
}

int heightSecond = 0;
Console.Write("Введите количество строк во второй матрице: ");
while (!int.TryParse(Console.ReadLine(), out heightSecond) || heightSecond != widthFirst)
{
    if (heightSecond != widthFirst)
    {
        Console.WriteLine("Количество строк во второй матрице должно совпадать с количеством столбцов в первой.");
    }

    Console.Write("Введите количество строк во второй матрице: ");
}

Console.Write("Введите колиество столбцов во второй матрице: ");
int widthSecond = 0;
while (!int.TryParse(Console.ReadLine(), out widthSecond))
{
    Console.Write("Введите количество столбцов во второй матрице: ");
}

Console.WriteLine("Нужно ли сравнить скорость параллельного и последовательного умножения?");
Console.WriteLine("Введите \"Да\" или \"Нет\"");
string? compareWord = Console.ReadLine();
while (compareWord != "Да" && compareWord != "Нет")
{
    Console.WriteLine("Нужно ли сравнить скорость параллельного и последовательного умножения?");
}

if (compareWord == "Да")
{
    Console.WriteLine();
    Console.WriteLine("Параллельные вычисления: ");
    ComputeTimes(heightFirst, widthFirst, heightSecond, widthSecond, Matrix.Operations.MultiplyParallel);

    Console.WriteLine();
    Console.WriteLine("Последовательные вычисления: ");
    ComputeTimes(heightFirst, widthFirst, heightSecond, widthSecond, Matrix.Operations.MultiplyConsecutive);
}
else
{

}



void ComputeTimes(int heightFirst, int widthFirst, int heightSecond, int widthSecond, Func<string, string, string, int[,]> Multiply)
{
    long[] data = new long[10];
    string outputPath = "../../../output.txt";
    for (int i = 0; i < 10; i++)
    {
        GenerateMatrix(heightFirst, widthFirst, "matrixOne");
        GenerateMatrix(heightSecond, widthSecond, "matrixTwo");
        string path1 = "../../../matrixOne.txt";
        string path2 = "../../../matrixTwo.txt";

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        Multiply(path1, path2, outputPath);
        watch.Stop();
        File.Delete(path1); 
        File.Delete(path2);
        data[i] = watch.ElapsedMilliseconds;
    }

    double expectedValue = CountExpectedValue(data, 10);
    double standardDeviation = CountStandardDeviation(data, 10, expectedValue);
    Console.WriteLine($"Математическое ожидание: {expectedValue} мс");
    Console.WriteLine($"Стандартное отклонение: {standardDeviation} мс");
    File.Delete(outputPath);
}

void GenerateMatrix(int height, int width, string fileName)
{
    using var writer = new StreamWriter(File.Create($"../../../{fileName}.txt"));
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

double CountExpectedValue(long[] data, int numberOfTries)
{
    long expectation = 0;
    for (int i = 0; i < numberOfTries; i++)
    {
        expectation += data[i];
    }

    return expectation / numberOfTries;
}

double CountStandardDeviation(long[] data, int numberOfTries, double expectation)
{
    double variation = 0;
    for (int i = 0; i < numberOfTries; i++)
    {
        variation += (data[i] - expectation) * (data[i] - expectation);
    }

    return Math.Sqrt(variation / 10);
}
