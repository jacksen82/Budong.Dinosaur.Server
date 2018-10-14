
/// <summary>
/// 消息类型
/// </summary>
public enum NotifyType : int
{
    /// <summary>
    /// 关注了我的故事
    /// </summary>
    StoryRelayBookmark = 1001,
    /// <summary>
    /// 续写了我的故事
    /// </summary>
    StoryRelayRelay = 2001,
    /// <summary>
    /// 续写了我关注的故事
    /// </summary>
    BookmarkRelay = 3001
}