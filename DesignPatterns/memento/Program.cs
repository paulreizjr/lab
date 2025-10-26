using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using System.IO;

/*
IMemento
	Timestamp
	Description

IOriginator<IMemento>
	CreateMemento()
	RestoreMemento(T memento)

MementoCaretaker<IMemento>
	-_mementos
	-_currentIndex
	-_maxSize
	+MementoCaretaker(int maxSize = 50)
	+Save(T memento)
	+Undo()
	+Redo()
	+CanUndo()
	+CanRedo()
	+Count()

TextEditorMemento
	+Text
	+CursorPosition
	+Timestamp
	+Description

TextEditor
	+Write(string text)
	+SetText(string text)
	+GetText()
	+CreateMemento()
	+RestoreMemento(TextEditorMemento memento)

EditorCaretaker
	-_caretaker
	+Save(TextEditorMemento memento)
	+Undo()
	+Redo()

TextEditorMemento--*>IMemento
TextEditor--*>IOriginator
EditorCaretaker-Save()/Undo()/Redo()->MementoCaretaker
EditorCaretaker-.->TextEditor
*/

/*
In the memento pattern you are abstracting the state of an object into a separate memento object.
Instead of the originator object managing its own state history, the memento object captures the state
and allows it to be restored later.
*/

/*
 * MEMENTO DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Memento pattern captures and externalizes an object's internal state without violating
 * encapsulation, so that the object can be restored to this state later. It provides a way
 * to implement undo/redo functionality by taking snapshots of an object's state at different
 * points in time and allowing restoration to any of those states.
 * 
 * CORE BENEFITS:
 * - Preserves encapsulation boundaries while allowing state capture
 * - Enables undo/redo functionality without exposing internal object structure
 * - Provides a clean way to implement checkpointing and rollback mechanisms
 * - Simplifies the originator by externalizing state management concerns
 * - Allows multiple snapshots without cluttering the originator class
 * 
 * SCENARIOS TO USE:
 * - Implementing undo/redo functionality in text editors, graphics applications
 * - Creating save/restore points in games or simulations
 * - Building transaction systems with rollback capabilities
 * - Implementing checkpoint mechanisms in long-running processes
 * - Creating versioning systems for document management
 * - Building debugging tools that need to capture program state
 * - Implementing wizard-like interfaces with step navigation
 * - Creating backup and recovery systems for configuration data
 * - Building state machines that need to revert to previous states
 * - Implementing audit trails with state reconstruction capabilities
 * 
 * SCENARIOS NOT TO USE:
 * - When objects have very large state that's expensive to copy
 * - If the object's state changes very frequently (creates too many mementos)
 * - When the cost of creating mementos exceeds the benefit of undo functionality
 * - If the object's internal state is simple and can be managed directly
 * - When memory constraints are tight and memento storage is problematic
 * - If the object's state contains non-serializable resources (file handles, network connections)
 * - When real-time performance is critical and memento creation causes delays
 * - If the application doesn't need undo/redo or state restoration functionality
 */

