using FlashMessage.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FlashMessage
{
    public interface IFlash
    {
        /// <summary>
        /// The list of messages that will be shown.
        /// </summary>
        public List<Message> Messages { get; set; }
        /// <summary>
        /// Adds a message to the lsit of messages.
        /// </summary>
        /// <param name="content">The content of the message.</param>
        /// <param name="category">The type of message, e.g. error, success, warning, info, etc.</param>
        /// <param name="cssClass">Allows the CSS to be overridden.</param>
        public void Add(string content, FlashMessageCategory category, string overrideCssClass = "");
        public void Clear();
        public bool HasFlash(FlashMessageCategory category);
        public FlashType MessageFlashType { get; set; }
        /// <summary>
        /// This ToString should return HTML for the flash.
        /// </summary>
        /// <returns></returns>
        public string ToString();
        public string ToString(FlashMessageCategory category);
        public void Error(string message);
        public void Info(string message);
        public void Warning(string message);
        public void Success(string message);
        public void SaveSession();
        public void ClearSession();
        public void LoadSession();
    }
}
