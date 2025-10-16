using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventExamples
{
    /// <summary>
    /// Comprehensive examples of events in C#
    /// 
    /// PURPOSE:
    /// - Events enable notification-based communication between objects
    /// - Implement the Observer pattern with built-in language support
    /// - Provide encapsulated multicast delegate with restricted access
    /// - Allow objects to notify subscribers without tight coupling
    /// - Core mechanism for reactive and event-driven architectures
    /// 
    /// SCENARIOS TO USE:
    /// - UI event handling (button clicks, mouse movements, key presses)
    /// - Domain events (order placed, payment processed, inventory changed)
    /// - Property change notifications (INotifyPropertyChanged)
    /// - Publish-subscribe patterns across application layers
    /// - Monitoring and logging systems
    /// - State change notifications in business logic
    /// - Asynchronous operation completion notifications
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Simple method calls where direct invocation is clearer
    /// - Performance-critical tight loops (event overhead)
    /// - When you need return values from subscribers
    /// - Synchronous request-response patterns (use methods instead)
    /// - When subscriber order matters (event order is undefined)
    /// - Overuse leads to hard-to-trace code flow
    /// 
    /// MEMORY ALLOCATION:
    /// - Each subscriber adds ~40-80 bytes to the invocation list
    /// - Event backing field is a multicast delegate (linked list)
    /// - Lambda subscribers create closure objects if capturing variables
    /// - Unsubscribed events can cause memory leaks (holds references)
    /// - Use weak event pattern for long-lived publishers/short-lived subscribers
    /// 
    /// MULTITHREAD ASPECTS:
    /// - event keyword provides thread-safe add/remove operations
    /// - Event invocation itself is NOT automatically thread-safe
    /// - Subscribers may execute on different threads than publisher
    /// - Copy event reference before invoking to avoid race conditions
    /// - Consider using SynchronizationContext for UI thread marshaling
    /// - Event handlers should be fast to avoid blocking publisher
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Event Examples ===\n");

            // Example 1: Basic Events
            BasicEventExamples();

            // Example 2: EventHandler and EventArgs Pattern
            EventHandlerPatternExamples();

            // Example 3: Custom EventArgs
            CustomEventArgsExamples();

            // Example 4: PropertyChanged Events
            PropertyChangedExamples();

            // Example 5: Event Subscription and Unsubscription
            SubscriptionLifecycleExamples();

            // Example 6: Event Invocation Patterns
            EventInvocationPatternExamples();

            // Example 7: Async Event Handlers
            await AsyncEventHandlerExamples();

            // Example 8: Thread Safety with Events
            ThreadSafetyExamples();

            // Example 9: Memory Leaks and Weak Events
            MemoryLeakExamples();

            // Example 10: Event Performance Considerations
            PerformanceExamples();

            Console.WriteLine("\n=== All Examples Completed ===");
        }

        #region Basic Event Examples

        /// <summary>
        /// Demonstrates basic event declaration and usage
        /// 
        /// SCENARIOS TO USE:
        /// - Simple notification without data
        /// - State change alerts
        /// - Completion signals
        /// 
        /// MEMORY ALLOCATION:
        /// - Event backing field is a delegate (null when no subscribers)
        /// - Each subscriber adds to multicast delegate chain
        /// </summary>
        static void BasicEventExamples()
        {
            Console.WriteLine("--- Basic Event Examples ---");

            var button = new SimpleButton();

            // Subscribe to event
            // MEMORY: Creates delegate instance (~40-80 bytes)
            button.Clicked += OnButtonClicked;
            button.Clicked += OnButtonClickedVerbose;

            // Trigger the event
            button.Click();

            Console.WriteLine("\nClicking again:");
            button.Click();

            // Unsubscribe
            button.Clicked -= OnButtonClickedVerbose;
            Console.WriteLine("\nAfter unsubscribing verbose handler:");
            button.Click();

            Console.WriteLine();
        }

        static void OnButtonClicked()
        {
            Console.WriteLine("  Button was clicked!");
        }

        static void OnButtonClickedVerbose()
        {
            Console.WriteLine($"  [Verbose] Button clicked at {DateTime.Now:HH:mm:ss.fff}");
        }

        class SimpleButton
        {
            // Event declaration using Action delegate
            // event keyword prevents external classes from invoking or assigning the event
            public event Action? Clicked;

            public void Click()
            {
                Console.WriteLine("Button.Click() called");
                // Invoke the event - notify all subscribers
                Clicked?.Invoke();
            }
        }

        #endregion

        #region EventHandler Pattern Examples

        /// <summary>
        /// Demonstrates standard .NET EventHandler pattern
        /// 
        /// PURPOSE:
        /// - EventHandler<TEventArgs> is the standard .NET event pattern
        /// - Provides sender object reference and event data
        /// - Consistent pattern across .NET framework
        /// 
        /// SCENARIOS TO USE:
        /// - Following .NET conventions
        /// - When subscribers need sender reference
        /// - When passing event-specific data
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Sender reference allows identifying event source
        /// - EventArgs can carry thread context information
        /// </summary>
        static void EventHandlerPatternExamples()
        {
            Console.WriteLine("--- EventHandler Pattern Examples ---");

            var counter = new Counter();

            // Subscribe using EventHandler delegate
            counter.ValueChanged += OnCounterValueChanged;
            counter.ThresholdReached += OnThresholdReached;

            // Trigger events through operations
            counter.Increment();
            counter.Increment();
            counter.Increment();
            counter.Increment();
            counter.Increment();
            counter.IncrementBy(10); // This will trigger threshold

            Console.WriteLine();
        }

        static void OnCounterValueChanged(object? sender, EventArgs e)
        {
            if (sender is Counter counter)
            {
                Console.WriteLine($"  Counter value changed to: {counter.Value}");
            }
        }

        static void OnThresholdReached(object? sender, EventArgs e)
        {
            if (sender is Counter counter)
            {
                Console.WriteLine($"  *** THRESHOLD REACHED! Value is now {counter.Value} ***");
            }
        }

        class Counter
        {
            private int _value;
            private const int Threshold = 10;

            public int Value => _value;

            // Standard EventHandler pattern
            public event EventHandler? ValueChanged;
            public event EventHandler? ThresholdReached;

            public void Increment()
            {
                _value++;
                OnValueChanged();
            }

            public void IncrementBy(int amount)
            {
                _value += amount;
                OnValueChanged();
            }

            // Protected virtual methods allow derived classes to override
            protected virtual void OnValueChanged()
            {
                // Raise ValueChanged event
                ValueChanged?.Invoke(this, EventArgs.Empty);

                // Check threshold
                if (_value >= Threshold)
                {
                    OnThresholdReached();
                }
            }

            protected virtual void OnThresholdReached()
            {
                ThresholdReached?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Custom EventArgs Examples

        /// <summary>
        /// Demonstrates custom EventArgs for passing event-specific data
        /// 
        /// SCENARIOS TO USE:
        /// - When event needs to pass specific data to subscribers
        /// - Providing context about what changed
        /// - Allowing subscribers to cancel operations (cancellable events)
        /// 
        /// MEMORY ALLOCATION:
        /// - EventArgs instances allocated on heap
        /// - Consider reusing EventArgs.Empty for parameterless events
        /// - Custom EventArgs adds size of contained data
        /// </summary>
        static void CustomEventArgsExamples()
        {
            Console.WriteLine("--- Custom EventArgs Examples ---");

            var account = new BankAccount("ACC-12345", 1000m);

            // Subscribe to events
            account.BalanceChanged += OnBalanceChanged;
            account.WithdrawalAttempted += OnWithdrawalAttempted;

            // Perform operations
            account.Deposit(500m);
            account.Withdraw(200m);
            account.Withdraw(2000m); // This will be cancelled

            Console.WriteLine($"Final balance: ${account.Balance}");
            Console.WriteLine();
        }

        static void OnBalanceChanged(object? sender, BalanceChangedEventArgs e)
        {
            Console.WriteLine($"  Balance changed: ${e.OldBalance} -> ${e.NewBalance} " +
                            $"(Change: ${e.ChangeAmount})");
        }

        static void OnWithdrawalAttempted(object? sender, WithdrawalEventArgs e)
        {
            Console.WriteLine($"  Withdrawal attempt: ${e.Amount}");

            // Cancellable event - subscriber can prevent the operation
            if (e.Amount > 1500m)
            {
                Console.WriteLine($"  *** Withdrawal cancelled: Amount exceeds limit ***");
                e.Cancel = true;
            }
        }

        class BalanceChangedEventArgs : EventArgs
        {
            public decimal OldBalance { get; }
            public decimal NewBalance { get; }
            public decimal ChangeAmount => NewBalance - OldBalance;

            public BalanceChangedEventArgs(decimal oldBalance, decimal newBalance)
            {
                OldBalance = oldBalance;
                NewBalance = newBalance;
            }
        }

        class WithdrawalEventArgs : EventArgs
        {
            public decimal Amount { get; }
            public bool Cancel { get; set; }

            public WithdrawalEventArgs(decimal amount)
            {
                Amount = amount;
            }
        }

        class BankAccount
        {
            private decimal _balance;
            public string AccountNumber { get; }
            public decimal Balance => _balance;

            public event EventHandler<BalanceChangedEventArgs>? BalanceChanged;
            public event EventHandler<WithdrawalEventArgs>? WithdrawalAttempted;

            public BankAccount(string accountNumber, decimal initialBalance)
            {
                AccountNumber = accountNumber;
                _balance = initialBalance;
            }

            public void Deposit(decimal amount)
            {
                Console.WriteLine($"Depositing ${amount}");
                decimal oldBalance = _balance;
                _balance += amount;
                OnBalanceChanged(new BalanceChangedEventArgs(oldBalance, _balance));
            }

            public void Withdraw(decimal amount)
            {
                Console.WriteLine($"Attempting to withdraw ${amount}");

                // Raise cancellable event
                var eventArgs = new WithdrawalEventArgs(amount);
                OnWithdrawalAttempted(eventArgs);

                if (eventArgs.Cancel)
                {
                    Console.WriteLine("Withdrawal cancelled by event handler");
                    return;
                }

                if (amount > _balance)
                {
                    Console.WriteLine("Insufficient funds");
                    return;
                }

                decimal oldBalance = _balance;
                _balance -= amount;
                OnBalanceChanged(new BalanceChangedEventArgs(oldBalance, _balance));
            }

            protected virtual void OnBalanceChanged(BalanceChangedEventArgs e)
            {
                BalanceChanged?.Invoke(this, e);
            }

            protected virtual void OnWithdrawalAttempted(WithdrawalEventArgs e)
            {
                WithdrawalAttempted?.Invoke(this, e);
            }
        }

        #endregion

        #region PropertyChanged Examples

        /// <summary>
        /// Demonstrates INotifyPropertyChanged for property change notifications
        /// 
        /// PURPOSE:
        /// - Standard interface for property change notifications
        /// - Essential for data binding in UI frameworks (WPF, MAUI, etc.)
        /// - Enables reactive programming patterns
        /// 
        /// SCENARIOS TO USE:
        /// - Data binding in MVVM applications
        /// - Tracking object state changes
        /// - Automatic UI updates
        /// - Validation triggers
        /// 
        /// MEMORY ALLOCATION:
        /// - PropertyChangedEventArgs cached in .NET for common property names
        /// - Each unique property name creates new EventArgs instance
        /// </summary>
        static void PropertyChangedExamples()
        {
            Console.WriteLine("--- PropertyChanged Examples ---");

            var person = new Person();

            // Subscribe to PropertyChanged event
            person.PropertyChanged += (sender, e) =>
            {
                if (sender is Person p)
                {
                    Console.WriteLine($"  Property '{e.PropertyName}' changed");
                    Console.WriteLine($"  Current state: {p.FirstName} {p.LastName}, Age: {p.Age}");
                }
            };

            // Changing properties triggers events
            person.FirstName = "John";
            person.LastName = "Doe";
            person.Age = 30;

            Console.WriteLine();
        }

        class Person : INotifyPropertyChanged
        {
            private string _firstName = string.Empty;
            private string _lastName = string.Empty;
            private int _age;

            public string FirstName
            {
                get => _firstName;
                set
                {
                    if (_firstName != value)
                    {
                        _firstName = value;
                        OnPropertyChanged(nameof(FirstName));
                    }
                }
            }

            public string LastName
            {
                get => _lastName;
                set
                {
                    if (_lastName != value)
                    {
                        _lastName = value;
                        OnPropertyChanged(nameof(LastName));
                    }
                }
            }

            public int Age
            {
                get => _age;
                set
                {
                    if (_age != value)
                    {
                        _age = value;
                        OnPropertyChanged(nameof(Age));
                    }
                }
            }

            // INotifyPropertyChanged implementation
            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Subscription Lifecycle Examples

        /// <summary>
        /// Demonstrates event subscription and unsubscription patterns
        /// 
        /// SCENARIOS TO USE:
        /// - Conditional subscription based on state
        /// - Temporary subscriptions
        /// - Dynamic subscriber management
        /// 
        /// SCENARIOS NOT TO USE:
        /// - Forgetting to unsubscribe causes memory leaks
        /// - Long-lived publishers with short-lived subscribers
        /// 
        /// MEMORY ALLOCATION:
        /// - Each subscription holds reference to subscriber
        /// - Prevents garbage collection of subscriber
        /// - Always unsubscribe when no longer needed
        /// </summary>
        static void SubscriptionLifecycleExamples()
        {
            Console.WriteLine("--- Subscription Lifecycle Examples ---");

            var publisher = new MessagePublisher();

            // Pattern 1: Explicit subscribe/unsubscribe
            EventHandler<MessageEventArgs> handler1 = (s, e) =>
                Console.WriteLine($"  [Handler1] {e.Message}");

            publisher.MessagePublished += handler1;
            publisher.PublishMessage("First message");

            publisher.MessagePublished -= handler1;
            publisher.PublishMessage("Second message (handler1 unsubscribed)");

            // Pattern 2: Using statement with IDisposable wrapper
            Console.WriteLine("\nUsing IDisposable subscription:");
            using (var subscription = new EventSubscription(publisher))
            {
                publisher.PublishMessage("Message while subscribed");
            } // Automatically unsubscribes here

            publisher.PublishMessage("Message after disposal");

            // Pattern 3: Lambda subscriptions (harder to unsubscribe)
            Console.WriteLine("\nLambda subscription pattern:");
            EventHandler<MessageEventArgs>? lambdaHandler = null;
            lambdaHandler = (s, e) =>
            {
                Console.WriteLine($"  [Lambda] {e.Message}");
                // Self-unsubscribe after first message
                if (publisher != null && lambdaHandler != null)
                {
                    publisher.MessagePublished -= lambdaHandler;
                    Console.WriteLine("  Lambda unsubscribed itself");
                }
            };

            publisher.MessagePublished += lambdaHandler;
            publisher.PublishMessage("First lambda message");
            publisher.PublishMessage("Second lambda message (should not appear)");

            Console.WriteLine();
        }

        class MessageEventArgs : EventArgs
        {
            public string Message { get; }
            public DateTime Timestamp { get; }

            public MessageEventArgs(string message)
            {
                Message = message;
                Timestamp = DateTime.Now;
            }
        }

        class MessagePublisher
        {
            public event EventHandler<MessageEventArgs>? MessagePublished;

            public void PublishMessage(string message)
            {
                Console.WriteLine($"Publishing: {message}");
                OnMessagePublished(new MessageEventArgs(message));
            }

            protected virtual void OnMessagePublished(MessageEventArgs e)
            {
                MessagePublished?.Invoke(this, e);
            }
        }

        class EventSubscription : IDisposable
        {
            private readonly MessagePublisher _publisher;
            private readonly EventHandler<MessageEventArgs> _handler;

            public EventSubscription(MessagePublisher publisher)
            {
                _publisher = publisher;
                _handler = OnMessagePublished;
                _publisher.MessagePublished += _handler;
                Console.WriteLine("  [Subscription] Subscribed");
            }

            private void OnMessagePublished(object? sender, MessageEventArgs e)
            {
                Console.WriteLine($"  [Subscription] Received: {e.Message}");
            }

            public void Dispose()
            {
                _publisher.MessagePublished -= _handler;
                Console.WriteLine("  [Subscription] Unsubscribed");
            }
        }

        #endregion

        #region Event Invocation Patterns

        /// <summary>
        /// Demonstrates safe event invocation patterns
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Event can be modified between null check and invocation
        /// - Use null-conditional operator (?.) for thread-safe invocation
        /// - Copy event reference to local variable for safety
        /// - Consider invoking on specific thread (UI thread, etc.)
        /// </summary>
        static void EventInvocationPatternExamples()
        {
            Console.WriteLine("--- Event Invocation Patterns ---");

            var source = new EventSource();

            // Pattern 1: Null-conditional operator (recommended)
            source.Pattern1Event += (s, e) => Console.WriteLine("  Pattern 1: Received");
            source.TriggerPattern1();

            // Pattern 2: Copy to local variable
            source.Pattern2Event += (s, e) => Console.WriteLine("  Pattern 2: Received");
            source.TriggerPattern2();

            // Pattern 3: EventHandler extension method
            source.Pattern3Event += (s, e) => Console.WriteLine("  Pattern 3: Received");
            source.TriggerPattern3();

            // Pattern 4: Collecting results from subscribers
            source.Pattern4Event += (s, e) => e.Results.Add("Result from subscriber 1");
            source.Pattern4Event += (s, e) => e.Results.Add("Result from subscriber 2");
            source.TriggerPattern4();

            Console.WriteLine();
        }

        class EventSource
        {
            public event EventHandler? Pattern1Event;
            public event EventHandler? Pattern2Event;
            public event EventHandler? Pattern3Event;
            public event EventHandler<ResultEventArgs>? Pattern4Event;

            // Pattern 1: Null-conditional operator (simplest and recommended)
            public void TriggerPattern1()
            {
                Console.WriteLine("Pattern 1: Null-conditional operator");
                Pattern1Event?.Invoke(this, EventArgs.Empty);
            }

            // Pattern 2: Copy to local variable (thread-safe)
            public void TriggerPattern2()
            {
                Console.WriteLine("Pattern 2: Local copy for thread safety");
                EventHandler? handler = Pattern2Event;
                handler?.Invoke(this, EventArgs.Empty);
            }

            // Pattern 3: Extension method pattern
            public void TriggerPattern3()
            {
                Console.WriteLine("Pattern 3: Extension method");
                Pattern3Event.Raise(this, EventArgs.Empty);
            }

            // Pattern 4: Collecting results from all subscribers
            public void TriggerPattern4()
            {
                Console.WriteLine("Pattern 4: Collecting subscriber results");
                var args = new ResultEventArgs();
                Pattern4Event?.Invoke(this, args);

                Console.WriteLine($"  Collected {args.Results.Count} results:");
                foreach (var result in args.Results)
                {
                    Console.WriteLine($"    - {result}");
                }
            }
        }

        class ResultEventArgs : EventArgs
        {
            public List<string> Results { get; } = new List<string>();
        }

        #endregion

        #region Async Event Handlers

        /// <summary>
        /// Demonstrates async event handlers and their challenges
        /// 
        /// SCENARIOS TO USE:
        /// - When event handlers need to perform async I/O
        /// - Long-running operations in response to events
        /// - Async validation or processing
        /// 
        /// SCENARIOS NOT TO USE:
        /// - When order of completion matters
        /// - When you need to await all handlers
        /// - Simple synchronous operations (overhead)
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Async handlers run on thread pool
        /// - Publisher doesn't wait for async handlers
        /// - Exception handling becomes more complex
        /// </summary>
        static async Task AsyncEventHandlerExamples()
        {
            Console.WriteLine("--- Async Event Handlers Examples ---");

            var asyncSource = new AsyncEventSource();

            // Regular synchronous handler
            asyncSource.DataReceived += (s, e) =>
            {
                Console.WriteLine($"  [Sync] Received: {e.Data}");
            };

            // Async void handler (fire-and-forget)
            asyncSource.DataReceived += async (s, e) =>
            {
                await Task.Delay(100); // Simulate async work
                Console.WriteLine($"  [Async void] Processed: {e.Data}");
            };

            // Fire event - doesn't wait for async handlers
            Console.WriteLine("Firing event...");
            asyncSource.RaiseDataReceived("Test data");
            Console.WriteLine("Event fired (async handlers may still be running)");

            // Wait for async handlers to complete
            await Task.Delay(200);

            // Better pattern: Awaitable event
            Console.WriteLine("\nAwaitable event pattern:");
            asyncSource.AsyncDataReceived += async (s, e) =>
            {
                await Task.Delay(100);
                Console.WriteLine($"  [Awaitable] Processed: {e.Data}");
                return "Handler 1 result";
            };

            asyncSource.AsyncDataReceived += async (s, e) =>
            {
                await Task.Delay(50);
                Console.WriteLine($"  [Awaitable] Validated: {e.Data}");
                return "Handler 2 result";
            };

            var results = await asyncSource.RaiseAsyncDataReceived("Async test data");
            Console.WriteLine($"All async handlers completed. Results: {string.Join(", ", results)}");

            Console.WriteLine();
        }

        class DataEventArgs : EventArgs
        {
            public string Data { get; }

            public DataEventArgs(string data)
            {
                Data = data;
            }
        }

        class AsyncEventSource
        {
            // Standard event - async void handlers
            public event EventHandler<DataEventArgs>? DataReceived;

            // Custom async event pattern
            public event Func<object, DataEventArgs, Task<string>>? AsyncDataReceived;

            public void RaiseDataReceived(string data)
            {
                DataReceived?.Invoke(this, new DataEventArgs(data));
            }

            public async Task<List<string>> RaiseAsyncDataReceived(string data)
            {
                var results = new List<string>();
                var handler = AsyncDataReceived;

                if (handler != null)
                {
                    // Invoke all handlers and await them
                    var delegates = handler.GetInvocationList()
                        .Cast<Func<object, DataEventArgs, Task<string>>>();

                    var tasks = delegates.Select(d => d(this, new DataEventArgs(data)));
                    results.AddRange(await Task.WhenAll(tasks));
                }

                return results;
            }
        }

        #endregion

        #region Thread Safety Examples

        /// <summary>
        /// Demonstrates thread safety considerations with events
        /// 
        /// MULTITHREAD ASPECTS:
        /// - event keyword provides thread-safe add/remove
        /// - Event invocation needs explicit thread safety
        /// - Handlers may execute on different threads
        /// - Consider SynchronizationContext for UI updates
        /// - Race conditions possible with event modification during invocation
        /// 
        /// MEMORY ALLOCATION:
        /// - Thread synchronization may require additional allocations
        /// - SynchronizationContext usage adds overhead
        /// </summary>
        static void ThreadSafetyExamples()
        {
            Console.WriteLine("--- Thread Safety Examples ---");

            var monitor = new ThreadSafeMonitor();

            // Subscribe from multiple threads
            var subscribeTasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                int threadId = i;
                subscribeTasks.Add(Task.Run(() =>
                {
                    monitor.StatusChanged += (s, status) =>
                    {
                        // This handler may run on different threads
                        Console.WriteLine($"  [Thread {Thread.CurrentThread.ManagedThreadId}] " +
                                        $"Subscriber {threadId} received: {status}");
                    };
                }));
            }

            Task.WaitAll(subscribeTasks.ToArray());
            Thread.Sleep(100); // Ensure all subscriptions complete

            // Raise events from multiple threads
            Console.WriteLine("\nRaising events from multiple threads:");
            var raiseTasks = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                int msgNum = i;
                raiseTasks.Add(Task.Run(() =>
                    monitor.UpdateStatus($"Status {msgNum}")));
            }

            Task.WaitAll(raiseTasks.ToArray());

            // Demonstrate thread-safe invocation pattern
            Console.WriteLine("\nThread-safe invocation with local copy:");
            monitor.UpdateStatusSafe("Safe status update");

            Thread.Sleep(100); // Let async operations complete

            Console.WriteLine();
        }

        class ThreadSafeMonitor
        {
            // Event is thread-safe for add/remove operations
            public event Action<object?, string>? StatusChanged;

            // Unsafe invocation (can have race conditions)
            public void UpdateStatus(string status)
            {
                // If subscriber unsubscribes between check and invoke, NullReferenceException possible
                StatusChanged?.Invoke(this, status);
            }

            // Safe invocation pattern
            public void UpdateStatusSafe(string status)
            {
                // Copy to local variable - thread-safe
                var handler = StatusChanged;
                handler?.Invoke(this, status);
            }
        }

        #endregion

        #region Memory Leak Examples

        /// <summary>
        /// Demonstrates memory leak scenarios with events and solutions
        /// 
        /// SCENARIOS NOT TO USE:
        /// - Long-lived publisher with short-lived subscribers (without unsubscribe)
        /// - Static events without cleanup
        /// - Captured 'this' in lambda handlers
        /// 
        /// MEMORY ALLOCATION:
        /// - Event holds strong reference to subscribers
        /// - Prevents garbage collection of subscriber objects
        /// - Can cause significant memory leaks in large applications
        /// 
        /// SOLUTIONS:
        /// - Always unsubscribe when done
        /// - Use weak event pattern for mismatched lifetimes
        /// - IDisposable pattern for subscription management
        /// </summary>
        static void MemoryLeakExamples()
        {
            Console.WriteLine("--- Memory Leak Examples ---");

            // Demonstration 1: Memory leak scenario
            Console.WriteLine("Creating potential memory leak:");
            var longLivedPublisher = new LongLivedPublisher();

            for (int i = 0; i < 3; i++)
            {
                var shortLivedSubscriber = new ShortLivedSubscriber(i);
                // BAD: Subscriber won't be collected because publisher holds reference
                longLivedPublisher.DataPublished += shortLivedSubscriber.OnDataPublished;
            }

            Console.WriteLine($"Publisher has {longLivedPublisher.SubscriberCount} subscribers");
            Console.WriteLine("Short-lived subscribers should be collected, but they're not!");

            // Demonstration 2: Proper cleanup
            Console.WriteLine("\nProper subscription management:");
            var subscribers = new List<ShortLivedSubscriber>();

            for (int i = 3; i < 6; i++)
            {
                var subscriber = new ShortLivedSubscriber(i);
                subscribers.Add(subscriber);
                longLivedPublisher.DataPublished += subscriber.OnDataPublished;
            }

            Console.WriteLine($"Publisher has {longLivedPublisher.SubscriberCount} subscribers");

            // Cleanup
            foreach (var subscriber in subscribers)
            {
                longLivedPublisher.DataPublished -= subscriber.OnDataPublished;
            }

            Console.WriteLine($"After unsubscribe: {longLivedPublisher.SubscriberCount} subscribers");

            // Demonstration 3: WeakEventManager pattern (conceptual)
            Console.WriteLine("\nWeak event pattern (prevents memory leaks):");
            var weakPublisher = new WeakEventPublisher();
            
            {
                var tempSubscriber = new ShortLivedSubscriber(100);
                weakPublisher.Subscribe(tempSubscriber.OnDataPublished);
                weakPublisher.Publish("Test message 1");
            } // tempSubscriber goes out of scope

            // Force garbage collection for demonstration
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            Console.WriteLine("After GC (subscriber should be collected):");
            weakPublisher.Publish("Test message 2 (subscriber collected)");

            Console.WriteLine();
        }

        class LongLivedPublisher
        {
            public event EventHandler<string>? DataPublished;

            public int SubscriberCount
            {
                get
                {
                    var handler = DataPublished;
                    return handler?.GetInvocationList().Length ?? 0;
                }
            }

            public void Publish(string data)
            {
                DataPublished?.Invoke(this, data);
            }
        }

        class ShortLivedSubscriber
        {
            private readonly int _id;

            public ShortLivedSubscriber(int id)
            {
                _id = id;
                Console.WriteLine($"  ShortLivedSubscriber {_id} created");
            }

            public void OnDataPublished(object? sender, string data)
            {
                Console.WriteLine($"  Subscriber {_id} received: {data}");
            }

            ~ShortLivedSubscriber()
            {
                Console.WriteLine($"  ShortLivedSubscriber {_id} finalized");
            }
        }

        // Simplified weak event pattern
        class WeakEventPublisher
        {
            private readonly List<WeakReference<EventHandler<string>>> _subscribers = new();

            public void Subscribe(EventHandler<string> handler)
            {
                _subscribers.Add(new WeakReference<EventHandler<string>>(handler));
                Console.WriteLine("  Subscribed with weak reference");
            }

            public void Publish(string data)
            {
                Console.WriteLine($"Publishing: {data}");
                
                // Invoke only alive subscribers
                var deadReferences = new List<WeakReference<EventHandler<string>>>();

                foreach (var weakRef in _subscribers)
                {
                    if (weakRef.TryGetTarget(out var handler))
                    {
                        handler(this, data);
                    }
                    else
                    {
                        deadReferences.Add(weakRef);
                        Console.WriteLine("  Subscriber was garbage collected");
                    }
                }

                // Clean up dead references
                foreach (var deadRef in deadReferences)
                {
                    _subscribers.Remove(deadRef);
                }
            }
        }

        #endregion

        #region Performance Examples

        /// <summary>
        /// Demonstrates performance considerations with events
        /// 
        /// SCENARIOS NOT TO USE:
        /// - Performance-critical tight loops
        /// - High-frequency event firing (thousands per second)
        /// - When direct method calls would suffice
        /// 
        /// MEMORY ALLOCATION:
        /// - Event invocation has overhead compared to direct calls
        /// - EventArgs allocation for each event raise
        /// - Consider caching EventArgs instances
        /// - Multicast delegates have linear invocation cost
        /// </summary>
        static void PerformanceExamples()
        {
            Console.WriteLine("--- Performance Examples ---");

            const int iterations = 1_000_000;
            var perfTest = new PerformanceTest();

            // Benchmark 1: Direct method call
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                perfTest.DirectMethodCall();
            }
            sw.Stop();
            Console.WriteLine($"Direct method calls: {sw.ElapsedMilliseconds}ms");

            // Benchmark 2: Event with single subscriber
            perfTest.SimpleEvent += () => { };
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                perfTest.RaiseSimpleEvent();
            }
            sw.Stop();
            Console.WriteLine($"Single subscriber event: {sw.ElapsedMilliseconds}ms");

            // Benchmark 3: Event with multiple subscribers
            perfTest.SimpleEvent += () => { };
            perfTest.SimpleEvent += () => { };
            perfTest.SimpleEvent += () => { };
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                perfTest.RaiseSimpleEvent();
            }
            sw.Stop();
            Console.WriteLine($"Multiple subscriber event (4 total): {sw.ElapsedMilliseconds}ms");

            // Benchmark 4: Event with EventArgs allocation
            perfTest.EventWithArgs += (s, e) => { };
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                perfTest.RaiseEventWithArgs(i);
            }
            sw.Stop();
            Console.WriteLine($"Event with EventArgs allocation: {sw.ElapsedMilliseconds}ms");

            // Benchmark 5: Event with cached EventArgs
            perfTest.EventWithArgs += (s, e) => { };
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                perfTest.RaiseEventWithCachedArgs();
            }
            sw.Stop();
            Console.WriteLine($"Event with cached EventArgs: {sw.ElapsedMilliseconds}ms");

            Console.WriteLine();
        }

        class PerformanceTest
        {
            private static readonly ValueEventArgs CachedEventArgs = new(0);

            public event Action? SimpleEvent;
            public event EventHandler<ValueEventArgs>? EventWithArgs;

            public void DirectMethodCall()
            {
                // Direct method call - baseline
            }

            public void RaiseSimpleEvent()
            {
                SimpleEvent?.Invoke();
            }

            public void RaiseEventWithArgs(int value)
            {
                // Allocates new EventArgs each time
                EventWithArgs?.Invoke(this, new ValueEventArgs(value));
            }

            public void RaiseEventWithCachedArgs()
            {
                // Reuses cached EventArgs - no allocation
                EventWithArgs?.Invoke(this, (ValueEventArgs)CachedEventArgs);
            }
        }

        class ValueEventArgs : EventArgs
        {
            public int Value { get; }

            public ValueEventArgs(int value)
            {
                Value = value;
            }
        }

        #endregion
    }

    // Extension method for event invocation pattern
    static class EventExtensions
    {
        public static void Raise(this EventHandler? handler, object sender, EventArgs e)
        {
            handler?.Invoke(sender, e);
        }
    }
}
