using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System;

namespace Shopping_Laptop.Repository
{
    public static class SessionExtensions
    {
        // Lưu đối tượng vào session dưới dạng JSON
        public static void SetJson(this ISession session, string key, object value)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            try
            {
                var jsonData = JsonConvert.SerializeObject(value);
                session.SetString(key, jsonData);
            }
            catch (Exception ex)
            {
                // Log the exception here if necessary
                throw new InvalidOperationException("Failed to serialize object to session.", ex);
            }
        }

        // Lấy đối tượng từ session và deserialize thành kiểu dữ liệu mong muốn
        public static T GetJson<T>(this ISession session, string key)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            try
            {
                var sessionData = session.GetString(key);
                if (sessionData == null)
                    return default(T);

                return JsonConvert.DeserializeObject<T>(sessionData);
            }
            catch (Exception ex)
            {
                // Log the exception here if necessary
                throw new InvalidOperationException("Failed to deserialize object from session.", ex);
            }
        }
    }
}
