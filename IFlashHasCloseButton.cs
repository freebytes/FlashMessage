using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashMessage
{
    public interface IFlashHasCloseButton
    {
        public bool ShowCloseButton { get; set; }
        public string CloseButtonHtml { get; set; }
    }
}
