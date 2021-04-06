using LCG.Template.Common.Extensions.PrimitiveTypes;
using Newtonsoft.Json;

namespace LCG.Template.Common.Tools.GridPagination
{
    public class GridOptionsModel
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public GridOptionsModelSort[] Sort { get; set; }
        public GridOptionsModelFilters Filter { get; set; }

        public T CastTo<T>() where T : class, new()
        {
            var data = JsonConvert.SerializeObject(this, Formatting.None);
            return JsonConvert.DeserializeObject<T>(data);
        }
    }

    public class GridOptionsModelSort
    {
        private string _field;
        public string Field
        {
            get
            {
                if (_field != null) return _field.ToUpperFirstLetter();
                return null;
            }
            set { _field = value; }
        }
        public string Dir { get; set; }
    }

    public class GridOptionsModelFilters
    {
        public string Logic { get; set; }
        public GridOptionsModelFilter[] Filters { get; set; }
    }

    public class GridOptionsModelFilter
    {
        private string _field;
        public string Field
        {
            get
            {
                if (_field != null) return _field.ToUpperFirstLetter();
                return null;
            }
            set { _field = value; }
        }
        public string Operator { get; set; }

        private object _value;
        public object Value
        {
            get
            {
                if (_value is System.DateTime)
                    _value = ((System.DateTime)_value).ToLocalTime();
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }
}
