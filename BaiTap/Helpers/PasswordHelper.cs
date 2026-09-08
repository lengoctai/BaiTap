using BCrypt.Net;

namespace BaiTap.Helpers
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Hash mật khẩu sử dụng BCrypt
        /// </summary>
        /// <param name="password">Mật khẩu cần hash</param>
        /// <returns>Mật khẩu đã được hash</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            // BCrypt tự động tạo salt và hash password với workFactor = 12
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        /// <summary>
        /// Verify mật khẩu với hash đã lưu
        /// </summary>
        /// <param name="password">Mật khẩu người dùng nhập</param>
        /// <param name="hashedPassword">Mật khẩu đã hash trong database</param>
        /// <returns>True nếu mật khẩu đúng, False nếu sai</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                // Kiểm tra xem hashedPassword có phải là BCrypt hash không
                // Nếu không phải (mật khẩu cũ chưa được hash), so sánh trực tiếp để tương thích ngược
                if (!hashedPassword.StartsWith("$2"))
                {
                    // Mật khẩu cũ chưa được hash, so sánh trực tiếp
                    return password == hashedPassword;
                }

                // Verify với BCrypt
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                // Nếu có lỗi trong quá trình verify, trả về false
                return false;
            }
        }
    }
}

