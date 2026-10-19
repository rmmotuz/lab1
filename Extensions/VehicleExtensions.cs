namespace Lab1.Extensions;

using Lab1.Models;

public static class VehicleExtensions
{
    public static double EstimateRemainingRangeKm(this Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        return vehicle.CalculateRemainingRangeKm();
    }

    public static string GetShortSummary(this Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        return $"'{vehicle.Model}': Залишок = {vehicle.FuelLevel:F1}%, Запас ходу: ~{vehicle.EstimateRemainingRangeKm():F1} км.";
    }

    public static void PrintShortSummary(this Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        Console.WriteLine($"   {vehicle.GetShortSummary()}");
    }
}
