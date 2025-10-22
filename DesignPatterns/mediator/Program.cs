using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

/*
 * MEDIATOR DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Mediator pattern defines how a set of objects interact with each other.
 * Instead of objects communicating directly, they communicate through a central mediator.
 * This promotes loose coupling by keeping objects from referring to each other explicitly.
 * 
 * CORE BENEFITS:
 * - Reduces dependencies between communicating objects (loose coupling)
 * - Centralizes complex communications and control logic
 * - Makes object interaction easier to understand and maintain
 * - Promotes reusability of individual components
 * - Follows the Single Responsibility Principle for communication logic
 * 
 * SCENARIOS TO USE:
 * - When objects need to communicate in complex ways with many-to-many relationships
 * - When you want to reuse objects in different contexts without tight coupling
 * - When communication logic becomes too complex and scattered across objects
 * - In GUI applications (dialog boxes, forms with interdependent controls)
 * - In workflow systems where steps need to coordinate
 * - In chat/messaging systems where users communicate through a central hub
 * - In air traffic control systems where planes communicate through a tower
 * 
 * SCENARIOS NOT TO USE:
 * - When objects have simple, direct one-to-one relationships
 * - When the mediator becomes too complex (God Object anti-pattern)
 * - In performance-critical code where direct communication is faster
 * - When objects rarely interact with each other
 * - When the system is small and coupling isn't a concern
 * - When communication patterns are unlikely to change
 */

