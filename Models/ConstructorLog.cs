namespace Lab1.Models;

// Вмикає/вимикає трасування порядку виклику конструкторів
public static class ConstructorLog
{
    public static bool Enabled { get; set; }

    public static void Write(string message)
    {
        if (Enabled)
            Console.WriteLine($"   [ctor] {message}");
    }
}
