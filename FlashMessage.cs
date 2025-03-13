using FlashMessage.Enums;
using FlashMessage.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Web;

namespace FlashMessage
{
    [Serializable]
    public class Flash : IFlash
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession? _session => _httpContextAccessor.HttpContext?.Session;
        //private readonly ISession _session;
        public List<Message> Messages { get; set; } = new List<Message>();
        public bool ShowCloseButton { get; set; } = true;
        public string CloseButtonHtml { get; set; } = "<button type='button' class='btn-close close' data-bs-dismiss='alert' data-dismiss='alert' aria-label='Close'><span aria-hidden='true' class='d-none'>&times;</span></button>";
        /// <summary>
        /// Indicates how the messages are rendered on the page, e.g. Toastr or BootstrapAlerts.
        /// </summary>
        public FlashType MessageFlashType { get; set; } = FlashType.Toastr;
        
        public Flash(IHttpContextAccessor httpContextAccessor, FlashType messageFlashType = FlashType.Toastr)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            MessageFlashType = messageFlashType;
            LoadSession();
        }
        /*
        public Flash(ISession session, FlashType messageFlashType = FlashType.Toastr)
        {
            _session = session;
            MessageFlashType = messageFlashType;
        }
        */
        public void Error(string content)
        {
            Add(content, FlashMessageCategory.Error);
        }

        public void Info(string content)
        {
            Add(content, FlashMessageCategory.Information);
        }

        public void Warning(string content)
        {
            Add(content, FlashMessageCategory.Warning);
        }

        public void Success(string content)
        {
            Add(content, FlashMessageCategory.Success);
        }

        public void Add(string content, FlashMessageCategory category = FlashMessageCategory.Error, string overrideCssClass = "")
        {
            Messages.Add(new Message(content, category, overrideCssClass));
        }

        /// <summary>
        /// SaveSession is used to pass flash messages to different pages.
        /// You must call this if you want to pass flash messages on redirects.
        /// </summary>
        public void SaveSession()
        {
            if (_session != null && _session.IsAvailable)
            {
                _session.SetString("FlashMessages", JsonSerializer.Serialize(this.Messages));
            }
        }

        /// <summary>
        /// Removes the Flash from the session.
        /// </summary>
        public void ClearSession()
        {
            if (_session != null && _session.IsAvailable)
            {
                // Remove all flash messages from session.
                _session.Remove("FlashMessages");                    
            }
        }

        /// <summary>
        /// Clears the session then saves the session again without the specified ID.
        /// The session must be loaded prior to calling this.
        /// </summary>
        /// <param name="id"></param>
        /*
        private void ClearMessageFromSession(long id)
        {
            // First clear the session.
            ClearSession();
            List<Message> messages = new List<Message>();
            foreach (Message m in Messages)
            {
                if (m.MessageId != id)
                    messages.Add(m);
            }
            Messages = messages;
            SaveSession();
        }
        */

        private void ClearRenderedMessages(List<long> clearList)
        {
            List<Message> messages = new List<Message>();
            foreach (Message m in Messages)
            {
                bool skip = false;
                foreach (long id in clearList)
                {
                    if (m.MessageId == id)
                    {
                        skip = true;
                        break;
                    }
                }
                if (!skip)
                    messages.Add(m);
            }
            Messages = messages;
            SaveSession();
        }

        public void Clear()
        {
            ClearSession(); // If we clear all messages, we will also clear the session.
            Messages = new List<Message>();
        }

        /// <summary>
        /// Checks whether a flash message exists.
        /// </summary>
        /// <param name="category">If supplied, will only return true if at least one flash message of the specified category exists.</param>
        /// <returns></returns>
        public bool HasFlash(FlashMessageCategory category = FlashMessageCategory.Any)
        {
            if (category == FlashMessageCategory.Any && Messages.Count > 0)
                return true;
            foreach (Message m in Messages)
            {
                if (m.Category == category)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Renders the flash message for the specified category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public string ToString(FlashMessageCategory category)
        {
            //LoadSession();
            SetCSSDefaults();
            StringBuilder sb = new StringBuilder();
            if (!HasFlash(category))
                return string.Empty;
            List<long> clearIds = new List<long>();
            foreach (Message m in Messages)
            {
                if (category == FlashMessageCategory.Any || m.Category.Equals(FlashMessageCategory.Any) || m.Category.Equals(category))
                {
                    if (MessageFlashType == FlashType.Toastr)
                    {
                        sb.Append("<script type='text/javascript'>");
                        sb.Append("$(function() { ");
                        string toastrFunction = m.OverrideCssClass.Replace("toastr-", "");
                        sb.Append($"toastr[\"{toastrFunction}\"]('");
                        sb.Append(HttpUtility.JavaScriptStringEncode(m.Content));
                        sb.Append("'); ");
                        sb.Append("});");
                        sb.Append("</script>");
                        if (ShowCloseButton)
                            sb.Append("<script type='text/javascript'>toastr.options = { 'closeButton': true }</script>\r\n");
                        clearIds.Add(m.MessageId);
                        continue;
                    }
                    if (MessageFlashType.Equals(FlashType.BootstrapAlert) || MessageFlashType.Equals(FlashType.Unset))
                    {
                        sb.Append($"<div class='flash-message {HttpUtility.HtmlEncode(m.OverrideCssClass)}");
                        if (ShowCloseButton)
                            sb.Append(" alert-dismissable");
                        sb.Append($"' role='alert'>");
                        sb.Append($"{HttpUtility.HtmlEncode(m.Content)}\r\n");
                        if (ShowCloseButton)
                            sb.Append(CloseButtonHtml);
                        sb.Append("</div>");
                        clearIds.Add(m.MessageId);
                        continue;
                    }
                }
            }
            ClearRenderedMessages(clearIds);
            return sb.ToString();
        }

        public override string ToString()
        {
            return ToString(FlashMessageCategory.Any);
        }

        public void LoadSession()
        {
            Messages = GetLatestMessagesFromSession();
        }

        private bool IsFlashSessionAvailable()
        {
            if (_session == null || !_session.IsAvailable)
                return false;
            if (_session.Keys != null && _session.Keys.Contains("FlashMessages"))
                return true;
            return false;
        }

        private List<Message> GetLatestMessagesFromSession()
        {
            if (!IsFlashSessionAvailable())
                return new List<Message>();
            string? json = _session?.GetString("FlashMessages");
            if (string.IsNullOrEmpty(json))
                return new List<Message>();
            List<Message>? messages = JsonSerializer.Deserialize<List<Message>>(json);
            if (messages == null || messages.Count == 0)
                return new List<Message>();
            return messages;
        }

        /// <summary>
        /// Sets the default values for each message if the CSS is not overridden.
        /// </summary>
        private void SetCSSDefaults()
        {
            if (MessageFlashType == FlashType.BootstrapAlert)
            {
                // If using Bootstrap, sets our default CSS for any categories.
                SetCSSByCategories(FlashMessageCategory.Default, "alert alert-primary");
                // This should not be possible, but we put it here just in case.
                SetCSSByCategories(FlashMessageCategory.Any, "alert alert-secondary");
                SetCSSByCategories(FlashMessageCategory.Error, "alert alert-danger");
                SetCSSByCategories(FlashMessageCategory.Success, "alert alert-success");
                SetCSSByCategories(FlashMessageCategory.Warning, "alert alert-warning");
                SetCSSByCategories(FlashMessageCategory.Information, "alert alert-info");
            }
            if (MessageFlashType == FlashType.Toastr)
            {
                // If using Bootstrap, sets our default CSS for any categories.
                SetCSSByCategories(FlashMessageCategory.Default, "toastr-info");
                // This should not be possible, but we put it here just in case.
                SetCSSByCategories(FlashMessageCategory.Any, "toastr-info");
                SetCSSByCategories(FlashMessageCategory.Error, "toastr-error");
                SetCSSByCategories(FlashMessageCategory.Success, "toastr-success");
                SetCSSByCategories(FlashMessageCategory.Warning, "toastr-warning");
                SetCSSByCategories(FlashMessageCategory.Information, "toastr-info");
            }
        }

        /// <summary>
        /// Cycles through each message and sets the appropriate CSS class for each category.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cssClass"></param>
        private void SetCSSByCategories(FlashMessageCategory category, string cssClass)
        {
            foreach (Message message in Messages)
            {
                if (message.Category.Equals(category) && string.IsNullOrEmpty(message.OverrideCssClass))
                {
                    message.OverrideCssClass = cssClass;
                }
            }
        }

    }
}
