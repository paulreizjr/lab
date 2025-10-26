using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;

/*
 * COMPOSITE DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Composite pattern composes objects into tree structures to represent part-whole hierarchies.
 * It allows clients to treat individual objects and compositions of objects uniformly, enabling
 * recursive composition and simplifying client code that works with complex tree structures.
 * 
 * CORE BENEFITS:
 * - Represents part-whole hierarchies as tree structures
 * - Enables uniform treatment of individual and composite objects
 * - Simplifies client code by eliminating conditional checks for object types
 * - Makes it easy to add new types of components without changing existing code
 * - Supports recursive operations on tree structures naturally
 * - Provides flexibility in building complex structures from simple components
 * - Enables polymorphic behavior across the entire hierarchy
 * - Facilitates implementation of operations that traverse the entire structure
 * 
 * SCENARIOS TO USE:
 * - File system hierarchies (files and directories)
 * - UI component trees (windows, panels, buttons, controls)
 * - Organizational structures (departments, teams, employees)
 * - Document object models (paragraphs, sections, pages)
 * - Menu systems with nested submenus
 * - Mathematical expressions with operators and operands
 * - Graphic design systems with shapes and groups
 * - Command structures with macro commands containing sub-commands
 * - Configuration hierarchies with sections and settings
 * - Game object hierarchies (scenes, game objects, components)
 * - Database query trees with conditions and operators
 * - Software architecture models with systems and subsystems
 * 
 * SCENARIOS NOT TO USE:
 * - Simple flat structures without hierarchical relationships
 * - When individual and composite objects have significantly different interfaces
 * - Performance-critical systems where tree traversal overhead is unacceptable
 * - When the hierarchy structure is very deep and causes stack overflow risks
 * - Systems where object types need to be explicitly differentiated
 * - When the pattern adds unnecessary complexity to simple operations
 * - Memory-constrained environments where tree structures are too expensive
 * - When direct access to specific child types is frequently required
 */

