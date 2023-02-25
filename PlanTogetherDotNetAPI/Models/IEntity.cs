using System;

namespace PlanTogetherDotNetAPI.Models
{
    public interface IEntity
    {
        DateTime CreateDate { get; set; }
        Guid Id { get; set; }
    }
}