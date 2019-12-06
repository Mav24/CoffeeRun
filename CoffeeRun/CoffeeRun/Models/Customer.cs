using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeRun.Models
{
    public class Customer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string Name { get; set; }
        public string CoffeeSize { get; set; }
        public string CoffeeType { get; set; }
        public bool AddToOrderChecked { get; set; }

    }
}
