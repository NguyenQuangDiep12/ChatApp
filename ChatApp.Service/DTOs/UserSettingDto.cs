namespace ChatApp.Service.DTOs
{
    public record UserSettingDto(bool ShowOnlineStatus, bool ShowLastSeen, bool SendReadReceipt);
}
