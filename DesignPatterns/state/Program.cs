using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/*
IMediaPlayerContext:interface
	+SetState(state: IMediaPlayerState): void
	+NotifyStateChanged(stateName: string): void
	+HasMedia: bool get
	+CurrentTrack: string get set
	+Volume: int get set
	+Position: TimeSpan get set
	+Duration: TimeSpan get

IMediaPlayerState:interface
	+StateName: string get
	+Play(context: IMediaPlayerContext): void
	+Pause(context: IMediaPlayerContext): void
	+Stop(context: IMediaPlayerContext): void
	+LoadMedia(context: IMediaPlayerContext, trackName: string): void
	+SetVolume(context: IMediaPlayerContext, volume: int): void
	+OnEnter(context: IMediaPlayerContext): void
	+OnExit(context: IMediaPlayerContext): void

MediaPlayerStateBase:abstract
	+abstract StateName: string get
	+virtual Play(context: IMediaPlayerContext): void
	+virtual Pause(context: IMediaPlayerContext): void
	+virtual Stop(context: IMediaPlayerContext): void
	+virtual LoadMedia(context: IMediaPlayerContext, trackName: string): void
	+virtual SetVolume(context: IMediaPlayerContext, volume: int): void
	+virtual OnEnter(context: IMediaPlayerContext): void
	+virtual OnExit(context: IMediaPlayerContext): void

MediaPlayer 
	-_currentState: IMediaPlayerState
	-_stateLock: object
	+CurrentTrack: string get set
	+Volume: int get set
	+Position: TimeSpan get set
	+Duration: TimeSpan get
	+HasMedia: bool get
	+StateChanged: Action<string, string>
	+MediaPlayer()
	+SetState(newState: IMediaPlayerState): void
	+NotifyStateChanged(stateName: string): void
	+Play(): void
	+Pause(): void
	+Stop(): void
	+LoadMedia(trackName: string): void
	+SetVolume(volume: int): void
	+GetCurrentState(): string
	+DisplayStatus(): void

NoMediaState 
	-static _instance: Lazy<NoMediaState>
	+static Instance: NoMediaState get
	-NoMediaState()
	+StateName: string get
	+Play(context: IMediaPlayerContext): void
	+Pause(context: IMediaPlayerContext): void
	+Stop(context: IMediaPlayerContext): void
	+LoadMedia(context: IMediaPlayerContext, trackName: string): void

StoppedState 
	-static _instance: Lazy<StoppedState>
	+static Instance: StoppedState get
	-StoppedState()
	+StateName: string get
	+Play(context: IMediaPlayerContext): void
	+Stop(context: IMediaPlayerContext): void
	+OnEnter(context: IMediaPlayerContext): void

PlayingState 
	-static _instance: Lazy<PlayingState>
	+static Instance: PlayingState get
	-PlayingState()
	+StateName: string get
	+Play(context: IMediaPlayerContext): void
	+Pause(context: IMediaPlayerContext): void
	+Stop(context: IMediaPlayerContext): void
	+OnEnter(context: IMediaPlayerContext): void

PausedState 
	-static _instance: Lazy<PausedState>
	+static Instance: PausedState get
	-PausedState()
	+StateName: string get
	+Play(context: IMediaPlayerContext): void
	+Pause(context: IMediaPlayerContext): void
	+Stop(context: IMediaPlayerContext): void
	+OnEnter(context: IMediaPlayerContext): void

MediaPlayerStateBase-.-*>IMediaPlayerState
NoMediaState--*>MediaPlayerStateBase
StoppedState--*>MediaPlayerStateBase
PlayingState--*>MediaPlayerStateBase
PausedState--*>MediaPlayerStateBase
MediaPlayer-.-*>IMediaPlayerContext
MediaPlayer-.->IMediaPlayerState
*/

