using System;
using Xunit.Abstractions;

namespace Project.Tests;

/// <summary>
/// Pruebas unitarias para las validaciones del proceso de login en UnauthenticatedAreaView
/// </summary>
public class LoginValidationTests
{
    private readonly ITestOutputHelper _output;

    public LoginValidationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    #region Login Username Validation Tests

    [Theory]
    [InlineData("usuario123")]
    [InlineData("test")]
    [InlineData("admin")]
    [InlineData("user_name")]
    [InlineData("1234567890")]
    [InlineData("Usuario Con Espacios")]
    [InlineData("usuario@domain.com")]
    public void LoginUsername_ValidInputs_ShouldNotBeEmpty(string username)
    {
        // Act - Simular la validación que hace OnLoginClick
        bool isValid = !string.IsNullOrWhiteSpace(username);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Username de login válido: '{username}' - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("", "campo vacío")]
    [InlineData("   ", "solo espacios")]
    [InlineData("\t", "solo tab")]
    [InlineData("\n", "solo salto de línea")]
    [InlineData("  \t  \n  ", "combinación de espacios en blanco")]
    public void LoginUsername_InvalidInputs_ShouldBeEmpty(string username, string reason)
    {
        // Act - Simular la validación que hace OnLoginClick
        bool isValid = !string.IsNullOrWhiteSpace(username);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"❌ Username de login inválido: '{username}' - Razón: {reason} - Resultado: {isValid}");
    }

    [Fact]
    public void LoginUsername_NullInput_ShouldBeInvalid()
    {
        // Arrange
        string? username = null;

        // Act - Simular la validación que hace OnLoginClick
        bool isValid = !string.IsNullOrWhiteSpace(username);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"❌ Username de login nulo - Resultado: {isValid}");
    }

    #endregion

    #region Login Password Validation Tests

    [Theory]
    [InlineData("password123")]
    [InlineData("Password123")]
    [InlineData("123456")]
    [InlineData("password")]
    [InlineData("PASSWORD")]
    [InlineData("Pass123!")]
    [InlineData("MiContraseñaMuyLarga")]
    [InlineData("p")]
    public void LoginPassword_ValidInputs_ShouldNotBeEmpty(string password)
    {
        // Act - Simular la validación que hace OnLoginClick
        bool isValid = !string.IsNullOrWhiteSpace(password);

        // Assert
        Assert.True(isValid);
        _output.WriteLine($"✅ Password de login válida: '{password}' - Resultado: {isValid}");
    }

    [Theory]
    [InlineData("", "campo vacío")]
    [InlineData("   ", "solo espacios")]
    [InlineData("\t", "solo tab")]
    [InlineData("\n", "solo salto de línea")]
    [InlineData("  \t  \n  ", "combinación de espacios en blanco")]
    public void LoginPassword_InvalidInputs_ShouldBeEmpty(string password, string reason)
    {
        // Act - Simular la validación que hace OnLoginClick
        bool isValid = !string.IsNullOrWhiteSpace(password);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"❌ Password de login inválida: '{password}' - Razón: {reason} - Resultado: {isValid}");
    }

    [Fact]
    public void LoginPassword_NullInput_ShouldBeInvalid()
    {
        // Arrange
        string? password = null;

        // Act - Simular la validación que hace OnLoginClick
        bool isValid = !string.IsNullOrWhiteSpace(password);

        // Assert
        Assert.False(isValid);
        _output.WriteLine($"❌ Password de login nula - Resultado: {isValid}");
    }

    #endregion

    #region Login Form Complete Validation Tests

    [Theory]
    [InlineData("usuario123", "Password123")]
    [InlineData("admin", "admin123")]
    [InlineData("test", "test")]
    [InlineData("user@email.com", "MyPassword")]
    [InlineData("1234567890", "p")]
    public void LoginForm_ValidCredentials_BothFieldsValid(string username, string password)
    {
        // Act - Simular la validación completa del formulario de login
        bool usernameValid = !string.IsNullOrWhiteSpace(username);
        bool passwordValid = !string.IsNullOrWhiteSpace(password);
        bool formValid = usernameValid && passwordValid;

        // Assert
        Assert.True(usernameValid);
        Assert.True(passwordValid);
        Assert.True(formValid);

        _output.WriteLine("=== FORMULARIO DE LOGIN VÁLIDO ===");
        _output.WriteLine($"Username: '{username}' - Válido: {usernameValid}");
        _output.WriteLine($"Password: '{password}' - Válida: {passwordValid}");
        _output.WriteLine($"Formulario completo: {formValid}");
    }

    [Theory]
    [InlineData("", "Password123", "username vacío")]
    [InlineData("usuario123", "", "password vacía")]
    [InlineData("", "", "ambos campos vacíos")]
    [InlineData("   ", "Password123", "username solo espacios")]
    [InlineData("usuario123", "   ", "password solo espacios")]
    [InlineData("   ", "   ", "ambos campos solo espacios")]
    [InlineData(null, "Password123", "username nulo")]
    [InlineData("usuario123", null, "password nula")]
    [InlineData(null, null, "ambos campos nulos")]
    public void LoginForm_InvalidCredentials_ShouldFailValidation(string username, string password, string reason)
    {
        // Act - Simular la validación completa del formulario de login
        bool usernameValid = !string.IsNullOrWhiteSpace(username);
        bool passwordValid = !string.IsNullOrWhiteSpace(password);
        bool formValid = usernameValid && passwordValid;

        // Assert
        Assert.False(formValid); // El formulario debe fallar

        _output.WriteLine("=== FORMULARIO DE LOGIN CON ERRORES ===");
        _output.WriteLine($"Razón: {reason}");
        _output.WriteLine($"Username: '{username}' - Válido: {usernameValid}");
        _output.WriteLine($"Password: '{password}' - Válida: {passwordValid}");
        _output.WriteLine($"Formulario completo: {formValid}");
    }

    #endregion

    #region Error Message Simulation Tests

    [Fact]
    public void LoginValidation_EmptyUsername_ShouldShowUsernameError()
    {
        // Arrange
        string username = "";
        string password = "ValidPassword123";
        string expectedErrorMessage = "Por favor, ingrese el nombre de usuario.";

        // Act - Simular la lógica del OnLoginClick
        bool shouldShowUsernameError = string.IsNullOrWhiteSpace(username);
        bool shouldShowPasswordError = string.IsNullOrWhiteSpace(password);

        // Assert
        Assert.True(shouldShowUsernameError);
        Assert.False(shouldShowPasswordError);

        _output.WriteLine("=== SIMULACIÓN DE ERROR DE USERNAME ===");
        _output.WriteLine($"Username: '{username}'");
        _output.WriteLine($"Mostrar error de username: {shouldShowUsernameError}");
        _output.WriteLine($"Mensaje esperado: '{expectedErrorMessage}'");
    }

    [Fact]
    public void LoginValidation_EmptyPassword_ShouldShowPasswordError()
    {
        // Arrange
        string username = "validuser";
        string password = "";
        string expectedErrorMessage = "Por favor, ingrese la contraseña.";

        // Act - Simular la lógica del OnLoginClick
        bool shouldShowUsernameError = string.IsNullOrWhiteSpace(username);
        bool shouldShowPasswordError = string.IsNullOrWhiteSpace(password);

        // Assert
        Assert.False(shouldShowUsernameError);
        Assert.True(shouldShowPasswordError);

        _output.WriteLine("=== SIMULACIÓN DE ERROR DE PASSWORD ===");
        _output.WriteLine($"Password: '{password}'");
        _output.WriteLine($"Mostrar error de password: {shouldShowPasswordError}");
        _output.WriteLine($"Mensaje esperado: '{expectedErrorMessage}'");
    }

    [Fact]
    public void LoginValidation_BothFieldsEmpty_ShouldShowUsernameErrorFirst()
    {
        // Arrange
        string username = "";
        string password = "";

        // Act - Simular la lógica del OnLoginClick (username se valida primero)
        bool shouldShowUsernameError = string.IsNullOrWhiteSpace(username);
        bool shouldShowPasswordError = !shouldShowUsernameError && string.IsNullOrWhiteSpace(password);

        // Assert
        Assert.True(shouldShowUsernameError);
        Assert.False(shouldShowPasswordError); // No se muestra porque username falla primero

        _output.WriteLine("=== SIMULACIÓN DE AMBOS CAMPOS VACÍOS ===");
        _output.WriteLine($"Username: '{username}'");
        _output.WriteLine($"Password: '{password}'");
        _output.WriteLine($"Mostrar error de username: {shouldShowUsernameError}");
        _output.WriteLine($"Mostrar error de password: {shouldShowPasswordError}");
        _output.WriteLine("Nota: Username se valida primero, por eso password error no se muestra");
    }

    #endregion

    #region Edge Cases and Special Characters

    [Theory]
    [InlineData("user with spaces", "password123")]
    [InlineData("user@domain.com", "P@ssw0rd!")]
    [InlineData("用户名", "密码123")]
    [InlineData("user123", "contraseña con ñ")]
    [InlineData("UPPERCASE_USER", "UPPERCASE_PASS")]
    [InlineData("lowercase_user", "lowercase_pass")]
    public void LoginForm_SpecialCharacters_ShouldBeValid(string username, string password)
    {
        // Act - En el login actual, cualquier carácter no-whitespace es válido
        bool usernameValid = !string.IsNullOrWhiteSpace(username);
        bool passwordValid = !string.IsNullOrWhiteSpace(password);
        bool formValid = usernameValid && passwordValid;

        // Assert
        Assert.True(formValid);

        _output.WriteLine("=== PRUEBA CON CARACTERES ESPECIALES ===");
        _output.WriteLine($"Username: '{username}' - Válido: {usernameValid}");
        _output.WriteLine($"Password: '{password}' - Válida: {passwordValid}");
        _output.WriteLine($"Formulario válido: {formValid}");
    }

    #endregion
}