using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

/*
Handler<T>:abstract
	-_nextHandler
	+SetNext(Handler<T> handler)
	+Handle(T request)
	+CanHandle(T request)
	+ProcessRequest(T request)
	+HandleDefault(T request)

ExpenseRequest
	+Description
	+Amount
	+Requester
	+IsApproved
	+ApprovedBy

TeamLeadApprover<ExpenseRequest>
	+CanHandle(T request)
	+ProcessRequest(T request)

ManagerApprover<ExpenseRequest>
	+CanHandle(T request)
	+ProcessRequest(T request)

TeamLeadApprover--*>Handler
ManagerApprover--*>Handler
*/

/*
In the chain of responsibility pattern you are abstracting the conditions (ifs and elses)
to treat each condition as a separate handler class.

Use chain of responsability pattern when you see conditional (ifs) processing based on request content
Handler classes should focus on a single responsibility
*/


/*
 * CHAIN OF RESPONSIBILITY DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Chain of Responsibility pattern allows you to pass requests along a chain of handlers.
 * Upon receiving a request, each handler decides either to process the request or to pass it
 * to the next handler in the chain. This pattern decouples the sender of a request from its
 * receivers by giving multiple objects a chance to handle the request.
 * 
 * CORE BENEFITS:
 * - Decouples request senders from receivers
 * - Allows dynamic configuration of handler chains
 * - Follows the Open/Closed Principle - easy to add new handlers
 * - Provides flexible request handling with multiple potential processors
 * - Enables conditional processing based on request content
 * 
 * SCENARIOS TO USE:
 * - When you have multiple objects that can handle a request, but you don't know which one will handle it
 * - When you want to issue a request without specifying the receiver explicitly
 * - For implementing approval workflows (expense approvals, loan processing)
 * - In middleware pipelines (HTTP request processing, logging, authentication)
 * - For event handling systems where multiple handlers might process an event
 * - In validation chains where multiple validators check different aspects
 * - For implementing support ticket routing systems
 * - In game development for handling different types of input or events
 * - For implementing parser chains in compilers or interpreters
 * 
 * SCENARIOS NOT TO USE:
 * - When you know exactly which object should handle the request
 * - If the chain is very long and performance is critical (overhead of chain traversal)
 * - When request handling is simple and doesn't require multiple potential handlers
 * - If the handler determination logic is complex and better suited for a factory pattern
 * - When you need guaranteed processing by a specific handler
 * - In scenarios where the chain setup is more complex than direct method calls
 */

namespace ChainOfResponsibilityPattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Chain of Responsibility Design Pattern Examples ===\n");

            // Example 1: Basic Expense Approval Chain
            ExpenseApprovalExample();

            // Example 2: HTTP Middleware Pipeline
            HttpMiddlewareExample();

            // Example 3: Support Ticket Routing
            SupportTicketExample();

            // Example 4: Input Validation Chain
            InputValidationExample();

            // Example 5: Thread-Safe Event Processing Chain
            await ThreadSafeEventProcessingExample();

            // Example 6: Dynamic Chain Configuration
            DynamicChainExample();

            // Example 7: Async Chain Processing
            await AsyncChainExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Basic Expense Approval Chain

        static void ExpenseApprovalExample()
        {
            Console.WriteLine("1. Expense Approval Chain:");
            Console.WriteLine("===========================");

            // Build the chain of responsibility
            var teamLead = new TeamLeadApprover();
            var manager = new ManagerApprover();
            var director = new DirectorApprover();
            var cfo = new CFOApprover();

            // Set up the chain
            teamLead.SetNext(manager).SetNext(director).SetNext(cfo);

            // Test different expense amounts
            var expenses = new[]
            {
                new ExpenseRequest("Office supplies", 150, "John Doe"),
                new ExpenseRequest("Team lunch", 800, "Jane Smith"),
                new ExpenseRequest("New laptops", 5000, "IT Department"),
                new ExpenseRequest("Company retreat", 25000, "HR Department")
            };

            foreach (var expense in expenses)
            {
                Console.WriteLine($"\nProcessing: {expense.Description} - ${expense.Amount}");
                teamLead.Handle(expense);
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 2: HTTP Middleware Pipeline

        static void HttpMiddlewareExample()
        {
            Console.WriteLine("2. HTTP Middleware Pipeline:");
            Console.WriteLine("=============================");

            // Build middleware chain
            var logger = new LoggingMiddleware();
            var auth = new AuthenticationMiddleware();
            var cors = new CorsMiddleware();
            var rateLimit = new RateLimitingMiddleware();
            var controller = new ControllerMiddleware();

            // Set up the pipeline
            logger.SetNext(auth).SetNext(cors).SetNext(rateLimit).SetNext(controller);

            // Simulate HTTP requests
            var requests = new[]
            {
                new HttpRequest("GET", "/api/users", new Dictionary<string, string> { {"Authorization", "Bearer valid-token"} }),
                new HttpRequest("POST", "/api/users", new Dictionary<string, string>()),
                new HttpRequest("GET", "/api/admin", new Dictionary<string, string> { {"Authorization", "Bearer invalid-token"} })
            };

            foreach (var request in requests)
            {
                Console.WriteLine($"\nProcessing: {request.Method} {request.Path}");
                var response = logger.Handle(request);
                Console.WriteLine($"Final response: {response.StatusCode} - {response.Message}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Support Ticket Routing

        static void SupportTicketExample()
        {
            Console.WriteLine("3. Support Ticket Routing:");
            Console.WriteLine("===========================");

            // Build support chain
            var level1 = new Level1SupportHandler();
            var level2 = new Level2SupportHandler();
            var level3 = new Level3SupportHandler();
            var escalation = new EscalationHandler();

            level1.SetNext(level2).SetNext(level3).SetNext(escalation);

            // Test different ticket types
            var tickets = new[]
            {
                new SupportTicket(1, "Password reset", TicketPriority.Low, TicketCategory.Account),
                new SupportTicket(2, "Server is down", TicketPriority.Critical, TicketCategory.Infrastructure),
                new SupportTicket(3, "Feature request", TicketPriority.Medium, TicketCategory.Development),
                new SupportTicket(4, "Security breach", TicketPriority.Critical, TicketCategory.Security)
            };

            foreach (var ticket in tickets)
            {
                Console.WriteLine($"\nRouting ticket #{ticket.Id}: {ticket.Description}");
                level1.Handle(ticket);
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 4: Input Validation Chain

        static void InputValidationExample()
        {
            Console.WriteLine("4. Input Validation Chain:");
            Console.WriteLine("===========================");

            // Build validation chain
            var notNull = new NotNullValidator();
            var length = new LengthValidator(3, 50);
            var format = new EmailFormatValidator();
            var blacklist = new BlacklistValidator();

            notNull.SetNext(length).SetNext(format).SetNext(blacklist);

            // Test different email inputs
            var emails = new[] { null, "", "ab", "valid@email.com", "spam@blocked.com", "invalid-email" };

            foreach (var email in emails)
            {
                Console.WriteLine($"\nValidating: '{email ?? "null"}'");
                var result = notNull.Handle(new ValidationRequest(email ?? string.Empty));
                Console.WriteLine($"Result: {(result.IsValid ? "Valid" : "Invalid")} - {result.ErrorMessage}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Thread-Safe Event Processing Chain

        static async Task ThreadSafeEventProcessingExample()
        {
            Console.WriteLine("5. Thread-Safe Event Processing Chain:");
            Console.WriteLine("=======================================");

            var processor = new ThreadSafeEventProcessor();

            // Create and start multiple event processing tasks
            var tasks = new List<Task>();

            for (int i = 1; i <= 5; i++)
            {
                int eventId = i;
                tasks.Add(Task.Run(async () =>
                {
                    var evt = new ProcessingEvent(eventId, $"Event {eventId}", DateTime.Now);
                    await processor.ProcessEventAsync(evt);
                }));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine();
        }

        #endregion

        #region Example 6: Dynamic Chain Configuration

        static void DynamicChainExample()
        {
            Console.WriteLine("6. Dynamic Chain Configuration:");
            Console.WriteLine("================================");

            var factory = new HandlerChainFactory();

            // Create different chains for different scenarios
            var basicChain = factory.CreateBasicApprovalChain();
            var strictChain = factory.CreateStrictApprovalChain();

            var expense = new ExpenseRequest("Conference attendance", 2000, "Alice Johnson");

            Console.WriteLine("Processing with basic chain:");
            basicChain.Handle(expense);

            Console.WriteLine("\nProcessing with strict chain:");
            strictChain.Handle(expense);

            Console.WriteLine();
        }

        #endregion

        #region Example 7: Async Chain Processing

        static async Task AsyncChainExample()
        {
            Console.WriteLine("7. Async Chain Processing:");
            Console.WriteLine("===========================");

            // Build async processing chain
            var preprocessor = new AsyncPreprocessor();
            var validator = new AsyncValidator();
            var processor = new AsyncDataProcessor();
            var postprocessor = new AsyncPostprocessor();

            preprocessor.SetNext(validator).SetNext(processor).SetNext(postprocessor);

            var data = new ProcessingData { Id = 1, Content = "Sample data for async processing" };

            Console.WriteLine("Starting async chain processing...");
            var result = await preprocessor.HandleAsync(data);
            Console.WriteLine($"Processing completed: {result}");

            Console.WriteLine();
        }

        #endregion
    }

    #region Core Chain of Responsibility Classes

    // Abstract base handler
    // MEMORY ALLOCATION: Each handler holds a reference to the next handler
    public abstract class Handler<T>
    {
        private Handler<T>? _nextHandler;

        public Handler<T> SetNext(Handler<T> handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual T Handle(T request)
        {
            if (CanHandle(request))
            {
                return ProcessRequest(request);
            }
            else if (_nextHandler != null)
            {
                return _nextHandler.Handle(request);
            }
            else
            {
                return HandleDefault(request);
            }
        }

        protected abstract bool CanHandle(T request);
        protected abstract T ProcessRequest(T request);
        protected virtual T HandleDefault(T request) => request;
    }

    // Async version of the handler
    public abstract class AsyncHandler<T>
    {
        private AsyncHandler<T>? _nextHandler;

        public AsyncHandler<T> SetNext(AsyncHandler<T> handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual async Task<T> HandleAsync(T request, CancellationToken cancellationToken = default)
        {
            if (await CanHandleAsync(request, cancellationToken))
            {
                return await ProcessRequestAsync(request, cancellationToken);
            }
            else if (_nextHandler != null)
            {
                return await _nextHandler.HandleAsync(request, cancellationToken);
            }
            else
            {
                return await HandleDefaultAsync(request, cancellationToken);
            }
        }

        protected abstract Task<bool> CanHandleAsync(T request, CancellationToken cancellationToken = default);
        protected abstract Task<T> ProcessRequestAsync(T request, CancellationToken cancellationToken = default);
        protected virtual Task<T> HandleDefaultAsync(T request, CancellationToken cancellationToken = default) => Task.FromResult(request);
    }

    #endregion

    #region Example 1: Expense Approval Classes

    public class ExpenseRequest
    {
        public string Description { get; }
        public decimal Amount { get; }
        public string Requester { get; }
        public bool IsApproved { get; set; }
        public string? ApprovedBy { get; set; }

        public ExpenseRequest(string description, decimal amount, string requester)
        {
            Description = description;
            Amount = amount;
            Requester = requester;
        }
    }

    public class TeamLeadApprover : Handler<ExpenseRequest>
    {
        private const decimal APPROVAL_LIMIT = 500;

        protected override bool CanHandle(ExpenseRequest request)
        {
            return request.Amount <= APPROVAL_LIMIT;
        }

        protected override ExpenseRequest ProcessRequest(ExpenseRequest request)
        {
            request.IsApproved = true;
            request.ApprovedBy = "Team Lead";
            Console.WriteLine($"  ✓ Approved by Team Lead (limit: ${APPROVAL_LIMIT})");
            return request;
        }
    }

    public class ManagerApprover : Handler<ExpenseRequest>
    {
        private const decimal APPROVAL_LIMIT = 2000;

        protected override bool CanHandle(ExpenseRequest request)
        {
            return request.Amount <= APPROVAL_LIMIT;
        }

        protected override ExpenseRequest ProcessRequest(ExpenseRequest request)
        {
            request.IsApproved = true;
            request.ApprovedBy = "Manager";
            Console.WriteLine($"  ✓ Approved by Manager (limit: ${APPROVAL_LIMIT})");
            return request;
        }
    }

    public class DirectorApprover : Handler<ExpenseRequest>
    {
        private const decimal APPROVAL_LIMIT = 10000;

        protected override bool CanHandle(ExpenseRequest request)
        {
            return request.Amount <= APPROVAL_LIMIT;
        }

        protected override ExpenseRequest ProcessRequest(ExpenseRequest request)
        {
            request.IsApproved = true;
            request.ApprovedBy = "Director";
            Console.WriteLine($"  ✓ Approved by Director (limit: ${APPROVAL_LIMIT})");
            return request;
        }
    }

    public class CFOApprover : Handler<ExpenseRequest>
    {
        protected override bool CanHandle(ExpenseRequest request)
        {
            return true; // CFO can approve any amount
        }

        protected override ExpenseRequest ProcessRequest(ExpenseRequest request)
        {
            request.IsApproved = true;
            request.ApprovedBy = "CFO";
            Console.WriteLine($"  ✓ Approved by CFO (no limit)");
            return request;
        }
    }

    #endregion

    #region Example 2: HTTP Middleware Classes

    public class HttpRequest
    {
        public string Method { get; }
        public string Path { get; }
        public Dictionary<string, string> Headers { get; }

        public HttpRequest(string method, string path, Dictionary<string, string> headers)
        {
            Method = method;
            Path = path;
            Headers = headers;
        }
    }

    public class HttpResponse
    {
        public int StatusCode { get; set; } = 200;
        public string Message { get; set; } = "OK";
    }

    public abstract class HttpMiddleware
    {
        private HttpMiddleware? _next;

        public HttpMiddleware SetNext(HttpMiddleware middleware)
        {
            _next = middleware;
            return middleware;
        }

        public virtual HttpResponse Handle(HttpRequest request)
        {
            var response = ProcessRequest(request);

            // If processing was successful and there's a next middleware, continue
            if (response.StatusCode == 200 && _next != null)
            {
                return _next.Handle(request);
            }

            return response;
        }

        protected abstract HttpResponse ProcessRequest(HttpRequest request);
    }

    public class LoggingMiddleware : HttpMiddleware
    {
        protected override HttpResponse ProcessRequest(HttpRequest request)
        {
            Console.WriteLine($"  [LOG] {DateTime.Now:HH:mm:ss} - {request.Method} {request.Path}");
            return new HttpResponse();
        }
    }

    public class AuthenticationMiddleware : HttpMiddleware
    {
        protected override HttpResponse ProcessRequest(HttpRequest request)
        {
            if (request.Headers.TryGetValue("Authorization", out var token))
            {
                if (token.Contains("valid-token"))
                {
                    Console.WriteLine("  [AUTH] Authentication successful");
                    return new HttpResponse();
                }
                else
                {
                    Console.WriteLine("  [AUTH] Invalid token");
                    return new HttpResponse { StatusCode = 401, Message = "Unauthorized" };
                }
            }

            Console.WriteLine("  [AUTH] No authorization header");
            return new HttpResponse { StatusCode = 401, Message = "Unauthorized" };
        }
    }

    public class CorsMiddleware : HttpMiddleware
    {
        protected override HttpResponse ProcessRequest(HttpRequest request)
        {
            Console.WriteLine("  [CORS] CORS headers added");
            return new HttpResponse();
        }
    }

    public class RateLimitingMiddleware : HttpMiddleware
    {
        private static readonly Dictionary<string, DateTime> _lastRequests = new Dictionary<string, DateTime>();
        private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(1);

        protected override HttpResponse ProcessRequest(HttpRequest request)
        {
            var clientId = "default"; // In real scenario, extract from IP or user ID

            if (_lastRequests.TryGetValue(clientId, out var lastRequest))
            {
                if (DateTime.Now - lastRequest < _rateLimitWindow)
                {
                    Console.WriteLine("  [RATE] Rate limit exceeded");
                    return new HttpResponse { StatusCode = 429, Message = "Too Many Requests" };
                }
            }

            _lastRequests[clientId] = DateTime.Now;
            Console.WriteLine("  [RATE] Rate limit check passed");
            return new HttpResponse();
        }
    }

    public class ControllerMiddleware : HttpMiddleware
    {
        protected override HttpResponse ProcessRequest(HttpRequest request)
        {
            Console.WriteLine($"  [CTRL] Processing {request.Method} {request.Path}");
            return new HttpResponse { StatusCode = 200, Message = "Success" };
        }
    }

    #endregion

    #region Example 3: Support Ticket Classes

    public enum TicketPriority { Low, Medium, High, Critical }
    public enum TicketCategory { Account, Technical, Infrastructure, Development, Security }

    public class SupportTicket
    {
        public int Id { get; }
        public string Description { get; }
        public TicketPriority Priority { get; }
        public TicketCategory Category { get; }
        public string? AssignedTo { get; set; }

        public SupportTicket(int id, string description, TicketPriority priority, TicketCategory category)
        {
            Id = id;
            Description = description;
            Priority = priority;
            Category = category;
        }
    }

    public class Level1SupportHandler : Handler<SupportTicket>
    {
        protected override bool CanHandle(SupportTicket request)
        {
            return request.Category == TicketCategory.Account && request.Priority != TicketPriority.Critical;
        }

        protected override SupportTicket ProcessRequest(SupportTicket request)
        {
            request.AssignedTo = "Level 1 Support";
            Console.WriteLine($"  ✓ Handled by Level 1 Support - Basic account issues");
            return request;
        }
    }

    public class Level2SupportHandler : Handler<SupportTicket>
    {
        protected override bool CanHandle(SupportTicket request)
        {
            return request.Category == TicketCategory.Technical && request.Priority != TicketPriority.Critical;
        }

        protected override SupportTicket ProcessRequest(SupportTicket request)
        {
            request.AssignedTo = "Level 2 Support";
            Console.WriteLine($"  ✓ Handled by Level 2 Support - Technical issues");
            return request;
        }
    }

    public class Level3SupportHandler : Handler<SupportTicket>
    {
        protected override bool CanHandle(SupportTicket request)
        {
            return request.Category == TicketCategory.Infrastructure ||
                   request.Category == TicketCategory.Development;
        }

        protected override SupportTicket ProcessRequest(SupportTicket request)
        {
            request.AssignedTo = "Level 3 Support";
            Console.WriteLine($"  ✓ Handled by Level 3 Support - Infrastructure/Development");
            return request;
        }
    }

    public class EscalationHandler : Handler<SupportTicket>
    {
        protected override bool CanHandle(SupportTicket request)
        {
            return true; // Handles everything else
        }

        protected override SupportTicket ProcessRequest(SupportTicket request)
        {
            request.AssignedTo = "Management Escalation";
            Console.WriteLine($"  ⚠ Escalated to Management - Critical/Security issue");
            return request;
        }
    }

    #endregion

    #region Example 4: Input Validation Classes

    public class ValidationRequest
    {
        public string Value { get; }

        public ValidationRequest(string value)
        {
            Value = value;
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public abstract class Validator : Handler<ValidationRequest>
    {
        protected override ValidationRequest ProcessRequest(ValidationRequest request)
        {
            // Validation passed, continue to next validator
            return request;
        }

        protected override ValidationRequest HandleDefault(ValidationRequest request)
        {
            // End of chain - validation successful
            return request;
        }

        public new ValidationResult Handle(ValidationRequest request)
        {
            try
            {
                base.Handle(request);
                return new ValidationResult { IsValid = true };
            }
            catch (ValidationException ex)
            {
                return new ValidationResult { IsValid = false, ErrorMessage = ex.Message };
            }
        }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class NotNullValidator : Validator
    {
        protected override bool CanHandle(ValidationRequest request)
        {
            return true; // Always check for null
        }

        protected override ValidationRequest ProcessRequest(ValidationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Value))
            {
                throw new ValidationException("Value cannot be null or empty");
            }
            return base.ProcessRequest(request);
        }
    }

    public class LengthValidator : Validator
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public LengthValidator(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override bool CanHandle(ValidationRequest request)
        {
            return !string.IsNullOrWhiteSpace(request.Value);
        }

        protected override ValidationRequest ProcessRequest(ValidationRequest request)
        {
            if (request.Value.Length < _minLength || request.Value.Length > _maxLength)
            {
                throw new ValidationException($"Length must be between {_minLength} and {_maxLength} characters");
            }
            return base.ProcessRequest(request);
        }
    }

    public class EmailFormatValidator : Validator
    {
        protected override bool CanHandle(ValidationRequest request)
        {
            return !string.IsNullOrWhiteSpace(request.Value);
        }

        protected override ValidationRequest ProcessRequest(ValidationRequest request)
        {
            if (!request.Value.Contains("@") || !request.Value.Contains("."))
            {
                throw new ValidationException("Invalid email format");
            }
            return base.ProcessRequest(request);
        }
    }

    public class BlacklistValidator : Validator
    {
        private readonly HashSet<string> _blacklistedDomains = new HashSet<string> { "blocked.com", "spam.com" };

        protected override bool CanHandle(ValidationRequest request)
        {
            return !string.IsNullOrWhiteSpace(request.Value) && request.Value.Contains("@");
        }

        protected override ValidationRequest ProcessRequest(ValidationRequest request)
        {
            var domain = request.Value.Split('@')[1];
            if (_blacklistedDomains.Contains(domain))
            {
                throw new ValidationException("Domain is blacklisted");
            }
            return base.ProcessRequest(request);
        }
    }

    #endregion

    #region Example 5: Thread-Safe Event Processing

    // MULTITHREAD ASPECTS: Thread-safe event processing with concurrent handlers
    public class ProcessingEvent
    {
        public int Id { get; }
        public string Name { get; }
        public DateTime Timestamp { get; }

        public ProcessingEvent(int id, string name, DateTime timestamp)
        {
            Id = id;
            Name = name;
            Timestamp = timestamp;
        }
    }

    public class ThreadSafeEventProcessor
    {
        private readonly ConcurrentQueue<ProcessingEvent> _eventQueue = new ConcurrentQueue<ProcessingEvent>();
        private readonly SemaphoreSlim _processingLock = new SemaphoreSlim(3, 3); // Allow 3 concurrent processors

        public async Task ProcessEventAsync(ProcessingEvent evt)
        {
            await _processingLock.WaitAsync();
            try
            {
                Console.WriteLine($"  [Thread {Thread.CurrentThread.ManagedThreadId}] Processing event {evt.Id}: {evt.Name}");

                // Simulate processing time
                await Task.Delay(Random.Shared.Next(100, 500));

                Console.WriteLine($"  [Thread {Thread.CurrentThread.ManagedThreadId}] Completed event {evt.Id}");
            }
            finally
            {
                _processingLock.Release();
            }
        }
    }

    #endregion

    #region Example 6: Dynamic Chain Configuration

    public class HandlerChainFactory
    {
        public Handler<ExpenseRequest> CreateBasicApprovalChain()
        {
            var teamLead = new TeamLeadApprover();
            var manager = new ManagerApprover();
            var director = new DirectorApprover();

            return teamLead.SetNext(manager).SetNext(director);
        }

        public Handler<ExpenseRequest> CreateStrictApprovalChain()
        {
            var teamLead = new TeamLeadApprover();
            var manager = new ManagerApprover();
            var director = new DirectorApprover();
            var cfo = new CFOApprover();
            var compliance = new ComplianceApprover();

            return teamLead.SetNext(manager).SetNext(director).SetNext(cfo).SetNext(compliance);
        }
    }

    public class ComplianceApprover : Handler<ExpenseRequest>
    {
        protected override bool CanHandle(ExpenseRequest request)
        {
            return request.Amount > 1000; // Compliance check for large amounts
        }

        protected override ExpenseRequest ProcessRequest(ExpenseRequest request)
        {
            Console.WriteLine($"  ✓ Compliance review completed for {request.Description}");
            return request;
        }
    }

    #endregion

    #region Example 7: Async Chain Processing

    public class ProcessingData
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    public class AsyncPreprocessor : AsyncHandler<ProcessingData>
    {
        protected override Task<bool> CanHandleAsync(ProcessingData request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        protected override async Task<ProcessingData> ProcessRequestAsync(ProcessingData request, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("  [ASYNC] Preprocessing data...");
            await Task.Delay(100, cancellationToken);
            request.Metadata["preprocessed"] = true;
            return request;
        }
    }

    public class AsyncValidator : AsyncHandler<ProcessingData>
    {
        protected override Task<bool> CanHandleAsync(ProcessingData request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(request.Metadata.ContainsKey("preprocessed"));
        }

        protected override async Task<ProcessingData> ProcessRequestAsync(ProcessingData request, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("  [ASYNC] Validating data...");
            await Task.Delay(200, cancellationToken);
            request.Metadata["validated"] = true;
            return request;
        }
    }

    public class AsyncDataProcessor : AsyncHandler<ProcessingData>
    {
        protected override Task<bool> CanHandleAsync(ProcessingData request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(request.Metadata.ContainsKey("validated"));
        }

        protected override async Task<ProcessingData> ProcessRequestAsync(ProcessingData request, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("  [ASYNC] Processing data...");
            await Task.Delay(300, cancellationToken);
            request.Metadata["processed"] = true;
            return request;
        }
    }

    public class AsyncPostprocessor : AsyncHandler<ProcessingData>
    {
        protected override Task<bool> CanHandleAsync(ProcessingData request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(request.Metadata.ContainsKey("processed"));
        }

        protected override async Task<ProcessingData> ProcessRequestAsync(ProcessingData request, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("  [ASYNC] Postprocessing data...");
            await Task.Delay(150, cancellationToken);
            request.Metadata["completed"] = true;
            return request;
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. CHAIN STRUCTURE:
 *    - Each handler holds a reference to the next handler in the chain
 *    - Long chains can create deep reference chains in memory
 *    - Consider using weak references for very long chains to prevent memory leaks
 *    - Chain setup typically happens once, so memory overhead is usually minimal
 * 
 * 2. REQUEST OBJECTS:
 *    - Request objects are passed through the entire chain
 *    - Avoid creating large request objects if the chain is frequently used
 *    - Consider using object pooling for high-frequency request processing
 *    - Be mindful of request object lifetime and disposal
 * 
 * 3. HANDLER STATE:
 *    - Stateless handlers are preferred to avoid memory accumulation
 *    - If handlers need state, ensure proper cleanup and memory management
 *    - Singleton handlers can reduce memory footprint but require thread safety
 * 
 * 4. CHAIN RECONFIGURATION:
 *    - Dynamic chain building can create temporary handler objects
 *    - Consider caching common chain configurations
 *    - Be careful with circular references when building chains programmatically
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. HANDLER THREAD SAFETY:
 *    - Handlers should be stateless or thread-safe if shared across threads
 *    - Use concurrent collections if handlers maintain internal state
 *    - Avoid shared mutable state between handlers without proper synchronization
 * 
 * 2. CHAIN MODIFICATION:
 *    - Chain structure modification is NOT thread-safe by default
 *    - Use locks or immutable chains if modification during execution is needed
 *    - Consider building separate chains per thread for high-concurrency scenarios
 * 
 * 3. REQUEST PROCESSING:
 *    - Multiple threads can process different requests through the same chain safely
 *    - if handlers are stateless
 *    - Use ThreadLocal storage if handlers need per-thread state
 * 
 * 4. ASYNC PROCESSING:
 *    - Async handlers allow non-blocking chain processing
 *    - Use ConfigureAwait(false) to avoid context switching overhead
 *    - Handle cancellation tokens properly throughout the chain
 *    - Be careful with exception handling in async chains
 * 
 * 5. PERFORMANCE CONSIDERATIONS:
 *    - Chain traversal has O(n) complexity where n is the number of handlers
 *    - Consider using a dispatch table for very long chains with known patterns
 *    - Profile chain performance under load to identify bottlenecks
 *    - Use early termination where possible to avoid unnecessary processing
 * 
 * 6. BEST PRACTICES:
 *    - Design handlers to be independent and stateless when possible
 *    - Use dependency injection for testability and flexibility
 *    - Implement proper logging and monitoring for chain execution
 *    - Consider using the decorator pattern for cross-cutting concerns
 *    - Handle exceptions gracefully to prevent chain breakage
 *    - Use configuration-driven chain building for flexibility
 */
