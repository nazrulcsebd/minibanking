using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniBanking.Core.Helper.Entities
{
    public class FilterObject
    {
        public string PropertyName { get; set; }
        public bool Desc { get; set; } //true mean desc

        public IQueryable<T> FilterQuery<T>(IQueryable<T> query)
        {
            if (PropertyName != null)
            {
                PropertyName = Utility.FirstCharToUpper(PropertyName);
                if (Desc)
                {
                    query = query.DynamicOrderByDescending(PropertyName);
                }
                else
                {
                    query = query.DynamicOrderBy(PropertyName);
                }
            }
            return query;
        }
    }
}
