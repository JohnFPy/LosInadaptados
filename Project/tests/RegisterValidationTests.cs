using Project.domain;
using System;
using Xunit.Abstractions;

namespace Project.Tests;

/// <summary>
/// Pruebas unitarias para las validaciones del proceso de registro
/// </summary>
public class RegisterValidationTests
{
    private readonly ITestOutputHelper _output;

    public RegisterValidationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    #region Username Validation Tests

    [Fact]
    public void IsValidUsername_ValidUsername_ReturnsTrue()
    {
        // Arrange
        string validUsername = "usuario123";

        // Act
        bool result = RegisterAutentification.IsValidUsername(validUsername);

        // Assert
        Assert.True(result);
        _output.WriteLine($"✅ Username válido: '{validUsername}' - Resultado: {result}");
    }

    [Theory]
    [InlineData("user")]
    [InlineData("test123")]
    [InlineData("a")]
    [InlineData("1234567890")]
    public void IsValidUsername_ValidUsernames_ReturnsTrue(string username)
    {
        // Act
        bool result = RegisterAutentification.IsValidUsername(username);

        // Assert
        Assert.True(result);
        _output.WriteLine($"✅ Username válido: '{username}' - Resultado: {result}");
    }

    [Theory]
    [InlineData("user name", "contiene espacios")]
    [InlineData("", "está vacío")]
    [InlineData(null, "es nulo")]
    [InlineData("   ", "solo espacios")]
    [InlineData("usuariomuylargo", "más de 10 caracteres")]
    [InlineData("usuario con espacios", "contiene espacios y es largo")]
    public void IsValidUsername_InvalidUsernames_ReturnsFalse(string username, string reason)
    {
        // Act
        bool result = RegisterAutentification.IsValidUsername(username);

        // Assert
        Assert.False(result);
        _output.WriteLine($"❌ Username inválido: '{username}' - Razón: {reason} - Resultado: {result}");
    }

    #endregion

    #region Password Validation Tests

    [Theory]
    [InlineData("Password123")]
    [InlineData("MiPass456")]
    [InlineData("Contraseña1")]
    [InlineData("Test1234")]
    [InlineData("AbC123defG")]
    public void IsValidPassword_ValidPasswords_ReturnsTrue(string password)
    {
        // Act
        bool result = RegisterAutentification.IsValidPassword(password);

        // Assert
        Assert.True(result);
        _output.WriteLine($"✅ Password válida: '{password}' - Resultado: {result}");
    }

    [Theory]
    [InlineData("pass", "muy corta (menos de 8 caracteres)")]
    [InlineData("password", "sin mayúsculas ni números")]
    [InlineData("PASSWORD123", "sin minúsculas")]
    [InlineData("Password", "sin números")]
    [InlineData("", "está vacía")]
    [InlineData(null, "es nula")]
    [InlineData("12345678", "solo números")]
    [InlineData("ABCDEFGH", "solo mayúsculas")]
    [InlineData("abcdefgh", "solo minúsculas")]
    [InlineData("Pass12", "muy corta aunque tenga mayús, minus y números")]
    public void IsValidPassword_InvalidPasswords_ReturnsFalse(string password, string reason)
    {
        // Act
        bool result = RegisterAutentification.IsValidPassword(password);

        // Assert
        Assert.False(result);
        _output.WriteLine($"❌ Password inválida: '{password}' - Razón: {reason} - Resultado: {result}");
    }

    #endregion

    #region Name Validation Tests

    [Theory]
    [InlineData("Juan")]
    [InlineData("María")]
    [InlineData("José")]
    [InlineData("Ana")]
    [InlineData("Nicolás")]
    [InlineData("Sofía")]
    public void IsValidName_ValidNames_ReturnsTrue(string name)
    {
        // Act
        bool result = RegisterAutentification.IsValidName(name);

        // Assert
        Assert.True(result);
        _output.WriteLine($"✅ Nombre válido: '{name}' - Resultado: {result}");
    }

    [Theory]
    [InlineData("Juan Carlos", "contiene espacios")]
    [InlineData("Juan123", "contiene números")]
    [InlineData("Juan@", "contiene símbolos")]
    [InlineData("", "está vacío")]
    [InlineData(null, "es nulo")]
    [InlineData("   ", "solo espacios")]
    [InlineData("Juan-Carlos", "contiene guión")]
    [InlineData("Juan.Carlos", "contiene punto")]
    public void IsValidName_InvalidNames_ReturnsFalse(string name, string reason)
    {
        // Act
        bool result = RegisterAutentification.IsValidName(name);

        // Assert
        Assert.False(result);
        _output.WriteLine($"❌ Nombre inválido: '{name}' - Razón: {reason} - Resultado: {result}");
    }

    #endregion

    #region Lastname Validation Tests

    [Theory]
    [InlineData("García")]
    [InlineData("Rodríguez")]
    [InlineData("López")]
    [InlineData("Martínez")]
    [InlineData("Pérez")]
    [InlineData("González")]
    public void IsValidLastname_ValidLastnames_ReturnsTrue(string lastname)
    {
        // Act
        bool result = RegisterAutentification.IsValidLastname(lastname);

        // Assert
        Assert.True(result);
        _output.WriteLine($"✅ Apellido válido: '{lastname}' - Resultado: {result}");
    }

