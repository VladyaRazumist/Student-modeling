using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class GSolver
    {
        private Student S;
        private Task T;
        private double[] G;
        Random rnd = new Random();
        public double P { get; private set; }
        public double N { get; private set; } // number passed
        private List<List<int>> activityDays = new List<List<int>>();
        private List<List<double>> valueInDays = new List<List<double>>();
        private double epsilon=0.01;
        int minday = 100500;
        int maxday = 0;
        private double prob = 0;
        private double probH = 0;
        
        public GSolver(Student S, Task T)
        {
            this.S = S;
            this.T = T;
        }


        public double ProbOfCompletion(int t11, int t22, int i = -1)
        {   
            if (i != -1)
            {
                List<int> list = new List<int>();
                List<double> dlist = new List<double>();
                activityDays.Add(list);
                valueInDays.Add(dlist);
            }
            if (t11 > t22)
            {
                Console.WriteLine("t1>t2");
                return -1;
            }

            double B = T.B; //
            int t1 = t11;
            int t2 = t22;
            G = new double[S.getlenL()];
            double q = S.q;
            double p = S.p;
            int t12 = t2 - t1;
            int h = 0; // counter, where G[k]>0
            G[0] = Math.Max(0, B * q - S.getLoad(t2 - 1));


            if (G[0] > 0)
            {
                h++;
                if (i != -1)
                {
                    activityDays[i].Add(t2 -1);
                    valueInDays[i].Add(G[0]);
                }
            }
            for (int k = 1; k < t12 - 1; k++)
            {

                G[k] = B * Math.Pow(q, k + 1) - G[k - 1] * q * (1 - p) - S.getLoad(t2 - k - 1);
                G[k] = Math.Max(0, G[k]);
                if (G[k] > 0)
                {
                    h = h + 1;
                    if (i != -1)
                    {
                        activityDays[i].Add(t2 - k - 1);
                        valueInDays[i].Add(G[k]);
                    }


                }


            }

            double P = 1 - Math.Pow(p, h);
       
            this.P = P;
            this.N = P * S.weight;
            return N;
        }
        public double ProbOfCompletionModern(List<StartFinish> deadlines, List<Task> tasks)
        {
            Clean();
            int k = 0;
            for (int i = 0; i < deadlines.Count; i++)
            {
                this.T = tasks[i];
                int t1 = deadlines[i].start;
                int t2 = deadlines[i].finish;
                ProbOfCompletion(t1, t2, i);
            }
            // Дни активности всех заданий
           
            int earliestTask = 0;
            for (int i = 0; i < activityDays.Count; i++)
            {
               // Console.WriteLine($" Task #{i} ");
                for (int j = 0; j < activityDays[i].Count; j++)
                {
                    k = activityDays[i][j];
                    if (maxday < k)
                    {
                        maxday = k;
                    }
                    if (minday > k)
                    {
                        minday = k;
                        earliestTask = i;
                    }
                 //   Console.Write($" Day {k} G[{k}]={valueInDays[i][j]}");

                }
           //     Console.WriteLine();
            }
           // Console.WriteLine($"Earliest task {earliestTask} Minday {minday} MaxDay {maxday}");
            TreeNode<int> tree = new TreeNode<int>(-1);
            tree.probability = 1;
            tree.day = minday-1;
            TreePath(tree);
         //   Console.WriteLine($"Full prob (should be 1) {prob}");
          //  Console.WriteLine($"H prob {probH}");
          //  Console.WriteLine($"H prob2 {probH1}");
            return probH*S.weight;
        }

        private void TreePath(TreeNode<int> tree) {
            
            minday = tree.day+1;
          //  Console.WriteLine("Path");
         //   foreach(int k in tree.distrTask)
          //  {
          //      Console.Write(k);
         //  }
           // Console.WriteLine();
          //  Console.WriteLine($"minDay {minday-1}");
            double maxvalue = -100;
             int   doneTask = -100;
            for (int day = minday; day <= maxday; day++)  
            {
                Dictionary<int, double> map = new Dictionary<int, double>();
                for (int i = 0; i < activityDays.Count; i++)
                {
                    if (activityDays[i].Contains(day))
                    {
                 //       Console.WriteLine($"Task {i} contains day {day}");
                        if (!InTree(tree, i) ) // задание i не выполнено
                        {
                            map.Add(i, valueInDays[i][activityDays[i].IndexOf(day)]);// Номер задания, значение функции в день
                            
                        }
                    }
                   
                }
                maxvalue = -100;
                doneTask = -100;
                foreach (var item in map)
                {
                    if (maxvalue < item.Value)  // Выполняемое задание ( c наибольшей функцией в день)
                    {
                        maxvalue = item.Value;
                        doneTask = item.Key;
                    }
             //       Console.WriteLine($"Task #{item.Key} Value of G {item.Value}");
                }
                double pr;
                if (doneTask != -100 && doneTask!=0)
                {
                    tree.AddChild(doneTask); // Child
             //      Console.WriteLine($"Task {doneTask} is done");
                    tree[0].probability = tree.probability * (1 - S.p);
                    tree[0].day = day;
                    tree[0].distrTask = new List<int> (tree.distrTask);
                    tree[0].distrTask.Add(0);
                    tree[0].h = tree.h;
                    tree.AddChild(-132); // nothing is done
                    tree[1].probability = tree.probability * S.p;
                    tree[1].day = day;
                    tree[1].distrTask = new List<int>(tree.distrTask);
                    tree[1].distrTask.Add(1);
                    tree[1].h = tree.h;
                    int counter = 0;
                    if (tree[1].probability > epsilon )
                    {
                        TreePath(tree[1]);
                        counter++;
                    }
                    else
                    {
                        counter++;
                        pr = tree.probability * (1 - Math.Pow(S.p, tree.h));
               //         Console.WriteLine($"Node prob { tree.probability} child1 h {tree.h}");
               //         Console.WriteLine($"h prob {pr}");
                        prob += tree.probability;
                        probH += pr;
                       
                    }
       
                    if (tree[0].probability > epsilon )
                    {
                       
                        TreePath(tree[0]);
                        counter++;
                    }
                    else
                    {
                        if (counter == 0)
                        {
                            pr = tree.probability * (1 - Math.Pow(S.p, tree.h));
               //             Console.WriteLine($"Node prob { tree.probability} child2 h {tree.h}");
               //             Console.WriteLine($"h prob {pr}");
                            prob += tree.probability;
                            probH += pr;
                           
                        }
                    }


                    return;
                }
                if (doneTask == 0)
                {
                    tree.h++;
                }
            }
            double prf = tree.probability * (1 - Math.Pow(S.p, tree.h));
         //   Console.WriteLine($"Node prob { tree.probability} loop has ended h {tree.h}");
         //   Console.WriteLine($"h prob {prf}");
            prob += tree.probability;
            probH += prf;
            


        }
        public TreeNode<int> ToRoot(TreeNode<int> tree,out double prob)
        {
            prob=tree.probability;
            while (tree.Value != -1)
            {
                Console.WriteLine($"Node prob {tree.probability}");
                tree = tree.Parent;
                prob *= tree.probability;
            }
            Console.WriteLine($"Prob branch {prob}");
            return tree;
        }
        public static Boolean InTree(TreeNode<int> tree, int x)
        {
            bool isHere = false;
            while (tree.Parent != null)
            {
                if (tree.Value == x) isHere = true;
                tree = tree.Parent;
            }
            return isHere;
        }

        public StartFinish GetNeighbour(int t1, int t2)
        {


            double random1 = rnd.NextDouble();

            StartFinish sf = new StartFinish(t1, t2);
            int max = T.t2;
            int min = T.t1;
            if ((t2 - t1) > max || (t2 - t1) < min)
            {
                throw new InvalidDistanceException("wrong arguments");
                

                return sf;
            }
            if (random1 <= 0.25)
            {  // move t1 left
                if (t1 == 0 || (t2 - t1) == max)
                {
                    return sf = GetNeighbour(t1, t2);

                }
                else
                {
                    t1 = t1 - 1;
                    return sf = new StartFinish(t1, t2);
                }

            }
            if (random1 > 0.25 & random1 <= 0.5)
            { // move t1 right
                if ((t2 - t1) == min)
                {
                    return sf = GetNeighbour(t1, t2);
                }
                else
                {
                    t1 = t1 + 1;
                    return sf = new StartFinish(t1, t2);
                }
            }
            if (random1 > 0.5 & random1 <= 0.75)
            { // move t2 left
                if ((t2 - t1) == min)
                {
                    return sf = GetNeighbour(t1, t2);
                }
                else
                {
                    t2 = t2 - 1;
                    return sf = new StartFinish(t1, t2);
                }
            }
            if (random1 > 0.75)
            {  // move t2 right
                if ((t2 - t1) == max || t2 == S.getlenL() - 1)
                {
                    return sf = GetNeighbour(t1, t2);
                }
                else
                {
                    t2 = t2 + 1;
                    return sf = GetNeighbour(t1, t2);
                }
            }

            return sf;
        }
        private void Clean()
        {
            activityDays.Clear();
            valueInDays.Clear();
            int minday = 100500;
            int maxday = 0;
            prob = 0;
            probH = 0;
        }


    }
}