using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Child()
        {
        }

        public Child(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }

    public class Parent
    {
        public string Name { get; set; }
        public List<Child> Children { get; set; }

        public Parent()
        {
        }

        public Parent(string name)
        {
            this.Name = name;
        }
    }
}
