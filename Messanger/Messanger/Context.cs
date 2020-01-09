using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messanger
{
    public class Context : DbContext
    {
        public DbSet<Message> Messages { get; set; }
    }
}
