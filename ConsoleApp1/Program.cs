using System.Collections.Generic;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Group students = new Group();
            students.pushStudent(new Student(8, 0.50, 0.05)); // w, p , q , i
            students.pushStudent(new Student(8, 0.50, 0.70));
            students.pushStudent(new Student(8, 0.55, 0.30));
            students.pushStudent(new Student(8, 0.65, 0.55));
            students.pushStudent(new Student(8, 0.65, 0.70));
            students.pushStudent(new Student(8, 0.75, 0.75));
            students.pushStudent(new Student(8, 0.85, 0.80));

            Task[] tasks = new Task[4];
            
            Random rnd = new Random();
            int B;
            int t1;
            int t2;
            for (int i = 0; i < tasks.Length; i++)
            {
                B = rnd.Next(5, 15);
                t1 = rnd.Next(1, 6);
                t2 = rnd.Next(6, 15);
                tasks[i] = new Task(B, t1, t2);
            }
          
            Annealer annealer = new Annealer(students[6],tasks);
            annealer.SimulatedAnnealing();

            annealer.StartManagement();
            annealer.StartPlaying();
             




            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
