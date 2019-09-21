using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Annealer
    {
        private int m1;  // number of transitions that improves strictly the value of obj function
        private int m2; // other transitions
         int m0 = 1000; // total transitions
        private double deltaF; // the average of the cost differences
        private double c; // initial temperature
        private double X; // acceptance rate
        private Student S;
        public Task[] tasks { get; }
        public Task T { get; private set; }
        public GSolver solver { get; private set; }
        const double alpha = 0.9;
        private double epsilon;
        List<double> solutions = new List<double>();
        public List<StartFinish> deadlines = new List<StartFinish>();
        public List<StartFinish> segment= new List<StartFinish>();
        public List<List<Task>> tasksInSegments = new List<List<Task>>();
        private double initWelfare;
        private double finalWelfare;

        public Annealer(Student S, Task[] T)
        {
            this.S = S;
            this.tasks = T;
        }

        private void Compute()
        {
            int m1 = 0;
            int m2 = 0;
            double currentSolution;
            double nextSolution;
            double delta = 0;
            int t = (S.getlenL() / 2) + T.t1;
            StartFinish position = new StartFinish(S.getlenL() / 2, t);
            currentSolution = solver.ProbOfCompletion(position.start, position.finish);

            for (int i = 0; i < m0; i++)
            {

                //  Console.WriteLine(" Iteration =" + i + " ");
                position = solver.GetNeighbour(position.start, position.finish); // new neighbor                                                       
                nextSolution = solver.ProbOfCompletion(position.start, position.finish);
                //   Console.WriteLine($"nextSolution {nextSolution}");
                if (nextSolution > currentSolution)
                {
                    m1++;
                    delta += nextSolution - currentSolution;
                }
                currentSolution = nextSolution;
                //  Console.WriteLine("Current =" + currentSolution);

            }
            m2 = m0 - m1;
            deltaF = delta / m1;
            X = 0.8;
            double f = (double)m2 / ((double)m2 * X - (double)m1 * (1 - X));
            // Console.WriteLine("f " + f);
              c = deltaF / (Math.Log(f));
          
            epsilon = c / m0;
            this.m1 = m1;
            this.m2 = m2;

            PrintParameters();
        }
        private void PrintParameters()
        {
            Console.WriteLine();
            Console.WriteLine("Total iterations :" + m0);
            Console.WriteLine("Increasing :" + m1);
            Console.WriteLine("Other :" + m2 + " Average cost difference :" + deltaF);
            Console.WriteLine(" Acceptance rate ( defined by the user ) : " + X);
            Console.WriteLine("Initial value in the cooling process :" + c);
            Console.WriteLine("Stopping criterion : The algorithm " +
                    "is stopped when the temperature reaches : \n epsilon = " + epsilon);
            Console.WriteLine("\n \n");
        }
        private void PrintSolutions()
        {
            int number = 0;
            foreach (var ds in deadlines.Zip(solutions, Tuple.Create))
            {
                number++;

                Console.Write(number + " " + "Deadline : " + ds.Item1.start + " " + ds.Item1.finish);
                Console.WriteLine(" Solution : " + ds.Item2);
            }
          

            /*   using (System.IO.StreamWriter file =
                          new System.IO.StreamWriter(@"D:\WriteText.txt", true))
               // if (k % 5 == 0)
               {
                   file.WriteLine(Welfare(new List<Task>(tasks)));
               }
               foreach (var item in deadlines)
               {
                   using (System.IO.StreamWriter file =
                          new System.IO.StreamWriter(@"D:\WriteText.txt", true))
                   // if (k % 5 == 0)
                   {
                       file.WriteLine(item.start +" "+item.finish);
                   }
               }
               */
        }
        public void SimulatedAnnealing()
        {

            int number = 0;
            foreach (Task T in tasks)
            {
                number++;
                Console.WriteLine($" Task number {number}");

                this.T = T;
                this.solver = new GSolver(this.S, T);
                int k = 0;
               
                Compute();

                double ck = c; // temperature
                double r;
                double acceptProbability;
                double bestSolution = 0;
                StartFinish bestDeadline = new StartFinish(0, 0);
                StartFinish currentDeadline = new StartFinish(0, 0);
                double currentSolution;
                double nextSolution;
                int t = (T.t2 + T.t1)/2;
                StartFinish deadline = new StartFinish(0, t);
                List<Task> intersectTasks = new List<Task>();
                intersectTasks.Add(T);
                List<StartFinish> intersectDd = new List<StartFinish>();
                intersectDd.Add(deadline);
                for (int j = 0; j < segment.Count; j++)
                {
                    if (Intersect(deadline, segment[j]))
                    {
                        foreach (Task tk in tasksInSegments[j])
                        {
                            intersectTasks.Add(tk);
                            intersectDd.Add(tk.deadline);
                        }
                    }
                }
                currentSolution = solver.ProbOfCompletionModern(intersectDd, intersectTasks);
                currentDeadline = deadline;
                Random rnd = new Random();
                while (ck > epsilon)
                {
                    for (int i = 0; i < m0; i++)
                    {
                        r = rnd.NextDouble();
                         intersectTasks = new List<Task>();
                         intersectTasks.Add(T);
                         intersectDd = new List<StartFinish>();
                         intersectDd.Add(deadline);
                      for (int j=0;j<segment.Count;j++)
                        {
                            if (Intersect(deadline, segment[j]))
                            {
                                
                                foreach(Task tk in tasksInSegments[j])
                                {
                                    intersectTasks.Add(tk);
                                    intersectDd.Add(tk.deadline);
                                }
                            }
                        }
                           nextSolution = solver.ProbOfCompletionModern(intersectDd, intersectTasks); 
                        if (nextSolution > bestSolution)
                        {
                            bestSolution = nextSolution;
                            bestDeadline = new StartFinish(deadline.start, deadline.finish);
                        }
                        if (nextSolution > currentSolution)
                        {
                            currentSolution = nextSolution;
                            currentDeadline = new StartFinish(deadline.start, deadline.finish);
                        }
                        else
                        {
                            acceptProbability = Math.Exp((currentSolution - nextSolution) / ck);
                            if (r < acceptProbability)
                            {
                                currentSolution = nextSolution;
                                currentDeadline = new StartFinish(deadline.start, deadline.finish); 
                            }

                        }
                        solver = new GSolver(this.S, T);
                        int t1 = currentDeadline.start;
                        int t2 = currentDeadline.finish;
                        deadline = solver.GetNeighbour(t1, t2);

                    }

                    ck = alpha * ck;
                }
                this.deadlines.Add(new StartFinish(bestDeadline.start, bestDeadline.finish));
                T.setDD(new StartFinish(bestDeadline.start, bestDeadline.finish));
                this.solutions.Add(bestSolution);
                T.solution = bestSolution;
                T.index = number;
                bool isbroken = false;
                for(int xy=0; xy<segment.Count;xy++)       //
                {
                    if (Intersect(bestDeadline, segment[xy]))
                    {
                        segment[xy] = Merge(bestDeadline, segment[xy]);
                        tasksInSegments[xy].Add(T);
                        isbroken = true;
                        
                    }
                }
                if (!isbroken)
                {
                    segment.Add(new StartFinish(bestDeadline.start, bestDeadline.finish));
                    Console.WriteLine(bestDeadline.start +" "+ bestDeadline.finish);
                    tasksInSegments.Add(new List<Task>() { T });
                }
            }
            PrintSolutions();
            List<StartFinish> Localdeadlines = new List<StartFinish>();
            List<Task> LocalTasks = new List<Task>();
            foreach (var item in tasks)
            {
                for (int i = 0; i < tasksInSegments.Count; i++)
                {
                    if (tasksInSegments[i].Contains(item))
                    {
                        Localdeadlines = new List<StartFinish>();
                         LocalTasks = new List<Task>();
                        GSolver solver = new GSolver(this.S, item);
                        LocalTasks.Add(item);
                        Localdeadlines.Add(item.deadline);
                        foreach (var td in tasksInSegments[i])
                        {
                            if (td != item)
                            {
                                LocalTasks.Add(td);
                                Localdeadlines.Add(td.deadline);
                            }
                        }

                       
                    }
                }
                item.solution = solver.ProbOfCompletionModern(Localdeadlines, LocalTasks);
                Console.WriteLine($"Deadline {item.deadline.start} {item.deadline.finish}  ObjectiveF {item.solution}");
            }

        }
        public void StartPlaying()
        {
            int iteraion = 0;
            Random rnd = new Random();
            double r;
            bool moved = true;
            
            while (moved)
            {
                int index = 0;
                iteraion++;
                moved = false;
                foreach (var item in tasks)
                {
                    index++;
                    double ck = 2.7;
                    m0 = 700;
                    Console.WriteLine($"Task{index} {item.deadline.start} {item.deadline.finish}");
                    List<StartFinish> Localdeadlines = new List<StartFinish>();
                    List<Task> LocalTasks = new List<Task>();
                    double acceptProbability;
                    StartFinish currentDeadline = new StartFinish(item.deadline.start, item.deadline.finish); 
                    StartFinish bestDeadline = new StartFinish(item.deadline.start, item.deadline.finish);
                    StartFinish deadline = new StartFinish(item.deadline.start, item.deadline.finish);
                    double nextSolution=0;
                    double currentSolution = item.solution; 
                    double bestSolution = item.solution;
                    while (ck > epsilon)

                    {
                        for (int k = 0; k < m0; k++)
                        {
                            
                            r = rnd.NextDouble();
                            GSolver solver = new GSolver(this.S, item);
                            deadline = solver.GetNeighbour(currentDeadline.start, currentDeadline.finish);
                            Rebuild(item, deadline);
                            item.deadline = new StartFinish(deadline.start, deadline.finish);
                            nextSolution = CurWelf(out LocalTasks, index);
                            
                            if (nextSolution > bestSolution)
                            {
                                bestSolution = nextSolution;
                                bestDeadline = new StartFinish(deadline.start, deadline.finish);
                                moved = true;
                                for (int y = 0; y < tasks.Length; y++)
                                {
                                    tasks[y].solution = LocalTasks[y].solution;
                                    tasks[y].deadline = new StartFinish(LocalTasks[y].deadline.start, LocalTasks[y].deadline.finish);
                                       
                                }

                                Console.WriteLine("New System");
                                int num = 0;
                                foreach(var ttt in tasks)
                                {
                                    num++;
                                    if (ttt != item)
                                    {
                                        Console.WriteLine($"{num}Deadline {ttt.deadline.start} {ttt.deadline.finish} {ttt.solution}");

                                    }
                                    else
                                    {
                                        Console.WriteLine($"{num}Deadline {bestDeadline.start} {bestDeadline.finish} {bestSolution} ");
                                    }
                                }
                               
                            }
                            if (nextSolution > currentSolution)
                            {
                                currentSolution = nextSolution;
                                currentDeadline = new StartFinish(deadline.start,deadline.finish);
                            }
                            else
                            {
                                acceptProbability = Math.Exp((currentSolution - nextSolution) / ck);
                                if (r < acceptProbability)
                                {
                                   
                                    currentSolution = nextSolution;
                                    currentDeadline = new StartFinish(deadline.start, deadline.finish); 

                                }

                            }
                           

                        }

                        ck = alpha * ck;
                    }
                   item.deadline = new StartFinish(bestDeadline.start, bestDeadline.finish);
                   item.solution = bestSolution;
                  
                }
            }
            Console.WriteLine("END");
            Console.WriteLine(iteraion);
            // Console.WriteLine($"Initial Welfare {Welfare(new List<Task>(tasks))}");
            initWelfare = Welfare(new List<Task>(tasks));


        }

        public void StartManagement()
        {
            m0 = 1000;
            int iteraion = 0;
            Random rnd = new Random();
            double r;
            bool moved = true;
            while (moved)
            {
                int number = 0;
                iteraion++;
                moved = false;
                foreach (var item in tasks)
                {
                    number++;
                    double ck = 2.7;
                    Console.WriteLine($"Task{number} {item.deadline.start} {item.deadline.finish}");
                    double acceptProbability;
                    List<Task> bestTasks = new List<Task>();
                    List<Task> localtask = new List<Task>();
                    StartFinish currentDeadline = new StartFinish(item.deadline.start, item.deadline.finish);
                    StartFinish bestDeadline = new StartFinish(item.deadline.start, item.deadline.finish);
                    StartFinish deadline = new StartFinish(item.deadline.start, item.deadline.finish);
                    double nextSolution = 0;
                    double currentSolution = Welfare(new List<Task>(tasks));
                    double bestSolution = currentSolution;
                    while (ck > epsilon)

                    {
                        for (int k = 0; k < m0; k++)
                        {
                            r = rnd.NextDouble();
                            GSolver solver = new GSolver(this.S, item);
                            deadline = solver.GetNeighbour(currentDeadline.start, currentDeadline.finish);
                            Rebuild(item, deadline);
                            item.deadline = new StartFinish(deadline.start, deadline.finish);
                            nextSolution = CurWelf(out localtask);
                            
                            if (nextSolution > bestSolution)
                            {
                                bestSolution = nextSolution;
                                bestDeadline = new StartFinish(deadline.start, deadline.finish);
                                moved = true;
                                bestTasks = new List<Task>();
                                foreach (var task1 in localtask)
                                {
                                    bestTasks.Add(new Task(task1.B, task1.t1, task1.t2) { deadline = new StartFinish(task1.deadline.start, task1.deadline.finish), solution=task1.solution});
                                }

                                Console.WriteLine("New System");
                                Console.WriteLine($"New welfare {bestSolution}");
                                int num = 0;
                                foreach (var ttt in localtask)
                                {
                                    num++;
                                
                                 Console.WriteLine($"{num}Deadline {ttt.deadline.start} {ttt.deadline.finish} {ttt.solution}");
                                }

                            }
                            if (nextSolution > currentSolution)
                            {
                                currentSolution = nextSolution;
                                currentDeadline = new StartFinish(deadline.start, deadline.finish);
                            }
                            else
                            {
                                acceptProbability = Math.Exp((currentSolution - nextSolution) / ck);
                                if (r < acceptProbability)
                                {

                                    currentSolution = nextSolution;
                                    currentDeadline = new StartFinish(deadline.start, deadline.finish);

                                }

                            }


                        }

                        ck = alpha * ck;
                    }
                    item.deadline = new StartFinish(bestDeadline.start, bestDeadline.finish);
                    int q = 0;
                    foreach(var Tt in bestTasks)
                    {
                        tasks[q].solution = Tt.solution;
                        tasks[q].deadline = new StartFinish(Tt.deadline.start, Tt.deadline.finish);
                        q++;
                    }

                }
            }
            
            finalWelfare=Welfare(new List<Task>(tasks));
            Console.WriteLine($"Initial objective function {initWelfare}");
            Console.WriteLine($"Final {finalWelfare}");
            double cost = finalWelfare - initWelfare;
            Console.WriteLine($"Cost of anarchy {cost}");
        }

        public static bool Intersect(StartFinish deadline1, StartFinish deadline2)
        {
            int t1s = deadline1.start;
            int t1f = deadline1.finish;
            int t2s = deadline2.start;
            int t2f = deadline2.finish;
            if (t1s <= t2s && t2s < t1f)
            {

                return true;
            }
            if (t1s < t2f && t1f >= t2f)
            {

                return true;
            }
            if (t2s <= t1s && t1s < t2f)
            {

                return true;
            }
            if (t2s < t1f && t2f >= t1f)
            {

                return true;
            }
            return false;
        }
        public static StartFinish Merge(StartFinish deadline1, StartFinish deadline2)
        {
            int t1=0;
            int t2=0;
            if (deadline1.start >= deadline2.start) // d2 старт раньше
            {
                t1 = deadline2.start;
            }
            else
            {
                t1 = deadline1.start;
            }
            if (deadline1.finish >= deadline2.finish)
            {
                t2 = deadline1.finish;
            }
            else
            {
                t2 = deadline2.finish;
            }
            return new StartFinish(t1, t2);
        }
        private void Rebuild(Task task, StartFinish deadline)
        {
            int numberOfSegment = 0;
            for (int i=0; i< tasksInSegments.Count; i++)
            {
                if (tasksInSegments[i].Contains(task))
                {
                    tasksInSegments[i].Remove(task);
                    numberOfSegment = i;
                    if (tasksInSegments[i].Count == 0)
                    {
                        tasksInSegments.RemoveAt(i);
                        segment.RemoveAt(i);
                    }
                    else
                    {
                        segment[numberOfSegment] = new StartFinish(-1, -1);
                        foreach (Task t in tasksInSegments[numberOfSegment])
                        {
                            if (Intersect(t.deadline, segment[numberOfSegment]))
                            {
                                segment[numberOfSegment] = Merge(t.deadline, segment[numberOfSegment]);
                            }
                            else
                            {
                                segment[numberOfSegment] = new StartFinish(t.deadline.start, t.deadline.finish);

                            }
                        }
                    }
                }
            }
            bool isbroken = false;
            for (int i = 0; i < segment.Count; i++)
            {
                if (Intersect(deadline, segment[i]))
                {
                    tasksInSegments[i].Add(task);
                    segment[i] = Merge(segment[i], deadline);
                    isbroken = true;
                }
            }
            if (!isbroken)
            {
               
                segment.Add(new StartFinish(deadline.start, deadline.finish));
                tasksInSegments.Add(new List<Task>() { task });
            }
       /*     for (int i=0; i<segment.Count;i++)
            {
                Console.WriteLine($"Segment {segment[i].start} {segment[i].finish}");
                Console.WriteLine($"Tasks in it");
                foreach(var item in tasksInSegments[i])
                {
                    Console.WriteLine(item.deadline.start+" "+item.deadline.finish);
                }
            }
            */
        }

        public double CurWelf(out List<Task> localtasks, int index = -1)
        {
            
            localtasks = new List<Task>();
            foreach(var tt in tasks)
            {
                localtasks.Add(new Task(tt.B, tt.t1, tt.t2) { deadline = new StartFinish(tt.deadline.start, tt.deadline.finish) });
            }
            double sum = 0;
            int k = 0;
            List<StartFinish> Localdeadlines = new List<StartFinish>();
            List<Task> LocalTasks = new List<Task>();
            foreach (var item in tasks)
            {
                
                for (int i = 0; i < tasksInSegments.Count; i++)
                {
                    if (tasksInSegments[i].Contains(item))
                    {
                         Localdeadlines = new List<StartFinish>();
                         LocalTasks = new List<Task>();
                        GSolver solver = new GSolver(this.S, item);
                        LocalTasks.Add(item);
                        Localdeadlines.Add(item.deadline);
                        foreach (var td in tasksInSegments[i])
                        {
                            if (td != item)
                            {
                                LocalTasks.Add(td);
                                Localdeadlines.Add(td.deadline);
                            }
                        }
                      
                       
                        

                    }
                }
                localtasks[k].solution = solver.ProbOfCompletionModern(Localdeadlines, LocalTasks);
                sum += localtasks[k].solution;
                k++;
            }
           
            if (index != -1)
            {
                return localtasks[index-1].solution;
            }
            
            
            return sum;
        }
        public double Welfare(List<Task> newTasks)
        {
            double wal = 0;
            foreach(var T in newTasks)
            {
               // Console.WriteLine($"Sol "+T.solution);
                wal += T.solution;
            }
            return wal;
        }
    }
}