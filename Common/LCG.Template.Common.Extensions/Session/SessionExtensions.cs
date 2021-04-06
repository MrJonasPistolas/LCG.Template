using LCG.Template.Common.Enums.Identity;
using LCG.Template.Common.Models.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace LCG.Template.Common.Extensions.Session
{
    public static class SessionExtensions
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        #region worker methods GET SET and asyncs
        public static void Set<T>(this ISession session, string key, T value)
        {
            var data = JsonConvert.SerializeObject(value, Formatting.None, _settings);
            session.SetString(key, data);
        }
        public static Task SetAsync<T>(this ISession session, string key, T value)
        {
            return Task.Run(() => { Set<T>(session, key, value); });
        }
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null)
                throw new SessionExpiredException();
            return JsonConvert.DeserializeObject<T>(value);
        }
        public static Task<T> GetAsync<T>(this ISession session, string key)
        {
            return Task.Run<T>(() => { return Get<T>(session, key); });
        }
        #endregion

        #region Account information basic methods
        public static void AccountInformation(this ISession session, SessionAccountModel value)
        {
            session.SetInt32(nameof(SessionAccountModel.SelectedAccountId), value.SelectedAccountId);
            SessionExtensions.Set<SessionAccountModel>(session, nameof(SessionAccountModel), value);
        }
        /// <summary>
        /// Set Account Information
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task AccountInformationAsync(this ISession session, SessionAccountModel value)
        {
            session.SetInt32(nameof(SessionAccountModel.SelectedAccountId), value.SelectedAccountId);
            return SessionExtensions.SetAsync<SessionAccountModel>(session, nameof(SessionAccountModel), value);
        }

        public static SessionAccountModel AccountInformation(this ISession session)
        {
            return SessionExtensions.Get<SessionAccountModel>(session, nameof(SessionAccountModel));
        }
        public static Task<SessionAccountModel> AccountInformationAsync(this ISession session)
        {
            return SessionExtensions.GetAsync<SessionAccountModel>(session, nameof(SessionAccountModel));
        }
        public static Task ClearAccountInformationAsync(this ISession session)
        {
            return Task.Run(() => {
                session.Remove(nameof(SessionAccountModel.SelectedAccountId));
                session.Remove(nameof(SessionAccountModel));
            });
        }

        #endregion

        #region Other methods based in account information


        public static int? SelectedAccountId(this ISession session)
        {
            return session.GetInt32(Enum.GetName(typeof(SessionExtensionKeys), SessionExtensionKeys.SelectedAccountId));
        }

        public static Task<int?> SelectedAccountIdAsync(this ISession session)
        {
            return Task.Run<int?>(() => { return SessionExtensions.SelectedAccountId(session); });
        }
        #endregion
    }
}
