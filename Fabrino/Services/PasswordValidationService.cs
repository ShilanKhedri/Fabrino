using System.Text.RegularExpressions;
using System.Linq;

namespace Fabrino.Services
{
    public class PasswordValidationService
    {
        public bool IsPasswordStrongEnough(string password)
        {
            var strength = CheckPasswordStrength(password);
            return strength >= PasswordStrength.Medium;
        }

        public PasswordStrength CheckPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password)) return PasswordStrength.Weak;

            int score = 0;
            
            // طول رمز عبور
            if (password.Length >= 12) score += 2;
            else if (password.Length >= 8) score++;

            // وجود اعداد
            if (Regex.IsMatch(password, @"[0-9]")) score++;
            if (Regex.IsMatch(password, @"[0-9].*[0-9]")) score++; // حداقل دو عدد

            // وجود حروف کوچک
            if (Regex.IsMatch(password, @"[a-z]")) score++;
            if (Regex.IsMatch(password, @"[a-z].*[a-z]")) score++; // حداقل دو حرف کوچک

            // وجود حروف بزرگ
            if (Regex.IsMatch(password, @"[A-Z]")) score++;
            if (Regex.IsMatch(password, @"[A-Z].*[A-Z]")) score++; // حداقل دو حرف بزرگ

            // وجود کاراکترهای خاص
            if (Regex.IsMatch(password, @"[^a-zA-Z0-9]")) score += 2;

            // ترکیب متنوع کاراکترها
            var uniqueChars = password.Distinct().Count();
            if (uniqueChars >= 8) score++;

            return score switch
            {
                <= 3 => PasswordStrength.Weak,
                <= 6 => PasswordStrength.Medium,
                _ => PasswordStrength.Strong
            };
        }
    }

    public enum PasswordStrength
    {
        Weak,
        Medium,
        Strong
    }

    public static class PasswordStrengthExtensions
    {
        public static string GetDescription(this PasswordStrength strength)
        {
            return strength switch
            {
                PasswordStrength.Weak => "رمز عبور ضعیف است! لطفاً رمز قوی‌تری انتخاب کنید",
                PasswordStrength.Medium => "رمز عبور متوسط است",
                PasswordStrength.Strong => "رمز عبور قوی است ✓",
                _ => ""
            };
        }

        public static System.Windows.Media.Brush GetColor(this PasswordStrength strength)
        {
            return strength switch
            {
                PasswordStrength.Weak => System.Windows.Media.Brushes.Red,
                PasswordStrength.Medium => System.Windows.Media.Brushes.Orange,
                PasswordStrength.Strong => System.Windows.Media.Brushes.Green,
                _ => System.Windows.Media.Brushes.Gray
            };
        }

        public static string GetRequirements(this PasswordStrength strength)
        {
            return strength switch
            {
                PasswordStrength.Weak => "برای بهبود: حداقل 8 کاراکتر، ترکیب حروف و اعداد",
                PasswordStrength.Medium => "برای بهبود: حروف بزرگ و کاراکترهای خاص اضافه کنید",
                PasswordStrength.Strong => "تمام شرایط رمز عبور رعایت شده است",
                _ => ""
            };
        }
    }
}