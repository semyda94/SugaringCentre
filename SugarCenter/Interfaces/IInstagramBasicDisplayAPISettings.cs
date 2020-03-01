namespace SugarCenter.Interfaces
{
    public interface IInstagramBasicDisplayAPISettings
    {
        long AppId { get; }
        string AppSecret { get; }
        string InstagramRedirectUrl { get; }
        string ApiBaseUrl { get; }
        string Scope { get; }
    }
}