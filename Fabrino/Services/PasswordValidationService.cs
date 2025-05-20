using System.Text.RegularExpressions;

namespace Fabrino.Services
{
    public class PasswordValidationService
    {
        public bool IsPasswordStrongEnough(string password)
        {
            return password.Length >= 8 &&
                   Regex.IsMatch(password, @"[a-zA-Z]") &&
                   Regex.IsMatch(password, @"[0-9]");
        }

        public PasswordStrength CheckPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password)) return PasswordStrength.Weak;

            int score = 0;
            if (password.Length >= 8) score++;
            if (Regex.IsMatch(password, @"[0-9]")) score++;
            if (Regex.IsMatch(password, @"[a-z]")) score++;
            if (Regex.IsMatch(password, @"[A-Z]")) score++;
            if (Regex.IsMatch(password, @"[^a-zA-Z0-9]")) score++;

            return score switch
            {
                < 3 => PasswordStrength.Weak,
                < 5 => PasswordStrength.Medium,
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
                PasswordStrength.Weak => " رمز عبور ضعیف",
                PasswordStrength.Medium => " رمز عبور متوسط",
                PasswordStrength.Strong => " رمز عبور قوی",
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
    }
}