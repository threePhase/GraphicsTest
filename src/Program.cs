using System;
using System.Collections.Generic;
using Triangle = GraphicsTest.Triangle;
using Triangle2 = GraphicsTest.Triangle2;

namespace GraphicsTest
{
    public class Program
    {
        public static void Main(string[] args) {
            Console.WriteLine("Welcome to GraphicsTest!");
            Console.WriteLine();
            Console.WriteLine("This is a simple drawing test using GLFW and OpenGL 3.3!");
            Console.WriteLine();
            Console.WriteLine("Please select a number from the list below to start a demo (or 0 to exit):");
            Console.WriteLine();

            var demos = new Dictionary<int, DemoSelection>() {
                {1, new DemoSelection("Triangle Demo", Triangle.Demo.Run)},
                {2, new DemoSelection("Triangle Demo (using Engine)",
                                      new Triangle2.Demo().Run)}
            };

            int selectedDemo = startInteractiveMenu(demos);

            if (selectedDemo != 0) {
                var demo = demos[selectedDemo];
                Console.WriteLine($"Starting {demo}");
                demo.Run();
            }
        }

        private static int startInteractiveMenu(Dictionary<int, DemoSelection> demos) {
            displayOptions(demos);
            string demoSelection = Console.ReadLine();
            int selectedDemo = 0;
            while(!int.TryParse(demoSelection, out selectedDemo) ||
                  (selectedDemo != 0 && !demos.ContainsKey(selectedDemo))) {
                Console.WriteLine("Invalid selection, please try again.");
                Console.WriteLine();
                displayOptions(demos);
                demoSelection = Console.ReadLine();
            }
            return selectedDemo;
        }

        private static void displayOptions(Dictionary<int, DemoSelection> demos) {
            foreach(var demo in demos) {
                Console.WriteLine($"{demo.Key}. {demo.Value}");
            }
            Console.Write("Demo: ");
        }
    }

    class DemoSelection
    {
        public delegate void DemoRunMethod();

        public string DemoName {get;}
        public DemoRunMethod Run {get;}

        public DemoSelection(string demoName, DemoRunMethod runMethod) {
            DemoName = demoName;
            Run = runMethod;
        }

        public override string ToString() {
            return DemoName;
        }
    }
}
