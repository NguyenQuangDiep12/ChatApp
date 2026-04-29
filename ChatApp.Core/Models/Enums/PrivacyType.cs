using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models.Enums
{
    /// <summary>
    /// Mô tả quyền truy cập vào phòng chat
    /// 
    /// public: người tham gia có thể tham gia tự do vào nhóm chat
    /// 
    /// private: người tham gia phải được mời thông qua link,...
    /// 
    /// password: người tham gia chỉ có thể tham gia thông qua password
    /// </summary>
    public enum PrivacyType
    {
        PUBLIC,
        PRIVATE, 
        PASSWORD 
    }
}
