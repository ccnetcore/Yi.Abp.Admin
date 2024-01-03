using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Yi.Framework.Core.Helper
{
    public class MD5Helper
    {
        /// <summary>
        /// 生成PasswordSalt
        /// </summary>
        /// <returns>返回string</returns>
        public static string GenerateSalt()
        {
            byte[] buf = new byte[16];
#pragma warning disable SYSLIB0023 // 类型或成员已过时
            new RNGCryptoServiceProvider().GetBytes(buf);
#pragma warning restore SYSLIB0023 // 类型或成员已过时
            return Convert.ToBase64String(buf);
        }

        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="pass">密码</param>
        /// <param name="passwordFormat">加密类型</param>
        /// <param name="salt">PasswordSalt</param>
        /// <returns>加密后的密码</returns>
        public static string SHA2Encode(string pass, string salt, int passwordFormat = 1)
        {
            if (passwordFormat == 0) // MembershipPasswordFormat.Clear
                return pass;

            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];
            byte[]? bRet = null;

            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);

#pragma warning disable SYSLIB0021 // 类型或成员已过时
            var s = SHA512.Create();
#pragma warning restore SYSLIB0021 // 类型或成员已过时
            bRet = s.ComputeHash(bAll);

            return ConvertEx.ToUrlBase64String(bRet);
        }

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt16(string password)
        {
            var md5 = MD5.Create();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(password)), 4, 8);
            t2 = t2.Replace("-", string.Empty);
            return t2;
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string password = "")
        {
            string pwd = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
                {
                    MD5 md5 = MD5.Create(); //实例化一个md5对像
                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                    byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                    // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                    foreach (var item in s)
                    {
                        // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                        pwd = string.Concat(pwd, item.ToString("X2"));
                    }
                }
            }
            catch
            {
                throw new Exception($"错误的 password 字符串:【{password}】");
            }
            return pwd;
        }

        /// <summary>
        /// 64位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt64(string password)
        {
            // 实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(s);
        }
    }
    public class ConvertEx
    {
        static readonly char[] padding = { '=' };
        public static string ToUrlBase64String(byte[] inArray)
        {
            var str = Convert.ToBase64String(inArray);
            str = str.TrimEnd(padding).Replace('+', '-').Replace('/', '_');

            return str;
        }

        public static byte[] FromUrlBase64String(string s)
        {
            string incoming = s.Replace('_', '/').Replace('-', '+');
            switch (s.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }
            byte[] bytes = Convert.FromBase64String(incoming);

            return bytes;
        }
    }
}
