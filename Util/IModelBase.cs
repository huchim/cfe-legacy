namespace Cfe.Util
{
    using System;

    public interface IModelBase
    {
        int Id { get; set; }        
        int Status { get; set; }
        DateTime Created { get; set; }
        DateTime Modified { get; set; }
    }
}
