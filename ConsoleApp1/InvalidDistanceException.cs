using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class InvalidDistanceException:ArgumentException
    {
        public InvalidDistanceException (string message): base(message)
        {

        }
    }
}
