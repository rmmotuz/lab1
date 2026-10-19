namespace Lab1.Models;

public abstract class Vehicle : IComparable<Vehicle>
{
    private const double MinFuelLevel = 0.0;
    private const double MaxFuelLevel = 100.0;
    private const double LowFuelThreshold = 10.0;
    private const double DefaultRangeKmPerFuelPercent = 6.0;

    private readonly string _vin;
    protected double _fuelLevel;
    internal string FleetRegistryCode;
    protected internal DateTime ManufactureDate;
    private protected string SecurityToken;
    private static int _totalVehiclesCreated;

    public double FuelLevel
    {
        get => _fuelLevel;
        set
        {
            if (value < MinFuelLevel || value > MaxFuelLevel)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Неприпустимий рівень пального: {value}%. Значення має бути в межах від {MinFuelLevel}% до {MaxFuelLevel}%.");
            }

            if (value <= LowFuelThreshold)
                Console.WriteLine($"   [!] Увага: низький рівень пального/заряду ({value:F1}%).");

            _fuelLevel = value;
        }
    }

    public string Model { get; }
    public string Vin => _vin;
    public static int TotalVehiclesCreated => _totalVehiclesCreated;

    static Vehicle()
    {
        _totalVehiclesCreated = 0;
        ConstructorLog.Write("static Vehicle()");
    }

    protected Vehicle() : this("Стандартний транспортний засіб", 50.0)
    {
        ConstructorLog.Write("Vehicle() [за замовчуванням]");
    }

    protected Vehicle(string model, double initialFuel)
    {
        Model = string.IsNullOrWhiteSpace(model) ? "Невідома модель" : model;
        FuelLevel = initialFuel;
        _vin = GenerateVin();
        FleetRegistryCode = GenerateRegistryCode();
        ManufactureDate = DateTime.Now.Date;
        SecurityToken = $"SEC-{Guid.NewGuid().ToString()[..4]}";

        _totalVehiclesCreated++;
        ConstructorLog.Write($"Vehicle(model: \"{Model}\", fuel: {initialFuel})");
    }

    protected Vehicle(Vehicle other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Model = other.Model;
        _fuelLevel = other._fuelLevel;
        _vin = GenerateVin();
        FleetRegistryCode = GenerateRegistryCode();
        ManufactureDate = DateTime.Now.Date;
        SecurityToken = other.SecurityToken;

        _totalVehiclesCreated++;
        ConstructorLog.Write("Vehicle(Vehicle other) [копіювання]");
    }

    private static string GenerateVin() => $"VIN-{Guid.NewGuid().ToString()[..8].ToUpper()}";

    private static string GenerateRegistryCode() => $"REG-{DateTime.Now.Year}-{_totalVehiclesCreated + 1:D4}";

    public abstract void StartEngine();
    public abstract double CalculateOperationalCost(double distanceKm);

    /// <summary>Орієнтовний запас ходу (км). Нащадки перевизначають формулу під свій тип двигуна.</summary>
    public virtual double CalculateRemainingRangeKm() => FuelLevel * DefaultRangeKmPerFuelPercent;

    public virtual void DisplayInfo() => Console.WriteLine($"   {this}");

    public virtual void Refuel(double amount)
    {
        if (amount <= 0)
            return;

        FuelLevel = Math.Min(MaxFuelLevel, _fuelLevel + amount);
    }

    public override string ToString() =>
        $"[{GetType().Name}] '{Model}' (VIN: {_vin}, Пальне/Заряд: {_fuelLevel:F1}%)";

    public override bool Equals(object? obj) =>
        obj is Vehicle other && string.Equals(Vin, other.Vin, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(Vin);

    public int CompareTo(Vehicle? other)
    {
        if (other is null) return 1;
        return _fuelLevel.CompareTo(other._fuelLevel);
    }

    public static Vehicle operator +(Vehicle vehicle, double amount)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        vehicle.Refuel(amount);
        return vehicle;
    }

    public static bool operator ==(Vehicle? left, Vehicle? right) => Equals(left, right);
    public static bool operator !=(Vehicle? left, Vehicle? right) => !Equals(left, right);

    public static bool operator <(Vehicle left, Vehicle right) => left.CompareTo(right) < 0;
    public static bool operator >(Vehicle left, Vehicle right) => left.CompareTo(right) > 0;
}
