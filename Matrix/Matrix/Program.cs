Console.WriteLine("Программа: умножение матриц.");
Console.WriteLine("Если нужно сравнить скорость вычислений, укажите \"-compare\" или \"-nocompare\" как первый аргумент командной строки.");
if (args.Length <= 0 || (args[0] != "-compare" && args[0] != "-nocompare"))
{
    Console.WriteLine("Ошибка: нет первого аргумента.");
    return;
}

if (args[0] == "-compare")
{
    int size = 100;
    Console.WriteLine("Размер | Мат. ожидание (Послед.) | Станд. отклонение (Послед.) | Мат. ожидание (Паралл.) | Станд. отклонение (Паралл.) |");
    for (int i = 1; i < 11; i++)
    {
        Console.Write($"{size * i}    |");
        ComputeTimes(Matrix.Matrix.MultiplySequential, size * i);
        ComputeTimes(Matrix.Matrix.MultiplyParallel, size * i);
        Console.WriteLine();
    }
}
else if (args[0] == "-nocompare")
{
    Console.WriteLine("Укажите тип вычисления, \"-parallel\" или \"-sequential\", как второй аргумент командной строки.");
    if (args.Length <= 1 || (args[1] != "-parallel" && args[1] != "-sequential"))
    {
        Console.WriteLine("Ошибка: нет типа вычислений.");
        return;
    }

    Console.WriteLine("Укажите пути до имеющихся матриц как третий и четвертый аргументы командной строки.");
    if (args.Length <= 3)
    {
        Console.WriteLine("Ошибка: нет путей до файлов.");
        return;
    }

    Console.WriteLine("Укажите путь, где должна появиться новая матрица, как пятый аргумент командной строки.");
    if (args.Length <= 4)
    {
        Console.WriteLine("Ошибка: нет результирующего пути.");
        return;
    }

    string path1 = args[2];
    string path2 = args[3];
    string outputPath = args[4];
    try
    {
        if (args[1] == "-parallel")
        {
            var matrix = Matrix.Matrix.MultiplyParallel(new Matrix.Matrix(path1), new Matrix.Matrix(path2));
            Matrix.Matrix.WriteToFile(outputPath, matrix);
        }
        else if (args[1] == "-sequential")
        {
            var matrix = Matrix.Matrix.MultiplySequential(new Matrix.Matrix(path1), new Matrix.Matrix(path2));
            Matrix.Matrix.WriteToFile(outputPath, matrix);
        }
    }
    catch (DirectoryNotFoundException)
    {
        Console.WriteLine("Указаны неверные пути для файлов:");
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine("Указаны неверные пути для файлов:");
    }
}
else
{
    Console.WriteLine("Ошибка: указан неверный аргумент.");
}

void ComputeTimes(Func<Matrix.Matrix, Matrix.Matrix, Matrix.Matrix> multiply, int size)
{
    var data = new long[10];
    var watch = new System.Diagnostics.Stopwatch();
    for (int i = 0; i < 10; i++)
    {
        var testMatrix1 = Matrix.Matrix.Generate(size, size, 100);
        var testMatrix2 = Matrix.Matrix.Generate(size, size, 100);

        watch.Start();
        multiply(testMatrix1, testMatrix2);
        watch.Stop();
        data[i] = watch.ElapsedMilliseconds;
        watch.Reset();
    }

    var expectedValue = CountExpectedValue(data, 10);
    var standardDeviation = CountStandardDeviation(data, 10, expectedValue);
    Console.Write($"         {expectedValue:G4} мс          |          {standardDeviation:G4} мс         | ");
}

double CountExpectedValue(long[] data, int numberOfTries)
{
    long expectation = 0;
    for (int i = 0; i < numberOfTries; i++)
    {
        expectation += data[i];
    }

    return (double)expectation / numberOfTries;
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
