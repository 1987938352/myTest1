using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UEditor.Core.Handlers;

namespace UI
{
    public class QiNiuAdd
    {

        public  static void UploadConfig(Stream stream,string key)
        {
            Mac mac = new Mac("QuJTEHVW7mQkRm3syu7tEAIFwSFRtnUN9maTei7V", "usEPW8UWUdmL6Pabx9wH6DmSmfsqDTdNJCV87yzg");
            // 上传文件名
            
            // 本地文件路径
            //string filePath = "D:\\che.png";
            // 存储空间名
            string Bucket = "hur";
            // 设置上传策略，详见：https://developer.qiniu.com/kodo/manual/1206/put-policy
            PutPolicy putPolicy = new PutPolicy();

            // 设置要上传的目标空间
            putPolicy.Scope = Bucket;
            // 上传策略的过期时间(单位:秒)
            putPolicy.SetExpires(3600);
            // 文件上传完毕后，在多少天后自动被删除
            putPolicy.DeleteAfterDays = 100;
            // 生成上传token
            string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            Config config = new Config();
            // 设置上传区域
            config.Zone = Zone.ZONE_CN_East;
            // 设置 http 或者 https 上传
            config.UseHttps = true;
            config.UseCdnDomains = true;
            config.ChunkSize = ChunkUnit.U512K;
            // 表单上传
            FormUploader target = new FormUploader(config);
            //HttpResult result = target.UploadFile(filePath, key, token, null);
            HttpResult result = target.UploadStream(stream, key, token, null);
           
        }
    }
}
