using UnityEngine;
using System;

public class AIModule
{
    public string ID { get; private set; }         // Identificador único de tres letras
    public string Role { get; private set; }       // Rol del módulo
    public float FlightHours { get; private set; } // Horas de vuelo del módulo

    public AIModule(string role)
    {
        ID = GenerateID();
        Role = role;
        FlightHours = 0f;
    }

    private string GenerateID()
    {
        // Generar un ID de tres letras aleatorias (de 'A' a 'Z')
        char letter1 = (char)UnityEngine.Random.Range('A', 'Z' + 1);
        char letter2 = (char)UnityEngine.Random.Range('A', 'Z' + 1);
        char letter3 = (char)UnityEngine.Random.Range('A', 'Z' + 1);
        return $"{letter1}{letter2}{letter3}";
    }

    public void AddFlightHours(float hours)
    {
        FlightHours += hours;
    }
}
