namespace Lab1.Interfaces;

public interface IDiagnosable
{
    string DiagnosticStatus { get; }
    void RunDiagnostics();
    void PerformCheck();
}
