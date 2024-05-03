using System.Security.Cryptography;
using System.Text;

namespace SmartHub.Extensions
{
    public static class StringExtension
    {
        public static string GenerateSHA256Signature(this string secretKey, string data)
        {
            // Преобразование секретного ключа в массив байтов
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);

            // Инициализация объекта HMACSHA256 с использованием секретного ключа
            using (var hmac = new HMACSHA256(keyBytes))
            {
                // Преобразование данных в массив байтов
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                // Вычисление хеша для данных
                byte[] hashBytes = hmac.ComputeHash(dataBytes);

                // Преобразование хеша в строку HEX
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashString;
            }
        }

    }
}