/*
IDocumentWorkflowContext:interface
	+SetState(state: IDocumentState): void
	+Author: string get
	+CurrentUser: string get set
	+Reviewers: List<string> get
	+LastModified: DateTime get set
	+Content: string get set

IDocumentState:interface
	+StateName: string get
	+Edit(context: IDocumentWorkflowContext, newContent: string): void
	+Submit(context: IDocumentWorkflowContext): void
	+Approve(context: IDocumentWorkflowContext): void
	+Reject(context: IDocumentWorkflowContext, reason: string): void
	+Publish(context: IDocumentWorkflowContext): void
	+CanTransitionTo(targetState: string, user: string): bool

DocumentWorkflow 
	-_currentState: IDocumentState
	-_stateLock: object
	+Author: string get
	+CurrentUser: string get set
	+Reviewers: List<string> get
	+LastModified: DateTime get set
	+Content: string get set
	+DocumentWorkflow(author: string, reviewers: List<string>)
	+SetState(newState: IDocumentState): void
	+Edit(newContent: string): void
	+Submit(): void
	+Approve(): void
	+Reject(reason: string): void
	+Publish(): void
	+GetCurrentState(): string
	+DisplayStatus(): void

DraftState 
	+StateName: string get
	+Edit(context: IDocumentWorkflowContext, newContent: string): void
	+Submit(context: IDocumentWorkflowContext): void
	+Approve(context: IDocumentWorkflowContext): void
	+Reject(context: IDocumentWorkflowContext, reason: string): void
	+Publish(context: IDocumentWorkflowContext): void
	+CanTransitionTo(targetState: string, user: string): bool

ReviewState 
	+StateName: string get
	+Edit(context: IDocumentWorkflowContext, newContent: string): void
	+Submit(context: IDocumentWorkflowContext): void
	+Approve(context: IDocumentWorkflowContext): void
	+Reject(context: IDocumentWorkflowContext, reason: string): void
	+Publish(context: IDocumentWorkflowContext): void
	+CanTransitionTo(targetState: string, user: string): bool

ApprovedState 
	+StateName: string get
	+Edit(context: IDocumentWorkflowContext, newContent: string): void
	+Submit(context: IDocumentWorkflowContext): void
	+Approve(context: IDocumentWorkflowContext): void
	+Reject(context: IDocumentWorkflowContext, reason: string): void
	+Publish(context: IDocumentWorkflowContext): void
	+CanTransitionTo(targetState: string, user: string): bool

RejectedState 
	+StateName: string get
	+Edit(context: IDocumentWorkflowContext, newContent: string): void
	+Submit(context: IDocumentWorkflowContext): void
	+Approve(context: IDocumentWorkflowContext): void
	+Reject(context: IDocumentWorkflowContext, reason: string): void
	+Publish(context: IDocumentWorkflowContext): void
	+CanTransitionTo(targetState: string, user: string): bool

PublishedState 
	+StateName: string get
	+Edit(context: IDocumentWorkflowContext, newContent: string): void
	+Submit(context: IDocumentWorkflowContext): void
	+Approve(context: IDocumentWorkflowContext): void
	+Reject(context: IDocumentWorkflowContext, reason: string): void
	+Publish(context: IDocumentWorkflowContext): void
	+CanTransitionTo(targetState: string, user: string): bool
	
DocumentWorkflow-.-*>IDocumentWorkflowContext
DocumentWorkflow-->IDocumentState
DraftState-.-*>IDocumentState
ReviewState-.-*>IDocumentState
ApprovedState-.-*>IDocumentState
RejectedState-.-*>IDocumentState
PublishedState-.-*>IDocumentState
*/

/*
 * STATE DESIGN PATTERN EXAMPLE
 * 
 * PURPOSE:
 * The State pattern allows an object to alter its behavior when its internal state changes.
 * It appears as if the object changed its class by delegating behavior to state objects.
 * This pattern eliminates complex conditional statements and makes state transitions explicit.
 * 
 * MEMORY ALLOCATION CONSIDERATIONS:
 * - State objects can be shared (flyweight) if they don't store context-specific data
 * - Singleton state instances reduce memory overhead for stateless behaviors
 * - Context switching may create temporary objects during transitions
 * - State history tracking can consume memory if not managed properly
 * - Lazy state initialization can defer memory allocation until needed
 * 
 * SCENARIOS TO USE:
 * - Objects with complex state-dependent behavior (state machines, workflows)
 * - Eliminating large conditional statements based on object state
 * - State transitions with specific rules and validations
 * - UI components with different modes (editing, viewing, disabled)
 * - Game entities with different behaviors per state (idle, moving, attacking)
 * - Network protocols with connection states
 * - Document workflows (draft, review, approved, published)
 * 
 * SCENARIOS NOT TO USE:
 * - Simple objects with few states or minimal state-dependent behavior
 * - When state transitions are rare or trivial
 * - Performance-critical code where state lookup overhead is unacceptable
 * - When the number of states and transitions is small and unlikely to grow
 * - Objects where state is purely data without behavioral differences
 * 
 * MULTITHREADING ASPECTS:
 * - State transitions must be atomic to prevent race conditions
 * - Shared state objects require thread-safety if they maintain any data
 * - Context object state changes need synchronization in concurrent environments
 * - State transition notifications may need thread-safe event handling
 * - Immutable state objects are inherently thread-safe
 */

