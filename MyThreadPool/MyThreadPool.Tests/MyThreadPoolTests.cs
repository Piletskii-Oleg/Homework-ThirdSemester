namespace MyThreadPool.Tests;

public class Tests
{
    [Test]
    public void ValueCalculatedInTaskShouldBeAvailable()
    {
        using var pool = new MyThreadPool(5);
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
        using var pool = new MyThreadPool(5);
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
        using var pool = new MyThreadPool(5);
        var results = new IMyTask<int>[10];

        Parallel.For(0, 10, index =>
        {
            results[index] = pool.Submit(() => index * 10);
        });
            
        Thread.Sleep(100);
        for (int i = 0; i < 10; i++)
        {
            Assert.That(results[i].Result, Is.EqualTo(i * 10));
        }
    }

    [Test]
    public void CannotSubmitAfterShutdown()
    {
        using var pool = new MyThreadPool(5);
        pool.Submit(() => 5);
        pool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => pool.Submit(() => 6));
    }

    [Test]
    public void AllTasksMustBeCalculatedAfterShutdown()
    {
        using var pool = new MyThreadPool(5);
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
        using var pool = new MyThreadPool(5);
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