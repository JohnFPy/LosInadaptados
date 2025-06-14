using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.domain
{
    public static class RegisterAutentification
    {
        public static bool EsEdadValida(string? ageText)
        {
            if (int.TryParse(ageText, out int edad))
            {
                return edad > 0;
            }
            return false;
        }
    }
}