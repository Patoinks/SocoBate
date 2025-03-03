using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


public static class Validator
{
    static bool IsSymbol(char c)
    {
        return c > 32 && c < 127 && !Char.IsDigit(c) && !Char.IsLetter(c);
    }
    public static bool ValidatePassword(string input)
    {
        List<Exception> exceptions = new List<Exception>();

        if (input.Length < 6)
        {
            exceptions.Add(new Exception("Password must be at least 6 characters long"));
        }

        if (!input.Any(c => Char.IsLetter(c)))
        {
            exceptions.Add(new Exception("Password must contain at least one letter"));
        }

        // if (!input.Any(c => Char.IsDigit(c)))
        // {
        //     exceptions.Add(new Exception("Password must contain at least one digit"));
        // }

        // if (!input.Any(c => IsSymbol(c)))
        // {
        //     exceptions.Add(new Exception("Password must contain at least one symbol"));
        // }

        // if (input.Where(char.IsUpper).Count() < 1)
        // {
        //     exceptions.Add(new Exception("Password must contain at least one uppercase letter"));
        // }

        if (exceptions.Count > 0)
        {
            return false;
            throw new AggregateException(exceptions);
        }

        return true;
    }

    public static bool HasDigit(string input)
    {
        return input.Any(c => Char.IsDigit(c));
    }

    public static bool HasSymbol(string input)
    {
        return input.Any(c => IsSymbol(c));
    }

    public static bool HasUppercase(string input)
    {
        return input.Any(c => Char.IsUpper(c));
    }

    public static bool HasChar(string input)
    {
        return input.Any(c => Char.IsLetter(c));
    }

    public static bool HasMinimumLength(string input, int minLength = 6)
    {
        return input.Length >= minLength;
    }
}
