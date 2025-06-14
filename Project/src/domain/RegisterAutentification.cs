using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.domain
{
    public static class RegisterAutentification
    {
        /// <summary>
        /// Verifica si el valor de edad es un entero positivo.
        /// </summary>
        
        /// <param name="edadTexto">El valor de la edad como string.</param>
        /// <returns>True si es un entero positivo, false en caso contrario.</returns>
        public static bool EsEdadValida(string? edadTexto)
        {
            if (int.TryParse(edadTexto, out int edad))
            {
                return edad > 0;
            }
            return false;
        }
    }
}