namespace StatePattern
{
    // Context interface that state objects can use to interact with the context
    public interface IMediaPlayerContext
    {
        void SetState(IMediaPlayerState state);
        void NotifyStateChanged(string stateName);
        bool HasMedia { get; }
        string CurrentTrack { get; set; }
        int Volume { get; set; }
        TimeSpan Position { get; set; }
        TimeSpan Duration { get; }
    }

    // State interface defining the contract for all concrete states
    public interface IMediaPlayerState
    {
        // State behavior methods - these will behave differently based on current state
        void Play(IMediaPlayerContext context);
        void Pause(IMediaPlayerContext context);
        void Stop(IMediaPlayerContext context);
        void LoadMedia(IMediaPlayerContext context, string trackName);
        void SetVolume(IMediaPlayerContext context, int volume);

        // State identification
        string StateName { get; }

        // Optional: State entry/exit methods for cleanup or initialization
        void OnEnter(IMediaPlayerContext context);
        void OnExit(IMediaPlayerContext context);
    }

    // Abstract base state with common functionality
    // MEMORY EFFICIENCY: Provides shared implementation to reduce code duplication
    public abstract class MediaPlayerStateBase : IMediaPlayerState
    {
        public abstract string StateName { get; }

        // Default implementations that can be overridden
        public virtual void Play(IMediaPlayerContext context)
        {
            Console.WriteLine($"[{StateName}] Play operation not supported in current state");
        }

        public virtual void Pause(IMediaPlayerContext context)
        {
            Console.WriteLine($"[{StateName}] Pause operation not supported in current state");
        }

        public virtual void Stop(IMediaPlayerContext context)
        {
            Console.WriteLine($"[{StateName}] Stop operation not supported in current state");
        }

        public virtual void LoadMedia(IMediaPlayerContext context, string trackName)
        {
            Console.WriteLine($"[{StateName}] Loading media: {trackName}");
            context.CurrentTrack = trackName;
            context.Position = TimeSpan.Zero;

            // Transition to Stopped state after loading
            context.SetState(StoppedState.Instance);
        }

        public virtual void SetVolume(IMediaPlayerContext context, int volume)
        {
            if (volume < 0 || volume > 100)
            {
                Console.WriteLine($"[{StateName}] Invalid volume: {volume}. Must be between 0-100");
                return;
            }

            context.Volume = volume;
            Console.WriteLine($"[{StateName}] Volume set to: {volume}%");
        }

        // Template methods for state transitions
        public virtual void OnEnter(IMediaPlayerContext context)
        {
            Console.WriteLine($"[STATE TRANSITION] Entering {StateName} state");
        }

        public virtual void OnExit(IMediaPlayerContext context)
        {
            Console.WriteLine($"[STATE TRANSITION] Exiting {StateName} state");
        }
    }

