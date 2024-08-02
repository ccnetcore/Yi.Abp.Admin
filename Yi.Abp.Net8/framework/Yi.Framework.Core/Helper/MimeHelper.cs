using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.Core.Enums;

namespace Yi.Framework.Core.Helper
{
    public static class MimeHelper
    {
        // 通过自己定义一个静态类
        // 将所有的Content Type都扔进去吧
        // 调用的时候直接调用静态方法即可。

        public static List<string> ImageType { get; set; } = new List<string>
        {
            ".jpg",".png",".jpeg"
        };

        private static Hashtable _mimeMappingTable;

        private static void AddMimeMapping(string extension, string MimeType)
        {
            _mimeMappingTable.Add(extension, MimeType);
        }

        public static string GetMimeMapping(string FileName)
        {
            string text = null!;
            int num = FileName.LastIndexOf('.');
            if (0 < num && num > FileName.LastIndexOf('\\'))
            {
                text = (string)_mimeMappingTable[FileName.Substring(num)]!;
            }
            if (text == null)
            {
                text = (string)_mimeMappingTable[".*"]!;
            }
            return text;
        }

        public static FileTypeEnum GetFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (ImageType.Contains(extension.ToLower()))
                return FileTypeEnum.Image;
            return FileTypeEnum.File;


        }

