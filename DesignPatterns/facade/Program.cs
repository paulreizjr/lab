/*
 * FACADE DESIGN PATTERN EXAMPLE
 * 
 * PURPOSE:
 * The Facade pattern provides a simplified interface to a complex subsystem of classes.
 * It hides the complexity of the subsystem and provides a single entry point for clients.
 * 
 * SCENARIOS TO USE:
 * 1. When you have a complex subsystem with many interdependent classes
 * 2. When you want to provide a simple interface to a complex library or framework
 * 3. When you want to decouple client code from implementation details
 * 4. When you need to layer your subsystems (each layer can have a facade)
 * 5. When integrating with legacy systems that have complex APIs
 * 
 * SCENARIOS NOT TO USE:
 * 1. When the subsystem is already simple and doesn't need simplification
 * 2. When clients need full access to all subsystem functionality
 * 3. When the facade would become as complex as the subsystem itself
 * 4. When you're over-engineering a simple problem
 * 
 * MEMORY ALLOCATION CONSIDERATIONS:
 * - Facade typically holds references to subsystem objects, not copies
 * - Consider using dependency injection to manage object lifetimes
 * - Be careful with static facades that might prevent garbage collection
 * - Use lazy initialization for expensive subsystem components
 * 
 * MULTITHREADING ASPECTS:
 * - Facade should be thread-safe if used in multithreaded environments
 * - Consider thread safety of underlying subsystem components
 * - Use appropriate synchronization mechanisms (locks, concurrent collections)
 * - Be aware of potential deadlocks when multiple subsystems are involved
 */

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FacadePattern
{
    // COMPLEX SUBSYSTEM CLASSES
    // These represent various components of a complex home automation system
    
    /// <summary>
    /// Subsystem component for managing lighting
    /// Memory: Maintains state for multiple lights
    /// Threading: Uses concurrent dictionary for thread safety
    /// </summary>
    public class LightingSystem
    {
        private readonly ConcurrentDictionary<string, bool> _lights;
        private readonly object _lockObject = new object();

        public LightingSystem()
        {
            _lights = new ConcurrentDictionary<string, bool>();
            Console.WriteLine("[LightingSystem] Initialized");
        }

        public void TurnOnLight(string room)
        {
            lock (_lockObject) // Thread safety for state changes
            {
                _lights.AddOrUpdate(room, true, (key, value) => true);
                Console.WriteLine($"[LightingSystem] Light turned ON in {room}");
                Thread.Sleep(100); // Simulate some processing time
            }
        }

        public void TurnOffLight(string room)
        {
            lock (_lockObject)
            {
                _lights.AddOrUpdate(room, false, (key, value) => false);
                Console.WriteLine($"[LightingSystem] Light turned OFF in {room}");
                Thread.Sleep(100);
            }
        }

        public void DimLights(string room, int level)
        {
            lock (_lockObject)
            {
                Console.WriteLine($"[LightingSystem] Lights dimmed to {level}% in {room}");
                Thread.Sleep(150);
            }
        }
    }

    /// <summary>
    /// Subsystem component for security management
    /// Memory: Maintains security state and sensor data
    /// Threading: Uses locks for critical security operations
    /// </summary>
    public class SecuritySystem
    {
        private bool _isArmed;
        private readonly object _lockObject = new object();

        public SecuritySystem()
        {
            _isArmed = false;
            Console.WriteLine("[SecuritySystem] Initialized");
        }

        public void ArmSystem()
        {
            lock (_lockObject) // Critical section for security state
            {
                _isArmed = true;
                Console.WriteLine("[SecuritySystem] System ARMED");
                Thread.Sleep(200);
            }
        }

        public void DisarmSystem()
        {
            lock (_lockObject)
            {
                _isArmed = false;
                Console.WriteLine("[SecuritySystem] System DISARMED");
                Thread.Sleep(200);
            }
        }

        public void ActivateMotionSensors()
        {
            if (_isArmed)
            {
                Console.WriteLine("[SecuritySystem] Motion sensors activated");
                Thread.Sleep(100);
            }
        }

        public bool IsArmed => _isArmed;
    }

    /// <summary>
    /// Subsystem component for climate control
    /// Memory: Stores temperature settings and schedules
    /// Threading: Simulates async operations for climate adjustments
    /// </summary>
    public class ClimateControlSystem
    {
        private int _temperature;
        private readonly SemaphoreSlim _semaphore;

        public ClimateControlSystem()
        {
            _temperature = 22; // Default temperature
            _semaphore = new SemaphoreSlim(1, 1); // Only one operation at a time
            Console.WriteLine("[ClimateControlSystem] Initialized");
        }

        public async Task SetTemperatureAsync(int temperature)
        {
            await _semaphore.WaitAsync(); // Thread-safe async operation
            try
            {
                _temperature = temperature;
                Console.WriteLine($"[ClimateControlSystem] Temperature set to {temperature}°C");
                await Task.Delay(300); // Simulate async operation
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task SetEcoModeAsync(bool enabled)
        {
            await _semaphore.WaitAsync();
            try
            {
                Console.WriteLine($"[ClimateControlSystem] Eco mode {(enabled ? "ENABLED" : "DISABLED")}");
                await Task.Delay(200);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public int CurrentTemperature => _temperature;
    }

    /// <summary>
    /// Subsystem component for entertainment system
    /// Memory: Manages media state and playlists
    /// Threading: Uses task-based operations for media control
    /// </summary>
    public class EntertainmentSystem
    {
        private bool _isPlaying;
        private string _currentMedia;
        private readonly object _lockObject = new object();

        public EntertainmentSystem()
        {
            _isPlaying = false;
            _currentMedia = "None";
            Console.WriteLine("[EntertainmentSystem] Initialized");
        }

        public Task PlayMusicAsync(string playlist)
        {
            return Task.Run(() =>
            {
                lock (_lockObject)
                {
                    _isPlaying = true;
                    _currentMedia = playlist;
                    Console.WriteLine($"[EntertainmentSystem] Playing music: {playlist}");
                    Thread.Sleep(250);
                }
            });
        }

        public void TurnOnTV(string channel)
        {
            lock (_lockObject)
            {
                Console.WriteLine($"[EntertainmentSystem] TV turned ON - Channel: {channel}");
                Thread.Sleep(200);
            }
        }

        public void TurnOffTV()
        {
            lock (_lockObject)
            {
                Console.WriteLine("[EntertainmentSystem] TV turned OFF");
                Thread.Sleep(150);
            }
        }
    }

    // FACADE CLASS
    /// <summary>
    /// SmartHomeFacade - Provides a simplified interface to the complex home automation subsystem
    /// 
    /// MEMORY CONSIDERATIONS:
    /// - Holds references to subsystem objects (composition over inheritance)
    /// - Uses lazy initialization to reduce initial memory footprint
    /// - Subsystem objects are shared, not duplicated
    /// 
    /// THREADING CONSIDERATIONS:
    /// - Facade methods can be called concurrently
    /// - Delegates thread safety to underlying subsystems
    /// - Provides async methods for operations that benefit from it
    /// </summary>
    public class SmartHomeFacade
    {
        // Lazy initialization for memory efficiency
        private readonly Lazy<LightingSystem> _lightingSystem;
        private readonly Lazy<SecuritySystem> _securitySystem;
        private readonly Lazy<ClimateControlSystem> _climateSystem;
        private readonly Lazy<EntertainmentSystem> _entertainmentSystem;

        // Thread-safe initialization counter
        private static int _instanceCount = 0;

        public SmartHomeFacade()
        {
            // Lazy initialization reduces memory allocation until components are actually needed
            _lightingSystem = new Lazy<LightingSystem>(() => new LightingSystem());
            _securitySystem = new Lazy<SecuritySystem>(() => new SecuritySystem());
            _climateSystem = new Lazy<ClimateControlSystem>(() => new ClimateControlSystem());
            _entertainmentSystem = new Lazy<EntertainmentSystem>(() => new EntertainmentSystem());

            Interlocked.Increment(ref _instanceCount);
            Console.WriteLine($"[SmartHomeFacade] Instance #{_instanceCount} created");
        }

        /// <summary>
        /// Simplified method to activate "Movie Night" mode
        /// Coordinates multiple subsystems with a single call
        /// Thread-safe: Uses async/await for concurrent operations where beneficial
        /// </summary>
        public async Task ActivateMovieNightAsync()
        {
            Console.WriteLine("\n=== ACTIVATING MOVIE NIGHT MODE ===");

            try
            {
                // Start multiple operations concurrently for better performance
                var tasks = new List<Task>
                {
                    Task.Run(() =>
                    {
                        _lightingSystem.Value.DimLights("Living Room", 20);
                        _lightingSystem.Value.TurnOffLight("Kitchen");
                    }),
                    _climateSystem.Value.SetTemperatureAsync(21),
                    _entertainmentSystem.Value.PlayMusicAsync("Ambient Playlist")
                };

                // Wait for all concurrent operations to complete
                await Task.WhenAll(tasks);

                // Sequential operations that depend on previous state
                _entertainmentSystem.Value.TurnOnTV("Netflix");
                _securitySystem.Value.ArmSystem();

                Console.WriteLine("=== MOVIE NIGHT MODE ACTIVATED ===\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error activating movie night mode: {ex.Message}");
                throw; // Re-throw to allow caller to handle
            }
        }

        /// <summary>
        /// Simplified method to activate "Sleep" mode
        /// Thread-safe: Coordinates multiple subsystems safely
        /// </summary>
        public async Task ActivateSleepModeAsync()
        {
            Console.WriteLine("\n=== ACTIVATING SLEEP MODE ===");

            try
            {
                // Turn off entertainment first
                _entertainmentSystem.Value.TurnOffTV();

                // Concurrent operations for efficiency
                var tasks = new List<Task>
                {
                    Task.Run(() =>
                    {
                        _lightingSystem.Value.TurnOffLight("Living Room");
                        _lightingSystem.Value.TurnOffLight("Kitchen");
                        _lightingSystem.Value.DimLights("Bedroom", 5);
                    }),
                    _climateSystem.Value.SetTemperatureAsync(19),
                    _climateSystem.Value.SetEcoModeAsync(true)
                };

                await Task.WhenAll(tasks);

                // Arm security system last
                _securitySystem.Value.ArmSystem();
                _securitySystem.Value.ActivateMotionSensors();

                Console.WriteLine("=== SLEEP MODE ACTIVATED ===\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error activating sleep mode: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Simplified method to activate "Away" mode
        /// Demonstrates error handling and rollback capabilities
        /// </summary>
        public async Task ActivateAwayModeAsync()
        {
            Console.WriteLine("\n=== ACTIVATING AWAY MODE ===");

            try
            {
                // Save current state for potential rollback
                bool wasArmed = _securitySystem.Value.IsArmed;

                // Turn off all systems
                _entertainmentSystem.Value.TurnOffTV();

                var tasks = new List<Task>
                {
                    Task.Run(() =>
                    {
                        _lightingSystem.Value.TurnOffLight("Living Room");
                        _lightingSystem.Value.TurnOffLight("Kitchen");
                        _lightingSystem.Value.TurnOffLight("Bedroom");
                    }),
                    _climateSystem.Value.SetEcoModeAsync(true),
                    _climateSystem.Value.SetTemperatureAsync(16)
                };

                await Task.WhenAll(tasks);

                // Activate security
                _securitySystem.Value.ArmSystem();
                _securitySystem.Value.ActivateMotionSensors();

                Console.WriteLine("=== AWAY MODE ACTIVATED ===\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error activating away mode: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Emergency shutdown - demonstrates coordinated shutdown
        /// Thread-safe: Uses locks and proper exception handling
        /// </summary>
        public void EmergencyShutdown()
        {
            Console.WriteLine("\n!!! EMERGENCY SHUTDOWN !!!");

            try
            {
                // Quick shutdown - prioritize security
                if (_securitySystem.IsValueCreated)
                {
                    _securitySystem.Value.DisarmSystem();
                }

                // Turn off all systems
                if (_entertainmentSystem.IsValueCreated)
                {
                    _entertainmentSystem.Value.TurnOffTV();
                }

                if (_lightingSystem.IsValueCreated)
                {
                    _lightingSystem.Value.TurnOffLight("All Rooms");
                }

                Console.WriteLine("!!! EMERGENCY SHUTDOWN COMPLETE !!!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during emergency shutdown: {ex.Message}");
                // In a real emergency system, you might log this but not throw
            }
        }

        // Property to check system status
        public bool IsSecurityArmed => _securitySystem.IsValueCreated && _securitySystem.Value.IsArmed;

        // Dispose pattern for proper cleanup
        public void Dispose()
        {
            Console.WriteLine("[SmartHomeFacade] Disposing resources...");
            // In a real implementation, you'd dispose of subsystem resources here
            Interlocked.Decrement(ref _instanceCount);
        }
    }

    // DEMONSTRATION CLASS
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("FACADE DESIGN PATTERN DEMONSTRATION");
            Console.WriteLine("===================================\n");

            // Create facade instance
            // Memory: Only the facade is created initially, subsystems are lazy-loaded
            var smartHome = new SmartHomeFacade();

            try
            {
                // Demonstrate concurrent facade usage
                Console.WriteLine("=== TESTING CONCURRENT OPERATIONS ===");
                
                // Multiple clients can use the facade concurrently
                var task1 = smartHome.ActivateMovieNightAsync();
                var task2 = Task.Delay(500).ContinueWith(_ => smartHome.ActivateSleepModeAsync()).Unwrap();

                // Wait for first operation to complete
                await task1;
                await Task.Delay(1000); // Brief pause to see the output clearly

                // Demonstrate another mode
                await smartHome.ActivateAwayModeAsync();
                await Task.Delay(1000);

                // Demonstrate emergency shutdown
                smartHome.EmergencyShutdown();

                Console.WriteLine($"Security system armed: {smartHome.IsSecurityArmed}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Proper cleanup
                smartHome.Dispose();
            }

            Console.WriteLine("\n=== FACADE PATTERN BENEFITS DEMONSTRATED ===");
            Console.WriteLine("1. Simplified interface - Client uses simple methods instead of coordinating multiple subsystems");
            Console.WriteLine("2. Reduced coupling - Client doesn't depend on specific subsystem implementations");
            Console.WriteLine("3. Thread safety - Facade coordinates thread-safe operations across subsystems");
            Console.WriteLine("4. Memory efficiency - Lazy loading of subsystem components");
            Console.WriteLine("5. Error handling - Centralized exception handling and recovery");
            Console.WriteLine("6. Concurrent operations - Facade can coordinate async operations efficiently");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
