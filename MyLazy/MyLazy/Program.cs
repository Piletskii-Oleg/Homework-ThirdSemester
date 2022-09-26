using MyLazy;

LazySingleThread<int> a = new(() => 2 * 2);
int b = a.Get();
Console.WriteLine(b);
Console.WriteLine(b);
Console.WriteLine(b); Console.WriteLine(b);