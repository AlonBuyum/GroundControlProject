using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class StateDO
    {

        [Key]
        public int Id { get; set; }
        public int Number { get; set; }
        public string State { get; set; }
        public DateTime StateDate { get; set; }
        public StateDO()
        {
            StateDate = DateTime.Now;
        }
    }
}
