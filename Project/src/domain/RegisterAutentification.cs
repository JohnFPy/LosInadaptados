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
        public static bool IsValidAge(string? ageText)
        {
            if (int.TryParse(ageText, out int age)) 
            {
                return age > 0;
            }
            return false;
        }
        public static bool IsValidName(string? nameText)
        {
            if (string.IsNullOrWhiteSpace(nameText))
                return false;
            // Solo letras, sin espacios ni números
            return Regex.IsMatch(nameText, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ]+$");
        }

        public static bool IsValidLastName(string? lastNameText)
        {
            if (string.IsNullOrWhiteSpace(lastNameText))
                return false;
            // Solo letras, sin espacios ni números
            return Regex.IsMatch(lastNameText, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ]+$");
        }

        public static bool IsValidPassword(string? password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            // Al menos 8 caracteres, una mayúscula, una minúscula y un número
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
            return regex.IsMatch(password);
        }


    }
}