using System;
using System.Collections.Generic;
using System.Text;

namespace TodoApi
{
    public class Todo
    {
        public bool Completed { get; set; }=false;
        public int Id { get; private set; } = 0;//private set so that it can't be set from the client
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
