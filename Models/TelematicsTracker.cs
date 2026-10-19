namespace Lab1.Models;

using System.Runtime.InteropServices;

public class TelematicsTracker : IDisposable
{
    private const int UnmanagedBufferSize = 1024;

    private bool _disposed;
    private StreamWriter? _managedLogWriter;
    private IntPtr _unmanagedBufferHandle;

    public string DeviceId { get; }

    public TelematicsTracker(string deviceId)
    {
        DeviceId = string.IsNullOrWhiteSpace(deviceId) ? "DEFAULT-TRACKER" : deviceId;
        _unmanagedBufferHandle = Marshal.AllocHGlobal(UnmanagedBufferSize);
        _managedLogWriter = new StreamWriter(new MemoryStream());
    }

    ~TelematicsTracker()
    {
        Console.WriteLine($"   [Деструктор]: Спрацював для '{DeviceId}'.");
        Dispose(false);
    }

    public void Dispose()
    {
        Console.WriteLine($"   [Dispose]: явне звільнення ресурсів '{DeviceId}'.");
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _managedLogWriter?.Dispose();
            _managedLogWriter = null;
        }

        if (_unmanagedBufferHandle != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_unmanagedBufferHandle);
            _unmanagedBufferHandle = IntPtr.Zero;
        }

        _disposed = true;
    }
}
