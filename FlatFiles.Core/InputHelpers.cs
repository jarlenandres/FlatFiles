using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFiles.Core;

public class InputHelpers
{
    public static string PromptRequired(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var input = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrWhiteSpace(input))
                return input;
            Console.WriteLine("⚠️ Este campo es obligatorio. Intenta de nuevo.");
        }
    }

    public static string? PromptOptional(string label, string currentValue)
    {
        Console.Write($"{label} ({currentValue}) [ENTER para mantener]: ");
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input)) return null; // mantener
        input = input.Trim();
        return input.Length == 0 ? null : input;
    }

    public static int PromptIntUnique(string label, Func<int, bool> isUnique)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var id))
            {
                if (isUnique(id)) return id;
                Console.WriteLine("⚠️ El ID ya existe. Debe ser único.");
            }
            else
            {
                Console.WriteLine("⚠️ El ID debe ser un número entero.");
            }
        }
    }

    public static int PromptExistingId(string label, Func<int, bool> exists)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var id))
            {
                if (exists(id)) return id;
                Console.WriteLine("⚠️ El ID no existe.");
            }
            else
            {
                Console.WriteLine("⚠️ El ID debe ser un número entero.");
            }
        }
    }

    public static string PromptTelefono()
    {
        while (true)
        {
            Console.Write("Teléfono (solo dígitos, mínimo 7): ");
            var t = Console.ReadLine()?.Trim() ?? "";
            if (t.All(char.IsDigit) && t.Length >= 7 && t.Length <= 15)
                return t;

            Console.WriteLine("⚠️ Teléfono inválido.");
        }
    }

    public static string? PromptTelefonoOptional(string currentValue)
    {
        Console.Write($"Teléfono ({currentValue}) [ENTER para mantener]: ");
        var t = Console.ReadLine();
        if (string.IsNullOrEmpty(t)) return null;
        t = t.Trim();
        if (t.All(char.IsDigit) && t.Length >= 7 && t.Length <= 15)
            return t;

        Console.WriteLine("⚠️ Teléfono inválido. Se mantiene el valor previo.");
        return null;
    }

    public static decimal PromptSaldoPositivo()
    {
        while (true)
        {
            Console.Write("Saldo (número positivo): ");
            var s = Console.ReadLine();
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var saldo) && saldo > 0)
                return saldo;

            Console.WriteLine("⚠️ Saldo inválido. Debe ser un número positivo (ej: 5000.00).");
        }
    }

    public static decimal? PromptSaldoPositivoOptional(decimal currentValue)
    {
        Console.Write($"Saldo ({currentValue.ToString("N2", CultureInfo.InvariantCulture)}) [ENTER para mantener]: ");
        var s = Console.ReadLine();
        if (string.IsNullOrEmpty(s)) return null;
        if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var saldo) && saldo > 0)
            return saldo;

        Console.WriteLine("⚠️ Saldo inválido. Se mantiene el valor previo.");
        return null;
    }

    public static string ReadPasswordMasked(string prompt = "Contraseña: ")
    {
        Console.Write(prompt);
        var pwd = new Stack<char>();
        ConsoleKeyInfo keyInfo;
        while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (pwd.Count > 0)
                {
                    Console.Write("\b \b");
                    pwd.Pop();
                }
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                pwd.Push(keyInfo.KeyChar);
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return new string(pwd.Reverse().ToArray());
    }
}
