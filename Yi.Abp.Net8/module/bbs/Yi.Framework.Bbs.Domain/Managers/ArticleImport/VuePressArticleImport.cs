using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Shared.Model;

namespace Yi.Framework.Bbs.Domain.Managers.ArticleImport
{
    internal class VuePressArticleImport : AbstractArticleImport
    {
        public override List<ArticleAggregateRoot> Convert(List<FileObject> fileObjs)
        {
            var logger = LoggerFactory.CreateLogger<VuePressArticleImport>();

            //排序及处理目录名称
            var fileNameHandler = fileObjs.OrderBy(x => x.FileName).Select(x =>
              {
                  var f = new FileObject { Content = x.Content };

                  //除去数字
                  f.FileName = x.FileName.Split('.')[1];
                  return f;
              });


            //处理内容
            var fileContentHandler = fileNameHandler.Select(x =>
             {
                 logger.LogError($"老的值：{x.Content}");
                 var f = new FileObject { FileName = x.FileName };
                 var lines = x.Content.SplitToLines();

                 var num = 0;
                 var startIndex = 0;
                 for (int i = 0; i < lines.Length; i++)
                 {
                     //编码问题
                     if (lines[i] == "---")
                     {
                         num++;
                         if (num == 2)
                         {
                             startIndex = i;
                             break;
                         }

                     }

                 }
                 var linesRef = lines.ToList();

                 if (startIndex != 0)
                 {
                     linesRef.RemoveRange(0, startIndex + 1);
                 }
                 else
                 {
                     //去除头部
                     linesRef.RemoveRange(0,6);
                 }
                 var result = string.Join(Environment.NewLine, linesRef);
                 f.Content = result;
                 return f;
             });
            var output = fileContentHandler.Select(x => new ArticleAggregateRoot() { Content = x.Content, Name = x.FileName }).ToList();

            return output;
        }
    }
}
