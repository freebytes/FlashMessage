using FlashMessage.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashMessage
{
    [Serializable]
    public class Message
    {
        /// <summary>
        /// Used to track the messages so they are cleared from the session after being rendered.
        /// </summary>
        public long MessageId { get; set; } = 0;
        public FlashMessageCategory Category { get; set; } = FlashMessageCategory.Error;
        public string OverrideCssClass { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        private static long _counter = 0;
        private static readonly object _lock = new object();

        public Message()
        {
            Category = FlashMessageCategory.Error;
        }

        /// <summary>
        /// Creates a new flash message with specific constructor elements.
        /// </summary>
        /// <param name="content">The error message to display when rendered to the screen.</param>
        /// <param name="category">The type of message, e.g. error, success, etc.  If Any is specified, it will be set to Default instead.  The Any category should be used for comparisons only.</param>
        /// <param name="overrideCssClass">Allows the CSS for the rendered message to be overridden.</param>
        public Message(string content, FlashMessageCategory category, string overrideCssClass = "")
        {
            if (category == FlashMessageCategory.Any)
                category = FlashMessageCategory.Default;
            Category = category;
            OverrideCssClass = overrideCssClass;
            Content = content;
            MessageId = GenerateUniqueId();
        }

        /// <summary>
        /// Creates a unique ID to be used by the message.
        /// </summary>
        /// <returns></returns>
        public long GenerateUniqueId()
        {
            lock (_lock)
            {
                long timestamp = DateTime.UtcNow.Ticks;
                return timestamp + _counter++;
            }
        }
    }
}
