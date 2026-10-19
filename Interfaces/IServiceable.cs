namespace Lab1.Interfaces;

public interface IServiceable
{
    DateTime LastServiceDate { get; }
    void PerformService(string serviceType);
    void PerformCheck();
}
