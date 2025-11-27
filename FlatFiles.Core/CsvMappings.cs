using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFiles.Core;

public sealed class CsvMappings : ClassMap<User>
{

    public CsvMappings()
    {
        Map(m => m.Username).Index(0);
        Map(m => m.Password).Index(1);
        Map(m => m.Active).Index(2);
    }
}

public sealed class PersonMap : ClassMap<Person>
{
    public PersonMap()
    {
        Map(m => m.Id).Index(0);
        Map(m => m.Name).Index(1);
        Map(m => m.FirstName).Index(2);
        Map(m => m.Phone).Index(3);
        Map(m => m.City).Index(4);
        Map(m => m.Balance).Index(5);
    }

}