namespace MementoPattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Memento Design Pattern Examples ===\n");

            // Example 1: Basic Text Editor with Undo/Redo
            TextEditorExample();

            // Example 2: Game State Management
            GameStateExample();

            // Example 3: Configuration Manager with Snapshots
            ConfigurationManagerExample();

            // Example 4: Drawing Application with Canvas States
            DrawingApplicationExample();

            // Example 5: Thread-Safe Memento Management
            await ThreadSafeMementoExample();

            // Example 6: Memory-Optimized Memento with Serialization
            SerializationMementoExample();

            // Example 7: Database Transaction Simulation
            DatabaseTransactionExample();

            // Example 8: Multi-Level Undo with Branching
            BranchingUndoExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Basic Text Editor with Undo/Redo

        static void TextEditorExample()
        {
            Console.WriteLine("1. Text Editor with Undo/Redo:");
            Console.WriteLine("================================");

            var editor = new TextEditor();
            var caretaker = new EditorCaretaker();

            // Create some text
            editor.Write("Hello ");
            caretaker.Save(editor.CreateMemento());
            Console.WriteLine($"Text: '{editor.GetText()}'");

            editor.Write("World!");
            caretaker.Save(editor.CreateMemento());
            Console.WriteLine($"Text: '{editor.GetText()}'");

            editor.Write(" How are you?");
            caretaker.Save(editor.CreateMemento());
            Console.WriteLine($"Text: '{editor.GetText()}'");

            // Undo operations
            Console.WriteLine("\nUndo operations:");
            var memento = caretaker.Undo();
            if (memento != null)
            {
                editor.RestoreMemento(memento);
                Console.WriteLine($"After undo: '{editor.GetText()}'");
            }

            memento = caretaker.Undo();
            if (memento != null)
            {
                editor.RestoreMemento(memento);
                Console.WriteLine($"After undo: '{editor.GetText()}'");
            }

            // Redo operations
            Console.WriteLine("\nRedo operations:");
            memento = caretaker.Redo();
            if (memento != null)
            {
                editor.RestoreMemento(memento);
                Console.WriteLine($"After redo: '{editor.GetText()}'");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 2: Game State Management

        static void GameStateExample()
        {
            Console.WriteLine("2. Game State Management:");
            Console.WriteLine("==========================");

            var game = new Game();
            var saveManager = new GameSaveManager();

            // Play the game
            game.SetLevel(1);
            game.SetScore(100);
            game.SetHealth(100);
            saveManager.SaveCheckpoint("start", game.CreateMemento());
            Console.WriteLine($"Game state: Level {game.Level}, Score {game.Score}, Health {game.Health}");

            // Progress in game
            game.SetLevel(2);
            game.SetScore(350);
            game.SetHealth(75);
            saveManager.SaveCheckpoint("level2", game.CreateMemento());
            Console.WriteLine($"Game state: Level {game.Level}, Score {game.Score}, Health {game.Health}");

            // Boss fight
            game.SetLevel(3);
            game.SetScore(500);
            game.SetHealth(25);
            saveManager.SaveCheckpoint("boss", game.CreateMemento());
            Console.WriteLine($"Game state: Level {game.Level}, Score {game.Score}, Health {game.Health}");

            // Player dies, reload from checkpoint
            Console.WriteLine("\nPlayer died! Loading from checkpoint...");
            var checkpoint = saveManager.LoadCheckpoint("level2");
            if (checkpoint != null)
            {
                game.RestoreMemento(checkpoint);
                Console.WriteLine($"Restored state: Level {game.Level}, Score {game.Score}, Health {game.Health}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Configuration Manager with Snapshots

        static void ConfigurationManagerExample()
        {
            Console.WriteLine("3. Configuration Manager with Snapshots:");
            Console.WriteLine("==========================================");

            var config = new ConfigurationManager();
            var snapshots = new ConfigurationSnapshots();

            // Set initial configuration
            config.SetSetting("database.host", "localhost");
            config.SetSetting("database.port", "5432");
            config.SetSetting("app.debug", "true");
            snapshots.TakeSnapshot("initial", config.CreateMemento());
            Console.WriteLine("Initial configuration set");
            config.PrintConfiguration();

            // Modify configuration
            config.SetSetting("database.host", "production.server.com");
            config.SetSetting("database.port", "5433");
            config.SetSetting("app.debug", "false");
            config.SetSetting("app.logging", "error");
            snapshots.TakeSnapshot("production", config.CreateMemento());
            Console.WriteLine("\nProduction configuration:");
            config.PrintConfiguration();

            // Rollback to initial configuration
            Console.WriteLine("\nRolling back to initial configuration:");
            var initialSnapshot = snapshots.GetSnapshot("initial");
            if (initialSnapshot != null)
            {
                config.RestoreMemento(initialSnapshot);
                config.PrintConfiguration();
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 4: Drawing Application with Canvas States

        static void DrawingApplicationExample()
        {
            Console.WriteLine("4. Drawing Application with Canvas States:");
            Console.WriteLine("===========================================");

            var canvas = new DrawingCanvas();
            var history = new CanvasHistory();

            // Draw some shapes
            canvas.AddShape(new Shape("Circle", 10, 10, "Red"));
            history.SaveState(canvas.CreateMemento());
            Console.WriteLine("Added red circle");
            canvas.PrintCanvas();

            canvas.AddShape(new Shape("Rectangle", 20, 20, "Blue"));
            history.SaveState(canvas.CreateMemento());
            Console.WriteLine("\nAdded blue rectangle");
            canvas.PrintCanvas();

            canvas.AddShape(new Shape("Triangle", 30, 30, "Green"));
            history.SaveState(canvas.CreateMemento());
            Console.WriteLine("\nAdded green triangle");
            canvas.PrintCanvas();

            // Undo last action
            Console.WriteLine("\nUndo last action:");
            var previousState = history.Undo();
            if (previousState != null)
            {
                canvas.RestoreMemento(previousState);
                canvas.PrintCanvas();
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Thread-Safe Memento Management

        static async Task ThreadSafeMementoExample()
        {
            Console.WriteLine("5. Thread-Safe Memento Management:");
            Console.WriteLine("====================================");

            var counter = new ThreadSafeCounter();
            var history = new ThreadSafeHistory<CounterMemento>();

            // Create multiple tasks that modify the counter
            var tasks = new List<Task>();

            for (int i = 1; i <= 5; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(async () =>
                {
                    for (int j = 0; j < 3; j++)
                    {
                        counter.Increment();
                        var memento = counter.CreateMemento();
                        history.SaveState($"Task{taskId}_Step{j}", memento);

                        Console.WriteLine($"[Task {taskId}] Counter: {counter.GetValue()}, Step: {j}");
                        await Task.Delay(50); // Simulate work
                    }
                }));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine($"\nFinal counter value: {counter.GetValue()}");
            Console.WriteLine($"Total states saved: {history.GetStateCount()}");

            // Restore to a specific state
            var earlierState = history.GetState("Task1_Step1");
            if (earlierState != null)
            {
                counter.RestoreMemento(earlierState);
                Console.WriteLine($"Restored to earlier state: {counter.GetValue()}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 6: Memory-Optimized Memento with Serialization

        static void SerializationMementoExample()
        {
            Console.WriteLine("6. Memory-Optimized Memento with Serialization:");
            Console.WriteLine("=================================================");

            var document = new Document();
            var versionControl = new DocumentVersionControl();

            // Create a large document
            document.SetTitle("Large Document");
            document.SetContent("This is a sample document with some content...");
            for (int i = 0; i < 5; i++)
            {
                document.AddMetadata($"key{i}", $"value{i}");
            }

            // Save version with serialization
            versionControl.SaveVersion("v1.0", document.CreateSerializedMemento());
            Console.WriteLine("Saved version 1.0");
            Console.WriteLine($"Memory usage: {versionControl.GetMemoryUsage()} bytes");

            // Modify document
            document.SetContent(document.GetContent() + "\n\nAdditional content added in version 2.0");
            document.AddMetadata("version", "2.0");
            versionControl.SaveVersion("v2.0", document.CreateSerializedMemento());
            Console.WriteLine("\nSaved version 2.0");
            Console.WriteLine($"Memory usage: {versionControl.GetMemoryUsage()} bytes");

            // Restore previous version
            Console.WriteLine("\nRestoring to version 1.0:");
            var v1Memento = versionControl.GetVersion("v1.0");
            if (v1Memento != null)
            {
                document.RestoreSerializedMemento(v1Memento);
                Console.WriteLine($"Title: {document.GetTitle()}");
                Console.WriteLine($"Content length: {document.GetContent().Length} characters");
                Console.WriteLine($"Metadata count: {document.GetMetadataCount()}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 7: Database Transaction Simulation

        static void DatabaseTransactionExample()
        {
            Console.WriteLine("7. Database Transaction Simulation:");
            Console.WriteLine("====================================");

            var database = new DatabaseSimulator();
            var transactionManager = new TransactionManager();

            // Start transaction
            transactionManager.BeginTransaction(database.CreateMemento());
            Console.WriteLine("Transaction started");

            // Perform operations
            database.Insert("users", "John Doe");
            database.Insert("users", "Jane Smith");
            database.Update("settings", "theme", "dark");
            Console.WriteLine("Performed database operations");
            database.PrintStatus();

            // Create savepoint
            transactionManager.CreateSavepoint("savepoint1", database.CreateMemento());

            // More operations
            database.Delete("users", "John Doe");
            database.Insert("products", "Laptop");
            Console.WriteLine("\nPerformed more operations");
            database.PrintStatus();

            // Rollback to savepoint
            Console.WriteLine("\nRolling back to savepoint1:");
            transactionManager.RollbackToSavepoint("savepoint1", database);
            database.PrintStatus();

            // Commit transaction
            Console.WriteLine("\nCommitting transaction");
            transactionManager.CommitTransaction();

            Console.WriteLine();
        }

        #endregion

        #region Example 8: Multi-Level Undo with Branching

        static void BranchingUndoExample()
        {
            Console.WriteLine("8. Multi-Level Undo with Branching:");
            Console.WriteLine("=====================================");

            var editor = new AdvancedTextEditor();
            var undoTree = new UndoTree();

            // Create initial state
            editor.SetText("Hello");
            undoTree.SaveState(editor.CreateMemento());
            Console.WriteLine($"Initial: '{editor.GetText()}'");

            // Make changes
            editor.SetText("Hello World");
            undoTree.SaveState(editor.CreateMemento());
            Console.WriteLine($"Edit 1: '{editor.GetText()}'");

            editor.SetText("Hello World!");
            undoTree.SaveState(editor.CreateMemento());
            Console.WriteLine($"Edit 2: '{editor.GetText()}'");

            // Undo and create branch
            Console.WriteLine("\nUndo to 'Hello World' and create branch:");
            var state = undoTree.UndoToState(1);
            if (state != null)
            {
                editor.RestoreMemento(state);
                Console.WriteLine($"Undone to: '{editor.GetText()}'");

                // Create branch
                editor.SetText("Hello Universe");
                undoTree.CreateBranch(editor.CreateMemento());
                Console.WriteLine($"Branch: '{editor.GetText()}'");
            }

            // Show undo tree structure
            Console.WriteLine("\nUndo tree structure:");
            undoTree.PrintTree();

            Console.WriteLine();
        }

        #endregion
    }

    #region Core Memento Pattern Classes

    // Generic memento interface
    public interface IMemento
    {
        DateTime Timestamp { get; }
        string Description { get; }
    }

    // Generic originator interface
    public interface IOriginator<T> where T : IMemento
    {
        T CreateMemento();
        void RestoreMemento(T memento);
    }

    // Generic caretaker for managing mementos
    // MEMORY ALLOCATION: Stores collection of mementos - can grow significantly
    public class MementoCaretaker<T> where T : IMemento
    {
        private readonly List<T> _mementos = new List<T>();
        private int _currentIndex = -1;
        private readonly int _maxSize;

        public MementoCaretaker(int maxSize = 50)
        {
            _maxSize = maxSize;
        }

        public void Save(T memento)
        {
            // Remove any mementos after current index (for redo functionality)
            if (_currentIndex < _mementos.Count - 1)
            {
                _mementos.RemoveRange(_currentIndex + 1, _mementos.Count - _currentIndex - 1);
            }

            _mementos.Add(memento);
            _currentIndex = _mementos.Count - 1;

            // Limit memory usage by removing old mementos
            if (_mementos.Count > _maxSize)
            {
                _mementos.RemoveAt(0);
                _currentIndex--;
            }
        }

        public T? Undo()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                return _mementos[_currentIndex];
            }
            return default(T);
        }

        public T? Redo()
        {
            if (_currentIndex < _mementos.Count - 1)
            {
                _currentIndex++;
                return _mementos[_currentIndex];
            }
            return default(T);
        }

        public bool CanUndo => _currentIndex > 0;
        public bool CanRedo => _currentIndex < _mementos.Count - 1;
        public int Count => _mementos.Count;
    }

    #endregion

    #region Example 1: Text Editor Classes

    // Memento for text editor state
    public class TextEditorMemento : IMemento
    {
        public string Text { get; }
        public int CursorPosition { get; }
        public DateTime Timestamp { get; }
        public string Description { get; }

        public TextEditorMemento(string text, int cursorPosition, string description = "")
        {
            Text = text;
            CursorPosition = cursorPosition;
            Timestamp = DateTime.Now;
            Description = description;
        }
    }

    // Originator - Text Editor
    public class TextEditor : IOriginator<TextEditorMemento>
    {
        private string _text = string.Empty;
        private int _cursorPosition = 0;

        public void Write(string text)
        {
            _text += text;
            _cursorPosition = _text.Length;
        }

        public void SetText(string text)
        {
            _text = text;
            _cursorPosition = text.Length;
        }

        public string GetText() => _text;

        public TextEditorMemento CreateMemento()
        {
            return new TextEditorMemento(_text, _cursorPosition, $"Text: {_text.Substring(0, Math.Min(_text.Length, 20))}...");
        }

        public void RestoreMemento(TextEditorMemento memento)
        {
            _text = memento.Text;
            _cursorPosition = memento.CursorPosition;
        }
    }

    // Caretaker for text editor
    public class EditorCaretaker
    {
        private readonly MementoCaretaker<TextEditorMemento> _caretaker = new MementoCaretaker<TextEditorMemento>();

        public void Save(TextEditorMemento memento) => _caretaker.Save(memento);
        public TextEditorMemento? Undo() => _caretaker.Undo();
        public TextEditorMemento? Redo() => _caretaker.Redo();
    }

    #endregion

    #region Example 2: Game State Classes

    public class GameMemento : IMemento
    {
        public int Level { get; }
        public int Score { get; }
        public int Health { get; }
        public DateTime Timestamp { get; }
        public string Description { get; }

        public GameMemento(int level, int score, int health)
        {
            Level = level;
            Score = score;
            Health = health;
            Timestamp = DateTime.Now;
            Description = $"Level {level}, Score {score}, Health {health}";
        }
    }

    public class Game : IOriginator<GameMemento>
    {
        public int Level { get; private set; } = 1;
        public int Score { get; private set; } = 0;
        public int Health { get; private set; } = 100;

        public void SetLevel(int level) => Level = level;
        public void SetScore(int score) => Score = score;
        public void SetHealth(int health) => Health = health;

        public GameMemento CreateMemento()
        {
            return new GameMemento(Level, Score, Health);
        }

        public void RestoreMemento(GameMemento memento)
        {
            Level = memento.Level;
            Score = memento.Score;
            Health = memento.Health;
        }
    }

    public class GameSaveManager
    {
        private readonly Dictionary<string, GameMemento> _checkpoints = new Dictionary<string, GameMemento>();

        public void SaveCheckpoint(string name, GameMemento memento)
        {
            _checkpoints[name] = memento;
        }

        public GameMemento? LoadCheckpoint(string name)
        {
            return _checkpoints.TryGetValue(name, out var memento) ? memento : null;
        }

        public IEnumerable<string> GetCheckpointNames() => _checkpoints.Keys;
    }

    #endregion

    #region Example 3: Configuration Manager Classes

    public class ConfigurationMemento : IMemento
    {
        public Dictionary<string, string> Settings { get; }
        public DateTime Timestamp { get; }
        public string Description { get; }

        public ConfigurationMemento(Dictionary<string, string> settings, string description = "")
        {
            Settings = new Dictionary<string, string>(settings);
            Timestamp = DateTime.Now;
            Description = description;
        }
    }

    public class ConfigurationManager : IOriginator<ConfigurationMemento>
    {
        private readonly Dictionary<string, string> _settings = new Dictionary<string, string>();

        public void SetSetting(string key, string value)
        {
            _settings[key] = value;
        }

        public string? GetSetting(string key)
        {
            return _settings.TryGetValue(key, out var value) ? value : null;
        }

        public void PrintConfiguration()
        {
            foreach (var setting in _settings)
            {
                Console.WriteLine($"  {setting.Key} = {setting.Value}");
            }
        }

        public ConfigurationMemento CreateMemento()
        {
            return new ConfigurationMemento(_settings, $"Configuration with {_settings.Count} settings");
        }

        public void RestoreMemento(ConfigurationMemento memento)
        {
            _settings.Clear();
            foreach (var setting in memento.Settings)
            {
                _settings[setting.Key] = setting.Value;
            }
        }
    }

    public class ConfigurationSnapshots
    {
        private readonly Dictionary<string, ConfigurationMemento> _snapshots = new Dictionary<string, ConfigurationMemento>();

        public void TakeSnapshot(string name, ConfigurationMemento memento)
        {
            _snapshots[name] = memento;
        }

        public ConfigurationMemento? GetSnapshot(string name)
        {
            return _snapshots.TryGetValue(name, out var snapshot) ? snapshot : null;
        }
    }

    #endregion

    #region Example 4: Drawing Application Classes

    public class Shape
    {
        public string Type { get; }
        public int X { get; }
        public int Y { get; }
        public string Color { get; }

        public Shape(string type, int x, int y, string color)
        {
            Type = type;
            X = x;
            Y = y;
            Color = color;
        }

        public override string ToString() => $"{Color} {Type} at ({X},{Y})";
    }

    public class CanvasMemento : IMemento
    {
        public List<Shape> Shapes { get; }
        public DateTime Timestamp { get; }
        public string Description { get; }

        public CanvasMemento(IEnumerable<Shape> shapes)
        {
            Shapes = new List<Shape>(shapes);
            Timestamp = DateTime.Now;
            Description = $"Canvas with {Shapes.Count} shapes";
        }
    }

    public class DrawingCanvas : IOriginator<CanvasMemento>
    {
        private readonly List<Shape> _shapes = new List<Shape>();

        public void AddShape(Shape shape)
        {
            _shapes.Add(shape);
        }

        public void RemoveShape(int index)
        {
            if (index >= 0 && index < _shapes.Count)
            {
                _shapes.RemoveAt(index);
            }
        }

        public void PrintCanvas()
        {
            Console.WriteLine($"  Canvas has {_shapes.Count} shapes:");
            foreach (var shape in _shapes)
            {
                Console.WriteLine($"    {shape}");
            }
        }

        public CanvasMemento CreateMemento()
        {
            return new CanvasMemento(_shapes);
        }

        public void RestoreMemento(CanvasMemento memento)
        {
            _shapes.Clear();
            _shapes.AddRange(memento.Shapes);
        }
    }

    public class CanvasHistory
    {
        private readonly MementoCaretaker<CanvasMemento> _history = new MementoCaretaker<CanvasMemento>();

        public void SaveState(CanvasMemento memento) => _history.Save(memento);
        public CanvasMemento? Undo() => _history.Undo();
        public CanvasMemento? Redo() => _history.Redo();
    }

    #endregion

    #region Example 5: Thread-Safe Classes

    // MULTITHREAD ASPECTS: Thread-safe counter with atomic operations
    public class CounterMemento : IMemento
    {
        public int Value { get; }
        public DateTime Timestamp { get; }
        public string Description { get; }

        public CounterMemento(int value)
        {
            Value = value;
            Timestamp = DateTime.Now;
            Description = $"Counter value: {value}";
        }
    }

    public class ThreadSafeCounter : IOriginator<CounterMemento>
    {
        private int _value = 0;

        public void Increment()
        {
            Interlocked.Increment(ref _value);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref _value);
        }

        public int GetValue() => _value;

        public CounterMemento CreateMemento()
        {
            return new CounterMemento(_value);
        }

        public void RestoreMemento(CounterMemento memento)
        {
            Interlocked.Exchange(ref _value, memento.Value);
        }
    }

    // Thread-safe history manager using concurrent collections
    public class ThreadSafeHistory<T> where T : IMemento
    {
        private readonly ConcurrentDictionary<string, T> _states = new ConcurrentDictionary<string, T>();
        private readonly object _lock = new object();

        public void SaveState(string key, T memento)
        {
            _states.AddOrUpdate(key, memento, (k, v) => memento);
        }

        public T? GetState(string key)
        {
            return _states.TryGetValue(key, out var state) ? state : default(T);
        }

        public int GetStateCount()
        {
            return _states.Count;
        }

        public IEnumerable<string> GetStateKeys()
        {
            return _states.Keys;
        }
    }

    #endregion

    #region Example 6: Serialization Classes

    // Serializable memento for memory optimization
    public class SerializedMemento : IMemento
    {
        public string SerializedData { get; }
        public DateTime Timestamp { get; }
        public string Description { get; }
        public int SizeInBytes => System.Text.Encoding.UTF8.GetByteCount(SerializedData);

        public SerializedMemento(string serializedData, string description = "")
        {
            SerializedData = serializedData;
            Timestamp = DateTime.Now;
            Description = description;
        }
    }

    public class Document
    {
        private string _title = string.Empty;
        private string _content = string.Empty;
        private readonly Dictionary<string, string> _metadata = new Dictionary<string, string>();

        public void SetTitle(string title) => _title = title;
        public void SetContent(string content) => _content = content;
        public void AddMetadata(string key, string value) => _metadata[key] = value;

        public string GetTitle() => _title;
        public string GetContent() => _content;
        public int GetMetadataCount() => _metadata.Count;

        public SerializedMemento CreateSerializedMemento()
        {
            var state = new
            {
                Title = _title,
                Content = _content,
                Metadata = _metadata,
                Timestamp = DateTime.Now
            };

            var json = JsonSerializer.Serialize(state);
            return new SerializedMemento(json, $"Document: {_title}");
        }

        public void RestoreSerializedMemento(SerializedMemento memento)
        {
            var state = JsonSerializer.Deserialize<dynamic>(memento.SerializedData);
            // Note: In a real implementation, you'd properly deserialize the JSON
            // For simplicity, we're using basic parsing here
            var jsonDoc = JsonDocument.Parse(memento.SerializedData);
            var root = jsonDoc.RootElement;

            _title = root.GetProperty("Title").GetString() ?? string.Empty;
            _content = root.GetProperty("Content").GetString() ?? string.Empty;

            _metadata.Clear();
            if (root.TryGetProperty("Metadata", out var metadataElement))
            {
                foreach (var property in metadataElement.EnumerateObject())
                {
                    _metadata[property.Name] = property.Value.GetString() ?? string.Empty;
                }
            }
        }
    }

    public class DocumentVersionControl
    {
        private readonly Dictionary<string, SerializedMemento> _versions = new Dictionary<string, SerializedMemento>();

        public void SaveVersion(string version, SerializedMemento memento)
        {
            _versions[version] = memento;
        }

        public SerializedMemento? GetVersion(string version)
        {
            return _versions.TryGetValue(version, out var memento) ? memento : null;
        }

        public long GetMemoryUsage()
        {
            return _versions.Values.Sum(m => m.SizeInBytes);
        }
    }

    #endregion

    #region Example 7: Database Transaction Classes

    public class DatabaseMemento : IMemento
    {
        public Dictionary<string, List<string>> Tables { get; }
        public DateTime Timestamp { get; }
        public string Description { get; }

        public DatabaseMemento(Dictionary<string, List<string>> tables)
        {
            Tables = new Dictionary<string, List<string>>();
            foreach (var table in tables)
            {
                Tables[table.Key] = new List<string>(table.Value);
            }
            Timestamp = DateTime.Now;
            Description = $"Database state with {tables.Count} tables";
        }
    }

    public class DatabaseSimulator : IOriginator<DatabaseMemento>
    {
        private readonly Dictionary<string, List<string>> _tables = new Dictionary<string, List<string>>();

        public void Insert(string table, string record)
        {
            if (!_tables.ContainsKey(table))
                _tables[table] = new List<string>();

            _tables[table].Add(record);
        }

        public void Update(string table, string oldRecord, string newRecord)
        {
            if (_tables.ContainsKey(table))
            {
                var index = _tables[table].IndexOf(oldRecord);
                if (index >= 0)
                    _tables[table][index] = newRecord;
            }
        }

        public void Delete(string table, string record)
        {
            if (_tables.ContainsKey(table))
            {
                _tables[table].Remove(record);
            }
        }

        public void PrintStatus()
        {
            foreach (var table in _tables)
            {
                Console.WriteLine($"  Table '{table.Key}': {table.Value.Count} records");
                foreach (var record in table.Value)
                {
                    Console.WriteLine($"    - {record}");
                }
            }
        }

        public DatabaseMemento CreateMemento()
        {
            return new DatabaseMemento(_tables);
        }

        public void RestoreMemento(DatabaseMemento memento)
        {
            _tables.Clear();
            foreach (var table in memento.Tables)
            {
                _tables[table.Key] = new List<string>(table.Value);
            }
        }
    }

    public class TransactionManager
    {
        private DatabaseMemento? _transactionStart;
        private readonly Dictionary<string, DatabaseMemento> _savepoints = new Dictionary<string, DatabaseMemento>();

        public void BeginTransaction(DatabaseMemento initialState)
        {
            _transactionStart = initialState;
            _savepoints.Clear();
        }

        public void CreateSavepoint(string name, DatabaseMemento state)
        {
            _savepoints[name] = state;
        }

        public void RollbackToSavepoint(string name, DatabaseSimulator database)
        {
            if (_savepoints.TryGetValue(name, out var savepoint))
            {
                database.RestoreMemento(savepoint);
            }
        }

        public void CommitTransaction()
        {
            _transactionStart = null;
            _savepoints.Clear();
        }

        public void RollbackTransaction(DatabaseSimulator database)
        {
            if (_transactionStart != null)
            {
                database.RestoreMemento(_transactionStart);
            }
        }
    }

    #endregion

    #region Example 8: Branching Undo Classes

    public class AdvancedTextEditor : IOriginator<TextEditorMemento>
    {
        private string _text = string.Empty;

        public void SetText(string text) => _text = text;
        public string GetText() => _text;

        public TextEditorMemento CreateMemento()
        {
            return new TextEditorMemento(_text, _text.Length, $"Text: {_text}");
        }

        public void RestoreMemento(TextEditorMemento memento)
        {
            _text = memento.Text;
        }
    }

    public class UndoNode
    {
        public TextEditorMemento State { get; }
        public List<UndoNode> Children { get; } = new List<UndoNode>();
        public UndoNode? Parent { get; set; }
        public int Id { get; }

        public UndoNode(TextEditorMemento state, int id)
        {
            State = state;
            Id = id;
        }
    }

    public class UndoTree
    {
        private UndoNode? _root;
        private UndoNode? _current;
        private int _nextId = 0;

        public void SaveState(TextEditorMemento memento)
        {
            var node = new UndoNode(memento, _nextId++);

            if (_root == null)
            {
                _root = node;
                _current = node;
            }
            else if (_current != null)
            {
                node.Parent = _current;
                _current.Children.Add(node);
                _current = node;
            }
        }

        public TextEditorMemento? UndoToState(int nodeId)
        {
            var node = FindNode(_root, nodeId);
            if (node != null)
            {
                _current = node;
                return node.State;
            }
            return null;
        }

        public void CreateBranch(TextEditorMemento memento)
        {
            if (_current != null)
            {
                var branch = new UndoNode(memento, _nextId++);
                branch.Parent = _current;
                _current.Children.Add(branch);
                _current = branch;
            }
        }

        public void PrintTree()
        {
            if (_root != null)
            {
                PrintNode(_root, 0);
            }
        }

        private void PrintNode(UndoNode node, int depth)
        {
            var indent = new string(' ', depth * 2);
            var marker = node == _current ? " *" : "";
            Console.WriteLine($"{indent}Node {node.Id}: '{node.State.Text}'{marker}");

            foreach (var child in node.Children)
            {
                PrintNode(child, depth + 1);
            }
        }

        private UndoNode? FindNode(UndoNode? node, int id)
        {
            if (node == null) return null;
            if (node.Id == id) return node;

            foreach (var child in node.Children)
            {
                var found = FindNode(child, id);
                if (found != null) return found;
            }

            return null;
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. MEMENTO STORAGE:
 *    - Each memento stores a complete snapshot of object state
 *    - Memory usage grows linearly with number of stored mementos
 *    - Consider implementing maximum history size to prevent memory leaks
 *    - Use weak references for large objects if appropriate
 * 
 * 2. DEEP VS SHALLOW COPYING:
 *    - Mementos should use deep copying to ensure state independence
 *    - Shallow copying can lead to unintended state modifications
 *    - Consider implementing ICloneable or custom copy methods
 *    - Be aware of reference types vs value types in state copying
 * 
 * 3. SERIALIZATION OPTIMIZATION:
 *    - Serialize mementos for large objects to reduce memory footprint
 *    - Use compression for text-heavy or repetitive state data
 *    - Consider binary serialization for performance-critical scenarios
 *    - Balance between memory usage and serialization/deserialization overhead
 * 
 * 4. CIRCULAR REFERENCES:
 *    - Avoid circular references in memento state
 *    - Use proper object disposal when removing old mementos
 *    - Consider using dependency injection to manage object lifecycles
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. ORIGINATOR THREAD SAFETY:
 *    - Originator objects may not be thread-safe by default
 *    - Use locks or atomic operations when multiple threads access state
 *    - Consider using ThreadLocal storage for per-thread state
 *    - Ensure memento creation is atomic to avoid partial state capture
 * 
 * 2. CARETAKER SYNCHRONIZATION:
 *    - Caretaker collections need synchronization for concurrent access
 *    - Use concurrent collections (ConcurrentDictionary, ConcurrentQueue)
 *    - Implement proper locking for undo/redo operations
 *    - Consider using reader-writer locks for read-heavy scenarios
 * 
 * 3. MEMENTO IMMUTABILITY:
 *    - Mementos should be immutable once created
 *    - Immutable mementos are inherently thread-safe for reading
 *    - Use readonly fields and properties in memento classes
 *    - Avoid exposing mutable collections directly
 * 
 * 4. ASYNC OPERATIONS:
 *    - Be careful with async operations that might modify state during memento creation
 *    - Use proper synchronization for async memento restoration
 *    - Consider using ConfigureAwait(false) for library code
 *    - Handle cancellation tokens appropriately in async scenarios
 * 
 * 5. PERFORMANCE CONSIDERATIONS:
 *    - Memento creation and restoration can be expensive operations
 *    - Consider lazy loading for large state objects
 *    - Use object pooling for frequently created/destroyed mementos
 *    - Profile memory usage and performance under load
 *    - Consider using incremental snapshots for large objects
 * 
 * 6. BEST PRACTICES:
 *    - Implement proper error handling for memento operations
 *    - Use version numbers to ensure memento compatibility
 *    - Consider encrypting sensitive data in mementos
 *    - Implement proper cleanup for temporary files or resources
 *    - Use dependency injection for testability and flexibility
 *    - Document the state captured by each memento type
 */