    [Theory]
    [InlineData("García López", "contiene espacios")]
    [InlineData("García123", "contiene números")]
    [InlineData("García@", "contiene símbolos")]
    [InlineData("", "está vacío")]
    [InlineData(null, "es nulo")]
    [InlineData("   ", "solo espacios")]
    [InlineData("García-López", "contiene guión")]
    [InlineData("García.López", "contiene punto")]
    public void IsValidLastname_InvalidLastnames_ReturnsFalse(string lastname, string reason)
    {
        // Act
        bool result = RegisterAutentification.IsValidLastname(lastname);

        // Assert
        Assert.False(result);
        _output.WriteLine($"❌ Apellido inválido: '{lastname}' - Razón: {reason} - Resultado: {result}");
    }

    #endregion

    #region Age Validation Tests

    [Theory]
    [InlineData("18")]
    [InlineData("25")]
    [InlineData("35")]
    [InlineData("50")]
    [InlineData("1")]
    [InlineData("100")]
    public void IsValidAge_ValidAges_ReturnsTrue(string age)
    {
        // Act
        bool result = RegisterAutentification.IsValidAge(age);

        // Assert
        Assert.True(result);
        _output.WriteLine($"✅ Edad válida: '{age}' - Resultado: {result}");
    }

    [Theory]
    [InlineData("0", "cero no es válido")]
    [InlineData("-5", "número negativo")]
    [InlineData("abc", "no es un número")]
    [InlineData("", "está vacío")]
    [InlineData(null, "es nulo")]
    [InlineData("   ", "solo espacios")]
    [InlineData("25.5", "número decimal")]
    [InlineData("25 años", "contiene texto")]
    [InlineData("-18", "edad negativa")]
    public void IsValidAge_InvalidAges_ReturnsFalse(string age, string reason)
    {
        // Act
        bool result = RegisterAutentification.IsValidAge(age);

        // Assert
        Assert.False(result);
        _output.WriteLine($"❌ Edad inválida: '{age}' - Razón: {reason} - Resultado: {result}");
    }

    #endregion

    #region Complete Registration Scenarios

    [Fact]
    public void CompleteRegistration_ValidData_AllValidationsPass()
    {
        // Arrange
        string username = "usuario123";
        string password = "Password123";
        string name = "Juan";
        string lastname = "García";
        string age = "25";

        // Act
        bool validUsername = RegisterAutentification.IsValidUsername(username);
        bool validPassword = RegisterAutentification.IsValidPassword(password);
        bool validName = RegisterAutentification.IsValidName(name);
        bool validLastname = RegisterAutentification.IsValidLastname(lastname);
        bool validAge = RegisterAutentification.IsValidAge(age);

        bool allValid = validUsername && validPassword && validName && validLastname && validAge;

        // Assert
        Assert.True(validUsername);
        Assert.True(validPassword);
        Assert.True(validName);
        Assert.True(validLastname);
        Assert.True(validAge);
        Assert.True(allValid);

        _output.WriteLine("=== REGISTRO COMPLETO VÁLIDO ===");
        _output.WriteLine($"Username: '{username}' - Válido: {validUsername}");
        _output.WriteLine($"Password: '{password}' - Válida: {validPassword}");
        _output.WriteLine($"Nombre: '{name}' - Válido: {validName}");
        _output.WriteLine($"Apellido: '{lastname}' - Válido: {validLastname}");
        _output.WriteLine($"Edad: '{age}' - Válida: {validAge}");
        _output.WriteLine($"Todas las validaciones: {allValid}");
    }

    [Theory]
    [InlineData("user name", "pass", "Juan123", "García López", "-5")]
    [InlineData("usuariomuylargo", "Password", "Juan Carlos", "García@", "abc")]
    [InlineData("", "", "", "", "")]
    [InlineData(null, null, null, null, null)]
    public void CompleteRegistration_InvalidData_SomeValidationsFail(
        string username, string password, string name, string lastname, string age)
    {
        // Act
        bool validUsername = RegisterAutentification.IsValidUsername(username);
        bool validPassword = RegisterAutentification.IsValidPassword(password);
        bool validName = RegisterAutentification.IsValidName(name);
        bool validLastname = RegisterAutentification.IsValidLastname(lastname);
        bool validAge = RegisterAutentification.IsValidAge(age);

        bool allValid = validUsername && validPassword && validName && validLastname && validAge;

        // Assert
        Assert.False(allValid); // Al menos una validación debe fallar

        _output.WriteLine("=== REGISTRO COMPLETO CON ERRORES ===");
        _output.WriteLine($"Username: '{username}' - Válido: {validUsername}");
        _output.WriteLine($"Password: '{password}' - Válida: {validPassword}");
        _output.WriteLine($"Nombre: '{name}' - Válido: {validName}");
        _output.WriteLine($"Apellido: '{lastname}' - Válido: {validLastname}");
        _output.WriteLine($"Edad: '{age}' - Válida: {validAge}");
        _output.WriteLine($"Todas las validaciones: {allValid}");
    }

    #endregion
}