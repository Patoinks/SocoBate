using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Cryptography
{
    public static string HashSHA512(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        using (SHA512 sha512algo = SHA512.Create())
        {
            byte[] hashBytes = sha512algo.ComputeHash(inputBytes);
            string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hashString;
        }
    }
}