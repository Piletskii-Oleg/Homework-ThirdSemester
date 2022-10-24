using Matrix.Exceptions;

namespace Matrix.Tests;

public class Tests
{
    [Test]
    public void SameMatricesMultipliedDifferentlyShouldGiveSameResult()
    {
        string path1 = "../../../TestFiles/matrix1.txt";
        string path2 = "../../../TestFiles/matrix2.txt";
        var matrixParallel = Matrix.MultiplyParallel(new Matrix(path1), new Matrix(path2));
        var matrixSequential = Matrix.MultiplySequential(new Matrix(path1), new Matrix(path2));

        Assert.Multiple(() =>
        {
            Assert.That(matrixParallel.Height, Is.EqualTo(matrixSequential.Height));
            Assert.That(matrixParallel.Width, Is.EqualTo(matrixSequential.Width));
        });

        for (int i = 0; i < matrixParallel.Height; i++)
        {
            for (int j = 0; j < matrixParallel.Width; j++)
            {
                Assert.That(matrixParallel[i, j], Is.EqualTo(matrixSequential[i, j]));
            }
        }
    }

    [Test]
    public void GeneratingMatrixWorksCorrectly()
    {
        var matrix = Matrix.Generate(20, 30, 100);
        Assert.Multiple(() =>
        {
            Assert.That(matrix, Is.Not.Null);
            Assert.That(matrix.Height, Is.EqualTo(20));
            Assert.That(matrix.Width, Is.EqualTo(30));
        });
    }

    [Test]
    public void MultiplyingIncompatibleMatricesShouldThrowException()
    {
        var matrix1 = Matrix.Generate(20, 30, 100);
        var matrix2 = Matrix.Generate(20, 30, 100);
        Assert.Throws<IncompatibleMatrixSizesException>(() => Matrix.MultiplySequential(matrix1, matrix2));
        Assert.Throws<IncompatibleMatrixSizesException>(() => Matrix.MultiplyParallel(matrix1, matrix2));
    }
}