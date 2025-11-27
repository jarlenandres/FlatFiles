using FlatFiles.Core;

Console.WriteLine("===========================================");
Console.WriteLine("1. Show Content");
Console.WriteLine("2. Add person");
Console.WriteLine("3. Save changes");
Console.WriteLine("4. Exit");
Console.Write("Choose an option: ");
Console.ReadLine();
Console.WriteLine("===========================================");

var helper = new NugetCsvHelper();
var people = new helper.Read($"people.csv").ToList();