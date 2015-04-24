using System;

namespace Ocon.Entity
{
    public interface IEntity
    {
        Guid Id { set; get; }
        DateTime LastUpdate { get; set; }
    }
}