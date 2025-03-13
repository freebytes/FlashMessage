using FlashMessage.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FlashMessage.TagHelpers
{
    /// <summary>
    /// A TagHelper for the Flash Message feature for easy use in Razor pages.
    /// </summary>
    [HtmlTargetElement("flash")]
    public class FlashTagHelper : TagHelper
    {
        private readonly IFlash _flash;

        [HtmlAttributeName("type")]
        public string Type { get; set; } = "any";

        public FlashTagHelper(IFlash flash)
        {
            _flash = flash;
            //flash.LoadSession();
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            FlashMessageCategory category;            
            if (!Enum.TryParse(Type, true, out category))
                category = FlashMessageCategory.Any;

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            string flashContent = _flash.ToString(category);
            if (!string.IsNullOrEmpty(flashContent))
            {
                output.Content.SetHtmlContent(flashContent);
            }
            else
                output.SuppressOutput();
        }
    }
}
