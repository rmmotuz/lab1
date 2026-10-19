namespace Lab1.Diagnostics;

using Lab1.Extensions;
using Lab1.Interfaces;
using Lab1.Models;

public static class OopDemo
{
    public static void Run()
    {
        Section("Порядок виклику конструкторів (new ElectricCar())");
        ConstructorLog.Enabled = true;
        var tesla = new ElectricCar();
        ConstructorLog.Enabled = false;

        Section("Інші конструктори та приватний конструктор (Singleton)");
        var car = new Car("Toyota Camry", 40.0, "Седан", 2.5);
        var truck = Truck.CreateHeavyDutyHauler();
        var registry = FleetRegistry.Instance;
        registry.RegisterVehicle(tesla);
        registry.RegisterVehicle(car);
        registry.RegisterVehicle(truck);
        Console.WriteLine($"Зареєстровано: {registry.GetAllVehicles().Count}, створено всього: {Vehicle.TotalVehiclesCreated}");

        Section("Поліморфізм: віртуальні та перевизначені методи");
        foreach (Vehicle v in registry.GetAllVehicles())
        {
            v.DisplayInfo();
            v.StartEngine();
            Console.WriteLine($"   Вартість 100 км: {v.CalculateOperationalCost(100):F2} грн");
        }

        Section("Явна реалізація інтерфейсів");
        ((IDiagnosable)car).RunDiagnostics();
        ((IDiagnosable)car).PerformCheck();
        ((IServiceable)car).PerformCheck();
        car.PerformService("Заміна мастила");

        Section("Метод-розширення та перевантажений оператор +");
        foreach (Vehicle v in registry.GetAllVehicles())
            v.PrintShortSummary();
        _ = car + 20.0;
        Console.WriteLine($"Після car + 20: {car.FuelLevel:F1}%");

        Section("Властивість із перевіркою та обробка помилок");
        Try(() => car.FuelLevel = 150.0);
        truck.LoadCargo(20.0);
        Try(() => truck.LoadCargo(10.0));
    }

    private static void Try(Action action)
    {
        try { action(); }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException or InvalidOperationException)
        {
            Console.WriteLine($"   Виняток {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void Section(string title) => Console.WriteLine($"\n=== {title} ===");
}
