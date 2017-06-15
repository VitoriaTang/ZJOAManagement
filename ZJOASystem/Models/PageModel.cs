using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZJOASystem.Models
{
    /// <summary>
    /// Page model
    /// </summary>
    public class BasePageModel
    {
        public string SearchKeyWord { get; set; }

        /// <summary>
        ///
        /// </summary>
        public virtual string ActionName
        {
            get
            {
                return "Index";
            }
        }

        public int TotalCount { get; set; }

        public int CurrentIndex { get; set; }

        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)TotalCount / (double)PageSize);
            }
        }

        /// <summary>
        /// The page size
        /// </summary>
        public virtual int PageSize
        {
            get { return 10; }
        }

        /// <summary>
        ///Max pages
        /// </summary>
        public virtual int DisplayMaxPages
        {
            get
            {
                return 10;
            }
        }

        public bool IsHasPrePage
        {
            get
            {
                return CurrentIndex != 1;
            }
        }

        public bool IsHasNextPage
        {
            get
            {
                return CurrentIndex != TotalPages;
            }
        }
    }
}