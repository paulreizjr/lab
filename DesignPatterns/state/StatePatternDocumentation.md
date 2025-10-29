# State Pattern Class Diagram Documentation

## Overview
This document explains the class diagram for the State Pattern implementation, which includes two main state machines: a Media Player and a Document Workflow system.

## Pattern Structure

### Core State Pattern Components

#### 1. Context Classes
- **MediaPlayer**: The main context for media playback operations
- **DocumentWorkflow**: The context for document lifecycle management

#### 2. State Interfaces
- **IMediaPlayerState**: Defines the contract for media player states
- **IDocumentState**: Defines the contract for document workflow states

#### 3. Context Interfaces
- **IMediaPlayerContext**: Allows states to interact with the media player context
- **IDocumentWorkflowContext**: Allows states to interact with the document context

## Key Design Decisions

### Memory Optimization
- **Singleton Pattern**: All concrete states use the Singleton pattern with `Lazy<T>` for thread-safe initialization
- **Shared State Objects**: Single instances of each state are shared across all contexts
- **Abstract Base Class**: `MediaPlayerStateBase` provides common functionality to reduce code duplication

### Thread Safety
- **Locking Mechanisms**: Both context classes use `object _stateLock` for thread-safe state transitions
- **Lazy Initialization**: `LazyThreadSafetyMode.ExecutionAndPublication` ensures thread-safe singleton creation
- **Immutable States**: State objects don't store mutable context-specific data

### Behavioral Delegation
- **State Methods**: Each state implements specific behavior for operations like Play(), Pause(), Stop()
- **Context Delegation**: Context classes delegate all behavior to the current state object
- **State Transitions**: States are responsible for triggering transitions to other states

## Class Relationships

### Media Player State Machine

```
MediaPlayer (Context)
├── Implements: IMediaPlayerContext
├── Contains: IMediaPlayerState _currentState
├── Delegates to: Current state for all operations
└── Thread Safety: Uses locks for state transitions

MediaPlayerStateBase (Abstract State)
├── Implements: IMediaPlayerState
├── Provides: Default implementations
└── Template methods: OnEnter(), OnExit()

Concrete States (Singleton instances)
├── NoMediaState: Initial state, no media loaded
├── StoppedState: Media loaded but not playing
├── PlayingState: Media currently playing
└── PausedState: Media paused, retains position
```

### Document Workflow State Machine

```
DocumentWorkflow (Context)
├── Implements: IDocumentWorkflowContext
├── Contains: IDocumentState _currentState
├── Business Logic: Role-based access control
└── Thread Safety: Uses locks for state transitions

Concrete Document States
├── DraftState: Author can edit and submit
├── ReviewState: Reviewers can approve or reject
├── ApprovedState: Ready for publishing
├── RejectedState: Needs author revision
└── PublishedState: Final state, no changes allowed
```

## State Transition Rules

### Media Player Transitions
1. **NoMedia → Stopped**: LoadMedia() operation
2. **Stopped → Playing**: Play() operation
3. **Playing → Paused**: Pause() operation
4. **Paused → Playing**: Play() operation (resume)
5. **Playing/Paused → Stopped**: Stop() operation

### Document Workflow Transitions
1. **Draft → Review**: Submit() by author
2. **Review → Approved**: Approve() by reviewer
3. **Review → Rejected**: Reject() by reviewer
4. **Approved → Published**: Publish() operation
5. **Rejected → Draft**: Edit() by author

## Key Features

### 1. Memory Efficiency
- **Single State Instances**: Each state class maintains only one instance
- **Shared Behavior**: Common state functionality in abstract base class
- **Lazy Creation**: States created only when needed

### 2. Thread Safety
- **Atomic Transitions**: State changes are synchronized with locks
- **Immutable States**: States don't store mutable data
- **Concurrent Access**: Multiple threads can safely operate on the same context

### 3. Extensibility
- **Open/Closed Principle**: Easy to add new states without modifying existing code
- **Polymorphism**: States implement common interface but provide specific behavior
- **State Validation**: `CanTransitionTo()` method for transition validation

### 4. Business Logic Enforcement
- **Role-based Access**: Document workflow enforces user permissions
- **State Validation**: Invalid operations are rejected with appropriate messages
- **Audit Trail**: State changes are logged and tracked

## Usage Patterns

### 1. Simple State Machine (Media Player)
```csharp
var player = new MediaPlayer();
player.LoadMedia("song.mp3");  // NoMedia → Stopped
player.Play();                 // Stopped → Playing
player.Pause();                // Playing → Paused
player.Play();                 // Paused → Playing
player.Stop();                 // Playing → Stopped
```

### 2. Complex Workflow (Document)
```csharp
var doc = new DocumentWorkflow("Alice", reviewers);
doc.Edit("content");           // Draft state
doc.Submit();                  // Draft → Review
doc.CurrentUser = "Reviewer";
doc.Approve();                 // Review → Approved
doc.Publish();                 // Approved → Published
```

## Benefits Demonstrated

1. **Eliminates Complex Conditionals**: No large switch/if statements
2. **Clean Separation**: Each state encapsulates its specific behavior
3. **Easy Maintenance**: Adding new states doesn't affect existing code
4. **Thread Safety**: Proper synchronization for concurrent access
5. **Memory Efficiency**: Singleton pattern minimizes memory usage
6. **Business Rule Enforcement**: Complex validation logic in states

## Testing and Validation

The implementation includes:
- **Multithreading Tests**: Concurrent access validation
- **State Transition Tests**: Verification of valid/invalid transitions
- **Business Logic Tests**: Role-based access control validation
- **Memory Usage Analysis**: Singleton pattern effectiveness