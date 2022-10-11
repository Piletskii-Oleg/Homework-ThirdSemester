using MyThreadPool;

var pool = new MyThreadPool.MyThreadPool(5);
var task = pool.Submit(() => 3);