    // Concrete State: No Media Loaded
    // THREAD SAFETY: Singleton pattern with thread-safe lazy initialization
    public sealed class NoMediaState : MediaPlayerStateBase
    {
        // Singleton instance to reduce memory allocation
        // MEMORY OPTIMIZATION: Single instance shared across all contexts
        private static readonly Lazy<NoMediaState> _instance =
            new Lazy<NoMediaState>(() => new NoMediaState(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static NoMediaState Instance => _instance.Value;

        private NoMediaState() { } // Private constructor for singleton

        public override string StateName => "NoMedia";

        public override void Play(IMediaPlayerContext context)
        {
            Console.WriteLine("[NoMedia] Cannot play - no media loaded");
        }

        public override void Pause(IMediaPlayerContext context)
        {
            Console.WriteLine("[NoMedia] Cannot pause - no media loaded");
        }

        public override void Stop(IMediaPlayerContext context)
        {
            Console.WriteLine("[NoMedia] Cannot stop - no media loaded");
        }

        public override void LoadMedia(IMediaPlayerContext context, string trackName)
        {
            Console.WriteLine($"[NoMedia] Loading first media: {trackName}");
            base.LoadMedia(context, trackName); // Use base implementation
        }
    }

    // Concrete State: Media Stopped
    public sealed class StoppedState : MediaPlayerStateBase
    {
        private static readonly Lazy<StoppedState> _instance =
            new Lazy<StoppedState>(() => new StoppedState(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static StoppedState Instance => _instance.Value;
        private StoppedState() { }

        public override string StateName => "Stopped";

        public override void Play(IMediaPlayerContext context)
        {
            if (!context.HasMedia)
            {
                Console.WriteLine("[Stopped] Cannot play - no media loaded");
                context.SetState(NoMediaState.Instance);
                return;
            }

            Console.WriteLine($"[Stopped] Starting playback of: {context.CurrentTrack}");
            context.SetState(PlayingState.Instance);
        }

        public override void Stop(IMediaPlayerContext context)
        {
            Console.WriteLine("[Stopped] Already stopped");
        }

        public override void OnEnter(IMediaPlayerContext context)
        {
            base.OnEnter(context);
            context.Position = TimeSpan.Zero; // Reset position when entering stopped state
        }
    }

    // Concrete State: Media Playing
    public sealed class PlayingState : MediaPlayerStateBase
    {
        private static readonly Lazy<PlayingState> _instance =
            new Lazy<PlayingState>(() => new PlayingState(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static PlayingState Instance => _instance.Value;
        private PlayingState() { }

        public override string StateName => "Playing";

        public override void Play(IMediaPlayerContext context)
        {
            Console.WriteLine("[Playing] Already playing");
        }

        public override void Pause(IMediaPlayerContext context)
        {
            Console.WriteLine($"[Playing] Pausing playback of: {context.CurrentTrack}");
            context.SetState(PausedState.Instance);
        }

        public override void Stop(IMediaPlayerContext context)
        {
            Console.WriteLine($"[Playing] Stopping playback of: {context.CurrentTrack}");
            context.SetState(StoppedState.Instance);
        }

        public override void OnEnter(IMediaPlayerContext context)
        {
            base.OnEnter(context);
            Console.WriteLine($"[Playing] Now playing: {context.CurrentTrack} at volume {context.Volume}%");
        }
    }

    // Concrete State: Media Paused
    public sealed class PausedState : MediaPlayerStateBase
    {
        private static readonly Lazy<PausedState> _instance =
            new Lazy<PausedState>(() => new PausedState(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static PausedState Instance => _instance.Value;
        private PausedState() { }

        public override string StateName => "Paused";

        public override void Play(IMediaPlayerContext context)
        {
            Console.WriteLine($"[Paused] Resuming playback of: {context.CurrentTrack}");
            context.SetState(PlayingState.Instance);
        }

        public override void Pause(IMediaPlayerContext context)
        {
            Console.WriteLine("[Paused] Already paused");
        }

        public override void Stop(IMediaPlayerContext context)
        {
            Console.WriteLine($"[Paused] Stopping playback of: {context.CurrentTrack}");
            context.SetState(StoppedState.Instance);
        }

        public override void OnEnter(IMediaPlayerContext context)
        {
            base.OnEnter(context);
            Console.WriteLine($"[Paused] Playback paused at position: {context.Position}");
        }
    }

    // Context class that maintains current state and delegates behavior
    // THREAD SAFETY: Uses locks for state transitions in multithreaded scenarios
    public class MediaPlayer : IMediaPlayerContext
    {
        private IMediaPlayerState _currentState;
        private readonly object _stateLock = new object(); // For thread-safe state transitions

        // Context data
        public string CurrentTrack { get; set; } = string.Empty;
        public int Volume { get; set; } = 50;
        public TimeSpan Position { get; set; } = TimeSpan.Zero;
        public TimeSpan Duration { get; private set; } = TimeSpan.FromMinutes(3); // Mock duration

        public bool HasMedia => !string.IsNullOrEmpty(CurrentTrack);

        // Event for state change notifications
        public event Action<string, string>? StateChanged; // oldState, newState

        public MediaPlayer()
        {
            // Initialize with no media state
            _currentState = NoMediaState.Instance;
            Console.WriteLine("[MediaPlayer] Created with NoMedia state");
        }

        // Thread-safe state transition method
        // THREAD SAFETY: Synchronized state changes to prevent race conditions
        public void SetState(IMediaPlayerState newState)
        {
            if (newState == null) throw new ArgumentNullException(nameof(newState));

            lock (_stateLock)
            {
                if (_currentState == newState)
                {
                    Console.WriteLine($"[MediaPlayer] Already in {newState.StateName} state");
                    return;
                }

                var oldStateName = _currentState.StateName;

                // Call exit method on current state
                _currentState.OnExit(this);

                // Change state
                _currentState = newState;

                // Call enter method on new state
                _currentState.OnEnter(this);

                // Notify observers
                NotifyStateChanged(newState.StateName);
                StateChanged?.Invoke(oldStateName, newState.StateName);
            }
        }

        public void NotifyStateChanged(string stateName)
        {
            Console.WriteLine($"[MediaPlayer] State changed to: {stateName}");
        }

        // Public interface methods that delegate to current state
        public void Play()
        {
            lock (_stateLock)
            {
                _currentState.Play(this);
            }
        }

        public void Pause()
        {
            lock (_stateLock)
            {
                _currentState.Pause(this);
            }
        }

        public void Stop()
        {
            lock (_stateLock)
            {
                _currentState.Stop(this);
            }
        }

        public void LoadMedia(string trackName)
        {
            lock (_stateLock)
            {
                _currentState.LoadMedia(this, trackName);
            }
        }

        public void SetVolume(int volume)
        {
            lock (_stateLock)
            {
                _currentState.SetVolume(this, volume);
            }
        }

        // Method to get current state information
        public string GetCurrentState()
        {
            lock (_stateLock)
            {
                return _currentState.StateName;
            }
        }

        // Method to display current player status
        public void DisplayStatus()
        {
            lock (_stateLock)
            {
                Console.WriteLine($"\n[STATUS] Current State: {_currentState.StateName}");
                Console.WriteLine($"[STATUS] Current Track: {(HasMedia ? CurrentTrack : "None")}");
                Console.WriteLine($"[STATUS] Volume: {Volume}%");
                Console.WriteLine($"[STATUS] Position: {Position:mm\\:ss}/{Duration:mm\\:ss}");
            }
        }
    }

    // Advanced example: Document Workflow State Machine
    // Demonstrates more complex state transitions with validation
    public interface IDocumentWorkflowContext
    {
        void SetState(IDocumentState state);
        string Author { get; }
        string CurrentUser { get; set; }
        List<string> Reviewers { get; }
        DateTime LastModified { get; set; }
        string Content { get; set; }
    }

    public interface IDocumentState
    {
        string StateName { get; }
        void Edit(IDocumentWorkflowContext context, string newContent);
        void Submit(IDocumentWorkflowContext context);
        void Approve(IDocumentWorkflowContext context);
        void Reject(IDocumentWorkflowContext context, string reason);
        void Publish(IDocumentWorkflowContext context);
        bool CanTransitionTo(string targetState, string user);
    }

    // Document states with complex business logic
    public class DraftState : IDocumentState
    {
        public string StateName => "Draft";

        public void Edit(IDocumentWorkflowContext context, string newContent)
        {
            if (context.CurrentUser != context.Author)
            {
                Console.WriteLine("[Draft] Only author can edit draft");
                return;
            }

            context.Content = newContent;
            context.LastModified = DateTime.Now;
            Console.WriteLine("[Draft] Document edited");
        }

        public void Submit(IDocumentWorkflowContext context)
        {
            if (context.CurrentUser != context.Author)
            {
                Console.WriteLine("[Draft] Only author can submit draft");
                return;
            }

            Console.WriteLine("[Draft] Submitting for review");
            context.SetState(new ReviewState());
        }

        public void Approve(IDocumentWorkflowContext context)
            => Console.WriteLine("[Draft] Cannot approve draft directly");

        public void Reject(IDocumentWorkflowContext context, string reason)
            => Console.WriteLine("[Draft] Cannot reject draft");

        public void Publish(IDocumentWorkflowContext context)
            => Console.WriteLine("[Draft] Cannot publish draft directly");

        public bool CanTransitionTo(string targetState, string user)
            => targetState == "Review";
    }

    public class ReviewState : IDocumentState
    {
        public string StateName => "Review";

        public void Edit(IDocumentWorkflowContext context, string newContent)
            => Console.WriteLine("[Review] Cannot edit during review");

        public void Submit(IDocumentWorkflowContext context)
            => Console.WriteLine("[Review] Already submitted for review");

        public void Approve(IDocumentWorkflowContext context)
        {
            if (!context.Reviewers.Contains(context.CurrentUser))
            {
                Console.WriteLine("[Review] Only reviewers can approve");
                return;
            }

            Console.WriteLine("[Review] Document approved");
            context.SetState(new ApprovedState());
        }

        public void Reject(IDocumentWorkflowContext context, string reason)
        {
            if (!context.Reviewers.Contains(context.CurrentUser))
            {
                Console.WriteLine("[Review] Only reviewers can reject");
                return;
            }

            Console.WriteLine($"[Review] Document rejected: {reason}");
            context.SetState(new RejectedState());
        }

        public void Publish(IDocumentWorkflowContext context)
            => Console.WriteLine("[Review] Cannot publish during review");

        public bool CanTransitionTo(string targetState, string user)
        {
            // For ReviewState, we need to pass the context to check reviewers
            // This is a limitation of this interface design - in a real implementation,
            // you might pass context or redesign the interface
            return targetState == "Approved" || targetState == "Rejected";
        }
    }

    public class ApprovedState : IDocumentState
    {
        public string StateName => "Approved";

        public void Edit(IDocumentWorkflowContext context, string newContent)
            => Console.WriteLine("[Approved] Cannot edit approved document");

        public void Submit(IDocumentWorkflowContext context)
            => Console.WriteLine("[Approved] Already approved");

        public void Approve(IDocumentWorkflowContext context)
            => Console.WriteLine("[Approved] Already approved");

        public void Reject(IDocumentWorkflowContext context, string reason)
            => Console.WriteLine("[Approved] Cannot reject approved document");

        public void Publish(IDocumentWorkflowContext context)
        {
            Console.WriteLine("[Approved] Publishing document");
            context.SetState(new PublishedState());
        }

        public bool CanTransitionTo(string targetState, string user)
            => targetState == "Published";
    }

    public class RejectedState : IDocumentState
    {
        public string StateName => "Rejected";

        public void Edit(IDocumentWorkflowContext context, string newContent)
        {
            if (context.CurrentUser != context.Author)
            {
                Console.WriteLine("[Rejected] Only author can edit rejected document");
                return;
            }

            context.Content = newContent;
            context.LastModified = DateTime.Now;
            Console.WriteLine("[Rejected] Document edited, returning to draft");
            context.SetState(new DraftState());
        }

        public void Submit(IDocumentWorkflowContext context)
            => Console.WriteLine("[Rejected] Edit document first before resubmitting");

        public void Approve(IDocumentWorkflowContext context)
            => Console.WriteLine("[Rejected] Cannot approve rejected document");

        public void Reject(IDocumentWorkflowContext context, string reason)
            => Console.WriteLine("[Rejected] Already rejected");

        public void Publish(IDocumentWorkflowContext context)
            => Console.WriteLine("[Rejected] Cannot publish rejected document");

        public bool CanTransitionTo(string targetState, string user)
        {
            // For RejectedState, only author can transition back to draft
            // In a real implementation, you'd pass context or store author info
            return targetState == "Draft";
        }
    }

    public class PublishedState : IDocumentState
    {
        public string StateName => "Published";

        public void Edit(IDocumentWorkflowContext context, string newContent)
            => Console.WriteLine("[Published] Cannot edit published document");

        public void Submit(IDocumentWorkflowContext context)
            => Console.WriteLine("[Published] Already published");

        public void Approve(IDocumentWorkflowContext context)
            => Console.WriteLine("[Published] Already published");

        public void Reject(IDocumentWorkflowContext context, string reason)
            => Console.WriteLine("[Published] Cannot reject published document");

        public void Publish(IDocumentWorkflowContext context)
            => Console.WriteLine("[Published] Already published");

        public bool CanTransitionTo(string targetState, string user)
            => false; // No transitions from published state
    }

    public class DocumentWorkflow : IDocumentWorkflowContext
    {
        private IDocumentState _currentState;
        private readonly object _stateLock = new object();

        public string Author { get; private set; }
        public string CurrentUser { get; set; }
        public List<string> Reviewers { get; private set; }
        public DateTime LastModified { get; set; }
        public string Content { get; set; } = string.Empty;

        public DocumentWorkflow(string author, List<string> reviewers)
        {
            Author = author;
            CurrentUser = author;
            Reviewers = reviewers ?? new List<string>();
            _currentState = new DraftState();
            LastModified = DateTime.Now;

            Console.WriteLine($"[DocumentWorkflow] Created by {author} with reviewers: {string.Join(", ", Reviewers)}");
        }

        public void SetState(IDocumentState newState)
        {
            lock (_stateLock)
            {
                var oldState = _currentState.StateName;
                _currentState = newState;
                Console.WriteLine($"[DocumentWorkflow] State changed from {oldState} to {newState.StateName}");
            }
        }

        // Public interface methods
        public void Edit(string newContent)
        {
            lock (_stateLock)
            {
                _currentState.Edit(this, newContent);
            }
        }

        public void Submit()
        {
            lock (_stateLock)
            {
                _currentState.Submit(this);
            }
        }

        public void Approve()
        {
            lock (_stateLock)
            {
                _currentState.Approve(this);
            }
        }

        public void Reject(string reason)
        {
            lock (_stateLock)
            {
                _currentState.Reject(this, reason);
            }
        }

        public void Publish()
        {
            lock (_stateLock)
            {
                _currentState.Publish(this);
            }
        }

        public string GetCurrentState()
        {
            lock (_stateLock)
            {
                return _currentState.StateName;
            }
        }

        public void DisplayStatus()
        {
            lock (_stateLock)
            {
                Console.WriteLine($"\n[DOCUMENT STATUS]");
                Console.WriteLine($"State: {_currentState.StateName}");
                Console.WriteLine($"Author: {Author}");
                Console.WriteLine($"Current User: {CurrentUser}");
                Console.WriteLine($"Last Modified: {LastModified:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"Content Length: {Content.Length} characters");
            }
        }
    }

    // Multithreading demonstration for state pattern
    public class MultithreadingDemo
    {
        public static async Task RunConcurrentMediaPlayerTest()
        {
            Console.WriteLine("\n[MULTITHREAD TEST] Testing concurrent media player operations...");

            var player = new MediaPlayer();
            player.LoadMedia("Concurrent Test Track");

            // Create tasks that perform different operations concurrently
            var tasks = new Task[]
            {
                Task.Run(() => {
                    for (int i = 0; i < 5; i++)
                    {
                        player.Play();
                        Thread.Sleep(100);
                        player.Pause();
                        Thread.Sleep(100);
                    }
                }),

                Task.Run(() => {
                    for (int i = 0; i < 10; i++)
                    {
                        player.SetVolume(i * 10);
                        Thread.Sleep(50);
                    }
                }),

                Task.Run(() => {
                    Thread.Sleep(250);
                    player.Stop();
                    Thread.Sleep(250);
                    player.Play();
                })
            };

            await Task.WhenAll(tasks);

            player.DisplayStatus();
            Console.WriteLine("[MULTITHREAD TEST] Concurrent operations completed successfully");
        }

        public static async Task RunConcurrentDocumentWorkflowTest()
        {
            Console.WriteLine("\n[MULTITHREAD TEST] Testing concurrent document workflow...");

            var document = new DocumentWorkflow("Alice", new List<string> { "Bob", "Charlie" });
            document.Edit("Initial content");

            var tasks = new Task[]
            {
                Task.Run(() => {
                    document.CurrentUser = "Alice";
                    document.Submit();
                }),

                Task.Run(() => {
                    Thread.Sleep(100);
                    document.CurrentUser = "Bob";
                    document.Approve();
                }),

                Task.Run(() => {
                    Thread.Sleep(200);
                    document.CurrentUser = "Alice";
                    document.Publish();
                })
            };

            await Task.WhenAll(tasks);

            document.DisplayStatus();
            Console.WriteLine("[MULTITHREAD TEST] Concurrent workflow operations completed");
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== STATE PATTERN DEMONSTRATION ===\n");

            // Example 1: Basic Media Player State Machine
            Console.WriteLine("1. BASIC MEDIA PLAYER STATE MACHINE:");
            Console.WriteLine(new string('-', 60));

            var player = new MediaPlayer();

            // Subscribe to state change events
            player.StateChanged += (oldState, newState) =>
                Console.WriteLine($"[EVENT] State transition: {oldState} -> {newState}");

            Console.WriteLine("\nTrying to play without media:");
            player.Play();
            player.DisplayStatus();

            Console.WriteLine("\nLoading media and testing state transitions:");
            player.LoadMedia("Song.mp3");
            player.DisplayStatus();

            Console.WriteLine("\nPlay -> Pause -> Play -> Stop sequence:");
            player.Play();
            player.Pause();
            player.Play();
            player.Stop();
            player.DisplayStatus();

            Console.WriteLine("\nTesting volume control in different states:");
            player.SetVolume(75);
            player.Play();
            player.SetVolume(25);
            player.DisplayStatus();

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // Example 2: Document Workflow State Machine
            Console.WriteLine("\n\n2. DOCUMENT WORKFLOW STATE MACHINE:");
            Console.WriteLine(new string('-', 60));

            var document = new DocumentWorkflow("Alice", new List<string> { "Bob", "Charlie" });

            Console.WriteLine("\nAuthor creates and edits document:");
            document.Edit("This is the initial draft content.");
            document.DisplayStatus();

            Console.WriteLine("\nAuthor submits for review:");
            document.Submit();
            document.DisplayStatus();

            Console.WriteLine("\nReviewer approves document:");
            document.CurrentUser = "Bob";
            document.Approve();
            document.DisplayStatus();

            Console.WriteLine("\nDocument gets published:");
            document.CurrentUser = "Alice";
            document.Publish();
            document.DisplayStatus();

            Console.WriteLine("\nTrying invalid operations:");
            document.Edit("Cannot edit published document");
            document.Reject("Cannot reject published document");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // Example 3: Rejected Document Flow
            Console.WriteLine("\n\n3. REJECTED DOCUMENT WORKFLOW:");
            Console.WriteLine(new string('-', 60));

            var rejectedDoc = new DocumentWorkflow("Dave", new List<string> { "Eve" });
            rejectedDoc.Edit("Initial content");
            rejectedDoc.Submit();

            Console.WriteLine("\nReviewer rejects document:");
            rejectedDoc.CurrentUser = "Eve";
            rejectedDoc.Reject("Content needs improvement");
            rejectedDoc.DisplayStatus();

            Console.WriteLine("\nAuthor edits and resubmits:");
            rejectedDoc.CurrentUser = "Dave";
            rejectedDoc.Edit("Improved content after feedback");
            rejectedDoc.Submit();
            rejectedDoc.DisplayStatus();

            Console.WriteLine("\nPress any key to continue to multithreading demo...");
            Console.ReadKey();

            // Example 4: Multithreading demonstrations
            Console.WriteLine("\n\n4. MULTITHREADING DEMONSTRATIONS:");
            Console.WriteLine(new string('-', 60));

            await MultithreadingDemo.RunConcurrentMediaPlayerTest();
            await MultithreadingDemo.RunConcurrentDocumentWorkflowTest();

            // Example 5: Memory and performance analysis
            Console.WriteLine("\n\n5. MEMORY AND PERFORMANCE ANALYSIS:");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("STATE PATTERN MEMORY CHARACTERISTICS:");
            Console.WriteLine("- Singleton state instances reduce memory overhead");
            Console.WriteLine("- Shared state objects minimize object creation");
            Console.WriteLine("- Context switching has minimal memory impact");
            Console.WriteLine("- Thread-safe lazy initialization prevents race conditions");
            Console.WriteLine("\nPERFORMANCE CONSIDERATIONS:");
            Console.WriteLine("- State lookup is O(1) with direct references");
            Console.WriteLine("- State transitions require synchronization in multithreaded scenarios");
            Console.WriteLine("- Eliminates complex conditional logic improving maintainability");
            Console.WriteLine("- Method delegation adds minimal overhead compared to switch statements");

            Console.WriteLine("\nBENEFITS DEMONSTRATED:");
            Console.WriteLine("- Clean separation of state-specific behavior");
            Console.WriteLine("- Easy to add new states without modifying existing code");
            Console.WriteLine("- Thread-safe state transitions with proper synchronization");
            Console.WriteLine("- Explicit state transition validation and business rules");

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
