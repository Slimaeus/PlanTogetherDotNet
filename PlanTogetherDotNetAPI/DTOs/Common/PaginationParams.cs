namespace PlanTogetherDotNetAPI.DTOs.Common
{
    public class PaginationParams
    {
        public string Query { set; get; }
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
                    value = _maxPageSize;
                }
                if (value < _minPageSize)
                {
                    value = _minPageSize;
                }
                _pageSize = value;
            }
        }
        public static int MinPageSize { get => _minPageSize; }
        public static int MaxPageSize { get => _maxPageSize; }
    }
}