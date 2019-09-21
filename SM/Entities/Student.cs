using System;
namespace ConsoleApp1
{
    public class Student
    {
        public int weight { get; } // quantity of this type 
        public  double p { get; } // distraction probability
        public double q { get; }// discount coefficient
        private int[] load;
        
        public Student(int weight, double p, double q)
        {
            this.weight = weight;
            this.p = p;
            this.q = q;
            
        }
        public void addLoad(int day)
        {
            load[day] += 1;
        }
        public void setLoad(int N)
        {      // same load for the group of students

            load = new int[N];
            for (int x = 0; x < N; x++)
            {
                Random random = new Random();
                switch (x % 7)
                {
                    case 0: load[x] = 0;
                        break;
                    case 1: load[x] = 2;
                        break;
                    case 2: load[x] = 1;
                        break;
                    case 3: load[x] = 3;
                        break;
                    case 4: load[x] = 4;
                        break;
                    case 5: load[x] = 3;
                        break;
                    case 6:load[x] = 2;
                        break;

                }
                

            }
        }
        public int getLoad(int i)
        {
            return load != null ? load[i] : -1;

        }
        public int getlenL()
        {
            return load!=null ? load.Length : 0;
        }
        public void printLoad()
        {
            if (load != null)
            {
               
                for (int i=0;i<load.Length;i++)
                {
                    Console.Write(i+":"+load[i] + "  ");
                }
                Console.WriteLine();
            }
        }
    }
}
