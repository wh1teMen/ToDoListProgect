using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListProgect
{
    internal class OutBase
    {
        public string Task { get; internal set; }
        public int TimeSek { get; internal set; }
        public OutBase( int timeSek,string task)
        {
            Task = task;
            TimeSek = timeSek;
        }
    }
}
