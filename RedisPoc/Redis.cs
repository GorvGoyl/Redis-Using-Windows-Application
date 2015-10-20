using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;

namespace RedisPoc
{
    public class RedisCache
    {
        private string _RedisHost;

        public RedisCache(string host)
        {
           // _PersonalizationHost = "Name:localhost;Host:redis.leadsquared.co;database:db0;Port:6379;";

            try
            {

                _RedisHost = host;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #region GET

        public string GetValueFromOrgCache(string orgid, string key)
        {
            return GetValueFromCache("ORG:" + orgid + ":" + key);
        }

        public string GetValueFromGlobalCache(string key)
        {
            return GetValueFromCache("GLOBAL:" + key);
        }

        public string GetValueFromUserCache(string userid, string key)
        {
            return GetValueFromCache("USER:" + userid + ":" + key);
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
                   
                    Value =  Client.GetAllItemsFromList(key);
                }
                string value = string.Join(",", Value.ToArray());
                return value;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Dictionary<string, string> GetMultipleValuesFromOrgCache(string orgid, string keyList)
        {
            try
            {
                Dictionary<string, string> CacheValueList = new Dictionary<string, string>();
                using (RedisClient client = GetRedisClientObject())
                {
                    string[] keyListTokens = keyList.Split('|');
                    for (int i = 0; i < keyListTokens.Length; i++)
                    {
                        CacheValueList.Add(keyListTokens[i], client.GetValue("ORG:" + orgid + ":" + keyListTokens[i]));
                    }
                }
                return CacheValueList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public string GetListFromOrgCache(string orgid, string key, string filter)
        {

            try
            {
                string FilterOutput = null;
                string CsvStr = GetValueFromCache("ORG:" + orgid + ":" + key);
                if (!string.IsNullOrEmpty(CsvStr))
                {
                    string[] CsvArr = CsvStr.Split(',');
                    if (CsvArr != null && CsvArr.Length > 1)
                    {
                        string[] FilterArr = CsvArr.Where(c => c.ToLower().StartsWith(filter.ToLower())).ToArray();
                        FilterOutput = String.Join(",", FilterArr);
                    }
                }
                return FilterOutput;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        #endregion GET

        #region PUT

        public void AddValueToOrgCache(string orgid, string key, string value, int? expireInHours = null)
        {
            AddValueToCache("ORG:" + orgid + ":" + key, value, expireInHours);
        }

        public void AddValueToUserCache(string userid, string key, string value, int? expireInHours = null)
        {
            AddValueToCache("USER:" + userid + ":" + key, value, expireInHours);
        }

        public void AddValueToGlobalCache(string key, string value, int? expireInHours = null)
        {
            AddValueToCache("GLOBAL:" + key, value, expireInHours);

        }

        public void AddListToOrgCache(string orgid, string key, string value)
        {
            AddValueToCache("ORG:" + orgid + ":" + key, value);

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

        #endregion PUT

        #region REMOVE

        public bool RemoveValueFromOrgCache(string orgid, string key)
        {
            return RemoveValueFromCache("ORG:" + orgid + ":" + key);
        }

        public bool RemoveValueFromGlobalCache(string key)
        {
            return RemoveValueFromCache("GLOBAL:" + key);
        }

        public bool RemoveValueFromUserCache(string userid, string key)
        {
            return RemoveValueFromCache("USER:" + userid + ":" + key);
        }

        public bool RemoveMultipleValuesFromOrgCache(string orgid, string keyStartsWith)
        {
            return RemoveMultipleValuesFromCache("ORG:" + orgid + ":" + keyStartsWith);
        }

        public bool RemoveMultipleValuesFromGlobalCache(string keyStartsWith)
        {
            return RemoveMultipleValuesFromCache("GLOBAL:" + keyStartsWith);
        }

        public bool RemoveMultipleValuesFromUserCache(string userid, string keyStartsWith)
        {
            return RemoveMultipleValuesFromCache("USER:" + userid + ":" + keyStartsWith);
        }


        private bool RemoveMultipleValuesFromCache(string keyStartsWith)
        {
            try
            {
                using (RedisClient client = GetRedisClientObject())
                {
                    List<string> keysWithTag = client.SearchKeys(keyStartsWith + "*");
                    client.RemoveAll(keysWithTag);

                }
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private bool RemoveValueFromCache(string key)
        {
            try
            {
                using (RedisClient client = GetRedisClientObject())
                {
                    return client.Remove(key);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion REMOVE


        private RedisClient GetRedisClientObject()
        {
            RedisClient Client = new RedisClient(_RedisHost);
            Client.SendTimeout = 5;
            return Client;
        }
    }
}