namespace MediatorPattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Mediator Design Pattern Examples ===\n");

            // Example 1: Basic Chat Room Mediator
            BasicChatRoomExample();

            // Example 2: Air Traffic Control System
            AirTrafficControlExample();

            // Example 3: GUI Form Mediator
            GuiMediatorExample();

            // Example 4: Workflow Coordination
            WorkflowMediatorExample();

            // Example 5: Thread-Safe Event Mediator
            await ThreadSafeEventMediatorExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Basic Chat Room Mediator

        static void BasicChatRoomExample()
        {
            Console.WriteLine("1. Basic Chat Room Mediator:");
            Console.WriteLine("=============================");

            var chatRoom = new ChatRoomMediator();

            var alice = new User("Alice", chatRoom);
            var bob = new User("Bob", chatRoom);
            var charlie = new User("Charlie", chatRoom);

            // Users communicate through the mediator
            alice.SendMessage("Hello everyone!");
            bob.SendMessage("Hi Alice!");
            charlie.SendMessage("Good morning!");

            // Private message through mediator
            alice.SendPrivateMessage("Bob", "How are you doing?");

            Console.WriteLine();
        }

        #endregion

        #region Example 2: Air Traffic Control System

        static void AirTrafficControlExample()
        {
            Console.WriteLine("2. Air Traffic Control System:");
            Console.WriteLine("===============================");

            var controlTower = new AirTrafficControlTower();

            var flight1 = new Aircraft("AA101", "Boeing 737", controlTower);
            var flight2 = new Aircraft("UA205", "Airbus A320", controlTower);
            var flight3 = new Aircraft("DL300", "Boeing 777", controlTower);

            // Aircraft communicate through control tower
            flight1.RequestLanding();
            flight2.RequestTakeoff();
            flight3.RequestLanding();
            
            flight1.CompleteLanding();
            flight2.CompleteTakeoff();

            Console.WriteLine();
        }

        #endregion

        #region Example 3: GUI Form Mediator

        static void GuiMediatorExample()
        {
            Console.WriteLine("3. GUI Form Mediator:");
            Console.WriteLine("======================");

            var formMediator = new DialogMediator();

            // Create form controls
            var loginButton = new Button("Login", formMediator);
            var usernameField = new TextBox("Username", formMediator);
            var passwordField = new TextBox("Password", formMediator);
            var rememberCheckbox = new CheckBox("Remember Me", formMediator);

            // Simulate user interactions
            Console.WriteLine("Simulating form interactions:");
            usernameField.SetText("john.doe");
            passwordField.SetText("password123");
            rememberCheckbox.SetChecked(true);
            loginButton.Click();

            Console.WriteLine();
        }

        #endregion

        #region Example 4: Workflow Coordination

        static void WorkflowMediatorExample()
        {
            Console.WriteLine("4. Workflow Coordination:");
            Console.WriteLine("=========================");

            var workflowEngine = new WorkflowMediator();

            var orderStep = new OrderProcessingStep(workflowEngine);
            var paymentStep = new PaymentProcessingStep(workflowEngine);
            var inventoryStep = new InventoryStep(workflowEngine);
            var shippingStep = new ShippingStep(workflowEngine);

            // Process an order through the workflow
            Console.WriteLine("Processing order #12345:");
            orderStep.ProcessOrder("12345");

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Thread-Safe Event Mediator

        static async Task ThreadSafeEventMediatorExample()
        {
            Console.WriteLine("5. Thread-Safe Event Mediator:");
            Console.WriteLine("===============================");

            var eventBus = new ThreadSafeEventMediator();

            var publisher1 = new EventPublisher("Publisher1", eventBus);
            var publisher2 = new EventPublisher("Publisher2", eventBus);
            var subscriber1 = new EventSubscriber("Subscriber1", eventBus);
            var subscriber2 = new EventSubscriber("Subscriber2", eventBus);

            // Subscribe to events
            subscriber1.SubscribeToEvent("OrderCreated");
            subscriber1.SubscribeToEvent("PaymentProcessed");
            subscriber2.SubscribeToEvent("OrderCreated");

            // Simulate concurrent event publishing
            var tasks = new[]
            {
                Task.Run(() => publisher1.PublishEvent("OrderCreated", "Order #1001 created")),
                Task.Run(() => publisher2.PublishEvent("OrderCreated", "Order #1002 created")),
                Task.Run(() => publisher1.PublishEvent("PaymentProcessed", "Payment for #1001 completed"))
            };

            await Task.WhenAll(tasks);
            
            // Small delay to see all events processed
            await Task.Delay(100);

            Console.WriteLine();
        }

        #endregion
    }

    #region Chat Room Mediator Example

    // Abstract mediator interface
    public interface IChatRoomMediator
    {
        void SendMessage(string message, User sender);
        void SendPrivateMessage(string message, User sender, string recipientName);
        void AddUser(User user);
        void RemoveUser(User user);
    }

    // Concrete mediator implementation
    // MEMORY ALLOCATION: Maintains list of users - memory grows with user count
    public class ChatRoomMediator : IChatRoomMediator
    {
        private readonly List<User> _users = new List<User>();

        public void AddUser(User user)
        {
            if (!_users.Contains(user))
            {
                _users.Add(user);
                Console.WriteLine($"  [SYSTEM] {user.Name} joined the chat room");
            }
        }

        public void RemoveUser(User user)
        {
            if (_users.Remove(user))
            {
                Console.WriteLine($"  [SYSTEM] {user.Name} left the chat room");
            }
        }

        public void SendMessage(string message, User sender)
        {
            Console.WriteLine($"  [{sender.Name}] {message}");
            
            // Deliver to all users except sender
            foreach (var user in _users.Where(u => u != sender))
            {
                user.ReceiveMessage(message, sender.Name);
            }
        }

        public void SendPrivateMessage(string message, User sender, string recipientName)
        {
            var recipient = _users.FirstOrDefault(u => u.Name == recipientName);
            if (recipient != null)
            {
                Console.WriteLine($"  [PRIVATE {sender.Name} → {recipientName}] {message}");
                recipient.ReceiveMessage(message, sender.Name);
            }
            else
            {
                sender.ReceiveMessage($"User '{recipientName}' not found", "SYSTEM");
            }
        }
    }

    // Colleague class - User
    public class User
    {
        public string Name { get; }
        private readonly IChatRoomMediator _mediator;

        public User(string name, IChatRoomMediator mediator)
        {
            Name = name;
            _mediator = mediator;
            _mediator.AddUser(this);
        }

        public void SendMessage(string message)
        {
            _mediator.SendMessage(message, this);
        }

        public void SendPrivateMessage(string recipientName, string message)
        {
            _mediator.SendPrivateMessage(message, this, recipientName);
        }

        public void ReceiveMessage(string message, string senderName)
        {
            // In a real application, this would update the UI or store the message
            // Console.WriteLine($"  {Name} received: {message} (from {senderName})");
        }
    }

    #endregion

    #region Air Traffic Control Example

    public interface IAirTrafficControlMediator
    {
        void RequestLanding(Aircraft aircraft);
        void RequestTakeoff(Aircraft aircraft);
        void NotifyLandingComplete(Aircraft aircraft);
        void NotifyTakeoffComplete(Aircraft aircraft);
    }

    // MEMORY ALLOCATION: Maintains queues and runway state - bounded memory usage
    public class AirTrafficControlTower : IAirTrafficControlMediator
    {
        private readonly Queue<Aircraft> _landingQueue = new Queue<Aircraft>();
        private readonly Queue<Aircraft> _takeoffQueue = new Queue<Aircraft>();
        private bool _runwayBusy = false;

        public void RequestLanding(Aircraft aircraft)
        {
            Console.WriteLine($"  [TOWER] {aircraft.CallSign} requesting landing permission");
            
            if (!_runwayBusy && _landingQueue.Count == 0)
            {
                _runwayBusy = true;
                Console.WriteLine($"  [TOWER] {aircraft.CallSign} cleared for landing");
                aircraft.Land();
            }
            else
            {
                _landingQueue.Enqueue(aircraft);
                Console.WriteLine($"  [TOWER] {aircraft.CallSign} added to landing queue (position {_landingQueue.Count})");
            }
        }

        public void RequestTakeoff(Aircraft aircraft)
        {
            Console.WriteLine($"  [TOWER] {aircraft.CallSign} requesting takeoff permission");
            
            if (!_runwayBusy && _landingQueue.Count == 0 && _takeoffQueue.Count == 0)
            {
                _runwayBusy = true;
                Console.WriteLine($"  [TOWER] {aircraft.CallSign} cleared for takeoff");
                aircraft.Takeoff();
            }
            else
            {
                _takeoffQueue.Enqueue(aircraft);
                Console.WriteLine($"  [TOWER] {aircraft.CallSign} added to takeoff queue (position {_takeoffQueue.Count})");
            }
        }

        public void NotifyLandingComplete(Aircraft aircraft)
        {
            Console.WriteLine($"  [TOWER] {aircraft.CallSign} landing complete");
            _runwayBusy = false;
            ProcessNextOperation();
        }

        public void NotifyTakeoffComplete(Aircraft aircraft)
        {
            Console.WriteLine($"  [TOWER] {aircraft.CallSign} takeoff complete");
            _runwayBusy = false;
            ProcessNextOperation();
        }

        private void ProcessNextOperation()
        {
            // Priority: Landing requests first, then takeoffs
            if (_landingQueue.Count > 0)
            {
                var nextAircraft = _landingQueue.Dequeue();
                _runwayBusy = true;
                Console.WriteLine($"  [TOWER] {nextAircraft.CallSign} cleared for landing");
                nextAircraft.Land();
            }
            else if (_takeoffQueue.Count > 0)
            {
                var nextAircraft = _takeoffQueue.Dequeue();
                _runwayBusy = true;
                Console.WriteLine($"  [TOWER] {nextAircraft.CallSign} cleared for takeoff");
                nextAircraft.Takeoff();
            }
        }
    }

    public class Aircraft
    {
        public string CallSign { get; }
        public string AircraftType { get; }
        private readonly IAirTrafficControlMediator _controlTower;

        public Aircraft(string callSign, string aircraftType, IAirTrafficControlMediator controlTower)
        {
            CallSign = callSign;
            AircraftType = aircraftType;
            _controlTower = controlTower;
        }

        public void RequestLanding()
        {
            _controlTower.RequestLanding(this);
        }

        public void RequestTakeoff()
        {
            _controlTower.RequestTakeoff(this);
        }

        public void Land()
        {
            // Simulate landing process
            Thread.Sleep(100);
            _controlTower.NotifyLandingComplete(this);
        }

        public void Takeoff()
        {
            // Simulate takeoff process
            Thread.Sleep(100);
            _controlTower.NotifyTakeoffComplete(this);
        }

        public void CompleteLanding()
        {
            // Called externally to simulate landing completion
            Land();
        }

        public void CompleteTakeoff()
        {
            // Called externally to simulate takeoff completion
            Takeoff();
        }
    }

    #endregion

    #region GUI Form Mediator Example

    public interface IDialogMediator
    {
        void Notify(Component sender, string eventType, object? data = null);
    }

    // Mediator coordinating form controls
    public class DialogMediator : IDialogMediator
    {
        private Button? _loginButton;
        private TextBox? _usernameField;
        private TextBox? _passwordField;
        private CheckBox? _rememberCheckbox;

        public void Notify(Component sender, string eventType, object? data = null)
        {
            // Cache component references
            if (sender is Button btn && btn.Name == "Login") _loginButton = btn;
            if (sender is TextBox txt && txt.Name == "Username") _usernameField = txt;
            if (sender is TextBox txt2 && txt2.Name == "Password") _passwordField = txt2;
            if (sender is CheckBox chk && chk.Name == "Remember Me") _rememberCheckbox = chk;

            switch (eventType)
            {
                case "TextChanged":
                    ValidateForm();
                    break;
                    
                case "Click" when sender == _loginButton:
                    HandleLogin();
                    break;
                    
                case "CheckedChanged":
                    Console.WriteLine($"    Remember me option: {(_rememberCheckbox?.IsChecked == true ? "Enabled" : "Disabled")}");
                    break;
            }
        }

        private void ValidateForm()
        {
            var isValid = !string.IsNullOrEmpty(_usernameField?.Text) && 
                         !string.IsNullOrEmpty(_passwordField?.Text);
            
            if (_loginButton != null)
            {
                _loginButton.IsEnabled = isValid;
                Console.WriteLine($"    Login button: {(isValid ? "Enabled" : "Disabled")}");
            }
        }

        private void HandleLogin()
        {
            if (_usernameField != null && _passwordField != null)
            {
                Console.WriteLine($"    Attempting login for user: {_usernameField.Text}");
                Console.WriteLine($"    Remember credentials: {(_rememberCheckbox?.IsChecked == true)}");
                Console.WriteLine("    Login successful!");
            }
        }
    }

    // Base component class
    public abstract class Component
    {
        public string Name { get; }
        protected readonly IDialogMediator? _mediator;

        protected Component(string name, IDialogMediator? mediator)
        {
            Name = name;
            _mediator = mediator;
        }
    }

    public class Button : Component
    {
        public bool IsEnabled { get; set; } = true;

        public Button(string name, IDialogMediator mediator) : base(name, mediator) { }

        public void Click()
        {
            if (IsEnabled)
            {
                Console.WriteLine($"    Button '{Name}' clicked");
                _mediator?.Notify(this, "Click");
            }
        }
    }

    public class TextBox : Component
    {
        public string Text { get; private set; } = string.Empty;

        public TextBox(string name, IDialogMediator mediator) : base(name, mediator) { }

        public void SetText(string text)
        {
            Text = text;
            Console.WriteLine($"    TextBox '{Name}' text changed to: '{text}'");
            _mediator?.Notify(this, "TextChanged", text);
        }
    }

    public class CheckBox : Component
    {
        public bool IsChecked { get; private set; }

        public CheckBox(string name, IDialogMediator mediator) : base(name, mediator) { }

        public void SetChecked(bool isChecked)
        {
            IsChecked = isChecked;
            Console.WriteLine($"    CheckBox '{Name}' checked: {isChecked}");
            _mediator?.Notify(this, "CheckedChanged", isChecked);
        }
    }

    #endregion

    #region Workflow Mediator Example

    public interface IWorkflowMediator
    {
        void NotifyStepComplete(string stepName, string orderId, object? result = null);
        void NotifyStepFailed(string stepName, string orderId, string error);
    }

    // MEMORY ALLOCATION: Tracks workflow state - memory per active workflow
    public class WorkflowMediator : IWorkflowMediator
    {
        private readonly Dictionary<string, WorkflowState> _activeWorkflows = new Dictionary<string, WorkflowState>();

        public void NotifyStepComplete(string stepName, string orderId, object? result = null)
        {
            if (!_activeWorkflows.ContainsKey(orderId))
            {
                _activeWorkflows[orderId] = new WorkflowState(orderId);
            }

            var workflow = _activeWorkflows[orderId];
            workflow.CompleteStep(stepName);

            Console.WriteLine($"    Step '{stepName}' completed for order {orderId}");

            // Determine next step based on current progress
            DetermineNextStep(orderId, workflow);
        }

        public void NotifyStepFailed(string stepName, string orderId, string error)
        {
            Console.WriteLine($"    Step '{stepName}' failed for order {orderId}: {error}");
            // In a real system, implement retry logic or error handling
        }

        private void DetermineNextStep(string orderId, WorkflowState workflow)
        {
            if (workflow.IsStepComplete("OrderProcessing") && !workflow.IsStepComplete("PaymentProcessing"))
            {
                Console.WriteLine($"    Triggering payment processing for order {orderId}");
                // Trigger payment processing
            }
            else if (workflow.IsStepComplete("PaymentProcessing") && !workflow.IsStepComplete("InventoryCheck"))
            {
                Console.WriteLine($"    Triggering inventory check for order {orderId}");
                // Trigger inventory check
            }
            else if (workflow.IsStepComplete("InventoryCheck") && !workflow.IsStepComplete("Shipping"))
            {
                Console.WriteLine($"    Triggering shipping for order {orderId}");
                // Trigger shipping
            }
            else if (workflow.IsStepComplete("Shipping"))
            {
                Console.WriteLine($"    Order {orderId} workflow completed successfully!");
                _activeWorkflows.Remove(orderId); // Clean up completed workflow
            }
        }
    }

    public class WorkflowState
    {
        public string OrderId { get; }
        private readonly HashSet<string> _completedSteps = new HashSet<string>();

        public WorkflowState(string orderId)
        {
            OrderId = orderId;
        }

        public void CompleteStep(string stepName)
        {
            _completedSteps.Add(stepName);
        }

        public bool IsStepComplete(string stepName)
        {
            return _completedSteps.Contains(stepName);
        }
    }

    // Workflow step base class
    public abstract class WorkflowStep
    {
        protected readonly IWorkflowMediator _mediator;
        protected readonly string _stepName;

        protected WorkflowStep(string stepName, IWorkflowMediator mediator)
        {
            _stepName = stepName;
            _mediator = mediator;
        }
    }

    public class OrderProcessingStep : WorkflowStep
    {
        public OrderProcessingStep(IWorkflowMediator mediator) : base("OrderProcessing", mediator) { }

        public void ProcessOrder(string orderId)
        {
            Console.WriteLine($"    Processing order {orderId}...");
            Thread.Sleep(50); // Simulate processing
            _mediator.NotifyStepComplete(_stepName, orderId);
        }
    }

    public class PaymentProcessingStep : WorkflowStep
    {
        public PaymentProcessingStep(IWorkflowMediator mediator) : base("PaymentProcessing", mediator) { }

        // This would be called by the mediator when payment processing is needed
    }

    public class InventoryStep : WorkflowStep
    {
        public InventoryStep(IWorkflowMediator mediator) : base("InventoryCheck", mediator) { }

        // This would be called by the mediator when inventory check is needed
    }

    public class ShippingStep : WorkflowStep
    {
        public ShippingStep(IWorkflowMediator mediator) : base("Shipping", mediator) { }

        // This would be called by the mediator when shipping is needed
    }

    #endregion

    #region Thread-Safe Event Mediator Example

    public interface IEventMediator
    {
        void PublishEvent(string eventType, object data);
        void Subscribe(string eventType, Action<object> handler);
        void Unsubscribe(string eventType, Action<object> handler);
    }

    // MULTITHREAD ASPECTS: Thread-safe event mediator using concurrent collections
    public class ThreadSafeEventMediator : IEventMediator
    {
        // Using ConcurrentDictionary for thread-safe operations
        private readonly ConcurrentDictionary<string, ConcurrentBag<Action<object>>> _subscribers
            = new ConcurrentDictionary<string, ConcurrentBag<Action<object>>>();

        public void PublishEvent(string eventType, object data)
        {
            if (_subscribers.TryGetValue(eventType, out var handlers))
            {
                Console.WriteLine($"    Publishing event '{eventType}' with data: {data}");
                
                // Create a snapshot to avoid concurrent modification issues
                var handlerList = handlers.ToArray();
                
                // Execute handlers in parallel for better performance
                Parallel.ForEach(handlerList, handler =>
                {
                    try
                    {
                        handler(data);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"    Error in event handler: {ex.Message}");
                    }
                });
            }
        }

        public void Subscribe(string eventType, Action<object> handler)
        {
            _subscribers.AddOrUpdate(eventType,
                new ConcurrentBag<Action<object>> { handler },
                (key, existing) =>
                {
                    existing.Add(handler);
                    return existing;
                });
        }

        public void Unsubscribe(string eventType, Action<object> handler)
        {
            // Note: ConcurrentBag doesn't support removal, so in production
            // you'd use a different concurrent collection or implement a wrapper
            // For this example, we'll keep it simple
        }
    }

    public class EventPublisher
    {
        private readonly string _name;
        private readonly IEventMediator _eventMediator;

        public EventPublisher(string name, IEventMediator eventMediator)
        {
            _name = name;
            _eventMediator = eventMediator;
        }

        public void PublishEvent(string eventType, object data)
        {
            Console.WriteLine($"    [{_name}] Publishing {eventType}");
            _eventMediator.PublishEvent(eventType, data);
        }
    }

    public class EventSubscriber
    {
        private readonly string _name;
        private readonly IEventMediator _eventMediator;

        public EventSubscriber(string name, IEventMediator eventMediator)
        {
            _name = name;
            _eventMediator = eventMediator;
        }

        public void SubscribeToEvent(string eventType)
        {
            _eventMediator.Subscribe(eventType, HandleEvent);
            Console.WriteLine($"    [{_name}] Subscribed to {eventType}");
        }

        private void HandleEvent(object data)
        {
            Console.WriteLine($"      [{_name}] Received event with data: {data}");
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. MEDIATOR STATE MANAGEMENT:
 *    - Mediator typically maintains collections of colleagues/components
 *    - Memory usage grows with number of participants
 *    - Use weak references for large systems to prevent memory leaks
 * 
 * 2. EVENT HANDLING:
 *    - Event handlers can create memory pressure if not properly unsubscribed
 *    - Use weak event patterns or explicit unsubscription for long-lived objects
 *    - Consider object pools for high-frequency event scenarios
 * 
 * 3. WORKFLOW STATE:
 *    - Workflow mediators may accumulate state over time
 *    - Implement cleanup mechanisms for completed workflows
 *    - Use bounded queues to prevent unbounded memory growth
 * 
 * 4. MESSAGE QUEUING:
 *    - Asynchronous mediators may queue messages
 *    - Monitor queue sizes and implement backpressure mechanisms
 *    - Consider persistent storage for critical message scenarios
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. CONCURRENT ACCESS:
 *    - Mediator is often accessed by multiple threads simultaneously
 *    - Use thread-safe collections (ConcurrentDictionary, ConcurrentBag, etc.)
 *    - Protect shared state with appropriate synchronization primitives
 * 
 * 2. EVENT HANDLING:
 *    - Event handlers may execute on different threads
 *    - Consider using async/await for I/O-bound event handlers
 *    - Implement proper exception handling to prevent thread crashes
 * 
 * 3. DEADLOCK PREVENTION:
 *    - Avoid nested locking when mediator coordinates multiple objects
 *    - Use lock-free algorithms where possible
 *    - Consider using actor model patterns for complex coordination
 * 
 * 4. PERFORMANCE CONSIDERATIONS:
 *    - Parallel event processing can improve throughput
 *    - Be careful with shared mutable state
 *    - Use immutable messages when possible
 *    - Consider using ThreadLocal<T> for thread-specific mediator state
 * 
 * 5. BEST PRACTICES:
 *    - Design mediator interfaces to be thread-safe by default
 *    - Use cancellation tokens for long-running operations
 *    - Implement proper logging and monitoring for debugging
 *    - Consider using message-passing instead of shared state
 */
