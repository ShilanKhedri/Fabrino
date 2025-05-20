using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;

public enum PasswordStrength
{
    Weak,      // ضعیف
    Medium,    // متوسط
    Strong     // قوی
}
public static class UserValidationService
{

    
    public static bool ValidatePassword(string password, string confirmPassword)
    {
        return password == confirmPassword && password.Length >= 8;
    }

    public static bool ValidateUsername(string username)
    {
        return !string.IsNullOrWhiteSpace(username) && username.Length >= 3 ;
    }

    public static bool ValidateUsernameFormat(string username)
    {
        return Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
    }

    public static bool ValidatePersianName(string name)
    {
        return Regex.IsMatch(name, @"^[\u0600-\u06FF\s]+$");
    }


    public static void ValidateUsernameRealTime(TextBox usernameBox)
    {
        string username = usernameBox.Text.Trim();
        bool isValid = ValidateUsernameFormat(username);

        usernameBox.BorderBrush = isValid ? Brushes.Gray : Brushes.Red;
        usernameBox.ToolTip = isValid ? null : "فقط حروف انگلیسی، اعداد و _ مجاز است";
    }

    public static bool ValidateSecurityQuestion(string question)
    {
        return !string.IsNullOrWhiteSpace(question);
    }

    public static bool ValidateSecurityAnswer(string answer)
    {
        return !string.IsNullOrWhiteSpace(answer);
    }
}