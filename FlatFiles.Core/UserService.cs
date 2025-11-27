using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFiles.Core;

public class UserService
{

    private readonly string _userFile;
    private readonly CsvConfiguration _cfg;
    private List<User> _users = new();

    public UserService(string userFile = "Users.txt")
    {
        _userFile = userFile;
        _cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = ",",
            TrimOptions = TrimOptions.Trim,
            ShouldQuote = args => true
        };
        EnsureUsersFileExists();
        LoadUsers();
    }

    private void EnsureUsersFileExists()
    {
        if (!File.Exists(_userFile))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_userFile)) ?? ".");
            // Crea con datos de ejemplo
            var sample = new[]
            {
                "jzuluaga,P@ssw0rd123!,true",
                "mbedoya,S0yS3gur02025*,false"
            };
            File.WriteAllLines(_userFile, sample);
        }
    }

    private void LoadUsers()
    {
        using var reader = new StreamReader(_userFile);
        using var csv = new CsvReader(reader, _cfg);
        csv.Context.RegisterClassMap<CsvMappings>();
        _users = csv.GetRecords<User>().ToList();
    }

    private void SaveUsers()
    {
        using var writer = new StreamWriter(_userFile, false);
        using var csv = new CsvWriter(writer, _cfg);
        csv.Context.RegisterClassMap<CsvMappings>();
        csv.WriteRecords(_users);
    }

    public bool Exists(string username) => _users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    public bool IsActive(string username) =>
        _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))?.Active ?? false;

    public bool Authenticate(string username, string password)
    {
        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user is null) return false;
        if (!user.Active) return false;
        return user.Password == password;
    }

    public void BlockUser(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user is null) return;
        if (!user.Active) return;
        user.Active = false;
        SaveUsers();
    }

}
