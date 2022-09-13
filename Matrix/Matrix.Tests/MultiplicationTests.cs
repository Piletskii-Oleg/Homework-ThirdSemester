namespace Matrix.Tests;

public class Tests
{
    [Test]
    public void SameMatricesMultipliedDifferentlyShouldGiveSameResult()
    {
        string path1 = "../../../TestFiles/matrix1.txt";
        string path2 = "../../../TestFiles/matrix2.txt";
        string outputPath = "../../../TestFiles/output.txt";
        var matrixParallel = Operations.MultiplyParallel(path1, path2, outputPath);
        var matrixSequential = Operations.MultiplySequential(path1, path2, outputPath);
        Assert.That(matrixSequential, Is.EqualTo(matrixParallel));
    }

    [Test]
    public void GeneratingMatrixWorksCorrectly()
    {
        string path = "../../../TestFiles/generatedMatrix.txt";
        Operations.Generate(20, 30, path);
        var lines = File.ReadAllLines(path);

        Assert.That(lines, Has.Length.EqualTo(20));
        for (int i = 0; i < 20; i++)
        {
            var numbers = lines[i].Split(" ");
            for (int j = 0; j < 30; j++)
            {
                Assert.That(int.TryParse(numbers[j], out int _), Is.True); 
            }

            Assert.Throws<IndexOutOfRangeException>(() => numbers[31] = "what");
        }

        File.Delete(path);
    }
}