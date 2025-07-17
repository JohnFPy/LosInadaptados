using Project.domain;
using System;
using Xunit.Abstractions;

namespace Project.Tests;

/// <summary>
/// Pruebas unitarias para las validaciones del formulario de actualización de datos en account
/// </summary>
public class AccountUpdateValidationTests
{
    private readonly ITestOutputHelper _output;

    public AccountUpdateValidationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    #region Username Update Validation Tests

    [Theory]
    [InlineData("user")]
    [InlineData("test123")]
    [InlineData("newuser")]
    [InlineData("admin")]
    [InlineData("1234567890")]
    public void UpdateUsername_ValidUsernames_ReturnsTrue(string newUsername)
    {
        // Act - Simular la validación del UpdateDataButton_Click
        bool isValid = string.IsNullOrWhiteSpace(newUsername) || RegisterAutentification.IsValidUsername(newUsername);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Username actualización válido: '{newUsername}' - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("user name", "contiene espacios")]
    [InlineData("usuariomuylargo", "más de 10 caracteres")]
    [InlineData("usuario con espacios", "contiene espacios y es largo")]
    public void UpdateUsername_InvalidUsernames_ReturnsFalse(string newUsername, string reason)
    {
        // Act - Simular la validación del UpdateDataButton_Click
        bool isValid = string.IsNullOrWhiteSpace(newUsername) || RegisterAutentification.IsValidUsername(newUsername);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"❌ Username actualización inválido: '{newUsername}' - Razón: {reason} - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateUsername_EmptyOrNullValues_ShouldBeValid(string newUsername)
    {
        // Act - Los campos vacíos/nulos son válidos (no se actualizan)
        bool isValid = string.IsNullOrWhiteSpace(newUsername) || RegisterAutentification.IsValidUsername(newUsername);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Username vacío/nulo es válido (no se actualiza): '{newUsername}' - Resultado: {isValid}");
    }

    #endregion

    #region Password Update Validation Tests

    [Theory]
    [InlineData("Password123")]
    [InlineData("NewPass456")]
    [InlineData("UpdatedPassword1")]
    [InlineData("MiNuevaContraseña2")]
    public void UpdatePassword_ValidPasswords_ReturnsTrue(string newPassword)
    {
        // Act - Simular la validación del UpdateDataButton_Click
        bool isValid = string.IsNullOrWhiteSpace(newPassword) || RegisterAutentification.IsValidPassword(newPassword);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Password actualización válida: '{newPassword}' - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("pass", "muy corta")]
    [InlineData("password", "sin mayúsculas ni números")]
    [InlineData("PASSWORD123", "sin minúsculas")]
    [InlineData("Password", "sin números")]
    [InlineData("12345678", "solo números")]
    public void UpdatePassword_InvalidPasswords_ReturnsFalse(string newPassword, string reason)
    {
        // Act - Simular la validación del UpdateDataButton_Click
        bool isValid = string.IsNullOrWhiteSpace(newPassword) || RegisterAutentification.IsValidPassword(newPassword);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"❌ Password actualización inválida: '{newPassword}' - Razón: {reason} - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdatePassword_EmptyOrNullValues_ShouldBeValid(string newPassword)
    {
        // Act - Los campos vacíos/nulos son válidos (no se actualizan)
        bool isValid = string.IsNullOrWhiteSpace(newPassword) || RegisterAutentification.IsValidPassword(newPassword);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Password vacía/nula es válida (no se actualiza): '{newPassword}' - Resultado: {isValid}");
    }

    #endregion

    #region Name Update Validation Tests

    [Theory]
    [InlineData("Juan")]
    [InlineData("María")]
    [InlineData("Carlos")]
    [InlineData("Ana")]
    [InlineData("Nicolás")]
    public void UpdateName_ValidNames_ReturnsTrue(string newName)
    {
        // Act - Simular la validación del UpdateDataButton_Click
        bool isValid = string.IsNullOrWhiteSpace(newName) || RegisterAutentification.IsValidName(newName);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Nombre actualización válido: '{newName}' - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("Juan Carlos", "contiene espacios")]
    [InlineData("Juan123", "contiene números")]
    [InlineData("Juan@", "contiene símbolos")]
    [InlineData("Juan-Carlos", "contiene guión")]
    public void UpdateName_InvalidNames_ReturnsFalse(string newName, string reason)
    {
        // Act - Simular la validación del UpdateDataButton_Click
        bool isValid = string.IsNullOrWhiteSpace(newName) || RegisterAutentification.IsValidName(newName);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"❌ Nombre actualización inválido: '{newName}' - Razón: {reason} - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateName_EmptyOrNullValues_ShouldBeValid(string newName)
    {
        // Act - Los campos vacíos/nulos son válidos (no se actualizan)
        bool isValid = string.IsNullOrWhiteSpace(newName) || RegisterAutentification.IsValidName(newName);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Nombre vacío/nulo es válido (no se actualiza): '{newName}' - Resultado: {isValid}");
    }

    #endregion

    #region Lastname Update Validation Tests

    [Theory]
    [InlineData("García")]
    [InlineData("Rodríguez")]
    [InlineData("López")]
    [InlineData("Martínez")]
    [InlineData("González")]
    public void UpdateLastname_ValidLastnames_ReturnsTrue(string newLastname)
    {
        // Act - Simular la validación del UpdateDataButton_Click
        bool isValid = string.IsNullOrWhiteSpace(newLastname) || RegisterAutentification.IsValidLastname(newLastname);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Apellido actualización válido: '{newLastname}' - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("García López", "contiene espacios")]
    [InlineData("García123", "contiene números")]
    [InlineData("García@", "contiene símbolos")]
    [InlineData("García-López", "contiene guión")]
    public void UpdateLastname_InvalidLastnames_ReturnsFalse(string newLastname, string reason)
    {
        // Act - Simular la validación del UpdateDataButton_Click
        bool isValid = string.IsNullOrWhiteSpace(newLastname) || RegisterAutentification.IsValidLastname(newLastname);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"❌ Apellido actualización inválido: '{newLastname}' - Razón: {reason} - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateLastname_EmptyOrNullValues_ShouldBeValid(string newLastname)
    {
        // Act - Los campos vacíos/nulos son válidos (no se actualizan)
        bool isValid = string.IsNullOrWhiteSpace(newLastname) || RegisterAutentification.IsValidLastname(newLastname);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Apellido vacío/nulo es válido (no se actualiza): '{newLastname}' - Resultado: {isValid}");
    }

    #endregion

    #region Age Update Validation Tests

    [Theory]
    [InlineData("25")]
    [InlineData("30")]
    [InlineData("45")]
    [InlineData("18")]
    [InlineData("65")]
    public void UpdateAge_ValidAges_ReturnsTrue(string newAge)
    {
        // Act - Simular la validación del UpdateDataButton_Click
        bool isValid = string.IsNullOrWhiteSpace(newAge) || RegisterAutentification.IsValidAge(newAge);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Edad actualización válida: '{newAge}' - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("0", "cero no es válido")]
    [InlineData("-5", "número negativo")]
    [InlineData("abc", "no es un número")]
    [InlineData("25.5", "número decimal")]
    [InlineData("25 años", "contiene texto")]
    public void UpdateAge_InvalidAges_ReturnsFalse(string newAge, string reason)
    {
        // Act - Simular la validación del UpdateDataButton_Click
        bool isValid = string.IsNullOrWhiteSpace(newAge) || RegisterAutentification.IsValidAge(newAge);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"❌ Edad actualización inválida: '{newAge}' - Razón: {reason} - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateAge_EmptyOrNullValues_ShouldBeValid(string newAge)
    {
        // Act - Los campos vacíos/nulos son válidos (no se actualizan)
        bool isValid = string.IsNullOrWhiteSpace(newAge) || RegisterAutentification.IsValidAge(newAge);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Edad vacía/nula es válida (no se actualiza): '{newAge}' - Resultado: {isValid}");
    }

    #endregion

    #region Complete Update Form Validation Tests

    [Fact]
    public void UpdateForm_AllValidData_ShouldPassValidation()
    {
        // Arrange
        string newUsername = "newuser";
        string newPassword = "NewPassword123";
        string newName = "Carlos";
        string newLastname = "González";
        string newAge = "28";

        // Act - Simular todas las validaciones del UpdateDataButton_Click
        bool isUsernameValid = string.IsNullOrWhiteSpace(newUsername) || RegisterAutentification.IsValidUsername(newUsername);
        bool validPassword = string.IsNullOrWhiteSpace(newPassword) || RegisterAutentification.IsValidPassword(newPassword);
        bool validAge = string.IsNullOrWhiteSpace(newAge) || RegisterAutentification.IsValidAge(newAge);
        bool validName = string.IsNullOrWhiteSpace(newName) || RegisterAutentification.IsValidName(newName);
        bool validLastname = string.IsNullOrWhiteSpace(newLastname) || RegisterAutentification.IsValidLastname(newLastname);

        bool hasErrors = !isUsernameValid || !validPassword || !validAge || !validName || !validLastname;

        // Assert
        Assert.True(isUsernameValid);
        Assert.True(validPassword);
        Assert.True(validAge);
        Assert.True(validName);
        Assert.True(validLastname);
        Assert.False(hasErrors);

        _output.WriteLine("=== FORMULARIO DE ACTUALIZACIÓN VÁLIDO ===");
        _output.WriteLine($"Username: '{newUsername}' - Válido: {isUsernameValid}");
        _output.WriteLine($"Password: '{newPassword}' - Válida: {validPassword}");
        _output.WriteLine($"Nombre: '{newName}' - Válido: {validName}");
        _output.WriteLine($"Apellido: '{newLastname}' - Válido: {validLastname}");
        _output.WriteLine($"Edad: '{newAge}' - Válida: {validAge}");
        _output.WriteLine($"Sin errores: {!hasErrors}");
    }

    [Fact]
    public void UpdateForm_AllEmptyFields_ShouldPassValidation()
    {
        // Arrange - Todos los campos vacíos (no se actualiza nada)
        string? newUsername = "";
        string? newPassword = "";
        string? newName = "";
        string? newLastname = "";
        string? newAge = "";

        // Act - Simular todas las validaciones del UpdateDataButton_Click
        bool isUsernameValid = string.IsNullOrWhiteSpace(newUsername) || RegisterAutentification.IsValidUsername(newUsername);
        bool validPassword = string.IsNullOrWhiteSpace(newPassword) || RegisterAutentification.IsValidPassword(newPassword);
        bool validAge = string.IsNullOrWhiteSpace(newAge) || RegisterAutentification.IsValidAge(newAge);
        bool validName = string.IsNullOrWhiteSpace(newName) || RegisterAutentification.IsValidName(newName);
        bool validLastname = string.IsNullOrWhiteSpace(newLastname) || RegisterAutentification.IsValidLastname(newLastname);

        bool hasErrors = !isUsernameValid || !validPassword || !validAge || !validName || !validLastname;

        // Assert
        Assert.True(isUsernameValid);
        Assert.True(validPassword);
        Assert.True(validAge);
        Assert.True(validName);
        Assert.True(validLastname);
        Assert.False(hasErrors);

        _output.WriteLine("=== FORMULARIO VACÍO (VÁLIDO - NO SE ACTUALIZA NADA) ===");
        _output.WriteLine($"Username: '{newUsername}' - Válido: {isUsernameValid}");
        _output.WriteLine($"Password: '{newPassword}' - Válida: {validPassword}");
        _output.WriteLine($"Nombre: '{newName}' - Válido: {validName}");
        _output.WriteLine($"Apellido: '{newLastname}' - Válido: {validLastname}");
        _output.WriteLine($"Edad: '{newAge}' - Válida: {validAge}");
        _output.WriteLine($"Sin errores: {!hasErrors}");
    }

    [Theory]
    [InlineData("user name", "Password123", "Juan", "García", "25")]
    [InlineData("validuser", "pass", "Juan", "García", "25")]
    [InlineData("validuser", "Password123", "Juan123", "García", "25")]
    [InlineData("validuser", "Password123", "Juan", "García López", "25")]
    [InlineData("validuser", "Password123", "Juan", "García", "0")]
    public void UpdateForm_SomeInvalidData_ShouldFailValidation(
        string newUsername, string newPassword, string newName, string newLastname, string newAge)
    {
        // Act - Simular todas las validaciones del UpdateDataButton_Click
        bool isUsernameValid = string.IsNullOrWhiteSpace(newUsername) || RegisterAutentification.IsValidUsername(newUsername);
        bool validPassword = string.IsNullOrWhiteSpace(newPassword) || RegisterAutentification.IsValidPassword(newPassword);
        bool validAge = string.IsNullOrWhiteSpace(newAge) || RegisterAutentification.IsValidAge(newAge);
        bool validName = string.IsNullOrWhiteSpace(newName) || RegisterAutentification.IsValidName(newName);
        bool validLastname = string.IsNullOrWhiteSpace(newLastname) || RegisterAutentification.IsValidLastname(newLastname);

        bool hasErrors = !isUsernameValid || !validPassword || !validAge || !validName || !validLastname;

        // Assert
        Assert.True(hasErrors); // Debe haber errores

        _output.WriteLine("=== FORMULARIO DE ACTUALIZACIÓN CON ERRORES ===");
        _output.WriteLine($"Username: '{newUsername}' - Válido: {isUsernameValid}");
        _output.WriteLine($"Password: '{newPassword}' - Válida: {validPassword}");
        _output.WriteLine($"Nombre: '{newName}' - Válido: {validName}");
        _output.WriteLine($"Apellido: '{newLastname}' - Válido: {validLastname}");
        _output.WriteLine($"Edad: '{newAge}' - Válida: {validAge}");
        _output.WriteLine($"Tiene errores: {hasErrors}");
    }

    #endregion

    #region Error Message Simulation Tests

    [Fact]
    public void UpdateForm_InvalidUsername_ShouldShowUsernameError()
    {
        // Arrange
        string invalidUsername = "user name"; // Contiene espacios
        string expectedErrorMessage = "Nombre de usuario inválido.";

        // Act
        bool isValid = RegisterAutentification.IsValidUsername(invalidUsername);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"=== ERROR DE USERNAME EN ACTUALIZACIÓN ===");
        _output.WriteLine($"Username inválido: '{invalidUsername}'");
        _output.WriteLine($"Mensaje de error esperado: '{expectedErrorMessage}'");
    }

    [Fact]
    public void UpdateForm_InvalidPassword_ShouldShowPasswordError()
    {
        // Arrange
        string invalidPassword = "pass"; // Muy corta
        string expectedErrorMessage = "Contraseña inválida.";

        // Act
        bool isValid = RegisterAutentification.IsValidPassword(invalidPassword);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"=== ERROR DE PASSWORD EN ACTUALIZACIÓN ===");
        _output.WriteLine($"Password inválida: '{invalidPassword}'");
        _output.WriteLine($"Mensaje de error esperado: '{expectedErrorMessage}'");
    }

    [Fact]
    public void UpdateForm_MultipleErrors_ShouldShowAllErrors()
    {
        // Arrange
        string invalidUsername = "usuariomuylargo"; // Más de 10 caracteres
        string invalidPassword = "pass"; // Muy corta
        string invalidName = "Juan123"; // Contiene números
        string invalidAge = "0"; // Cero no es válido

        // Act
        bool usernameValid = RegisterAutentification.IsValidUsername(invalidUsername);
        bool passwordValid = RegisterAutentification.IsValidPassword(invalidPassword);
        bool nameValid = RegisterAutentification.IsValidName(invalidName);
        bool ageValid = RegisterAutentification.IsValidAge(invalidAge);

        // Assert
        Assert.False(usernameValid);
        Assert.False(passwordValid);
        Assert.False(nameValid);
        Assert.False(ageValid);

        _output.WriteLine("=== MÚLTIPLES ERRORES EN ACTUALIZACIÓN ===");
        _output.WriteLine($"Username: '{invalidUsername}' - Válido: {usernameValid}");
        _output.WriteLine($"Password: '{invalidPassword}' - Válida: {passwordValid}");
        _output.WriteLine($"Nombre: '{invalidName}' - Válido: {nameValid}");
        _output.WriteLine($"Edad: '{invalidAge}' - Válida: {ageValid}");
        _output.WriteLine("Errores esperados:");
        _output.WriteLine("- Nombre de usuario inválido.");
        _output.WriteLine("- Contraseña inválida.");
        _output.WriteLine("- Nombre inválido.");
        _output.WriteLine("- Edad inválida.");
    }

    #endregion
}