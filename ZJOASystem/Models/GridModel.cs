using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZJOASystem.Models
{
    public class GridModel
    {
        public static object GridData(int pageIndex, int rows, int total, IEnumerable<object> objects)
        {
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)total / pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = pageIndex,
                records = total,
                rows = objects.ToArray()
            };

            return jsonData;
        }
    }
}