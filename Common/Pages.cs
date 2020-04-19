using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Pages
    {
        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总数据条数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 显示页码最多数
        /// </summary>
        public int MaxPageCount { get; set; }
        /// <summary>
        /// 当前页的页码(从1开始)
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 约定的格式 约定其中页面用{pn}占位符
        /// </summary>
        public string UrlPattern { get; set; }
        /// <summary>
        /// 当前页码样式Class名
        /// </summary>
        public string CurrentPageClassName { get; set; }
        public string GetPagerHtml()
        {
            StringBuilder html = new StringBuilder();
            html.Append("<ul>");
            //总页数
            int pageCount = (int)Math.Ceiling(TotalCount * 1.0 / PageSize);
            //显示出页码的起始页码
            int startPageIndex = Math.Max(1, (PageIndex - MaxPageCount / 2));
            //显示出来页码的结束页码
            int endPageIndex = Math.Min(pageCount, (startPageIndex + MaxPageCount));
            if (startPageIndex > 1)
            {

            }
            for (int i = startPageIndex; i <= endPageIndex; i++)
            {
                if (i == PageIndex)
                {
                    html.Append("<li class='").Append(CurrentPageClassName).Append("'>").Append(i).Append("</li>");
                }
                else
                {
                    string href = UrlPattern.Replace("{pn}", i.ToString());
                    html.Append("<li><a href='").Append(href).Append("'>").Append(i).Append("</a></li>");

                }
            }
            html.Append("</ul>");
            return html.ToString();
        }
    }
}

