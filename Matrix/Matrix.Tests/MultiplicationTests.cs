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
        var matrixParallel = Operations.MultiplyParallel3(path1, path2);
        var matrixConsecutive = Operations.MultiplyConsecutive(path1, path2);
        Assert.That(matrixConsecutive, Is.EqualTo(matrixParallel));
    }
}