using System;
using System.Collections.Generic;
using System.Linq;
using GraphicsTest.Interfaces;
using Rectangle = GraphicsTest.Rectangle;
using Triangle = GraphicsTest.Triangle;
using Triangle2 = GraphicsTest.Triangle2;
using TwoTriangles = GraphicsTest.TwoTriangles;
using TwoTriangles2 = GraphicsTest.TwoTriangles2;

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

            var demos = new List<DemoSelection>() {
                new DemoSelection("Triangle Demo", Triangle.Demo.Run),
                new DemoSelection(new Triangle2.Demo()),
                new DemoSelection(new Rectangle.Demo()),
                new DemoSelection(new TwoTriangles.Demo()),
                new DemoSelection(new TwoTriangles2.Demo())
            };

            int selectedDemo = (int)startInteractiveMenu(demos);

            if (selectedDemo != 0) {
                var demo = demos[selectedDemo-1];
                Console.WriteLine($"Starting {demo}");
                demo.Run();
            }
        }

        private static uint startInteractiveMenu(IEnumerable<DemoSelection> demos) {
            displayOptions(demos);
            string demoSelection = Console.ReadLine();
            int totalDemos = demos.Count();
            uint selectedDemo = 0;
            while(!uint.TryParse(demoSelection, out selectedDemo) ||
                  (selectedDemo != 0 && selectedDemo > totalDemos)) {
                Console.WriteLine("Invalid selection, please try again.");
                Console.WriteLine();
                displayOptions(demos);
                demoSelection = Console.ReadLine();
            }
            return selectedDemo;
        }

        private static void displayOptions(IEnumerable<DemoSelection> demos) {
            int i = 1;
            foreach(var demo in demos) {
                Console.WriteLine($"{i}. {demo}");
                i++;
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

        public DemoSelection(IDemo demo) {
            DemoName = demo.Name;
            Run = demo.Run;
        }

        public override string ToString() {
            return DemoName;
        }
    }
}
