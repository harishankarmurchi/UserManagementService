using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class TokenVM
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = String.Empty;

    }
}
