namespace MyLazy.Tests;

public class LazyTests
{
    private static IEnumerable<TestCaseData> LazyInt
        => new[]
        {
            new TestCaseData(new LazySingleThread<int>(() => 5)),
            new TestCaseData(new LazyMultiThread<int>(() => 5))
        };

    private static IEnumerable<TestCaseData> LazyCurrentSecond
        => new[]
        {
            new TestCaseData(new LazySingleThread<int>(() => DateTime.Now.Second)),
            new TestCaseData(new LazyMultiThread<int>(() => DateTime.Now.Second))
        };

    [TestCaseSource(nameof(LazyInt))]
    public void LazyValueIsCalculatedCorrectly(ILazy<int> lazy)
        => Assert.That(lazy.Get(), Is.EqualTo(5));

    [TestCaseSource(nameof(LazyCurrentSecond))]
    public void ChangingValueIsCalculatedOnlyOnce(ILazy<int> lazy)
    {
        var currentSecond = DateTime.Now.Second;
        Assert.That(lazy.Get(), Is.EqualTo(currentSecond));
        Thread.Sleep(2000);
        Assert.That(lazy.Get(), Is.EqualTo(currentSecond));
    }

    [Test]
    public void MultiThreadLazyShouldNotCauseRaces()
    {
        for (int j = 0; j < 10; j++)
        {
            var threadNumber = 2000;
            var count = 0;
            var threads = new Thread[threadNumber];
            var lazyMultiThread = new LazyMultiThread<int>(() => count++);
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