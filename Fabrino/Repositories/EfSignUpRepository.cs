using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

public class EfSignUpRepository : ISignUpRepository
{
    private readonly AppDbContext _db;

    public EfSignUpRepository(AppDbContext db)
    {
        _db = db;
    }

    public bool RegisterUser(UserModel user)
    {
        try
        {
            _db.Users.Add(user);
            return _db.SaveChanges() > 0;
        }
        catch (Exception ex) // <-- تغییر اینجا
        {
            MessageBox.Show($"خطای دیتابیس: {ex.Message}\n{ex.InnerException?.Message}");
            return false;
        }
    }

    public bool UpdateUser(UserModel user)
    {
        try
        {
            _db.Users.Update(user);
            _db.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsUsernameTaken(string username)
    {
        return _db.Users.Any(u => u.username == username);
    }
}