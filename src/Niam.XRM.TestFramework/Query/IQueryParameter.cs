using System;

namespace Niam.XRM.TestFramework.Query
{
    public interface IQueryParameter
    {
        Guid UserId { get; set; }
        Guid BusinessUnitId { get; set; }
        DateTime Now { get; set; }
    }
}
