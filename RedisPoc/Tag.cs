using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;

namespace RedisPoc
{
    public class Tag
    {
        private string _RedisHost;

        Tag(String host)
        {
            _RedisHost = host;
        }

        private RedisClient GetRedisClientObject()
        {
            RedisClient Client = new RedisClient(_RedisHost);
            Client.SendTimeout = 5;
            return Client;
        }

        public string GetValueFromCache(string key) //redis get
        {
            //string Value = null;
            List<string> Value = null;
            try
            {
                using (RedisClient Client = GetRedisClientObject())
                {
                    //Value = Client.GetValue(key);

                    Value = Client.GetAllItemsFromList(key);
                }
                string value = string.Join(",", Value.ToArray());
                return value;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void AddValueToCache(string key, string value, int? expireInHours = null) //redis put
        {
            try
            {
                using (IRedisClient client = GetRedisClientObject())
                {
                    if (expireInHours == null || expireInHours.Value == 0)
                    {
                        //client.SetEntry(key, value);
                        client.AddItemToList(key, value);
                    }
                    else
                    {
                        TimeSpan ExpireIn = new TimeSpan(expireInHours.Value, 0, 0);
                        client.SetEntry(key, value, ExpireIn);
                    }
                }
            }
            catch (Exception ex)
            {
                ;
                throw;
            }
        }
    }
}
