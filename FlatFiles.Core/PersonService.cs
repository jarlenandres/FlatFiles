using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFiles.Core;

public class PersonService
{
    private readonly string _personFile;
    private readonly CsvConfiguration _cfg;
    private List<Person> _persons = new();

    public PersonService(string personFile = "Persons.csv")
    {
        _personFile = personFile;
        _cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = ",",
            TrimOptions = TrimOptions.Trim,
            ShouldQuote = args => true
        };
        EnsurePersonsFileExists();
        LoadPersons();
    }

    private void EnsurePersonsFileExists()
    {
        if (!File.Exists(_personFile))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_personFile)) ?? ".");
            File.WriteAllText(_personFile, "");
        }
    }

    private void LoadPersons()
    {
        using var reader = new StreamReader(_personFile);
        using var csv = new CsvReader(reader, _cfg);
        csv.Context.RegisterClassMap<PersonMap>();
        _persons = csv.GetRecords<Person>().ToList();
    }

    private void SavePersons()
    {
        using var writer = new StreamWriter(_personFile, false);
        using var csv = new CsvWriter(writer, _cfg);
        csv.Context.RegisterClassMap<PersonMap>();
        csv.WriteRecords(_persons);
    }

    public IEnumerable<Person> GetAll() => _persons;

    public bool ExistsId(int id) => _persons.Any(p => p.Id == id);

    public Person? GetById(int id) => _persons.FirstOrDefault(p => p.Id == id);

    public void Create(Person person)
    {
        _persons.Add(person);
        SavePersons();
    }

    public void Update(Person person)
    {
        var idx = _persons.FindIndex(p => p.Id == person.Id);
        if (idx >= 0)
        {
            _persons[idx] = person;
            SavePersons();
        }
    }

    public bool Delete(int id)
    {
        var p = GetById(id);
        if (p is null) return false;
        _persons.Remove(p);
        SavePersons();
        return true;
    }

    public void Report()
    {
        var culture = CultureInfo.InvariantCulture;
        var groups = _persons
            .OrderBy(p => p.City)
            .ThenBy(p => p.Id)
            .GroupBy(p => p.City);

        decimal grandTotal = 0m;

        foreach (var g in groups)
        {
            Console.WriteLine();
            Console.WriteLine($"Ciudad: {g.Key}");
            Console.WriteLine();
            Console.WriteLine($"{"ID",-4}{"Nombres",-15}{"Apellidos",-15}{"Saldo",12}");
            Console.WriteLine(new string('—', 4) + new string('—', 15) + new string('—', 15) + new string('—', 12));

            foreach (var p in g)
            {
                Console.WriteLine($"{p.Id,-4}{p.Name,-15}{p.FirstName,-15}{p.Balance.ToString("N2", culture),12}");
            }

            Console.WriteLine($"{new string(' ', 34)}{"=======",12}");
            var subtotal = g.Sum(x => x.Balance);
            Console.WriteLine($"Total: {g.Key}{new string(' ', 17)}{subtotal.ToString("N2", culture),12}");
            grandTotal += subtotal;
        }

        Console.WriteLine($"{new string(' ', 34)}{"=======",12}");
        Console.WriteLine($"Total General:{new string(' ', 17)}{grandTotal.ToString("N2", culture),12}");
    }
}