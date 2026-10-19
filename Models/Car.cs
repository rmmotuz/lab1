namespace Lab1.Models;

using Lab1.Interfaces;

public class Car : LandVehicle, IDiagnosable, IServiceable
{
    private const double FuelPricePerLiter = 56.50;
    private const double BaseConsumptionPer100Km = 6.0;
    private const double ConsumptionPerLiterOfEngine = 1.5;
    private const double TankCapacityLiters = 60.0;
    private const double DefaultEngineDisplacementLiters = 1.6;

    private string _diagnosticStatus = "OK: Усі системи працюють у штатному режимі";
    private DateTime _lastServiceDate = DateTime.Now.AddMonths(-3);

    public string BodyType { get; set; }
    public double EngineDisplacementLiters { get; }

    static Car()
    {
        ConstructorLog.Write("static Car()");
    }

    public Car() : this("Стандартне легкове авто", 60.0, "Седан", DefaultEngineDisplacementLiters)
    {
        ConstructorLog.Write("Car() [за замовчуванням]");
    }

    public Car(string model, double initialFuel, string bodyType, double engineDisplacement)
        : base(model, initialFuel, wheelCount: 4, transmissionType: "Автоматична")
    {
        BodyType = string.IsNullOrWhiteSpace(bodyType) ? "Седан" : bodyType;
        EngineDisplacementLiters = engineDisplacement >= 0 ? engineDisplacement : DefaultEngineDisplacementLiters;
        ConstructorLog.Write($"Car(body: \"{BodyType}\", engine: {EngineDisplacementLiters} л)");
    }

    public Car(Car other) : base(other)
    {
        BodyType = other.BodyType;
        EngineDisplacementLiters = other.EngineDisplacementLiters;
        ConstructorLog.Write("Car(Car other) [копіювання]");
    }

    public void Deconstruct(out string model, out double fuel, out string bodyType)
    {
        model = Model;
        fuel = FuelLevel;
        bodyType = BodyType;
    }

    private double FuelConsumptionPer100Km => BaseConsumptionPer100Km + EngineDisplacementLiters * ConsumptionPerLiterOfEngine;

    public override void StartEngine() =>
        Console.WriteLine($"   {Model}: двигун {EngineDisplacementLiters:F1} л запущено.");

    public override double CalculateOperationalCost(double distanceKm)
    {
        double litersNeeded = distanceKm / 100.0 * FuelConsumptionPer100Km;
        return litersNeeded * FuelPricePerLiter;
    }

    public override double CalculateRemainingRangeKm() =>
        FuelLevel / 100.0 * (TankCapacityLiters / FuelConsumptionPer100Km * 100.0);

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"   Кузов: {BodyType}, об'єм двигуна: {EngineDisplacementLiters:F1} л");
    }

    public string DiagnosticStatus => _diagnosticStatus;
    public DateTime LastServiceDate => _lastServiceDate;

    public void PerformService(string serviceType)
    {
        _lastServiceDate = DateTime.Now;
        Console.WriteLine($"   {Model}: виконано обслуговування «{serviceType}».");
    }

    void IDiagnosable.RunDiagnostics()
    {
        _diagnosticStatus = "OK: Діагностику пройдено без зауважень";
        Console.WriteLine($"   {Model}: {_diagnosticStatus}");
    }

    void IDiagnosable.PerformCheck() =>
        Console.WriteLine("   IDiagnosable.PerformCheck(): перевірка електроніки та датчиків.");

    void IServiceable.PerformCheck() =>
        Console.WriteLine("   IServiceable.PerformCheck(): перевірка рідин, гальм і шин.");
}
