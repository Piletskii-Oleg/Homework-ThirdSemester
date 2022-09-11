namespace Matrix.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

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
}