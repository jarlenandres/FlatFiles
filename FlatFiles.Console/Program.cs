using FlatFiles.Core;

var userService = new UserService("Users.txt");
var personService = new PersonService("Persons.txt");

// LOG de arranque
LogWriter.LogSystem("Aplicación iniciada.");

string? loggedUser = null;

// ======= LOGIN =======
for (int attemptsLeft = 3; attemptsLeft > 0; attemptsLeft--)
{
    Console.Clear();
    Console.WriteLine("=== Login ===");
    Console.Write("Usuario: ");
    var username = Console.ReadLine()?.Trim() ?? "";
    var password = InputHelpers.ReadPasswordMasked();

    if (!userService.Exists(username))
    {
        LogWriter.Log(username, "Intento de login: usuario no existe.");
        Console.WriteLine("⚠️ Usuario no registrado.");
        Pause();
        continue;
    }

    if (!userService.IsActive(username))
    {
        LogWriter.Log(username, "Intento de login: usuario bloqueado.");
        Console.WriteLine("⚠️ Usuario bloqueado. Solicite desbloqueo al administrador.");
        Pause();
        continue;
    }

    var ok = userService.Authenticate(username, password);
    if (ok)
    {
        loggedUser = username;
        LogWriter.Log(loggedUser, "Login exitoso.");
        break;
    }
    else
    {
        LogWriter.Log(username, $"Login fallido. Intentos restantes: {attemptsLeft - 1}");
        Console.WriteLine($" Usuario/contraseña incorrectos. Intentos restantes: {attemptsLeft - 1}");
        if (attemptsLeft - 1 == 0)
        {
            // bloquea al usuario que intentó
            userService.BlockUser(username);
            LogWriter.Log(username, "Usuario bloqueado por exceder intentos fallidos.");
            Console.WriteLine("🔒 Usuario bloqueado por exceder intentos fallidos.");
        }
        Pause();
    }
}

if (loggedUser is null)
{
    LogWriter.LogSystem("Aplicación finalizada: no se logró autenticación.");
    return;
}

// ======= MENÚ PRINCIPAL =======
while (true)
{
    Console.Clear();
    Console.WriteLine("=== Gestión de Personas ===");
    Console.WriteLine("1) Crear persona");
    Console.WriteLine("2) Editar persona");
    Console.WriteLine("3) Borrar persona");
    Console.WriteLine("4) Listar personas");
    Console.WriteLine("5) Informe por ciudad");
    Console.WriteLine("0) Salir");
    Console.Write("Selecciona una opción: ");
    var opt = Console.ReadLine();

    try
    {
        switch (opt)
        {
            case "1":
                CrearPersona(personService, loggedUser);
                break;
            case "2":
                EditarPersona(personService, loggedUser);
                break;
            case "3":
                BorrarPersona(personService, loggedUser);
                break;
            case "4":
                ListarPersonas(personService, loggedUser);
                break;
            case "5":
                Informe(personService, loggedUser);
                break;
            case "0":
                LogWriter.Log(loggedUser, "Salida de la aplicación.");
                return;
            default:
                Console.WriteLine("⚠️ Opción inválida.");
                Pause();
                break;
        }
    }
    catch (Exception ex)
    {
        LogWriter.Log(loggedUser, $"Error: {ex.Message}");
        Console.WriteLine($"Ocurrió un error: {ex.Message}");
        Pause();
    }
}

// ======= FUNCIONES =======

static void CrearPersona(PersonService personService, string user)
{
    Console.Clear();
    Console.WriteLine("=== Crear Persona ===");

    int id = InputHelpers.PromptIntUnique("ID (único, numérico)", id => !personService.ExistsId(id));

    string _name = InputHelpers.PromptRequired("Nombres");
    string _firsname = InputHelpers.PromptRequired("Apellidos");
    string _phone = InputHelpers.PromptTelefono();
    Console.Write("Ciudad: ");
    string _city = Console.ReadLine()?.Trim() ?? "";
    if (string.IsNullOrWhiteSpace(_city))
    {
        Console.WriteLine("⚠️ La ciudad es obligatoria.");
        _city = InputHelpers.PromptRequired("Ciudad");
    }
    decimal _balance = InputHelpers.PromptSaldoPositivo();

    var p = new Person
    {
        Id = id,
        Name = _name,
        FirstName = _firsname,
        Phone = _phone,
        City = _city,
        Balance = _balance
    };

    personService.Create(p);
    LogWriter.Log(user, $"Creó persona ID={p.Id}, {p.Name} {p.FirstName}, Ciudad={p.City}, Saldo={p.Balance}");
    Console.WriteLine("Persona creada correctamente.");
    Pause();
}

