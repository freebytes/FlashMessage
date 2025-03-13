using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashMessage.Enums
{
    [Serializable]
    public enum FlashMessageCategory
    {
        Any = 0,
        Default = 1,
        Error = 2,
        Warning = 3,
        Information = 4,
        Success = 5
    }
}
