namespace Lab1.Models;

public class ElectricCar : Car
{
    private const double ElectricityPricePerKwh = 4.32;
    private const double ConsumptionKwhPer100Km = 16.0;
    private const double DefaultBatteryCapacityKwh = 60.0;

    private double _batteryChargePercentage;

    public double BatteryCapacityKwh { get; }

    public double BatteryChargePercentage
    {
        get => _batteryChargePercentage;
        set
        {
            if (value < 0.0 || value > 100.0)
                throw new ArgumentOutOfRangeException(nameof(value), "Заряд батареї має бути в межах від 0.0% до 100.0%.");

            _batteryChargePercentage = value;
            FuelLevel = _batteryChargePercentage;
        }
    }

    static ElectricCar()
    {
        ConstructorLog.Write("static ElectricCar()");
    }

    public ElectricCar() : this("Tesla Model 3", 85.0, 75.0)
    {
        ConstructorLog.Write("ElectricCar() [за замовчуванням]");
    }

    public ElectricCar(string model, double initialCharge, double batteryCapacityKwh)
        : base(model, initialCharge, bodyType: "Седан / Fastback", engineDisplacement: 0.0)
    {
        BatteryCapacityKwh = batteryCapacityKwh > 0 ? batteryCapacityKwh : DefaultBatteryCapacityKwh;
        BatteryChargePercentage = initialCharge;
        ConstructorLog.Write($"ElectricCar(battery: {BatteryCapacityKwh} кВт*год)");
    }

    public override void StartEngine() =>
        Console.WriteLine($"   {Model}: електродвигун активовано (безшумно).");

    public override double CalculateOperationalCost(double distanceKm)
    {
        double energyNeeded = distanceKm / 100.0 * ConsumptionKwhPer100Km;
        return energyNeeded * ElectricityPricePerKwh;
    }

    public override double CalculateRemainingRangeKm() =>
        FuelLevel / 100.0 * (BatteryCapacityKwh / ConsumptionKwhPer100Km * 100.0);

    public override string ToString() =>
        $"{base.ToString()} | Батарея: {BatteryCapacityKwh} кВт*год (Заряд: {_batteryChargePercentage:F1}%)";
}
