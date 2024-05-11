using SmartHub.DataContext.DbModels;
using System.Security.Cryptography;
using System.Text;

namespace SmartHub.Extensions
{
    public static class StringExtension
    {
        public static string GenerateSHA256Signature(this string secretKey, string data)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] hashBytes = hmac.ComputeHash(dataBytes);
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashString;
            }
        }

        public static string GenerateMD5Signature(this string secretKey, string data)
        {
            using (var md5 = MD5.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                md5.TransformBlock(keyBytes, 0, keyBytes.Length, keyBytes, 0);
                md5.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                byte[] hashBytes = md5.Hash;

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }

        public static DataType GetDataTypeForString(this string str)
        {
            if (str.Equals("Boolean"))
            {
                return DataType.Boolean;
            }
            if (str.Equals("String"))
            {
                return DataType.String;
            }
            if (str.Equals("Int"))
            {
                return DataType.Int;
            }
            throw new Exception("DataType Error");
        }
    }
}
