using Newtonsoft.Json;
using System;

namespace PlanTogetherDotNetAPI.DTOs.Common
{
    public class PaginationHeader
    {
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }
        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }
        [JsonProperty("totalItems")]
        public int TotalItems { get; set; }
        [JsonProperty("totalPages")]
        public int TotalPages => ItemsPerPage > 0 ? (int)Math.Ceiling(TotalItems / (decimal)ItemsPerPage) : 0;
        [JsonProperty("hasNext")]
        public bool HasNext => CurrentPage < TotalPages;
        [JsonProperty("hasPrevious")]
        public bool HasPrevious => CurrentPage > 1;

        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
        }
    }
}