using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeRun.Models
{
    public class TypesOfCoffee
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string CoffeeType { get; set; }
        public string Size { get; set; }
    }
}
