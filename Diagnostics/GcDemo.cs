namespace Lab1.Diagnostics;

using Lab1.Models;
using System.Runtime.CompilerServices;

public static class GcDemo
{
    // Об'єкти понад ~85 000 байт потрапляють у Large Object Heap (логічно це покоління 2).
    private const int LargeObjectSizeBytes = 100_000;
    private const int SmallObjectSizeBytes = 1_000;

    public static void Run()
    {
        Section("9.1. Покоління об'єктів (GC.GetGeneration)");
        RunGenerationsDemo();

        Section("9.2. Великі об'єкти на етапі створення (LOH)");
        RunLargeObjectDemo();

        Section("9.3. Примусове збирання сміття та фіналізація");
        RunFinalizationDemo();

        Section("9.4. Керування чергою фіналізації (SuppressFinalize / ReRegisterForFinalize)");
        RunFinalizationQueueDemo();

        Section("9.5. Dispose (using) проти фіналізатора");
        RunDisposeDemo();

        Section("9.6. WeakReference: коротке та довге посилання");
        RunWeakReferenceDemo();
    }

    private static void Section(string title) => Console.WriteLine($"\n=== {title} ===");

    // 9.1 Покоління
    private static void RunGenerationsDemo()
    {
        var tracker = new TelematicsTracker("Gen-Tracker");
        Console.WriteLine($"Після створення:      Gen {GC.GetGeneration(tracker)}");

        GC.Collect(0);
        Console.WriteLine($"Після GC.Collect(0):  Gen {GC.GetGeneration(tracker)}");

        GC.Collect(1);
        Console.WriteLine($"Після GC.Collect(1):  Gen {GC.GetGeneration(tracker)}");

        GC.KeepAlive(tracker); // об'єкт гарантовано живий
        tracker.Dispose();
    }

    // 9.2 Великі об'єкти
    private static void RunLargeObjectDemo()
    {
        long before = GC.GetTotalMemory(forceFullCollection: true);

        var small = new byte[SmallObjectSizeBytes];
        Console.WriteLine($"Малий масив ({SmallObjectSizeBytes} Б):   Gen {GC.GetGeneration(small)}");

        var large = new byte[LargeObjectSizeBytes];
        Console.WriteLine($"Великий масив ({LargeObjectSizeBytes} Б): Gen {GC.GetGeneration(large)} (одразу в LOH)");

        long after = GC.GetTotalMemory(forceFullCollection: false);
        Console.WriteLine($"Приріст керованої пам'яті: ~{after - before} Б");

        GC.KeepAlive(small);
        GC.KeepAlive(large);
    }

    // 9.3 Примусова збірка та фіналізація
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateUnreferencedTracker(string id)
    {
        _ = new TelematicsTracker(id);
    }

    private static void RunFinalizationDemo()
    {
        CreateUnreferencedTracker("Tracker-1");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("(деструктор мав спрацювати, бо посилань на об'єкт немає)");
    }

    // 9.4 Черга фіналізації
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateSuppressedTracker(string id)
    {
        var tracker = new TelematicsTracker(id);
        GC.SuppressFinalize(tracker);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateSuppressedThenRestoredTracker(string id)
    {
        var tracker = new TelematicsTracker(id);
        GC.SuppressFinalize(tracker);
        GC.ReRegisterForFinalize(tracker);
    }

    private static void RunFinalizationQueueDemo()
    {
        Console.WriteLine("Сценарій A: GC.SuppressFinalize");
        CreateSuppressedTracker("Tracker-2");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("(деструктор не спрацював – об'єкт видалено з черги)");

        Console.WriteLine("Сценарій B: SuppressFinalize + ReRegisterForFinalize");
        CreateSuppressedThenRestoredTracker("Tracker-3");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("(деструктор спрацював – об'єкт повернуто в чергу)");
    }

    // 9.5 Dispose
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateDisposedTracker(string id)
    {
        using var tracker = new TelematicsTracker(id);
    }

    private static void RunDisposeDemo()
    {
        CreateDisposedTracker("Tracker-4");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("(після Dispose деструктор не потрібен – SuppressFinalize уже викликано)");
    }

    // 9.6 WeakReference
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference Short, WeakReference Long) CreateWeaklyReferencedTracker()
    {
        var tracker = new TelematicsTracker("Weak-Tracker");
        return (new WeakReference(tracker, trackResurrection: false),
                new WeakReference(tracker, trackResurrection: true));
    }

    private static void PrintWeakState(string stage, WeakReference shortRef, WeakReference longRef) =>
        Console.WriteLine($"{stage,-38} коротке: {shortRef.IsAlive,-5} | довге: {longRef.IsAlive}");

    private static void RunWeakReferenceDemo()
    {
        var (shortRef, longRef) = CreateWeaklyReferencedTracker();
        PrintWeakState("1) до GC", shortRef, longRef);

        GC.Collect();
        PrintWeakState("2) після першого GC (до фіналізатора)", shortRef, longRef);

        GC.WaitForPendingFinalizers();
        PrintWeakState("3) після виконання фіналізатора", shortRef, longRef);

        GC.Collect();
        PrintWeakState("4) після остаточного збору", shortRef, longRef);
    }
}
