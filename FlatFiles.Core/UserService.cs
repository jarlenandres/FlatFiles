using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFiles.Core;

internal class UserService
{
    private readonly string userFile = "User.txt";
    private List<User> _users;

    public UserService()
    {
        LoadUsers();
    }

    private void LoadUsers()
    {
        using var sr = new StreamReader(userFile);
        using var cr = new CsvReader(sr, CultureInfo.InvariantCulture);
        _users = cr.GetRecords<User>().ToList();
    }

    public bool Authenticate(string username, string password)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);
        if (user is null || !user.Active)
        {
            return false;
        }
        return user.Password == password;
    }

    public void BlockUser(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);
        if (user != null)
        {
            user.Active = false;
            SaveUsers();
        }
    }

    private void SaveUsers()
    {
        using var sw = new StreamWriter(userFile);
        using var cw = new CsvWriter(sw, CultureInfo.InvariantCulture);
        cw.WriteRecords(_users);
    }
}