        static MimeHelper()
        {
            _mimeMappingTable = new Hashtable(190, StringComparer.CurrentCultureIgnoreCase);
            AddMimeMapping(".323", "text/h323");
            AddMimeMapping(".asx", "video/x-ms-asf");
            AddMimeMapping(".acx", "application/internet-property-stream");
            AddMimeMapping(".ai", "application/postscript");
            AddMimeMapping(".aif", "audio/x-aiff");
            AddMimeMapping(".aiff", "audio/aiff");
            AddMimeMapping(".axs", "application/olescript");
            AddMimeMapping(".aifc", "audio/aiff");
            AddMimeMapping(".asr", "video/x-ms-asf");
            AddMimeMapping(".avi", "video/x-msvideo");
            AddMimeMapping(".asf", "video/x-ms-asf");
            AddMimeMapping(".au", "audio/basic");
            AddMimeMapping(".application", "application/x-ms-application");
            AddMimeMapping(".bin", "application/octet-stream");
            AddMimeMapping(".bas", "text/plain");
            AddMimeMapping(".bcpio", "application/x-bcpio");
            AddMimeMapping(".bmp", "image/bmp");
            AddMimeMapping(".cdf", "application/x-cdf");
            AddMimeMapping(".cat", "application/vndms-pkiseccat");
            AddMimeMapping(".crt", "application/x-x509-ca-cert");
            AddMimeMapping(".c", "text/plain");
            AddMimeMapping(".css", "text/css");
            AddMimeMapping(".cer", "application/x-x509-ca-cert");
            AddMimeMapping(".crl", "application/pkix-crl");
            AddMimeMapping(".cmx", "image/x-cmx");
            AddMimeMapping(".csh", "application/x-csh");
            AddMimeMapping(".cod", "image/cis-cod");
            AddMimeMapping(".cpio", "application/x-cpio");
            AddMimeMapping(".clp", "application/x-msclip");
            AddMimeMapping(".crd", "application/x-mscardfile");
            AddMimeMapping(".deploy", "application/octet-stream");
            AddMimeMapping(".dll", "application/x-msdownload");
            AddMimeMapping(".dot", "application/msword");
            AddMimeMapping(".doc", "application/msword");
            AddMimeMapping(".dvi", "application/x-dvi");
            AddMimeMapping(".dir", "application/x-director");
            AddMimeMapping(".dxr", "application/x-director");
            AddMimeMapping(".der", "application/x-x509-ca-cert");
            AddMimeMapping(".dib", "image/bmp");
            AddMimeMapping(".dcr", "application/x-director");
            AddMimeMapping(".disco", "text/xml");
            AddMimeMapping(".exe", "application/octet-stream");
            AddMimeMapping(".etx", "text/x-setext");
            AddMimeMapping(".evy", "application/envoy");
            AddMimeMapping(".eml", "message/rfc822");
            AddMimeMapping(".eps", "application/postscript");
            AddMimeMapping(".flr", "x-world/x-vrml");
            AddMimeMapping(".fif", "application/fractals");
            AddMimeMapping(".gtar", "application/x-gtar");
            AddMimeMapping(".gif", "image/gif");
            AddMimeMapping(".gz", "application/x-gzip");
            AddMimeMapping(".hta", "application/hta");
            AddMimeMapping(".htc", "text/x-component");
            AddMimeMapping(".htt", "text/webviewhtml");
            AddMimeMapping(".h", "text/plain");
            AddMimeMapping(".hdf", "application/x-hdf");
            AddMimeMapping(".hlp", "application/winhlp");
            AddMimeMapping(".html", "text/html");
            AddMimeMapping(".htm", "text/html");
            AddMimeMapping(".hqx", "application/mac-binhex40");
            AddMimeMapping(".isp", "application/x-internet-signup");
            AddMimeMapping(".iii", "application/x-iphone");
            AddMimeMapping(".ief", "image/ief");
            AddMimeMapping(".ivf", "video/x-ivf");
            AddMimeMapping(".ins", "application/x-internet-signup");
            AddMimeMapping(".ico", "image/x-icon");
            AddMimeMapping(".jpg", "image/jpeg");
            AddMimeMapping(".jfif", "image/pjpeg");
            AddMimeMapping(".jpe", "image/jpeg");
            AddMimeMapping(".jpeg", "image/jpeg");
            AddMimeMapping(".js", "application/x-javascript");
            AddMimeMapping(".lsx", "video/x-la-asf");
            AddMimeMapping(".latex", "application/x-latex");
            AddMimeMapping(".lsf", "video/x-la-asf");
            AddMimeMapping(".manifest", "application/x-ms-manifest");
            AddMimeMapping(".mhtml", "message/rfc822");
            AddMimeMapping(".mny", "application/x-msmoney");
            AddMimeMapping(".mht", "message/rfc822");
            AddMimeMapping(".mid", "audio/mid");
            AddMimeMapping(".mpv2", "video/mpeg");
            AddMimeMapping(".man", "application/x-troff-man");
            AddMimeMapping(".mvb", "application/x-msmediaview");
            AddMimeMapping(".mpeg", "video/mpeg");
            AddMimeMapping(".m3u", "audio/x-mpegurl");
            AddMimeMapping(".mdb", "application/x-msaccess");
            AddMimeMapping(".mpp", "application/vnd.ms-project");
            AddMimeMapping(".m1v", "video/mpeg");
            AddMimeMapping(".mpa", "video/mpeg");
            AddMimeMapping(".me", "application/x-troff-me");
            AddMimeMapping(".m13", "application/x-msmediaview");
            AddMimeMapping(".movie", "video/x-sgi-movie");
            AddMimeMapping(".m14", "application/x-msmediaview");
            AddMimeMapping(".mpe", "video/mpeg");
            AddMimeMapping(".mp2", "video/mpeg");
            AddMimeMapping(".mov", "video/quicktime");
            AddMimeMapping(".mp3", "audio/mpeg");
            AddMimeMapping(".mpg", "video/mpeg");
            AddMimeMapping(".ms", "application/x-troff-ms");
            AddMimeMapping(".nc", "application/x-netcdf");
            AddMimeMapping(".nws", "message/rfc822");
            AddMimeMapping(".oda", "application/oda");
            AddMimeMapping(".ods", "application/oleobject");
            AddMimeMapping(".pmc", "application/x-perfmon");
            AddMimeMapping(".p7r", "application/x-pkcs7-certreqresp");
            AddMimeMapping(".p7b", "application/x-pkcs7-certificates");
            AddMimeMapping(".p7s", "application/pkcs7-signature");
            AddMimeMapping(".pmw", "application/x-perfmon");
            AddMimeMapping(".ps", "application/postscript");
            AddMimeMapping(".p7c", "application/pkcs7-mime");
            AddMimeMapping(".pbm", "image/x-portable-bitmap");
            AddMimeMapping(".ppm", "image/x-portable-pixmap");
            AddMimeMapping(".pub", "application/x-mspublisher");
            AddMimeMapping(".pnm", "image/x-portable-anymap");
            AddMimeMapping(".png", "image/png");
            AddMimeMapping(".pml", "application/x-perfmon");
            AddMimeMapping(".p10", "application/pkcs10");
            AddMimeMapping(".pfx", "application/x-pkcs12");
            AddMimeMapping(".p12", "application/x-pkcs12");
            AddMimeMapping(".pdf", "application/pdf");
            AddMimeMapping(".pps", "application/vnd.ms-powerpoint");
            AddMimeMapping(".p7m", "application/pkcs7-mime");
            AddMimeMapping(".pko", "application/vndms-pkipko");
            AddMimeMapping(".ppt", "application/vnd.ms-powerpoint");
            AddMimeMapping(".pmr", "application/x-perfmon");
            AddMimeMapping(".pma", "application/x-perfmon");
            AddMimeMapping(".pot", "application/vnd.ms-powerpoint");
            AddMimeMapping(".prf", "application/pics-rules");
            AddMimeMapping(".pgm", "image/x-portable-graymap");
            AddMimeMapping(".qt", "video/quicktime");
            AddMimeMapping(".ra", "audio/x-pn-realaudio");
            AddMimeMapping(".rgb", "image/x-rgb");
            AddMimeMapping(".ram", "audio/x-pn-realaudio");
            AddMimeMapping(".rmi", "audio/mid");
            AddMimeMapping(".ras", "image/x-cmu-raster");
            AddMimeMapping(".roff", "application/x-troff");
            AddMimeMapping(".rtf", "application/rtf");
            AddMimeMapping(".rtx", "text/richtext");
            AddMimeMapping(".sv4crc", "application/x-sv4crc");
            AddMimeMapping(".spc", "application/x-pkcs7-certificates");
            AddMimeMapping(".setreg", "application/set-registration-initiation");
            AddMimeMapping(".snd", "audio/basic");
            AddMimeMapping(".stl", "application/vndms-pkistl");
            AddMimeMapping(".setpay", "application/set-payment-initiation");
            AddMimeMapping(".stm", "text/html");
            AddMimeMapping(".shar", "application/x-shar");
            AddMimeMapping(".sh", "application/x-sh");
            AddMimeMapping(".sit", "application/x-stuffit");
            AddMimeMapping(".spl", "application/futuresplash");
            AddMimeMapping(".sct", "text/scriptlet");
            AddMimeMapping(".scd", "application/x-msschedule");
            AddMimeMapping(".sst", "application/vndms-pkicertstore");
            AddMimeMapping(".src", "application/x-wais-source");
            AddMimeMapping(".sv4cpio", "application/x-sv4cpio");
            AddMimeMapping(".tex", "application/x-tex");
            AddMimeMapping(".tgz", "application/x-compressed");
            AddMimeMapping(".t", "application/x-troff");
            AddMimeMapping(".tar", "application/x-tar");
            AddMimeMapping(".tr", "application/x-troff");
            AddMimeMapping(".tif", "image/tiff");
            AddMimeMapping(".txt", "text/plain");
            AddMimeMapping(".texinfo", "application/x-texinfo");
            AddMimeMapping(".trm", "application/x-msterminal");
            AddMimeMapping(".tiff", "image/tiff");
            AddMimeMapping(".tcl", "application/x-tcl");
            AddMimeMapping(".texi", "application/x-texinfo");
            AddMimeMapping(".tsv", "text/tab-separated-values");
            AddMimeMapping(".ustar", "application/x-ustar");
            AddMimeMapping(".uls", "text/iuls");
            AddMimeMapping(".vcf", "text/x-vcard");
            AddMimeMapping(".wps", "application/vnd.ms-works");
            AddMimeMapping(".wav", "audio/wav");
            AddMimeMapping(".wrz", "x-world/x-vrml");
            AddMimeMapping(".wri", "application/x-mswrite");
            AddMimeMapping(".wks", "application/vnd.ms-works");
            AddMimeMapping(".wmf", "application/x-msmetafile");
            AddMimeMapping(".wcm", "application/vnd.ms-works");
            AddMimeMapping(".wrl", "x-world/x-vrml");
            AddMimeMapping(".wdb", "application/vnd.ms-works");
            AddMimeMapping(".wsdl", "text/xml");
            AddMimeMapping(".xap", "application/x-silverlight-app");
            AddMimeMapping(".xml", "text/xml");
            AddMimeMapping(".xlm", "application/vnd.ms-excel");
            AddMimeMapping(".xaf", "x-world/x-vrml");
            AddMimeMapping(".xla", "application/vnd.ms-excel");
            AddMimeMapping(".xls", "application/vnd.ms-excel");
            AddMimeMapping(".xlsx", "application/vnd.ms-excel");
            AddMimeMapping(".xof", "x-world/x-vrml");
            AddMimeMapping(".xlt", "application/vnd.ms-excel");
            AddMimeMapping(".xlc", "application/vnd.ms-excel");
            AddMimeMapping(".xsl", "text/xml");
            AddMimeMapping(".xbm", "image/x-xbitmap");
            AddMimeMapping(".xlw", "application/vnd.ms-excel");
            AddMimeMapping(".xpm", "image/x-xpixmap");
            AddMimeMapping(".xwd", "image/x-xwindowdump");
            AddMimeMapping(".xsd", "text/xml");
            AddMimeMapping(".z", "application/x-compress");
            AddMimeMapping(".zip", "application/x-zip-compressed");
            AddMimeMapping(".*", "application/octet-stream");
        }
    }



}
