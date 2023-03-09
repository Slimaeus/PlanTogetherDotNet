using System;
using System.ComponentModel.DataAnnotations;

namespace PlanTogetherDotNetAPI.DTOs.Common
{
    public class EditDTO
    {
        [Required]
        public Guid Id { get; set; }
    }
}