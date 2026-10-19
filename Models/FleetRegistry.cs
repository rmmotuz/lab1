namespace Lab1.Models;

public sealed class FleetRegistry
{
    private static readonly Lazy<FleetRegistry> _instance =
        new(() => new FleetRegistry());

    private readonly List<Vehicle> _vehicles = [];

    private FleetRegistry()
    {
    }

    public static FleetRegistry Instance => _instance.Value;

    public void RegisterVehicle(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        _vehicles.Add(vehicle);
    }

    public IReadOnlyList<Vehicle> GetAllVehicles() => _vehicles.AsReadOnly();

    public Vehicle? FindByVin(string vin)
    {
        return _vehicles.FirstOrDefault(v => v.Vin.Equals(vin, StringComparison.OrdinalIgnoreCase));
    }

    public Vehicle this[int index] => _vehicles[index];

    public Vehicle? this[string vin] => FindByVin(vin);

    public void SortByFuelLevel() => _vehicles.Sort();
}
