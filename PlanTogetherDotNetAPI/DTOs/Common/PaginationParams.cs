using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanTogetherDotNetAPI.DTOs.Common
{
    public class PaginationParams
    {
        public string SearchTerm { set; get; }
        private const int _maxPageSize = 50;
        private const int _minPageSize = 1;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
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