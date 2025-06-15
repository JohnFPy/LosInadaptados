using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public static bool EsNombreValido(string? nameText)
        {
            if (string.IsNullOrWhiteSpace(nameText))
                return false;
            // Solo letras, sin espacios ni números
            return Regex.IsMatch(nameText, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ]+$");
        }

        public static bool EsApellidoValido(string? lastNameText)
        {
            if (string.IsNullOrWhiteSpace(lastNameText))
                return false;
            // Solo letras, sin espacios ni números
            return Regex.IsMatch(lastNameText, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ]+$");
        }

    }
}