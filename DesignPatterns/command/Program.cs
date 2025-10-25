using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

/*
 * Class diagram to use on https://app.gleek.io/
 
In the command pattern you are abstracting actions or operations over an object.
Instead of directly calling methods on an object to perform actions, 
you create command objects that encapsulate the actions. 
Each command object implements a common interface with methods to execute and undo the action.
A controller or invoker class is responsible for executing the commands and managing their lifecycle.

 * Use the command pattern when you are building something in multiple steps
 * and the build process requires the ability to undo and redo steps.
 * Sample: you are building a text from a text editor. The editor itself has 
 * all the commands needed to build the text. With the command patter you put
 * a layer over the editor where you complete manage the commands used to 
 * build a text.

 Another way to think about the command pattern is that you are indirectly
 controlling an object (the text editor in this case) by using
 command objects that you can manage.

 *  CommandHistory
 *  	-Stack<ICommand> _undoStack
 *  	-Stack<ICommand> _redoStack
 *  	+ExecuteCommand(ICommand command)
 *  	+Undo()
 *  	+Redo()
 *  
 *  DeleteTextCommand
 *  	+DeleteTextCommand(TextEditor editor, int length)
 *  	+public void Execute()
 *  	+public void Undo()
 *  
 *  InsertTextCommand
 *  	+InsertTextCommand(TextEditor editor, string text)
 *  	+public void Execute()
 *  	+public void Undo()
 *  
 *  TextEditor
 *  	+InsertText(string text)
 *  	+DeleteText(int length)
 *  	+RestoreState(string previousText)
 *  	+GetText()
 *  	+GetLastState()
 *  
 *  InsertTextCommand-GetText/InsertText/RestoreState->TextEditor
 *  DeleteTextCommand-GetText/DeleteText/RestoreState->TextEditor
 *  CommandHistory-Execute/Undo->InsertTextCommand
 *  CommandHistory-Execute/Undo->DeleteTextCommand
 */

/*
Light
	+Location
	+IsOn
	+TurnOn()
	+TurnOff()

SecuritySystem
	+IsArmed
	+Arm()
	+Disarm()

LightOnCommand
	+public void Execute()
	+public void Undo()
	+LightOnCommand(Light light)

LightOffCommand
	+public void Execute()
	+public void Undo()
	+LightOffCommand(Light light)

SecuritySystemArmCommand
	+public void Execute()
	+public void Undo()
	+SecuritySystemArmCommand(SecuritySystem system)

SecuritySystemDesarmCommand
	+public void Execute()
	+public void Undo()
	+SecuritySystemDisarmCommand(SecuritySystem system)

RemoteControl
	-ICommand[] _onCommands
	-ICommand[] _offCommands
	+SetCommand(int slot, ICommand onCommand, ICommand offCommand)
	+OnButtonPressed(int slot)
	+OffButtonPressed(int slot)
	+UndoButtonPressed()

LightOnCommand-->Light
LightOffCommand-->Light
SecuritySystemDesarmCommand-->SecuritySystem
SecuritySystemArmCommand-->SecuritySystem
RemoteControl-->LightOffCommand
RemoteControl-->LightOnCommand
RemoteControl-->SecuritySystemArmCommand
RemoteControl-->SecuritySystemDesarmCommand
*/

/*
 * COMMAND DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Command pattern encapsulates a request as an object, allowing you to:
 * - Parameterize objects with different requests
 * - Queue or log requests
 * - Support undoable operations
 * - Decouple the object that invokes the operation from the object that performs it
 * 
 * CORE BENEFITS:
 * - Decouples sender and receiver of requests
 * - Allows queuing, logging, and undoing operations
 * - Supports macro commands (composite commands)
 * - Easy to add new commands without changing existing code
 * - Enables transactional behavior and rollback capabilities
 * 
 * SCENARIOS TO USE:
 * - When you need to queue operations, schedule them, or log them
 * - When you want to support undo/redo functionality
 * - When you need to support transactional operations
 * - In GUI applications for menu items, toolbar buttons, keyboard shortcuts
 * - In multi-level undo systems (text editors, graphics applications)
 * - For implementing macro recording and playback
 * - In remote procedure calls or distributed systems
 * - For implementing wizards with step-by-step operations
 * 
 * SCENARIOS NOT TO USE:
 * - Simple, direct method calls where no additional functionality is needed
 * - When the overhead of creating command objects is too high for performance
 * - In systems where undo functionality is not required
 * - When the relationship between invoker and receiver is simple and stable
 * - For one-time operations that don't need to be stored or repeated
 */

