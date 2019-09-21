

namespace ConsoleApp1
{
    public class Task
    {
        public double B { get; } // price of Task
        public int t1 { get; } // the earliest time to start
        public int t2{ get ; } // the latest time to start
        public StartFinish deadline {   get;  set; }
        public double solution;
        public int index;
        public void setDD(StartFinish st)
        {
            int t1 = st.start;
            int t2 = st.finish;
            deadline = new StartFinish(t1, t2);
        }
        public Task(double B, int t1, int t2)
        {

            this.B = B;
            this.t1 = t1;
            this.t2 = t2;
        }
    }
}
