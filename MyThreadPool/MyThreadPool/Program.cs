using MyThreadPool;

var pool = new MyThreadPool.MyThreadPool(5);
var task = pool.Submit(() => 3);

var collection = new System.Collections.Concurrent.BlockingCollection<int>();
collection.Add(2);
Console.WriteLine(collection.Take());