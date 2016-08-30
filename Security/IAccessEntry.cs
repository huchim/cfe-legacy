namespace Cfe.Security
{
    public interface IAccessEntry
    {
        string Module { get; set; }
        string Privilege { get; set; }
    }
}