using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models.Enums
{
    /// <summary>
    /// Mô tả trạng thái của người dùng
    /// unknown: chưa biết trạng thái hiện tại của người dùng
    /// online: trạng thái hiện tại đang online
    /// offline: trạng thái hiện tại đang offline
    /// </summary>
    public enum UserStatus
    {
        UNKNOWN,
        ONLINE,
        OFFLINE
    }
}
