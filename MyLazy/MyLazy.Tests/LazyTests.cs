namespace MyLazy.Tests;

public class LazyTests
{
    private static IEnumerable<TestCaseData> LazyInt
        => new[]
        {
            new TestCaseData(new LazySingleThread<int>(() => 5)),
            new TestCaseData(new LazyMultiThread<int>(() => 5))
        };

    private static IEnumerable<TestCaseData> LazyString
        => new[]
        {
            new TestCaseData(new LazySingleThread<string>(() => "u234aaa3")),
            new TestCaseData(new LazyMultiThread<string>(() => "u234aaa3"))
        };

    private static IEnumerable<TestCaseData> LazyNull
        => new[]
        {
            new TestCaseData(new LazySingleThread<object>(() => null)),
            new TestCaseData(new LazyMultiThread<object>(() => null))
        };

    private static IEnumerable<TestCaseData> LazyCurrentSecond
        => new[]
        {
            new TestCaseData(new LazySingleThread<int>(() => DateTime.Now.Second)),
            new TestCaseData(new LazyMultiThread<int>(() => DateTime.Now.Second))
        };

    [TestCaseSource(nameof(LazyInt))]
    public void LazyIntValueIsCalculatedCorrectly(ILazy<int> lazy)
        => Assert.That(lazy.Get(), Is.EqualTo(5));

    [TestCaseSource(nameof(LazyString))]
    public void LazyStringValueIsCalculatedCorrectly(ILazy<string?> lazy)
        => Assert.That(lazy.Get(), Is.EqualTo("u234aaa3"));

    [TestCaseSource(nameof(LazyCurrentSecond))]
    public void ChangingValueIsCalculatedOnlyOnce(ILazy<int> lazy)
    {
        var currentSecond = DateTime.Now.Second;
        Assert.That(lazy.Get(), Is.EqualTo(currentSecond));
        Thread.Sleep(5000);
        Assert.That(lazy.Get(), Is.EqualTo(currentSecond));
    }
    
    [TestCaseSource(nameof(LazyNull))]
    public void NullReturningFunctionShouldNotThrowException(ILazy<object?> lazy)
        => Assert.DoesNotThrow(() => lazy.Get());

    [Test]
    public void MultiThreadLazyShouldNotCauseRaces()
    {
        for (int j = 0; j < 10; j++)
        {
            const int threadNumber = 100;
            var count = 0;
            var threads = new Thread[threadNumber];
            var lazyMultiThread = new LazyMultiThread<int>(() => Interlocked.Increment(ref count));
            for (int i = 0; i < threadNumber; i++)
            {
                threads[i] = new Thread(() => lazyMultiThread.Get());
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.That(count, Is.EqualTo(1));
        }
    }
}