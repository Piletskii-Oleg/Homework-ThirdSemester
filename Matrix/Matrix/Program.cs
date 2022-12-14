if (args.Length != 1 && args.Length != 4)
{
    Console.WriteLine("Первый аргумент: -compare или -nocompare");
    Console.WriteLine("Если выбран -nocompare: ");
    Console.WriteLine("Второй аргумент: -parallel или -sequential");
    Console.WriteLine("Третий и четвертый аргументы - пути до файлов");
    Console.WriteLine("Пятый аргумент - путь, где необходимо создать файл с результатом");

    return;
}

if (args[0] != "-compare" && args[0] != "-nocompare")
{
    Console.WriteLine("Укажите -compare или -nocompare");
    return;
}

if (args[0] == "-compare")
{
    int size = 100;
    Console.WriteLine(
        "Размер | Мат. ожидание (Послед.) | Станд. отклонение (Послед.) | Мат. ожидание (Паралл.) | Станд. отклонение (Паралл.) |");

    for (int i = 1; i < 11; i++)
    {
        Console.Write($"{size * i}    |");
        ComputeTimes(Matrix.Matrix.MultiplySequential, size * i);
        ComputeTimes(Matrix.Matrix.MultiplyParallel, size * i);
        Console.WriteLine();
    }
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
    Console.WriteLine("Указаны неверные пути для файлов.");
}
catch (FileNotFoundException)
{
    Console.WriteLine("Указаны неверные пути для файлов.");
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

    var expectedValue = CountExpectedValue(data);
    var standardDeviation = CountStandardDeviation(data, expectedValue);
    Console.Write($"         {expectedValue:G4} мс          |          {standardDeviation:G4} мс         | ");
}

double CountExpectedValue(long[] data)
    => data.Average();

double CountStandardDeviation(long[] data, double expectation)
{
    double variation = 0;

    for (int i = 0; i < data.Length; i++)
    {
        variation += (data[i] - expectation) * (data[i] - expectation);
    }

    return Math.Sqrt(variation / data.Length);
}