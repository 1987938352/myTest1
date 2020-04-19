using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;

namespace Common
{
    public  class MVCHelper
    {
        public static string GetValidMsg(ModelStateDictionary modelState)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ms in modelState.Values)
            {
                foreach (var modelError in ms.Errors)
                {
                    sb.AppendLine(modelError.ErrorMessage);
                }
            }
            return sb.ToString();
        }
        public static string GetFileExt(string FileName)
        {
            string[] strings = FileName.Split(".");
            return strings[1];
        }
    }
}
