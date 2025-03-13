using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashMessage.ExtensionMethods
{
    public static class SessionExtensions
    {
        /// <summary>
        /// Sets a <see cref="string"/> value in the <see cref="ISession"/>.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/>.</param>
        /// <param name="key">The key to assign.</param>
        /// <param name="value">The value to assign.</param>
        public static void SetString(this ISession session, string key, string value)
        {
            session.Set(key, Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Gets a string value from <see cref="ISession"/>.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/>.</param>
        /// <param name="key">The key to read.</param>
        public static string? GetString(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Gets a byte-array value from <see cref="ISession"/>.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/>.</param>
        /// <param name="key">The key to read.</param>
        public static byte[]? Get(this ISession session, string key)
        {
            session.TryGetValue(key, out var value);
            return value;
        }
    }
}
