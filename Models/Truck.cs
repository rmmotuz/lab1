namespace Lab1.Models;

public class Truck : LandVehicle
{
    private const double DieselPricePerLiter = 55.0;
    private const double BaseConsumptionPer100Km = 28.0;
    private const double ConsumptionPerCargoTon = 1.2;
    private const double TankCapacityLiters = 300.0;
    private const double DefaultMaxCargoTons = 10.0;

    private double _currentCargoTons;

    public double MaxCargoCapacityTons { get; }

    public double CurrentCargoTons
    {
        get => _currentCargoTons;
        private set
        {
            if (value < 0 || value > MaxCargoCapacityTons)
                throw new ArgumentOutOfRangeException(nameof(value), $"Вага вантажу повинна бути від 0 до {MaxCargoCapacityTons} тонн.");
            _currentCargoTons = value;
        }
    }

    static Truck()
    {
        ConstructorLog.Write("static Truck()");
    }

    public Truck() : this("Стандартна вантажівка MAN", 70.0, 6, "Механічна 12-ступенева", 15.0)
    {
        ConstructorLog.Write("Truck() [за замовчуванням]");
    }

    public Truck(string model, double initialFuel, int wheelCount, string transmissionType, double maxCargoCapacityTons)
        : base(model, initialFuel, wheelCount, transmissionType)
    {
        MaxCargoCapacityTons = maxCargoCapacityTons > 0 ? maxCargoCapacityTons : DefaultMaxCargoTons;
        _currentCargoTons = 0.0;
        ConstructorLog.Write($"Truck(maxCargo: {MaxCargoCapacityTons} т)");
    }

    /// <summary>Фабричний метод для важкого тягача.</summary>
    public static Truck CreateHeavyDutyHauler() =>
        new("MAN TGX Heavy Hauler", 90.0, 8, "Автоматична 16-ступенева", 25.0);

    public void LoadCargo(double tons)
    {
        if (tons <= 0)
            throw new ArgumentOutOfRangeException(nameof(tons), "Вага вантажу для завантаження має бути додатною.");

        if (_currentCargoTons + tons > MaxCargoCapacityTons)
            throw new InvalidOperationException(
                $"Перевищено вантажопідйомність: {_currentCargoTons + tons:F1} т > {MaxCargoCapacityTons:F1} т.");

        CurrentCargoTons += tons;
    }

    public void UnloadCargo() => CurrentCargoTons = 0;

    private double FuelConsumptionPer100Km => BaseConsumptionPer100Km + CurrentCargoTons * ConsumptionPerCargoTon;

    public override void StartEngine() =>
        Console.WriteLine($"   {Model}: дизельний двигун запущено.");

    public override double CalculateOperationalCost(double distanceKm)
    {
        double litersNeeded = distanceKm / 100.0 * FuelConsumptionPer100Km;
        return litersNeeded * DieselPricePerLiter;
    }

    public override double CalculateRemainingRangeKm() =>
        FuelLevel / 100.0 * (TankCapacityLiters / FuelConsumptionPer100Km * 100.0);

    public override string ToString() =>
        $"{base.ToString()} | Вантаж: {CurrentCargoTons:F1}/{MaxCargoCapacityTons:F1} т";
}