namespace CompositePattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Composite Design Pattern Examples ===\n");

            // Example 1: File System Hierarchy
            FileSystemExample();

            // Example 2: UI Component Tree
            UIComponentExample();

            // Example 3: Organizational Structure
            OrganizationalExample();

            // Example 4: Menu System
            MenuSystemExample();

            // Example 5: Mathematical Expressions
            MathematicalExpressionExample();

            // Example 6: Thread-Safe Composite Operations
            await ThreadSafeCompositeExample();

            // Example 7: Document Structure
            DocumentStructureExample();

            // Example 8: Graphics Composition
            GraphicsCompositionExample();

            // Example 9: Command Hierarchy
            CommandHierarchyExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: File System Hierarchy

        static void FileSystemExample()
        {
            Console.WriteLine("1. File System Hierarchy:");
            Console.WriteLine("==========================");

            // Create a file system structure
            var root = new Directory("C:");
            var programFiles = new Directory("Program Files");
            var windows = new Directory("Windows");
            var users = new Directory("Users");

            root.Add(programFiles);
            root.Add(windows);
            root.Add(users);

            // Add files and subdirectories
            programFiles.Add(new File("app.exe", 1024));
            programFiles.Add(new File("readme.txt", 256));

            var system32 = new Directory("System32");
            windows.Add(system32);
            system32.Add(new File("notepad.exe", 2048));
            system32.Add(new File("calc.exe", 1536));

            var userFolder = new Directory("John");
            users.Add(userFolder);
            userFolder.Add(new File("document.docx", 4096));
            userFolder.Add(new File("photo.jpg", 8192));

            // Display the structure
            Console.WriteLine("File system structure:");
            root.Display(0);

            Console.WriteLine($"\nTotal size: {root.GetSize()} bytes");
            Console.WriteLine($"Total files: {root.GetFileCount()}");

            Console.WriteLine();
        }

        #endregion

        #region Example 2: UI Component Tree

        static void UIComponentExample()
        {
            Console.WriteLine("2. UI Component Tree:");
            Console.WriteLine("======================");

            // Create a UI hierarchy
            var window = new Window("Main Window", 800, 600);
            var panel = new Panel("Content Panel", 780, 550);
            var toolbar = new Panel("Toolbar", 780, 50);

            window.Add(panel);
            window.Add(toolbar);

            // Add controls to panel
            panel.Add(new Button("OK", 100, 30));
            panel.Add(new Button("Cancel", 100, 30));
            panel.Add(new TextBox("Input", 200, 25));

            // Add toolbar items
            toolbar.Add(new Button("New", 80, 25));
            toolbar.Add(new Button("Open", 80, 25));
            toolbar.Add(new Button("Save", 80, 25));

            // Create a nested panel structure
            var leftPanel = new Panel("Left Panel", 200, 500);
            var rightPanel = new Panel("Right Panel", 560, 500);
            panel.Add(leftPanel);
            panel.Add(rightPanel);

            leftPanel.Add(new Button("Button 1", 180, 30));
            leftPanel.Add(new Button("Button 2", 180, 30));

            rightPanel.Add(new TextBox("Large Text Area", 540, 400));

            // Render the UI hierarchy
            Console.WriteLine("UI Component hierarchy:");
            window.Render();

            Console.WriteLine($"\nTotal components: {window.GetComponentCount()}");
            Console.WriteLine($"Total bounds area: {window.GetTotalArea()}");

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Organizational Structure

        static void OrganizationalExample()
        {
            Console.WriteLine("3. Organizational Structure:");
            Console.WriteLine("=============================");

            // Create organizational hierarchy
            var company = new Department("TechCorp", "CEO Office");
            var engineering = new Department("Engineering", "CTO Office");
            var marketing = new Department("Marketing", "CMO Office");
            var sales = new Department("Sales", "Sales Office");

            company.Add(engineering);
            company.Add(marketing);
            company.Add(sales);

            // Add engineering teams
            var backend = new Department("Backend Team", "Backend Office");
            var frontend = new Department("Frontend Team", "Frontend Office");
            var devops = new Department("DevOps Team", "DevOps Office");

            engineering.Add(backend);
            engineering.Add(frontend);
            engineering.Add(devops);

            // Add employees
            backend.Add(new Employee("Alice Johnson", "Senior Developer", 95000));
            backend.Add(new Employee("Bob Smith", "Junior Developer", 65000));
            
            frontend.Add(new Employee("Carol Davis", "UI/UX Designer", 75000));
            frontend.Add(new Employee("David Wilson", "Frontend Developer", 80000));

            marketing.Add(new Employee("Eve Brown", "Marketing Manager", 85000));
            sales.Add(new Employee("Frank Miller", "Sales Representative", 70000));

            // Display organizational structure
            Console.WriteLine("Organizational structure:");
            company.ShowStructure(0);

            Console.WriteLine($"\nTotal employees: {company.GetEmployeeCount()}");
            Console.WriteLine($"Total salary budget: ${company.GetTotalSalary():N0}");

            Console.WriteLine();
        }

        #endregion

        #region Example 4: Menu System

        static void MenuSystemExample()
        {
            Console.WriteLine("4. Menu System:");
            Console.WriteLine("================");

            // Create menu hierarchy
            var mainMenu = new Menu("Main Menu");
            var fileMenu = new Menu("File");
            var editMenu = new Menu("Edit");
            var viewMenu = new Menu("View");

            mainMenu.Add(fileMenu);
            mainMenu.Add(editMenu);
            mainMenu.Add(viewMenu);

            // Add file menu items
            fileMenu.Add(new MenuItem("New", "Ctrl+N", () => Console.WriteLine("  Creating new file...")));
            fileMenu.Add(new MenuItem("Open", "Ctrl+O", () => Console.WriteLine("  Opening file...")));
            fileMenu.Add(new MenuItem("Save", "Ctrl+S", () => Console.WriteLine("  Saving file...")));

            var recentMenu = new Menu("Recent Files");
            fileMenu.Add(recentMenu);
            recentMenu.Add(new MenuItem("Document1.txt", "", () => Console.WriteLine("  Opening Document1.txt")));
            recentMenu.Add(new MenuItem("Document2.txt", "", () => Console.WriteLine("  Opening Document2.txt")));

            // Add edit menu items
            editMenu.Add(new MenuItem("Cut", "Ctrl+X", () => Console.WriteLine("  Cutting text...")));
            editMenu.Add(new MenuItem("Copy", "Ctrl+C", () => Console.WriteLine("  Copying text...")));
            editMenu.Add(new MenuItem("Paste", "Ctrl+V", () => Console.WriteLine("  Pasting text...")));

            // Add view menu items
            viewMenu.Add(new MenuItem("Zoom In", "Ctrl++", () => Console.WriteLine("  Zooming in...")));
            viewMenu.Add(new MenuItem("Zoom Out", "Ctrl+-", () => Console.WriteLine("  Zooming out...")));

            // Display menu structure
            Console.WriteLine("Menu structure:");
            mainMenu.Display();

            Console.WriteLine("\nExecuting some menu commands:");
            // Execute some commands (simulate user clicks)
            fileMenu.GetMenuItem("New")?.Execute();
            editMenu.GetMenuItem("Copy")?.Execute();
            recentMenu.GetMenuItem("Document1.txt")?.Execute();

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Mathematical Expressions

        static void MathematicalExpressionExample()
        {
            Console.WriteLine("5. Mathematical Expressions:");
            Console.WriteLine("=============================");

            // Build expression: (5 + 3) * (10 - 2) / 4
            var five = new Number(5);
            var three = new Number(3);
            var ten = new Number(10);
            var two = new Number(2);
            var four = new Number(4);

            var addition = new BinaryOperation(five, three, BinaryOperation.OperationType.Add);
            var subtraction = new BinaryOperation(ten, two, BinaryOperation.OperationType.Subtract);
            var multiplication = new BinaryOperation(addition, subtraction, BinaryOperation.OperationType.Multiply);
            var division = new BinaryOperation(multiplication, four, BinaryOperation.OperationType.Divide);

            Console.WriteLine($"Expression: {division.ToString()}");
            Console.WriteLine($"Result: {division.Evaluate()}");

            // Build more complex expression: sqrt((3 + 2) * 5)
            var threeNum = new Number(3);
            var twoNum = new Number(2);
            var fiveNum = new Number(5);

            var addOperation = new BinaryOperation(threeNum, twoNum, BinaryOperation.OperationType.Add);
            var multiplyOperation = new BinaryOperation(addOperation, fiveNum, BinaryOperation.OperationType.Multiply);
            var sqrtOperation = new UnaryOperation(multiplyOperation, UnaryOperation.OperationType.Sqrt);

            Console.WriteLine($"\nExpression: {sqrtOperation.ToString()}");
            Console.WriteLine($"Result: {sqrtOperation.Evaluate():F2}");

            // Show expression tree structure
            Console.WriteLine("\nExpression tree structure:");
            division.DisplayStructure(0);

            Console.WriteLine();
        }

        #endregion

        #region Example 6: Thread-Safe Composite Operations

        static async Task ThreadSafeCompositeExample()
        {
            Console.WriteLine("6. Thread-Safe Composite Operations:");
            Console.WriteLine("=====================================");

            var taskManager = new ThreadSafeTaskContainer("Main Tasks");
            var parallelTasks = new ThreadSafeTaskContainer("Parallel Tasks");
            var sequentialTasks = new ThreadSafeTaskContainer("Sequential Tasks");

            taskManager.Add(parallelTasks);
            taskManager.Add(sequentialTasks);

            // Add individual tasks
            for (int i = 1; i <= 3; i++)
            {
                int taskId = i;
                parallelTasks.Add(new ThreadSafeTask($"Parallel Task {taskId}", async () =>
                {
                    Console.WriteLine($"  Starting Parallel Task {taskId}");
                    await Task.Delay(1000 + (taskId * 200));
                    Console.WriteLine($"  Completed Parallel Task {taskId}");
                    return $"Result from Task {taskId}";
                }));
            }

            for (int i = 1; i <= 2; i++)
            {
                int taskId = i;
                sequentialTasks.Add(new ThreadSafeTask($"Sequential Task {taskId}", async () =>
                {
                    Console.WriteLine($"  Executing Sequential Task {taskId}");
                    await Task.Delay(500);
                    Console.WriteLine($"  Finished Sequential Task {taskId}");
                    return $"Sequential Result {taskId}";
                }));
            }

            // Execute tasks with thread safety
            Console.WriteLine("Executing composite task structure:");
            var results = await taskManager.ExecuteAsync();

            Console.WriteLine("\nTask execution results:");
            foreach (var result in results)
            {
                Console.WriteLine($"  - {result}");
            }

            Console.WriteLine($"\nTotal tasks executed: {taskManager.GetTaskCount()}");
            Console.WriteLine();
        }

        #endregion

        #region Example 7: Document Structure

        static void DocumentStructureExample()
        {
            Console.WriteLine("7. Document Structure:");
            Console.WriteLine("=======================");

            // Create document hierarchy
            var document = new Document("Technical Report");
            var chapter1 = new Chapter("Introduction");
            var chapter2 = new Chapter("Implementation");
            var chapter3 = new Chapter("Conclusion");

            document.Add(chapter1);
            document.Add(chapter2);
            document.Add(chapter3);

            // Add content to chapters
            chapter1.Add(new Paragraph("This document describes the implementation of design patterns."));
            chapter1.Add(new Paragraph("Design patterns provide reusable solutions to common problems."));

            var section1 = new Section("Architecture Overview");
            chapter2.Add(section1);
            section1.Add(new Paragraph("The system follows a layered architecture approach."));
            section1.Add(new Paragraph("Each layer has specific responsibilities and interfaces."));

            var section2 = new Section("Implementation Details");
            chapter2.Add(section2);
            section2.Add(new Paragraph("The composite pattern is used for hierarchical structures."));

            chapter3.Add(new Paragraph("The implementation successfully demonstrates the composite pattern."));
            chapter3.Add(new Paragraph("Future work could include additional pattern implementations."));

            // Display document structure
            Console.WriteLine("Document structure:");
            document.Display(0);

            Console.WriteLine($"\nDocument statistics:");
            Console.WriteLine($"  Total words: {document.GetWordCount()}");
            Console.WriteLine($"  Total characters: {document.GetCharacterCount()}");
            Console.WriteLine($"  Reading time: {document.GetReadingTime()} minutes");

            Console.WriteLine();
        }

        #endregion

        #region Example 8: Graphics Composition

        static void GraphicsCompositionExample()
        {
            Console.WriteLine("8. Graphics Composition:");
            Console.WriteLine("=========================");

            // Create graphics hierarchy
            var canvas = new GraphicsGroup("Main Canvas");
            var background = new GraphicsGroup("Background Layer");
            var foreground = new GraphicsGroup("Foreground Layer");

            canvas.Add(background);
            canvas.Add(foreground);

            // Add background elements
            background.Add(new Rectangle("Background Rect", 0, 0, 800, 600));
            background.Add(new Circle("Background Circle", 400, 300, 100));

            // Add foreground elements
            var buttonGroup = new GraphicsGroup("Button Group");
            foreground.Add(buttonGroup);

            buttonGroup.Add(new Rectangle("Button Background", 350, 250, 100, 50));
            buttonGroup.Add(new TextElement("OK", 375, 270));

            var logoGroup = new GraphicsGroup("Logo Group");
            foreground.Add(logoGroup);

            logoGroup.Add(new Circle("Logo Circle", 100, 100, 50));
            logoGroup.Add(new TextElement("LOGO", 85, 105));

            // Add individual elements
            foreground.Add(new Line("Border Line", 0, 0, 800, 0));
            foreground.Add(new Line("Border Line", 0, 600, 800, 600));

            // Render graphics hierarchy
            Console.WriteLine("Graphics composition:");
            canvas.Render();

            Console.WriteLine($"\nGraphics statistics:");
            Console.WriteLine($"  Total elements: {canvas.GetElementCount()}");
            Console.WriteLine($"  Bounding box: {canvas.GetBoundingBox()}");

            // Apply transformations
            Console.WriteLine("\nApplying transformations:");
            canvas.Transform(1.2, 1.2, 50, 50); // Scale and translate

            Console.WriteLine();
        }

        #endregion

        #region Example 9: Command Hierarchy

        static void CommandHierarchyExample()
        {
            Console.WriteLine("9. Command Hierarchy:");
            Console.WriteLine("======================");

            // Create command hierarchy
            var macroCommand = new MacroCommand("Initialization Sequence");
            var setupCommands = new MacroCommand("Setup Commands");
            var configCommands = new MacroCommand("Configuration Commands");

            macroCommand.Add(setupCommands);
            macroCommand.Add(configCommands);

            // Add individual commands
            setupCommands.Add(new SimpleCommand("Initialize Database", () => 
                Console.WriteLine("  Database initialized")));
            setupCommands.Add(new SimpleCommand("Load Configuration", () => 
                Console.WriteLine("  Configuration loaded")));
            setupCommands.Add(new SimpleCommand("Start Services", () => 
                Console.WriteLine("  Services started")));

            configCommands.Add(new SimpleCommand("Set Log Level", () => 
                Console.WriteLine("  Log level set to INFO")));
            configCommands.Add(new SimpleCommand("Configure Cache", () => 
                Console.WriteLine("  Cache configured")));

            // Add cleanup commands
            var cleanupCommands = new MacroCommand("Cleanup Commands");
            macroCommand.Add(cleanupCommands);

            cleanupCommands.Add(new SimpleCommand("Clear Temp Files", () => 
                Console.WriteLine("  Temporary files cleared")));
            cleanupCommands.Add(new SimpleCommand("Update Status", () => 
                Console.WriteLine("  Status updated")));

            // Display command structure
            Console.WriteLine("Command hierarchy:");
            macroCommand.Display(0);

            // Execute command hierarchy
            Console.WriteLine("\nExecuting command hierarchy:");
            macroCommand.Execute();

            Console.WriteLine($"\nTotal commands executed: {macroCommand.GetCommandCount()}");

            // Demonstrate undo functionality
            Console.WriteLine("\nUndoing commands:");
            macroCommand.Undo();

            Console.WriteLine();
        }

        #endregion
    }

    #region Core Composite Pattern Classes

    // Component interface
    public abstract class Component
    {
        protected string name;

        public Component(string name)
        {
            this.name = name;
        }

        public abstract void Operation();
        public virtual void Add(Component component) => throw new NotSupportedException();
        public virtual void Remove(Component component) => throw new NotSupportedException();
        public virtual Component GetChild(int index) => throw new NotSupportedException();
        public virtual int GetChildCount() => 0;
    }

    // Leaf
    public class Leaf : Component
    {
        public Leaf(string name) : base(name) { }

        public override void Operation()
        {
            Console.WriteLine($"Leaf {name} operation");
        }
    }

    // Composite
    public class Composite : Component
    {
        private readonly List<Component> children = new List<Component>();

        public Composite(string name) : base(name) { }

        public override void Operation()
        {
            Console.WriteLine($"Composite {name} operation");
            foreach (var child in children)
            {
                child.Operation();
            }
        }

        public override void Add(Component component)
        {
            children.Add(component);
        }

        public override void Remove(Component component)
        {
            children.Remove(component);
        }

        public override Component GetChild(int index)
        {
            return children[index];
        }

        public override int GetChildCount()
        {
            return children.Count;
        }
    }

    #endregion

    #region Example 1: File System Classes

    public abstract class FileSystemItem
    {
        protected string name;

        public FileSystemItem(string name)
        {
            this.name = name;
        }

        public string Name => name;
        public abstract long GetSize();
        public abstract void Display(int depth);
        public abstract int GetFileCount();
    }

    public class File : FileSystemItem
    {
        private readonly long size;

        public File(string name, long size) : base(name)
        {
            this.size = size;
        }

        public override long GetSize()
        {
            return size;
        }

        public override void Display(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}📄 {name} ({size} bytes)");
        }

        public override int GetFileCount()
        {
            return 1;
        }
    }

    public class Directory : FileSystemItem
    {
        private readonly List<FileSystemItem> items = new List<FileSystemItem>();

        public Directory(string name) : base(name) { }

        public void Add(FileSystemItem item)
        {
            items.Add(item);
        }

        public void Remove(FileSystemItem item)
        {
            items.Remove(item);
        }

        public override long GetSize()
        {
            return items.Sum(item => item.GetSize());
        }

        public override void Display(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}📁 {name}/");
            foreach (var item in items)
            {
                item.Display(depth + 1);
            }
        }

        public override int GetFileCount()
        {
            return items.Sum(item => item.GetFileCount());
        }
    }

    #endregion

    #region Example 2: UI Component Classes

    public abstract class UIComponent
    {
        protected string name;
        protected int width, height;

        public UIComponent(string name, int width, int height)
        {
            this.name = name;
            this.width = width;
            this.height = height;
        }

        public string Name => name;
        public int Width => width;
        public int Height => height;

        public abstract void Render();
        public abstract int GetComponentCount();
        public abstract int GetTotalArea();
    }

    public class Button : UIComponent
    {
        public Button(string name, int width, int height) : base(name, width, height) { }

        public override void Render()
        {
            Console.WriteLine($"  🔘 Button: {name} ({width}x{height})");
        }

        public override int GetComponentCount()
        {
            return 1;
        }

        public override int GetTotalArea()
        {
            return width * height;
        }
    }

    public class TextBox : UIComponent
    {
        public TextBox(string name, int width, int height) : base(name, width, height) { }

        public override void Render()
        {
            Console.WriteLine($"  📝 TextBox: {name} ({width}x{height})");
        }

        public override int GetComponentCount()
        {
            return 1;
        }

        public override int GetTotalArea()
        {
            return width * height;
        }
    }

    public class Panel : UIComponent
    {
        private readonly List<UIComponent> components = new List<UIComponent>();

        public Panel(string name, int width, int height) : base(name, width, height) { }

        public void Add(UIComponent component)
        {
            components.Add(component);
        }

        public void Remove(UIComponent component)
        {
            components.Remove(component);
        }

        public override void Render()
        {
            Console.WriteLine($"  📦 Panel: {name} ({width}x{height})");
            foreach (var component in components)
            {
                component.Render();
            }
        }

        public override int GetComponentCount()
        {
            return 1 + components.Sum(c => c.GetComponentCount());
        }

        public override int GetTotalArea()
        {
            return width * height + components.Sum(c => c.GetTotalArea());
        }
    }

    public class Window : UIComponent
    {
        private readonly List<UIComponent> components = new List<UIComponent>();

        public Window(string name, int width, int height) : base(name, width, height) { }

        public void Add(UIComponent component)
        {
            components.Add(component);
        }

        public void Remove(UIComponent component)
        {
            components.Remove(component);
        }

        public override void Render()
        {
            Console.WriteLine($"🪟 Window: {name} ({width}x{height})");
            foreach (var component in components)
            {
                component.Render();
            }
        }

        public override int GetComponentCount()
        {
            return 1 + components.Sum(c => c.GetComponentCount());
        }

        public override int GetTotalArea()
        {
            return width * height + components.Sum(c => c.GetTotalArea());
        }
    }

    #endregion

    #region Example 3: Organizational Classes

    public abstract class OrganizationalUnit
    {
        protected string name;
        protected string location;

        public OrganizationalUnit(string name, string location)
        {
            this.name = name;
            this.location = location;
        }

        public string Name => name;
        public string Location => location;

        public abstract void ShowStructure(int depth);
        public abstract int GetEmployeeCount();
        public abstract decimal GetTotalSalary();
    }

    public class Employee : OrganizationalUnit
    {
        private readonly string position;
        private readonly decimal salary;

        public Employee(string name, string position, decimal salary) : base(name, "")
        {
            this.position = position;
            this.salary = salary;
        }

        public string Position => position;
        public decimal Salary => salary;

        public override void ShowStructure(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}👤 {name} - {position} (${salary:N0})");
        }

        public override int GetEmployeeCount()
        {
            return 1;
        }

        public override decimal GetTotalSalary()
        {
            return salary;
        }
    }

    public class Department : OrganizationalUnit
    {
        private readonly List<OrganizationalUnit> members = new List<OrganizationalUnit>();

        public Department(string name, string location) : base(name, location) { }

        public void Add(OrganizationalUnit unit)
        {
            members.Add(unit);
        }

        public void Remove(OrganizationalUnit unit)
        {
            members.Remove(unit);
        }

        public override void ShowStructure(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}🏢 {name} ({location})");
            foreach (var member in members)
            {
                member.ShowStructure(depth + 1);
            }
        }

        public override int GetEmployeeCount()
        {
            return members.Sum(m => m.GetEmployeeCount());
        }

        public override decimal GetTotalSalary()
        {
            return members.Sum(m => m.GetTotalSalary());
        }
    }

    #endregion

    #region Example 4: Menu System Classes

    public abstract class MenuComponent
    {
        protected string name;

        public MenuComponent(string name)
        {
            this.name = name;
        }

        public string Name => name;
        public abstract void Display();
        public virtual void Execute() => throw new NotSupportedException();
        public virtual MenuComponent? GetMenuItem(string name) => null;
    }

    public class MenuItem : MenuComponent
    {
        private readonly string shortcut;
        private readonly Action action;

        public MenuItem(string name, string shortcut, Action action) : base(name)
        {
            this.shortcut = shortcut;
            this.action = action;
        }

        public override void Display()
        {
            var shortcutText = string.IsNullOrEmpty(shortcut) ? "" : $" ({shortcut})";
            Console.WriteLine($"    📋 {name}{shortcutText}");
        }

        public override void Execute()
        {
            action?.Invoke();
        }

        public override MenuComponent? GetMenuItem(string name)
        {
            return this.name == name ? this : null;
        }
    }

    public class Menu : MenuComponent
    {
        private readonly List<MenuComponent> menuItems = new List<MenuComponent>();

        public Menu(string name) : base(name) { }

        public void Add(MenuComponent item)
        {
            menuItems.Add(item);
        }

        public void Remove(MenuComponent item)
        {
            menuItems.Remove(item);
        }

        public override void Display()
        {
            Console.WriteLine($"  📁 {name}");
            foreach (var item in menuItems)
            {
                item.Display();
            }
        }

        public override MenuComponent? GetMenuItem(string name)
        {
            if (this.name == name) return this;

            foreach (var item in menuItems)
            {
                var found = item.GetMenuItem(name);
                if (found != null) return found;
            }
            return null;
        }
    }

    #endregion

    #region Example 5: Mathematical Expression Classes

    public abstract class Expression
    {
        public abstract double Evaluate();
        public abstract void DisplayStructure(int depth);
    }

    public class Number : Expression
    {
        private readonly double value;

        public Number(double value)
        {
            this.value = value;
        }

        public override double Evaluate()
        {
            return value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public override void DisplayStructure(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}🔢 {value}");
        }
    }

    public class BinaryOperation : Expression
    {
        public enum OperationType { Add, Subtract, Multiply, Divide }

        private readonly Expression left;
        private readonly Expression right;
        private readonly OperationType operation;

        public BinaryOperation(Expression left, Expression right, OperationType operation)
        {
            this.left = left;
            this.right = right;
            this.operation = operation;
        }

        public override double Evaluate()
        {
            var leftValue = left.Evaluate();
            var rightValue = right.Evaluate();

            return operation switch
            {
                OperationType.Add => leftValue + rightValue,
                OperationType.Subtract => leftValue - rightValue,
                OperationType.Multiply => leftValue * rightValue,
                OperationType.Divide => rightValue != 0 ? leftValue / rightValue : throw new DivideByZeroException(),
                _ => throw new InvalidOperationException("Unknown operation")
            };
        }

        public override string ToString()
        {
            var op = operation switch
            {
                OperationType.Add => "+",
                OperationType.Subtract => "-",
                OperationType.Multiply => "*",
                OperationType.Divide => "/",
                _ => "?"
            };
            return $"({left} {op} {right})";
        }

        public override void DisplayStructure(int depth)
        {
            var op = operation switch
            {
                OperationType.Add => "➕",
                OperationType.Subtract => "➖",
                OperationType.Multiply => "✖️",
                OperationType.Divide => "➗",
                _ => "❓"
            };
            Console.WriteLine($"{new string(' ', depth * 2)}{op} {operation}");
            left.DisplayStructure(depth + 1);
            right.DisplayStructure(depth + 1);
        }
    }

    public class UnaryOperation : Expression
    {
        public enum OperationType { Sqrt, Abs, Negate }

        private readonly Expression operand;
        private readonly OperationType operation;

        public UnaryOperation(Expression operand, OperationType operation)
        {
            this.operand = operand;
            this.operation = operation;
        }

        public override double Evaluate()
        {
            var value = operand.Evaluate();

            return operation switch
            {
                OperationType.Sqrt => value >= 0 ? Math.Sqrt(value) : throw new ArgumentException("Cannot take square root of negative number"),
                OperationType.Abs => Math.Abs(value),
                OperationType.Negate => -value,
                _ => throw new InvalidOperationException("Unknown operation")
            };
        }

        public override string ToString()
        {
            return operation switch
            {
                OperationType.Sqrt => $"sqrt({operand})",
                OperationType.Abs => $"abs({operand})",
                OperationType.Negate => $"-({operand})",
                _ => $"?({operand})"
            };
        }

        public override void DisplayStructure(int depth)
        {
            var op = operation switch
            {
                OperationType.Sqrt => "√",
                OperationType.Abs => "||",
                OperationType.Negate => "⊖",
                _ => "❓"
            };
            Console.WriteLine($"{new string(' ', depth * 2)}{op} {operation}");
            operand.DisplayStructure(depth + 1);
        }
    }

    #endregion

    #region Example 6: Thread-Safe Task Classes

    public abstract class TaskComponent
    {
        protected string name;
        protected readonly object lockObject = new object();

        public TaskComponent(string name)
        {
            this.name = name;
        }

        public string Name => name;
        public abstract Task<List<string>> ExecuteAsync();
        public abstract int GetTaskCount();
    }

    public class ThreadSafeTask : TaskComponent
    {
        private readonly Func<Task<string>> taskFunction;

        public ThreadSafeTask(string name, Func<Task<string>> taskFunction) : base(name)
        {
            this.taskFunction = taskFunction;
        }

        public override async Task<List<string>> ExecuteAsync()
        {
            lock (lockObject)
            {
                // Thread-safe logging or state management could go here
            }

            var result = await taskFunction();
            return new List<string> { result };
        }

        public override int GetTaskCount()
        {
            return 1;
        }
    }

    public class ThreadSafeTaskContainer : TaskComponent
    {
        private readonly ConcurrentBag<TaskComponent> tasks = new ConcurrentBag<TaskComponent>();

        public ThreadSafeTaskContainer(string name) : base(name) { }

        public void Add(TaskComponent task)
        {
            tasks.Add(task);
        }

        public override async Task<List<string>> ExecuteAsync()
        {
            var allResults = new List<string>();
            var taskList = tasks.ToList();

            // Execute all tasks concurrently
            var taskResults = await Task.WhenAll(taskList.Select(task => task.ExecuteAsync()));

            foreach (var results in taskResults)
            {
                allResults.AddRange(results);
            }

            return allResults;
        }

        public override int GetTaskCount()
        {
            return tasks.Sum(task => task.GetTaskCount());
        }
    }

    #endregion

    #region Example 7: Document Structure Classes

    public abstract class DocumentElement
    {
        protected string content;

        public DocumentElement(string content)
        {
            this.content = content;
        }

        public abstract void Display(int depth);
        public abstract int GetWordCount();
        public abstract int GetCharacterCount();
        public abstract int GetReadingTime(); // in minutes
    }

    public class Paragraph : DocumentElement
    {
        public Paragraph(string content) : base(content) { }

        public override void Display(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}📝 Paragraph: {content.Substring(0, Math.Min(50, content.Length))}...");
        }

        public override int GetWordCount()
        {
            return content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public override int GetCharacterCount()
        {
            return content.Length;
        }

        public override int GetReadingTime()
        {
            // Assume 200 words per minute reading speed
            return Math.Max(1, GetWordCount() / 200);
        }
    }

    public class Section : DocumentElement
    {
        private readonly List<DocumentElement> elements = new List<DocumentElement>();

        public Section(string title) : base(title) { }

        public void Add(DocumentElement element)
        {
            elements.Add(element);
        }

        public void Remove(DocumentElement element)
        {
            elements.Remove(element);
        }

        public override void Display(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}📑 Section: {content}");
            foreach (var element in elements)
            {
                element.Display(depth + 1);
            }
        }

        public override int GetWordCount()
        {
            return elements.Sum(e => e.GetWordCount());
        }

        public override int GetCharacterCount()
        {
            return elements.Sum(e => e.GetCharacterCount());
        }

        public override int GetReadingTime()
        {
            return elements.Sum(e => e.GetReadingTime());
        }
    }

    public class Chapter : DocumentElement
    {
        private readonly List<DocumentElement> elements = new List<DocumentElement>();

        public Chapter(string title) : base(title) { }

        public void Add(DocumentElement element)
        {
            elements.Add(element);
        }

        public void Remove(DocumentElement element)
        {
            elements.Remove(element);
        }

        public override void Display(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}📖 Chapter: {content}");
            foreach (var element in elements)
            {
                element.Display(depth + 1);
            }
        }

        public override int GetWordCount()
        {
            return elements.Sum(e => e.GetWordCount());
        }

        public override int GetCharacterCount()
        {
            return elements.Sum(e => e.GetCharacterCount());
        }

        public override int GetReadingTime()
        {
            return elements.Sum(e => e.GetReadingTime());
        }
    }

    public class Document : DocumentElement
    {
        private readonly List<DocumentElement> elements = new List<DocumentElement>();

        public Document(string title) : base(title) { }

        public void Add(DocumentElement element)
        {
            elements.Add(element);
        }

        public void Remove(DocumentElement element)
        {
            elements.Remove(element);
        }

        public override void Display(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}📄 Document: {content}");
            foreach (var element in elements)
            {
                element.Display(depth + 1);
            }
        }

        public override int GetWordCount()
        {
            return elements.Sum(e => e.GetWordCount());
        }

        public override int GetCharacterCount()
        {
            return elements.Sum(e => e.GetCharacterCount());
        }

        public override int GetReadingTime()
        {
            return Math.Max(1, elements.Sum(e => e.GetReadingTime()));
        }
    }

    #endregion

    #region Example 8: Graphics Composition Classes

    public abstract class GraphicsElement
    {
        protected string name;

        public GraphicsElement(string name)
        {
            this.name = name;
        }

        public string Name => name;
        public abstract void Render();
        public abstract int GetElementCount();
        public abstract string GetBoundingBox();
        public abstract void Transform(double scaleX, double scaleY, double translateX, double translateY);
    }

    public class Rectangle : GraphicsElement
    {
        private double x, y, width, height;

        public Rectangle(string name, double x, double y, double width, double height) : base(name)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public override void Render()
        {
            Console.WriteLine($"  ▭ Rectangle {name}: ({x}, {y}) {width}x{height}");
        }

        public override int GetElementCount()
        {
            return 1;
        }

        public override string GetBoundingBox()
        {
            return $"({x}, {y}, {x + width}, {y + height})";
        }

        public override void Transform(double scaleX, double scaleY, double translateX, double translateY)
        {
            x = x * scaleX + translateX;
            y = y * scaleY + translateY;
            width *= scaleX;
            height *= scaleY;
            Console.WriteLine($"    Transformed {name}: ({x:F1}, {y:F1}) {width:F1}x{height:F1}");
        }
    }

    public class Circle : GraphicsElement
    {
        private double x, y, radius;

        public Circle(string name, double x, double y, double radius) : base(name)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
        }

        public override void Render()
        {
            Console.WriteLine($"  ● Circle {name}: ({x}, {y}) radius {radius}");
        }

        public override int GetElementCount()
        {
            return 1;
        }

        public override string GetBoundingBox()
        {
            return $"({x - radius}, {y - radius}, {x + radius}, {y + radius})";
        }

        public override void Transform(double scaleX, double scaleY, double translateX, double translateY)
        {
            x = x * scaleX + translateX;
            y = y * scaleY + translateY;
            radius *= Math.Min(scaleX, scaleY); // Use minimum scale for radius
            Console.WriteLine($"    Transformed {name}: ({x:F1}, {y:F1}) radius {radius:F1}");
        }
    }

    public class Line : GraphicsElement
    {
        private double x1, y1, x2, y2;

        public Line(string name, double x1, double y1, double x2, double y2) : base(name)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        public override void Render()
        {
            Console.WriteLine($"  ─ Line {name}: ({x1}, {y1}) to ({x2}, {y2})");
        }

        public override int GetElementCount()
        {
            return 1;
        }

        public override string GetBoundingBox()
        {
            return $"({Math.Min(x1, x2)}, {Math.Min(y1, y2)}, {Math.Max(x1, x2)}, {Math.Max(y1, y2)})";
        }

        public override void Transform(double scaleX, double scaleY, double translateX, double translateY)
        {
            x1 = x1 * scaleX + translateX;
            y1 = y1 * scaleY + translateY;
            x2 = x2 * scaleX + translateX;
            y2 = y2 * scaleY + translateY;
            Console.WriteLine($"    Transformed {name}: ({x1:F1}, {y1:F1}) to ({x2:F1}, {y2:F1})");
        }
    }

    public class TextElement : GraphicsElement
    {
        private double x, y;
        private readonly string text;

        public TextElement(string text, double x, double y) : base(text)
        {
            this.text = text;
            this.x = x;
            this.y = y;
        }

        public override void Render()
        {
            Console.WriteLine($"  📝 Text \"{text}\": ({x}, {y})");
        }

        public override int GetElementCount()
        {
            return 1;
        }

        public override string GetBoundingBox()
        {
            // Approximate text bounds
            var width = text.Length * 8; // Approximate character width
            var height = 16; // Approximate character height
            return $"({x}, {y}, {x + width}, {y + height})";
        }

        public override void Transform(double scaleX, double scaleY, double translateX, double translateY)
        {
            x = x * scaleX + translateX;
            y = y * scaleY + translateY;
            Console.WriteLine($"    Transformed {name}: ({x:F1}, {y:F1})");
        }
    }

    public class GraphicsGroup : GraphicsElement
    {
        private readonly List<GraphicsElement> elements = new List<GraphicsElement>();

        public GraphicsGroup(string name) : base(name) { }

        public void Add(GraphicsElement element)
        {
            elements.Add(element);
        }

        public void Remove(GraphicsElement element)
        {
            elements.Remove(element);
        }

        public override void Render()
        {
            Console.WriteLine($"🎨 Group {name}:");
            foreach (var element in elements)
            {
                element.Render();
            }
        }

        public override int GetElementCount()
        {
            return elements.Sum(e => e.GetElementCount());
        }

        public override string GetBoundingBox()
        {
            if (!elements.Any()) return "(0, 0, 0, 0)";

            // Calculate combined bounding box
            var boxes = elements.Select(e => e.GetBoundingBox()).ToList();
            // Simplified implementation - in real scenario, would parse and calculate actual bounds
            return $"Combined bounds of {elements.Count} elements";
        }

        public override void Transform(double scaleX, double scaleY, double translateX, double translateY)
        {
            Console.WriteLine($"  Transforming group {name}:");
            foreach (var element in elements)
            {
                element.Transform(scaleX, scaleY, translateX, translateY);
            }
        }
    }

    #endregion

    #region Example 9: Command Hierarchy Classes

    public abstract class Command
    {
        protected string name;

        public Command(string name)
        {
            this.name = name;
        }

        public string Name => name;
        public abstract void Execute();
        public abstract void Undo();
        public abstract void Display(int depth);
        public abstract int GetCommandCount();
    }

    public class SimpleCommand : Command
    {
        private readonly Action executeAction;
        private readonly Action? undoAction;

        public SimpleCommand(string name, Action executeAction, Action? undoAction = null) : base(name)
        {
            this.executeAction = executeAction;
            this.undoAction = undoAction;
        }

        public override void Execute()
        {
            executeAction?.Invoke();
        }

        public override void Undo()
        {
            if (undoAction != null)
            {
                undoAction.Invoke();
            }
            else
            {
                Console.WriteLine($"  Undoing {name} (no specific undo action)");
            }
        }

        public override void Display(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}⚡ Command: {name}");
        }

        public override int GetCommandCount()
        {
            return 1;
        }
    }

    public class MacroCommand : Command
    {
        private readonly List<Command> commands = new List<Command>();

        public MacroCommand(string name) : base(name) { }

        public void Add(Command command)
        {
            commands.Add(command);
        }

        public void Remove(Command command)
        {
            commands.Remove(command);
        }

        public override void Execute()
        {
            Console.WriteLine($"  Executing macro: {name}");
            foreach (var command in commands)
            {
                command.Execute();
            }
        }

        public override void Undo()
        {
            Console.WriteLine($"  Undoing macro: {name}");
            // Undo in reverse order
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                commands[i].Undo();
            }
        }

        public override void Display(int depth)
        {
            Console.WriteLine($"{new string(' ', depth * 2)}📋 Macro: {name}");
            foreach (var command in commands)
            {
                command.Display(depth + 1);
            }
        }

        public override int GetCommandCount()
        {
            return commands.Sum(c => c.GetCommandCount());
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. TREE STRUCTURE OVERHEAD:
 *    - Each composite node maintains a collection of child references
 *    - Memory usage grows with tree depth and breadth
 *    - Consider using arrays instead of lists for fixed-size compositions
 *    - Implement object pooling for frequently created/destroyed composites
 * 
 * 2. RECURSIVE OPERATIONS:
 *    - Deep tree structures can cause stack overflow in recursive operations
 *    - Consider iterative traversal with explicit stack for very deep trees
 *    - Implement tail recursion optimization where possible
 *    - Monitor stack depth in recursive method implementations
 * 
 * 3. CHILD COLLECTION MANAGEMENT:
 *    - Use appropriate collection types based on access patterns
 *    - List<T> for sequential access and frequent additions
 *    - Dictionary<K,V> for key-based child lookup
 *    - ConcurrentBag<T> for thread-safe scenarios with minimal ordering requirements
 *    - Consider lazy initialization of child collections for memory efficiency
 * 
 * 4. MEMORY OPTIMIZATION STRATEGIES:
 *    - Implement weak references for parent-child relationships to prevent circular references
 *    - Use flyweight pattern for shared leaf objects
 *    - Implement copy-on-write semantics for immutable composite structures
 *    - Consider using structs for simple leaf objects to reduce heap allocation
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. CONCURRENT ACCESS PATTERNS:
 *    - Composite structures are typically not thread-safe by default
 *    - Multiple readers can safely traverse immutable composite structures
 *    - Writers require exclusive access to maintain structure integrity
 *    - Consider using ReaderWriterLockSlim for read-heavy scenarios
 * 
 * 2. THREAD-SAFE COLLECTION USAGE:
 *    - Use ConcurrentBag<T> or ConcurrentQueue<T> for thread-safe child collections
 *    - Implement proper locking strategies for composite modifications
 *    - Avoid lock contention by using fine-grained locking per subtree
 *    - Consider lock-free algorithms for high-performance scenarios
 * 
 * 3. RECURSIVE OPERATION SAFETY:
 *    - Recursive traversals can be interrupted by concurrent modifications
 *    - Take snapshots of child collections before traversal
 *    - Use immutable collections or defensive copying for thread safety
 *    - Implement cancellation token support for long-running operations
 * 
 * 4. ASYNC COMPOSITE OPERATIONS:
 *    - Composite operations can benefit from parallel execution
 *    - Use Task.WhenAll for independent parallel operations on children
 *    - Implement proper exception handling and aggregation
 *    - Consider ConfigureAwait(false) to avoid deadlocks in library code
 * 
 * 5. PERFORMANCE CONSIDERATIONS:
 *    - Tree traversal operations have O(n) complexity where n is node count
 *    - Cache frequently computed aggregate values (size, count) when possible
 *    - Implement lazy evaluation for expensive composite operations
 *    - Use parallel processing for independent operations on large trees
 * 
 * 6. BEST PRACTICES:
 *    - Design composite interfaces to minimize the need for type checking
 *    - Implement proper exception handling for malformed tree structures
 *    - Use visitor pattern for complex operations on composite structures
 *    - Provide both synchronous and asynchronous operation variants
 *    - Document thread safety guarantees of composite implementations
 *    - Implement proper disposal patterns for composites managing resources
 *    - Use generic types to avoid boxing/unboxing overhead
 *    - Consider implementing IEnumerable<T> for easy LINQ integration
 */
