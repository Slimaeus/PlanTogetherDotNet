using Newtonsoft.Json;
using System;

namespace PlanTogetherDotNetAPI.DTOs.Common
{
    public class PaginationParams
    {
        [JsonProperty("query")]
        public string SearchTerm { set; get; }
        private const int _maxPageSize = 50;
        private const int _minPageSize = 1;
        [JsonProperty("page")]
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        [JsonProperty("size")]
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value > _maxPageSize)
                {
                    throw new Exception($"The maximum page size allowed is {_maxPageSize}. Please select a smaller page size.");
                }
                if (value < _minPageSize)
                {
                    throw new Exception($"The minimum page size allowed is {_minPageSize}. Please select a larger page size.");
                }
                _pageSize = value;
            }
        }
        public static int MinPageSize { get => _minPageSize; }
        public static int MaxPageSize { get => _maxPageSize; }
    }
}