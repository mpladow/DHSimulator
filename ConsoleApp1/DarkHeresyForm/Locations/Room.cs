﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Room:Location
    {
        public int Size { get; set; }
        public Room(string name, int size, string lightlevel)
        {
            Name = name;
            Size = size;
        }
    }
}
