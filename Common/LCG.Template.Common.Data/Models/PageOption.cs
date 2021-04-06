namespace LCG.Template.Common.Data.Models
{
    public class PageOption
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public PageSort[] Sort { get; set; }
        public PageFilters Filter { get; set; }
    }

    public class PageSort
    {
        public string Field { get; set; }
        public string Dir { get; set; }
    }

    public class PageFilters
    {
        public string Logic { get; set; }
        public PageFilter[] Filters { get; set; }
    }

    public class PageFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
    }
}
