using StackExchange.Redis;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
           var  db = redis.GetDatabase();
            db.SortedSetIncrement("PicScore", 3, 1);
          string a =  db.StringGet("234");
            Console.ReadKey();
        }
    }
}
