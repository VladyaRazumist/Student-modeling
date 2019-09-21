using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Group
    {
        private const int n=20;
        private  Student[] students;
        public int Length=0;
        public Group()
        {
           
        }
        public Group (Student [] students)
        {
            this.students = students;
            Length = students.Length;
            foreach(Student st in students)
            {
                st.setLoad(n);
            }
        }
        public Group (Student student)
        {
            students[0] = student;
            students[0].setLoad(n);
            Length = 1;
        }
        public Student this[int index]
        {
            get
            {
                return students[index];
            }
            set
            {
                students[index] = value;
            }
        }
        public void pushStudent(Student student)
        {
            student.setLoad(n);
            if (students != null)
            {
                int len = students.Length;
                Student[] st_s = new Student[len + 1];
                for (int i = 0; i < len; i++)
                {
                    st_s[i] = students[i];
                }
                st_s[len] = student;
                students = st_s;
            }
            else
            {
                students = new Student[1];
                students[0] = student;
            }
            Length++;
        }
        public void pullStudent()
        {
            int len = students.Length;
            Student[] st_s = new Student[len - 1];
            for (int i = 0; i < st_s.Length; i++)
            {
                st_s[i] = students[i];
            }
            students = st_s;
            Length--;
        }
        public void print()
        {
            foreach(Student st in students)
            {
                Console.WriteLine($" p={st.p} q={st.q} w={st.weight}");
            }
        }
    }
}
