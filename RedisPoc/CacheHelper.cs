using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedisPoc
{
    public enum PersonalizationType { User, Org, Global }
    public enum PersonalizationAction { Get, GetList, Put, PutList, Remove, RemoveMultiple, GetMultiple }

    public class CacheHelper
    {
        private static readonly int _WaitTimeBetWeenAttempts = 2000;
        private static readonly string _PersonalizationHost;

        #region Constructor

        static CacheHelper()
        {
            try
            {


                //_PersonalizationHost = ConfigurationManager.AppSettings["RedisHost"].ToString();
                _PersonalizationHost = "Name:localhost;Host:redis.leadsquared.co;database:db0;Port:6379;";


            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Public Methods

        #region [GET]

        public static string GetPersonalizationSetting(PersonalizationType type, string id, string key, int timeOutInSeconds = 2)
        {
            try
            {
                RedisCache Redis = new RedisCache(_PersonalizationHost);
                string Value = null;
                switch (type)
                {
                    case PersonalizationType.Global:
                        Value = Redis.GetValueFromGlobalCache(key);
                        break;

                    case PersonalizationType.Org:
                        Value = Redis.GetValueFromOrgCache(id, key);
                        break;
                    default:
                        Value = Redis.GetValueFromUserCache(id, key);
                        break;
                }
                return Value;
            }
            catch (Exception ex)
            {

                // throw; NOT Thorwing error to avoid checking exception everywhere
            }

            return null;
        }

        public static Dictionary<string, string> GetMultiplePersonalizationSetting(PersonalizationType type, string id, string keys)
        {
            try
            {
                Dictionary<string, string> PersonalizationSettings = new Dictionary<string, string>();
                RedisCache Redis = new RedisCache(_PersonalizationHost);

                switch (type)
                {
                    case PersonalizationType.Org:
                        PersonalizationSettings = Redis.GetMultipleValuesFromOrgCache(id, keys);
                        break;
                    default:
                        throw new NotImplementedException();
                        break;
                }
                return PersonalizationSettings;
            }
            catch (NotImplementedException ex) { }
            catch (Exception ex)
            {

                // throw;
            }

            return null;
        }

        public static string GetPersonalizationList(PersonalizationType type, string id, string key, string filter)
        {
            try
            {
                RedisCache Redis = new RedisCache(_PersonalizationHost);
                string Value = null;
                switch (type)
                {
                    case PersonalizationType.Org:
                        Value = Redis.GetListFromOrgCache(id, key, filter);
                        break;
                    default:
                        throw new NotImplementedException();
                        break;
                }

            }
            catch (NotImplementedException ex) {  }
            catch (Exception ex)
            {

               
            }
            return null;
        }

        #endregion

        #region [PUT]

        public static bool PutPersonalizationSetting(PersonalizationType type, string id, string key, object value, int timeOutInSeconds = 2, int? expireInHours = null)
        {
            try
            {
                RedisCache Redis = new RedisCache(_PersonalizationHost);
                string ValueForKey = value != null ? value.ToString() : null;
                switch (type)
                {
                    case PersonalizationType.Global:
                        Redis.AddValueToGlobalCache(key, ValueForKey, expireInHours);
                        break;

                    case PersonalizationType.Org:
                        Redis.AddValueToOrgCache(id, key, ValueForKey, expireInHours);
                        break;
                    default:
                        Redis.AddValueToUserCache(id, key, ValueForKey, expireInHours);
                        break;
                }
                return true;
            }
            catch (Exception ex) { }

            return false;
        }

        public static bool PutPersonalizationList(PersonalizationType type, string id, string key, object value, int timeOutInSeconds = 5)
        {
            try
            {
                RedisCache Redis = new RedisCache(_PersonalizationHost);
                string ValueForKey = value != null ? value.ToString() : null;
                switch (type)
                {
                    case PersonalizationType.Org:
                        Redis.AddListToOrgCache(id, key, ValueForKey);
                        break;
                    default:
                        throw new NotImplementedException("Put List is only allowed for `Org` PersonalizationType!");
                        break;
                }
            }
            catch (NotImplementedException ex) { }
            catch (Exception ex)
            {

                
            }
            return false;
        }

        #endregion

        #region [REMOVE]

        public static bool RemovePersonalizationSetting(PersonalizationType type, string id, string key)
        {
            try
            {
                RedisCache Redis = new RedisCache(_PersonalizationHost);
                bool IsRemoved;
                switch (type)
                {
                    case PersonalizationType.Global:
                        IsRemoved = Redis.RemoveValueFromGlobalCache(key);
                        break;

                    case PersonalizationType.Org:
                        IsRemoved = Redis.RemoveValueFromOrgCache(id, key);
                        break;
                    default:
                        IsRemoved = Redis.RemoveValueFromUserCache(id, key);
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {

                // throw;
            }

            return false;
        }

        public static bool RemoveMultiplePersonalizationSettings(PersonalizationType type, string id, string key, int retryCount = 0, int timeOutInSeconds = 2)
        {
            try
            {
                int AttemptsToSave = 0;
                bool IsSaveSuccess = false;
                RedisCache Redis = new RedisCache(_PersonalizationHost);

                while (!IsSaveSuccess && AttemptsToSave <= retryCount)
                {
                    try
                    {
                        switch (type)
                        {
                            case PersonalizationType.Global:
                                IsSaveSuccess = Redis.RemoveMultipleValuesFromGlobalCache(key);
                                break;

                            case PersonalizationType.Org:
                                IsSaveSuccess = Redis.RemoveMultipleValuesFromOrgCache(id, key);
                                break;
                            default:
                                IsSaveSuccess = Redis.RemoveMultipleValuesFromUserCache(id, key);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (AttemptsToSave == retryCount)
                        {
                            throw;
                        }

                    }
                    AttemptsToSave++;
                    System.Threading.Thread.Sleep(_WaitTimeBetWeenAttempts);
                }
            }
            catch (Exception ex)
            {

                // throw;
            }

            return false;
        }

        #endregion

        #endregion

       // #region Private Methods

        //private static Newtonsoft.Json.Linq.JObject DoPost(object value, string url, int timeOutInSeconds)
        //{
        //    Newtonsoft.Json.Linq.JObject Personalization = null;

        //    try
        //    {

        //        Personalization = (JsonServiceConsumer.Post(url, value, "text/plain; charset=utf-8", timeOutInSeconds) as Newtonsoft.Json.Linq.JObject);
        //    }
        //    catch (Exception ex)
        //    {

        //        return Personalization;
        //    }

        //    //private static string GetPersonalizationUrl(PersonalizationType type, PersonalizationAction action, string id, string key)
        //    //{
        //    //    _Log.Debug("Get personalizationurl..");
        //    //    string IdCon = type == PersonalizationType.Global ? "" : "&" + type.ToString() + "ID=" + id;
        //    //    string MethodName = type.ToString() + "Data" + "." + action.ToString();

        //    //    string PersonalizationUrl = _PersonalizationUrl + "/" + MethodName + "?AuthToken=" + GetPersonalizationKey(action) + IdCon;
        //    //    switch (action)
        //    //    {
        //    //        case PersonalizationAction.Get:
        //    //        case PersonalizationAction.Put:
        //    //        case PersonalizationAction.Remove:
        //    //        case PersonalizationAction.PutList:
        //    //        case PersonalizationAction.GetList:
        //    //            PersonalizationUrl += "&KeyName=" + key;
        //    //            break;
        //    //        case PersonalizationAction.RemoveMultiple:
        //    //            PersonalizationUrl += "&KeyStartsWith=" + key;
        //    //            break;
        //    //        case PersonalizationAction.GetMultiple:
        //    //            PersonalizationUrl += "&KeyList=" + key;
        //    //            break;
        //    //    }
        //    //    _Log.Debug("PersonalizationUrl: " + PersonalizationUrl);

        //    //    return PersonalizationUrl;
        //    //}

        //    //private static string GetPersonalizationKey(PersonalizationAction action)
        //    //{
        //    //    string PersonalizationKey = null;
        //    //    switch (action)
        //    //    {
        //    //        case PersonalizationAction.Get:
        //    //        case PersonalizationAction.GetList:
        //    //        case PersonalizationAction.GetMultiple:
        //    //            PersonalizationKey = _GetPersonalizationKey;
        //    //            break;
        //    //        case PersonalizationAction.Put:
        //    //        case PersonalizationAction.PutList:
        //    //            PersonalizationKey = _PutPersonalizationKey;
        //    //            break;
        //    //        case PersonalizationAction.Remove:
        //    //        case PersonalizationAction.RemoveMultiple:
        //    //            PersonalizationKey = _RemovePersonalizationKey;
        //    //            break;

        //    //    }
        //    //    return PersonalizationKey;
        //    //}

        //#endregion
        //}
    }
}