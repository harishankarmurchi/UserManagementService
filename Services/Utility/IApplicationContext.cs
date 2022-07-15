using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utility
{
    public interface IApplicationContext
    {
        public int UserId { get; }
        public string Token { get; }
    }
}
