using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
  public  class RedisHelper
    {
        private static IDatabase db { get; set; }
        private static object locker=new object();
        private RedisHelper()
        {

        }
        public static IDatabase GetRedisDatabase()
        {
            if (db == null)
            {
                lock (locker)
                {
                    if (db==null)
                    {
                        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
                        db = redis.GetDatabase();
                        return db;  
                    }
                }
            }
            return db;
        }

    }
}
