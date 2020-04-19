using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class AjaxResult
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        ///数据
        /// </summary>
        public object Data { get; set; }
    }
}
