# Лабораторна робота №1: Реалізація основних принципів ООП мовою C#

**Тема:** «C# .NET. Реалізація основних принципів ООП мовою C#»  
**Платформа:** .NET 10 (C# 13)

---

## Інструкція із запуску

Для збірки та запуску проєкту виконайте у терміналі з кореневої папки проєкту:

```bash
dotnet build
dotnet run
```

---

## Таблиця відповідності вимог лабораторної роботи

| № | Вимога викладача | Реалізація в коді (файли та класи) | Опис та демонстрація |
|---|-------------------|-----------------------------------|----------------------|
| **1** | **Інтерфейси (не менше 2, один клас реалізує обидва, явна реалізація)** | `Interfaces/IDiagnosable.cs`<br>`Interfaces/IServiceable.cs`<br>`Models/Car.cs` | Клас `Car` реалізує обидва інтерфейси. Метод `RunDiagnostics()` та метод `PerformCheck()` реалізовано явно (`explicit implementation`). Виклик здійснюється через `((IDiagnosable)car)`. Метод прихований з публічного API `Car`. |
| **2** | **Абстрактний клас та обов'язкові абстрактні методи** | `Models/Vehicle.cs`<br>`Models/LandVehicle.cs`<br>`Models/Car.cs`<br>`Models/Truck.cs`<br>`Models/ElectricCar.cs` | Абстрактний базовий клас `Vehicle` містить абстрактні методи `StartEngine()` та `CalculateOperationalCost(distanceKm)`. Базовий абстрактний клас `LandVehicle` задає характеристики колісного транспорту. Методи перевизначені у всіх похідних класах під відповідний тип двигуна. |
| **3** | **Модифікатори доступу з поясненням вибору** | `Models/Vehicle.cs` | `private readonly _vin` (критичний ID), `protected _fuelLevel` (доступний нащадкам для формул), `internal FleetRegistryCode` (доступний у збірці для реєстру), `protected internal ManufactureDate`, `private protected SecurityToken`. |
| **4** | **Властивість зі складнішою логікою (не просто get; set;)** | `Models/Vehicle.cs` (`FuelLevel`)<br>`Models/Truck.cs` (`CurrentCargoTons`)<br>`Models/ElectricCar.cs` (`BatteryChargePercentage`) | Властивість `FuelLevel` валідує діапазон `0..100%`, викидає `ArgumentOutOfRangeException` при некоректних значеннях та попереджає про низький залишок (`<= 10%`). У `Truck` контролюється ліміт вантажу, а в `ElectricCar` синхронізується заряд батареї. |
| **5** | **Конструктори (default, params, private, static, порядок виклику)** | `Models/Vehicle.cs`<br>`Models/LandVehicle.cs`<br>`Models/Car.cs`<br>`Models/Truck.cs`<br>`Models/ElectricCar.cs`<br>`Models/FleetRegistry.cs`<br>`Models/ConstructorLog.cs` | Статичні конструктори (`static`), конструктори за замовчуванням (`this(...)`), з параметрами (`base(...)`), конструктори копіювання. Приватний конструктор у патерні `Singleton` (`FleetRegistry`) та `Factory Method` (`Truck.CreateHeavyDutyHauler`). Клас `ConstructorLog` логує точний порядок виклику при `new ElectricCar()`. |
| **6** | **Віртуальні та перевизначені методи (поліморфізм)** | `Models/Vehicle.cs`<br>`Models/LandVehicle.cs`<br>`Models/Car.cs`<br>`Models/Truck.cs`<br>`Models/ElectricCar.cs` | Віртуальні методи `DisplayInfo()`, `CalculateRemainingRangeKm()`, `Refuel()`, `ToString()`. Поліморфний виклик через колекцію базового типу `List<Vehicle>` у реєстрі автопарку. |
| **7** | **Перевантаження операторів, індексатори та інтерфейси** | `Models/Vehicle.cs`<br>`Models/FleetRegistry.cs`<br>`Models/Car.cs` | Перевантажено оператор `+` (дозаправка `car + 20`), оператори порівняння `==`, `!=`, `<`, `>` та реалізовано `IComparable<Vehicle>`. У `FleetRegistry` додано індексатори `this[int]` та `this[string vin]`. У `Car` реалізовано метод `Deconstruct`. |
| **8** | **Метод-розширення (Extension Method)** | `Extensions/VehicleExtensions.cs` | Статичний клас `VehicleExtensions`, метод `EstimateRemainingRangeKm(this Vehicle vehicle)` для розрахунку запасу ходу, `GetShortSummary()` та `PrintShortSummary()`. |
| **9** | **IDisposable, деструктор і Dispose(bool)** | `Models/TelematicsTracker.cs` | Клас володіє некерованою пам'яттю (`Marshal.AllocHGlobal`) та керованим потоком (`StreamWriter`). Реалізує фіналізатор `~TelematicsTracker()`, `Dispose()`, захищений `Dispose(bool disposing)` та `GC.SuppressFinalize(this)`. |
| **10.1** | **Покоління об'єктів (GC.GetGeneration)** | `Diagnostics/GcDemo.cs` (`RunGenerationsDemo`) | Простежено перехід об'єкта `Gen 0 -> Gen 1 -> Gen 2` після примусових зборів `GC.Collect(0)` та `GC.Collect(1)`. Використано `GC.KeepAlive`. |
| **10.2** | **Великі об'єкти на етапі створення (LOH)** | `Diagnostics/GcDemo.cs` (`RunLargeObjectDemo`) | Досліджено розміщення великих об'єктів (> 85 КБ) одразу у Large Object Heap (Gen 2) та відстежено витрати пам'яті через `GC.GetTotalMemory`. |
| **10.3** | **Примусове збирання сміття та фіналізація** | `Diagnostics/GcDemo.cs` (`RunFinalizationDemo`) | Об'єкт створюється в ізольованому методі `CreateUnreferencedTracker()`. Після виходу сильне посилання втрачається, викликаються `GC.Collect()` та `GC.WaitForPendingFinalizers()`, що спричиняє реальний виклик деструктора. |
| **10.4** | **Керування чергою фіналізації** | `Diagnostics/GcDemo.cs` (`RunFinalizationQueueDemo`) | Розділено на 2 сценарії: сценарій A демонструє пригнічення фіналізації через `GC.SuppressFinalize` (фіналізатор не викликається), сценарій B демонструє повернення в чергу через `GC.ReRegisterForFinalize` (фіналізатор спрацьовує). |
| **10.5** | **Dispose (using) проти фіналізатора** | `Diagnostics/GcDemo.cs` (`RunDisposeDemo`) | Демонстрація детермінованого очищення ресурсів через `using`: виклик `Dispose()` звільняє ресурси негайно і виключає об'єкт з черги фіналізатора. |
| **10.6** | **WeakReference (коротке vs довге)** | `Diagnostics/GcDemo.cs` (`RunWeakReferenceDemo`) | Досліджено коротке (`trackResurrection: false`) та довге (`trackResurrection: true`) слабкі посилання на 4 етапах життєвого циклу об'єкта (до GC, після першого GC, після фіналізатора, після остаточного збору). |
| **11** | **Демонстрація всіх сценаріїв ООП** | `Diagnostics/OopDemo.cs`<br>`Program.cs` | Окремий клас `OopDemo`, що об'єднує та покроково запускає демонстрацію виклику конструкторів, реєстрації автопарку, поліморфізму, явних інтерфейсів, методів-розширень, перевантажених операторів та обробки винятків. |

---

## Структура папок проєкту

```text
lab1/
├── Interfaces/
│   ├── IDiagnosable.cs
│   └── IServiceable.cs
├── Models/
│   ├── Vehicle.cs
│   ├── LandVehicle.cs
│   ├── Car.cs
│   ├── ConstructorLog.cs     
│   ├── Truck.cs
│   ├── ElectricCar.cs
│   ├── FleetRegistry.cs
│   └── TelematicsTracker.cs
├── Extensions/
│   └── VehicleExtensions.cs
├── Diagnostics/
│   ├── GcDemo.cs
│   └── OopDemo.cs
├── Program.cs
├── lab1.csproj
└── README.md
```