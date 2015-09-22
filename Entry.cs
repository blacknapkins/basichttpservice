
using System;

namespace PhoneBookApp.Models
{

    public class Entry
    {
        public const int INVALID_ID = 0;

        public int Id { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
    }
}