static void EditarPersona(PersonService personService, string user)
{
    Console.Clear();
    Console.WriteLine("=== Editar Persona ===");

    int id = InputHelpers.PromptExistingId("ID de la persona", personService.ExistsId);
    var p = personService.GetById(id)!;

    Console.WriteLine($"Editando ID={p.Id} | {p.Name} {p.FirstName} | Tel: {p.Phone} | Ciudad: {p.City} | Saldo: {p.Balance}");

    var nombres = InputHelpers.PromptOptional("Nombres", p.Name);
    var apellidos = InputHelpers.PromptOptional("Apellidos", p.FirstName);
    var telefono = InputHelpers.PromptTelefonoOptional(p.Phone);
    var ciudad = InputHelpers.PromptOptional("Ciudad", p.City);
    var saldo = InputHelpers.PromptSaldoPositivoOptional(p.Balance);

    if (nombres is not null) p.Name = nombres;
    if (apellidos is not null) p.FirstName = apellidos;
    if (telefono is not null) p.Phone = telefono;
    if (ciudad is not null) p.City = ciudad;
    if (saldo is not null) p.Balance = saldo.Value;

    personService.Update(p);
    LogWriter.Log(user, $"Editó persona ID={p.Id}. Nuevos datos: {p.Name} {p.FirstName}, Tel: {p.Phone}, Ciudad: {p.City}, Saldo: {p.Balance}");
    Console.WriteLine("Persona actualizada.");
    Pause();
}

static void BorrarPersona(PersonService personService, string user)
{
    Console.Clear();
    Console.WriteLine("=== Borrar Persona ===");

    int id = InputHelpers.PromptExistingId("ID de la persona", personService.ExistsId);
    var p = personService.GetById(id)!;

    Console.WriteLine();
    Console.WriteLine($"ID: {p.Id}");
    Console.WriteLine($"Nombres: {p.Name}");
    Console.WriteLine($"Apellidos: {p.FirstName}");
    Console.WriteLine($"Teléfono: {p.Phone}");
    Console.WriteLine($"Ciudad: {p.City}");
    Console.WriteLine($"Saldo: {p.Balance}");
    Console.WriteLine();
    Console.Write("¿Confirmas borrar? (S/N): ");
    var conf = Console.ReadLine()?.Trim().ToUpperInvariant();

    if (conf == "S" || conf == "SI")
    {
        if (personService.Delete(id))
        {
            LogWriter.Log(user, $"Borró persona ID={id}");
            Console.WriteLine("🗑️ Persona borrada.");
        }
        else
        {
            Console.WriteLine("No se pudo borrar.");
        }
    }
    else
    {
        Console.WriteLine("Operación cancelada.");
    }
    Pause();
}

static void ListarPersonas(PersonService personService, string user)
{
    Console.Clear();
    Console.WriteLine("=== Listado de Personas ===");
    Console.WriteLine($"{"ID",-4}{"Nombres",-15}{"Apellidos",-15}{"Teléfono",-15}{"Ciudad",-15}{"Saldo",12}");
    Console.WriteLine(new string('-', 76));
    foreach (var p in personService.GetAll().OrderBy(p => p.Id))
    {
        Console.WriteLine($"{p.Id,-4}{p.Name,-15}{p.FirstName,-15}{p.Phone,-15}{p.City,-15}{p.Balance.ToString("N2", System.Globalization.CultureInfo.InvariantCulture),12}");
    }
    LogWriter.Log(user, "Listó personas.");
    Pause();
}

static void Informe(PersonService personService, string user)
{
    Console.Clear();
    Console.WriteLine("=== Informe por Ciudad ===");
    personService.Report();
    LogWriter.Log(user, "Generó informe por ciudad.");
    Pause();
}

static void Pause()
{
    Console.WriteLine();
    Console.Write("Presiona ENTER para continuar...");
    Console.ReadLine();
}
