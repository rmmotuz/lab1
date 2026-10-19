namespace Lab1.Models;

public abstract class LandVehicle : Vehicle
{
    public int WheelCount { get; }
    public string TransmissionType { get; set; }

    static LandVehicle()
    {
        ConstructorLog.Write("static LandVehicle()");
    }

    protected LandVehicle() : base("Невідомий наземний транспорт", 50.0)
    {
        WheelCount = 4;
        TransmissionType = "Механічна";
        ConstructorLog.Write("LandVehicle() [за замовчуванням]");
    }

    protected LandVehicle(string model, double initialFuel, int wheelCount, string transmissionType)
        : base(model, initialFuel)
    {
        if (wheelCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(wheelCount), "Кількість коліс має бути додатною.");

        WheelCount = wheelCount;
        TransmissionType = string.IsNullOrWhiteSpace(transmissionType) ? "Автоматична" : transmissionType;
        ConstructorLog.Write($"LandVehicle(wheels: {WheelCount}, transmission: \"{TransmissionType}\")");
    }

    protected LandVehicle(LandVehicle other) : base(other)
    {
        WheelCount = other.WheelCount;
        TransmissionType = other.TransmissionType;
        ConstructorLog.Write("LandVehicle(LandVehicle other) [копіювання]");
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"   Коліс: {WheelCount}, коробка передач: {TransmissionType}");
    }
}
