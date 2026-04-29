using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models.Enums
{
    /// <summary>
    /// Mô tả trạng thái của kiểu phòng chat
    /// Direct: Chat trực tiếp giữa 2 người (1 - 1)
    /// 
    /// Group: Nhóm nhiều người, tất cả thành viên đều có thể gửi tin nhắn
    /// 
    /// Chanel: Kênh broadcast, chỉ một số người được ủy quyền có thể gửi tin
    /// Các member khác chỉ đọc
    /// </summary>
    public enum RoomType
    {
        DIRECT,
        GROUP,
        CHANEL
    }
}
