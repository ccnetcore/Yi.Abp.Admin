using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Rbac.Domain.Shared.Options
{
    public class AliyunOptions
    {

        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public AliyunSms Sms { get; set; }
    }

    public class AliyunSms
    {
        public string SignName { get; set; }
        public string TemplateCode { get; set; }
    }
}