namespace CommandPattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Command Design Pattern Examples ===\n");

            // Example 1: Basic Text Editor Commands
            BasicTextEditorExample();

            // Example 2: Smart Home Automation
            SmartHomeExample();

            // Example 3: Database Transaction Commands
            DatabaseTransactionExample();

            // Example 4: Macro Commands (Composite)
            MacroCommandExample();

            // Example 5: Thread-Safe Command Queue
            await ThreadSafeCommandQueueExample();

            // Example 6: Async Commands with Cancellation
            await AsyncCommandExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Basic Text Editor Commands

        static void BasicTextEditorExample()
        {
            Console.WriteLine("1. Basic Text Editor Commands:");
            Console.WriteLine("===============================");

            var editor = new TextEditor();
            var commandHistory = new CommandHistory();

            // Create and execute commands
            var insertCommand = new InsertTextCommand(editor, "Hello ");
            var insertCommand2 = new InsertTextCommand(editor, "World!");
            var deleteCommand = new DeleteTextCommand(editor, 6); // Delete "World!"

            Console.WriteLine("Executing commands:");
            commandHistory.ExecuteCommand(insertCommand);
            Console.WriteLine($"Text: '{editor.GetText()}'");

            commandHistory.ExecuteCommand(insertCommand2);
            Console.WriteLine($"Text: '{editor.GetText()}'");

            commandHistory.ExecuteCommand(deleteCommand);
            Console.WriteLine($"Text: '{editor.GetText()}'");

            Console.WriteLine("\nUndoing commands:");
            commandHistory.Undo();
            Console.WriteLine($"Text: '{editor.GetText()}'");

            commandHistory.Undo();
            Console.WriteLine($"Text: '{editor.GetText()}'");

            Console.WriteLine("\nRedoing commands:");
            commandHistory.Redo();
            Console.WriteLine($"Text: '{editor.GetText()}'");

            Console.WriteLine();
        }

        #endregion

        #region Example 2: Smart Home Automation

        static void SmartHomeExample()
        {
            Console.WriteLine("2. Smart Home Automation:");
            Console.WriteLine("==========================");

            var livingRoomLight = new Light("Living Room");
            var bedroomLight = new Light("Bedroom");
            var airConditioner = new AirConditioner("Main AC");
            var securitySystem = new SecuritySystem();

            var remoteControl = new RemoteControl();

            // Program remote control buttons
            remoteControl.SetCommand(0, new LightOnCommand(livingRoomLight), new LightOffCommand(livingRoomLight));
            remoteControl.SetCommand(1, new LightOnCommand(bedroomLight), new LightOffCommand(bedroomLight));
            remoteControl.SetCommand(2, new AirConditionerOnCommand(airConditioner), new AirConditionerOffCommand(airConditioner));
            remoteControl.SetCommand(3, new SecuritySystemArmCommand(securitySystem), new SecuritySystemDisarmCommand(securitySystem));

            Console.WriteLine("Using remote control:");
            remoteControl.OnButtonPressed(0); // Living room light on
            remoteControl.OnButtonPressed(1); // Bedroom light on
            remoteControl.OnButtonPressed(2); // AC on

            Console.WriteLine("\nTurning things off:");
            remoteControl.OffButtonPressed(0); // Living room light off
            remoteControl.OffButtonPressed(2); // AC off

            Console.WriteLine("\nUsing undo:");
            remoteControl.UndoButtonPressed(); // Undo last command (AC off)

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Database Transaction Commands

        static void DatabaseTransactionExample()
        {
            Console.WriteLine("3. Database Transaction Commands:");
            Console.WriteLine("==================================");

            var database = new Database();
            var transactionManager = new TransactionManager();

            // Create transaction with multiple commands
            var transaction = new DatabaseTransaction();
            transaction.AddCommand(new InsertRecordCommand(database, "users", "John Doe"));
            transaction.AddCommand(new InsertRecordCommand(database, "users", "Jane Smith"));
            transaction.AddCommand(new UpdateRecordCommand(database, "users", 1, "John Updated"));

            Console.WriteLine("Executing database transaction:");
            try
            {
                transactionManager.ExecuteTransaction(transaction);
                Console.WriteLine("Transaction completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transaction failed: {ex.Message}");
                Console.WriteLine("Rolling back transaction...");
                transactionManager.RollbackTransaction(transaction);
            }

            database.PrintContents();
            Console.WriteLine();
        }

        #endregion

        #region Example 4: Macro Commands

        static void MacroCommandExample()
        {
            Console.WriteLine("4. Macro Commands (Composite):");
            Console.WriteLine("===============================");

            var light1 = new Light("Living Room");
            var light2 = new Light("Bedroom");
            var light3 = new Light("Kitchen");
            var airConditioner = new AirConditioner("Main AC");

            // Create a "Party Mode" macro
            var partyMacro = new MacroCommand("Party Mode");
            partyMacro.AddCommand(new LightOnCommand(light1));
            partyMacro.AddCommand(new LightOnCommand(light2));
            partyMacro.AddCommand(new LightOnCommand(light3));
            partyMacro.AddCommand(new AirConditionerOnCommand(airConditioner));

            // Create an "All Off" macro
            var allOffMacro = new MacroCommand("All Off");
            allOffMacro.AddCommand(new LightOffCommand(light1));
            allOffMacro.AddCommand(new LightOffCommand(light2));
            allOffMacro.AddCommand(new LightOffCommand(light3));
            allOffMacro.AddCommand(new AirConditionerOffCommand(airConditioner));

            var commandHistory = new CommandHistory();

            Console.WriteLine("Executing Party Mode macro:");
            commandHistory.ExecuteCommand(partyMacro);

            Console.WriteLine("\nExecuting All Off macro:");
            commandHistory.ExecuteCommand(allOffMacro);

            Console.WriteLine("\nUndoing All Off (should turn everything back on):");
            commandHistory.Undo();

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Thread-Safe Command Queue

        static async Task ThreadSafeCommandQueueExample()
        {
            Console.WriteLine("5. Thread-Safe Command Queue:");
            Console.WriteLine("==============================");

            var commandQueue = new ThreadSafeCommandQueue();
            var sharedResource = new SharedResource();

            // Start the command processor
            var processorTask = commandQueue.StartProcessing();

            // Create multiple tasks that add commands concurrently
            var tasks = new List<Task>();

            for (int i = 1; i <= 5; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 1; j <= 3; j++)
                    {
                        var command = new SharedResourceCommand(sharedResource, $"Task{taskId}-Command{j}");
                        commandQueue.EnqueueCommand(command);
                        Thread.Sleep(100); // Simulate some work
                    }
                }));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            // Wait a bit for commands to process
            await Task.Delay(2000);

            // Stop the processor
            commandQueue.StopProcessing();
            await processorTask;

            Console.WriteLine($"Final shared resource value: {sharedResource.GetValue()}");
            Console.WriteLine();
        }

        #endregion

        #region Example 6: Async Commands with Cancellation

        static async Task AsyncCommandExample()
        {
            Console.WriteLine("6. Async Commands with Cancellation:");
            Console.WriteLine("=====================================");

            var fileProcessor = new FileProcessor();
            var asyncCommandExecutor = new AsyncCommandExecutor();

            // Create file processing commands
            var commands = new[]
            {
                new AsyncFileProcessCommand(fileProcessor, "file1.txt", 1000),
                new AsyncFileProcessCommand(fileProcessor, "file2.txt", 2000),
                new AsyncFileProcessCommand(fileProcessor, "file3.txt", 1500)
            };

            Console.WriteLine("Starting async command execution...");

            // Execute commands with cancellation after 3 seconds
            using var cts = new CancellationTokenSource(3000);

            try
            {
                await asyncCommandExecutor.ExecuteCommandsAsync(commands, cts.Token);
                Console.WriteLine("All commands completed successfully");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Command execution was cancelled");
            }

            Console.WriteLine();
        }

        #endregion
    }

    #region Command Interface and Base Classes

    // Basic command interface
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    // Async command interface for operations that need cancellation support
    public interface IAsyncCommand
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
        Task UndoAsync(CancellationToken cancellationToken = default);
    }

    // No-operation command (Null Object pattern)
    public class NoOpCommand : ICommand
    {
        public void Execute() { }
        public void Undo() { }
    }

    #endregion

    #region Text Editor Example Classes

    // Receiver - Text Editor
    public class TextEditor
    {
        private string _text = string.Empty;
        private readonly List<string> _history = new List<string>();

        public void InsertText(string text)
        {
            _history.Add(_text);
            _text += text;
            Console.WriteLine($"  Inserted: '{text}'");
        }

        public void DeleteText(int length)
        {
            _history.Add(_text);
            if (length <= _text.Length)
            {
                var deletedText = _text.Substring(_text.Length - length);
                _text = _text.Substring(0, _text.Length - length);
                Console.WriteLine($"  Deleted: '{deletedText}'");
            }
        }

        public void RestoreState(string previousText)
        {
            _text = previousText;
            Console.WriteLine($"  Restored to: '{_text}'");
        }

        public string GetText() => _text;

        public string GetLastState()
        {
            if (_history.Count > 0)
            {
                var lastState = _history[_history.Count - 1];
                _history.RemoveAt(_history.Count - 1);
                return lastState;
            }
            return string.Empty;
        }
    }

    // Concrete Commands for Text Editor
    public class InsertTextCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly string _text;
        private string _previousState = string.Empty;

        public InsertTextCommand(TextEditor editor, string text)
        {
            _editor = editor;
            _text = text;
        }

        public void Execute()
        {
            _previousState = _editor.GetText();
            _editor.InsertText(_text);
        }

        public void Undo()
        {
            _editor.RestoreState(_previousState);
        }
    }

    public class DeleteTextCommand : ICommand
    {
        private readonly TextEditor _editor;
        private readonly int _length;
        private string _previousState = string.Empty;

        public DeleteTextCommand(TextEditor editor, int length)
        {
            _editor = editor;
            _length = length;
        }

        public void Execute()
        {
            _previousState = _editor.GetText();
            _editor.DeleteText(_length);
        }

        public void Undo()
        {
            _editor.RestoreState(_previousState);
        }
    }

    // Command History (Invoker)
    // MEMORY ALLOCATION: Stores command history - memory grows with number of operations
    public class CommandHistory
    {
        private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();
        private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();
        private const int MAX_HISTORY_SIZE = 100; // Prevent unbounded memory growth

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear(); // Clear redo stack when new command is executed

            // Limit history size to prevent memory issues
            if (_undoStack.Count > MAX_HISTORY_SIZE)
            {
                var commands = _undoStack.ToArray();
                _undoStack.Clear();
                for (int i = commands.Length - MAX_HISTORY_SIZE; i < commands.Length; i++)
                {
                    _undoStack.Push(commands[i]);
                }
            }
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }
    }

    #endregion

    #region Smart Home Example Classes

    // Receivers
    public class Light
    {
        public string Location { get; }
        public bool IsOn { get; private set; }

        public Light(string location)
        {
            Location = location;
        }

        public void TurnOn()
        {
            IsOn = true;
            Console.WriteLine($"  {Location} light is ON");
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"  {Location} light is OFF");
        }
    }

    public class AirConditioner
    {
        public string Location { get; }
        public bool IsOn { get; private set; }

        public AirConditioner(string location)
        {
            Location = location;
        }

        public void TurnOn()
        {
            IsOn = true;
            Console.WriteLine($"  {Location} AC is ON");
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"  {Location} AC is OFF");
        }
    }

    public class SecuritySystem
    {
        public bool IsArmed { get; private set; }

        public void Arm()
        {
            IsArmed = true;
            Console.WriteLine("  Security system is ARMED");
        }

        public void Disarm()
        {
            IsArmed = false;
            Console.WriteLine("  Security system is DISARMED");
        }
    }

    // Smart Home Commands
    public class LightOnCommand : ICommand
    {
        private readonly Light _light;

        public LightOnCommand(Light light) => _light = light;

        public void Execute() => _light.TurnOn();
        public void Undo() => _light.TurnOff();
    }

    public class LightOffCommand : ICommand
    {
        private readonly Light _light;

        public LightOffCommand(Light light) => _light = light;

        public void Execute() => _light.TurnOff();
        public void Undo() => _light.TurnOn();
    }

    public class AirConditionerOnCommand : ICommand
    {
        private readonly AirConditioner _ac;

        public AirConditionerOnCommand(AirConditioner ac) => _ac = ac;

        public void Execute() => _ac.TurnOn();
        public void Undo() => _ac.TurnOff();
    }

    public class AirConditionerOffCommand : ICommand
    {
        private readonly AirConditioner _ac;

        public AirConditionerOffCommand(AirConditioner ac) => _ac = ac;

        public void Execute() => _ac.TurnOff();
        public void Undo() => _ac.TurnOn();
    }

    public class SecuritySystemArmCommand : ICommand
    {
        private readonly SecuritySystem _system;

        public SecuritySystemArmCommand(SecuritySystem system) => _system = system;

        public void Execute() => _system.Arm();
        public void Undo() => _system.Disarm();
    }

    public class SecuritySystemDisarmCommand : ICommand
    {
        private readonly SecuritySystem _system;

        public SecuritySystemDisarmCommand(SecuritySystem system) => _system = system;

        public void Execute() => _system.Disarm();
        public void Undo() => _system.Arm();
    }

    // Remote Control (Invoker)
    public class RemoteControl
    {
        private readonly ICommand[] _onCommands = new ICommand[7];
        private readonly ICommand[] _offCommands = new ICommand[7];
        private ICommand _undoCommand = new NoOpCommand();

        public RemoteControl()
        {
            var noOpCommand = new NoOpCommand();
            for (int i = 0; i < 7; i++)
            {
                _onCommands[i] = noOpCommand;
                _offCommands[i] = noOpCommand;
            }
        }

        public void SetCommand(int slot, ICommand onCommand, ICommand offCommand)
        {
            _onCommands[slot] = onCommand;
            _offCommands[slot] = offCommand;
        }

        public void OnButtonPressed(int slot)
        {
            _onCommands[slot].Execute();
            _undoCommand = _onCommands[slot];
        }

        public void OffButtonPressed(int slot)
        {
            _offCommands[slot].Execute();
            _undoCommand = _offCommands[slot];
        }

        public void UndoButtonPressed()
        {
            _undoCommand.Undo();
        }
    }

    #endregion

    #region Database Transaction Example

    public class Database
    {
        private readonly Dictionary<string, List<string>> _tables = new Dictionary<string, List<string>>();
        private int _nextId = 1;

        public int InsertRecord(string table, string data)
        {
            if (!_tables.ContainsKey(table))
                _tables[table] = new List<string>();

            var id = _nextId++;
            _tables[table].Add($"ID:{id} - {data}");
            Console.WriteLine($"  Inserted record {id} in table '{table}': {data}");
            return id;
        }

        public void UpdateRecord(string table, int id, string newData)
        {
            if (_tables.ContainsKey(table))
            {
                var record = _tables[table].FirstOrDefault(r => r.StartsWith($"ID:{id}"));
                if (record != null)
                {
                    var index = _tables[table].IndexOf(record);
                    _tables[table][index] = $"ID:{id} - {newData}";
                    Console.WriteLine($"  Updated record {id} in table '{table}': {newData}");
                }
            }
        }

        public void DeleteRecord(string table, int id)
        {
            if (_tables.ContainsKey(table))
            {
                var record = _tables[table].FirstOrDefault(r => r.StartsWith($"ID:{id}"));
                if (record != null)
                {
                    _tables[table].Remove(record);
                    Console.WriteLine($"  Deleted record {id} from table '{table}'");
                }
            }
        }

        public void PrintContents()
        {
            Console.WriteLine("Database contents:");
            foreach (var table in _tables)
            {
                Console.WriteLine($"  Table '{table.Key}':");
                foreach (var record in table.Value)
                {
                    Console.WriteLine($"    {record}");
                }
            }
        }
    }

    // Database Commands
    public class InsertRecordCommand : ICommand
    {
        private readonly Database _database;
        private readonly string _table;
        private readonly string _data;
        private int _insertedId;

        public InsertRecordCommand(Database database, string table, string data)
        {
            _database = database;
            _table = table;
            _data = data;
        }

        public void Execute()
        {
            _insertedId = _database.InsertRecord(_table, _data);
        }

        public void Undo()
        {
            _database.DeleteRecord(_table, _insertedId);
        }
    }

    public class UpdateRecordCommand : ICommand
    {
        private readonly Database _database;
        private readonly string _table;
        private readonly int _id;
        private readonly string _newData;
        private string _oldData = string.Empty;

        public UpdateRecordCommand(Database database, string table, int id, string newData)
        {
            _database = database;
            _table = table;
            _id = id;
            _newData = newData;
        }

        public void Execute()
        {
            // In a real implementation, you'd store the old data first
            _database.UpdateRecord(_table, _id, _newData);
        }

        public void Undo()
        {
            // In a real implementation, you'd restore the old data
            Console.WriteLine($"  Would restore old data for record {_id}");
        }
    }

    // Transaction Command (Composite)
    public class DatabaseTransaction : ICommand
    {
        private readonly List<ICommand> _commands = new List<ICommand>();

        public void AddCommand(ICommand command)
        {
            _commands.Add(command);
        }

        public void Execute()
        {
            foreach (var command in _commands)
            {
                command.Execute();
            }
        }

        public void Undo()
        {
            // Undo in reverse order
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }
        }
    }

    public class TransactionManager
    {
        public void ExecuteTransaction(DatabaseTransaction transaction)
        {
            try
            {
                transaction.Execute();
            }
            catch
            {
                transaction.Undo();
                throw;
            }
        }

        public void RollbackTransaction(DatabaseTransaction transaction)
        {
            transaction.Undo();
        }
    }

    #endregion

    #region Macro Command Example

    // Macro Command (Composite Command)
    public class MacroCommand : ICommand
    {
        private readonly List<ICommand> _commands = new List<ICommand>();
        private readonly string _name;

        public MacroCommand(string name)
        {
            _name = name;
        }

        public void AddCommand(ICommand command)
        {
            _commands.Add(command);
        }

        public void Execute()
        {
            Console.WriteLine($"  Executing macro: {_name}");
            foreach (var command in _commands)
            {
                command.Execute();
            }
        }

        public void Undo()
        {
            Console.WriteLine($"  Undoing macro: {_name}");
            // Undo in reverse order
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }
        }
    }

    #endregion

    #region Thread-Safe Command Queue Example

    // MULTITHREAD ASPECTS: Thread-safe command queue for concurrent execution
    public class ThreadSafeCommandQueue
    {
        private readonly ConcurrentQueue<ICommand> _commandQueue = new ConcurrentQueue<ICommand>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        public void EnqueueCommand(ICommand command)
        {
            _commandQueue.Enqueue(command);
            _semaphore.Release(); // Signal that a command is available
            Console.WriteLine($"  Enqueued command: {command.GetType().Name}");
        }

        public async Task StartProcessing()
        {
            await Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        await _semaphore.WaitAsync(_cancellationTokenSource.Token);

                        if (_commandQueue.TryDequeue(out var command))
                        {
                            Console.WriteLine($"  Processing command: {command.GetType().Name}");
                            command.Execute();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Error processing command: {ex.Message}");
                    }
                }
            });
        }

        public void StopProcessing()
        {
            _cancellationTokenSource.Cancel();
        }
    }

    public class SharedResource
    {
        private int _value = 0;
        private readonly object _lock = new object();

        public void Increment(string commandId)
        {
            lock (_lock)
            {
                _value++;
                Console.WriteLine($"    {commandId}: Incremented shared resource to {_value}");
            }
        }

        public int GetValue()
        {
            lock (_lock)
            {
                return _value;
            }
        }
    }

    public class SharedResourceCommand : ICommand
    {
        private readonly SharedResource _resource;
        private readonly string _commandId;

        public SharedResourceCommand(SharedResource resource, string commandId)
        {
            _resource = resource;
            _commandId = commandId;
        }

        public void Execute()
        {
            _resource.Increment(_commandId);
        }

        public void Undo()
        {
            // In a real implementation, you might decrement or restore previous state
            Console.WriteLine($"    Undo not implemented for {_commandId}");
        }
    }

    #endregion

    #region Async Commands Example

    public class FileProcessor
    {
        public async Task ProcessFileAsync(string fileName, int processingTimeMs, CancellationToken cancellationToken)
        {
            Console.WriteLine($"  Starting to process {fileName}...");
            await Task.Delay(processingTimeMs, cancellationToken);
            Console.WriteLine($"  Completed processing {fileName}");
        }
    }

    public class AsyncFileProcessCommand : IAsyncCommand
    {
        private readonly FileProcessor _processor;
        private readonly string _fileName;
        private readonly int _processingTime;

        public AsyncFileProcessCommand(FileProcessor processor, string fileName, int processingTime)
        {
            _processor = processor;
            _fileName = fileName;
            _processingTime = processingTime;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _processor.ProcessFileAsync(_fileName, _processingTime, cancellationToken);
        }

        public async Task UndoAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"  Undoing processing of {_fileName}");
            await Task.CompletedTask;
        }
    }

    public class AsyncCommandExecutor
    {
        public async Task ExecuteCommandsAsync(IAsyncCommand[] commands, CancellationToken cancellationToken = default)
        {
            var tasks = commands.Select(cmd => cmd.ExecuteAsync(cancellationToken)).ToArray();
            await Task.WhenAll(tasks);
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. COMMAND OBJECT CREATION:
 *    - Each command creates an object that encapsulates the operation
 *    - For high-frequency operations, consider object pooling
 *    - Use flyweight pattern for commands with shared state
 * 
 * 2. COMMAND HISTORY:
 *    - Undo/redo stacks can grow unbounded without limits
 *    - Implement maximum history size to prevent memory leaks
 *    - Consider using weak references for large command objects
 * 
 * 3. COMPOSITE COMMANDS:
 *    - Macro commands hold references to multiple child commands
 *    - Can create deep object hierarchies with significant memory usage
 *    - Consider lazy evaluation for infrequently executed macros
 * 
 * 4. QUEUED COMMANDS:
 *    - Command queues can accumulate commands faster than they're processed
 *    - Implement backpressure mechanisms and queue size limits
 *    - Monitor memory usage in long-running applications
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. COMMAND EXECUTION:
 *    - Commands themselves should be stateless or thread-safe
 *    - Receivers (target objects) may need synchronization
 *    - Consider using concurrent collections for command storage
 * 
 * 2. UNDO/REDO OPERATIONS:
 *    - Command history manipulation is not thread-safe by default
 *    - Use locks or concurrent collections for thread-safe history
 *    - Be careful with race conditions between execute and undo
 * 
 * 3. ASYNC COMMANDS:
 *    - Support cancellation tokens for long-running operations
 *    - Handle exceptions properly in async command execution
 *    - Consider using Task.WhenAll for parallel command execution
 * 
 * 4. COMMAND QUEUES:
 *    - Use thread-safe queues (ConcurrentQueue) for multi-producer scenarios
 *    - Implement proper synchronization for queue processing
 *    - Consider using channels for advanced producer-consumer scenarios
 * 
 * 5. BEST PRACTICES:
 *    - Design commands to be immutable when possible
 *    - Avoid shared mutable state in command objects
 *    - Use dependency injection for testability
 *    - Implement proper error handling and rollback mechanisms
 *    - Consider using the actor model for complex command coordination
 */
