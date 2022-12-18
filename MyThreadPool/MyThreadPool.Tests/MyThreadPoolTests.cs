namespace MyThreadPool.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void ValueCalculatedInTaskShouldBeAvailable()
    {
        var pool = new MyThreadPool(3);
        var task = pool.Submit(() => 2 * 2);
        Thread.Sleep(100);

        Assert.Multiple(() =>
        {
            Assert.That(task.IsCompleted, Is.True);
            Assert.That(task.Result, Is.EqualTo(4));
        });
    }

    [Test]
    public void MultipleContinuedTasksAreCalculatedCorrectly()
    {
        var pool = new MyThreadPool(3);
        var task = pool.Submit(() => 2 * 2);
        Thread.Sleep(10);

        var continued = task.ContinueWith(x => x + 4.0);
        var continuedString = task.ContinueWith(x => x.ToString());

        Assert.Multiple(() =>
        {
            Assert.That(continued.Result, Is.EqualTo(8.0));
            Assert.That(continuedString.Result, Is.EqualTo("4"));
        });
    }

    [Test]
    public void SubmittingInMultipleThreadsGivesCorrectResults()
    {
        var pool = new MyThreadPool(5);
        var threads = new Thread[10];
        var results = new IMyTask<int>[10];
        for (int i = 0; i < 10; i++)
        {
            var localI = i;
            threads[i] = new Thread(() => results[localI] = pool.Submit(() => localI * 10));
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        Thread.Sleep(100);
        for (int i = 0; i < 10; i++)
        {
            Assert.That(results[i].Result, Is.EqualTo(i * 10));
        }
    }

    [Test]
    public void Test()
    {
        var pool = new MyThreadPool(5);
        var task = pool.Submit(() => 4 * 4);
    }

    [Test]
    public void CannotSubmitAfterShutdown()
    {
        var pool = new MyThreadPool(5);
        pool.Submit(() => 5);
        pool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => pool.Submit(() => 6));
    }

    [Test]
    public void AllTasksMustBeCalculatedAfterShutdown()
    {
        var pool = new MyThreadPool(2);

        var amount = 30;
        var tasks = new IMyTask<double>[amount];
        var results = new double[amount];

        for (int i = 0; i < amount; i++)
        {
            var localI = i;
            tasks[i] = pool.Submit(() =>
            {
                Thread.Sleep(100);
                results[localI] = Math.Sin(Math.Pow(5.3241 * 9.21312 + Math.Sqrt(8923498282), localI));
                return results[localI];
            });
        }

        pool.Shutdown();

        for (int i = 0; i < tasks.Length; i++)
        {
            while (!tasks[i].IsCompleted)
            {
                Thread.Sleep(10);
            }

            Assert.Multiple(() =>
            {
                Assert.That(tasks[i].IsCompleted, Is.True);
                Assert.That(tasks[i].Result, Is.EqualTo(results[i]));
            });
        }
    }

    [Test]
    public void AggregateExceptionAndInnerExceptionAreReceived()
    {
        var pool = new MyThreadPool(2);
        var array = new int[3];
        var task = pool.Submit(() => array[4]);

        try
        {
            Assert.Throws<AggregateException>(() => array[0] = task.Result);
        }
        catch (AggregateException exception)
        {
            Assert.That(exception.InnerException, Is.TypeOf<IndexOutOfRangeException>());
        }
    }
}