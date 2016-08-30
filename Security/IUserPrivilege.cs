namespace Cfe.Security
{
    public interface IUserPrivilege : Util.IModelBase
    {
        int UserId { get; set; }
        int RoleId { get; set; }
    }
}