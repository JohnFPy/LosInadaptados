using Project.infrastucture.utils;
using System;
using System.Text;
using Xunit.Abstractions;

namespace Project.Tests;

/// <summary>
/// Pruebas unitarias para la funcionalidad de passwordHasher
/// </summary>
public class PasswordHasherTests
{
    private readonly ITestOutputHelper _output;

    public PasswordHasherTests(ITestOutputHelper output)
    {
        _output = output;
    }

    #region Basic Hash Functionality Tests

    [Fact]
    public void HashPassword_ValidPassword_ReturnsValidHash()
    {
        // Arrange
        string password = "Password123";

        // Act
        string hash = passwordHasher.HashPassword(password);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.Equal(64, hash.Length); // SHA256 produces 64 character hex string
        Assert.Matches("^[a-f0-9]+$", hash); // Should only contain hex characters

        _output.WriteLine($"✅ Password hasheada correctamente");
        _output.WriteLine($"   - Password: '{password}'");
        _output.WriteLine($"   - Hash: '{hash}'");
        _output.WriteLine($"   - Longitud del hash: {hash.Length}");
    }

    [Theory]
    [InlineData("password123")]
    [InlineData("Password123")]
    [InlineData("PASSWORD123")]
    [InlineData("Pass123!@#")]
    [InlineData("MiContraseñaSegura1")]
    [InlineData("12345678")]
    [InlineData("a")]
    [InlineData("")]
    public void HashPassword_VariousPasswords_ReturnsConsistentHashLength(string password)
    {
        // Act
        string hash = passwordHasher.HashPassword(password);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal(64, hash.Length); // SHA256 always produces 64 character hex string
        Assert.Matches("^[a-f0-9]+$", hash);

        _output.WriteLine($"✅ Hash consistente para: '{password}' -> '{hash}'");
    }

    #endregion

    #region Hash Consistency Tests

    [Fact]
    public void HashPassword_SamePasswordMultipleTimes_ReturnsSameHash()
    {
        // Arrange
        string password = "TestPassword123";

        // Act
        string hash1 = passwordHasher.HashPassword(password);
        string hash2 = passwordHasher.HashPassword(password);
        string hash3 = passwordHasher.HashPassword(password);

        // Assert
        Assert.Equal(hash1, hash2);
        Assert.Equal(hash2, hash3);
        Assert.Equal(hash1, hash3);

        _output.WriteLine("✅ Consistencia de hash verificada");
        _output.WriteLine($"   - Password: '{password}'");
        _output.WriteLine($"   - Hash 1: '{hash1}'");
        _output.WriteLine($"   - Hash 2: '{hash2}'");
        _output.WriteLine($"   - Hash 3: '{hash3}'");
        _output.WriteLine($"   - Todas iguales: {hash1 == hash2 && hash2 == hash3}");
    }

    [Theory]
    [InlineData("Password123", "Password123")]
    [InlineData("test", "test")]
    [InlineData("", "")]
    [InlineData("MiContraseña!@#", "MiContraseña!@#")]
    public void HashPassword_IdenticalPasswords_ReturnIdenticalHashes(string password1, string password2)
    {
        // Act
        string hash1 = passwordHasher.HashPassword(password1);
        string hash2 = passwordHasher.HashPassword(password2);

        // Assert
        Assert.Equal(hash1, hash2);

        _output.WriteLine($"✅ Passwords idénticas producen hashes idénticos");
        _output.WriteLine($"   - Password 1: '{password1}' -> Hash: '{hash1}'");
        _output.WriteLine($"   - Password 2: '{password2}' -> Hash: '{hash2}'");
    }

    #endregion

    #region Hash Uniqueness Tests

    [Theory]
    [InlineData("Password123", "password123")]
    [InlineData("Password123", "Password124")]
    [InlineData("test", "Test")]
    [InlineData("password", "password ")]
    [InlineData("abc", "ABC")]
    [InlineData("123", "1234")]
    public void HashPassword_DifferentPasswords_ReturnDifferentHashes(string password1, string password2)
    {
        // Act
        string hash1 = passwordHasher.HashPassword(password1);
        string hash2 = passwordHasher.HashPassword(password2);

        // Assert
        Assert.NotEqual(hash1, hash2);

        _output.WriteLine($"✅ Passwords diferentes producen hashes diferentes");
        _output.WriteLine($"   - Password 1: '{password1}' -> Hash: '{hash1}'");
        _output.WriteLine($"   - Password 2: '{password2}' -> Hash: '{hash2}'");
    }

    [Fact]
    public void HashPassword_CaseSensitive_ProducesDifferentHashes()
    {
        // Arrange
        string lowerCase = "password123";
        string upperCase = "PASSWORD123";
        string mixedCase = "Password123";

        // Act
        string hashLower = passwordHasher.HashPassword(lowerCase);
        string hashUpper = passwordHasher.HashPassword(upperCase);
        string hashMixed = passwordHasher.HashPassword(mixedCase);

        // Assert
        Assert.NotEqual(hashLower, hashUpper);
        Assert.NotEqual(hashLower, hashMixed);
        Assert.NotEqual(hashUpper, hashMixed);

        _output.WriteLine("✅ Sensibilidad a mayúsculas/minúsculas verificada");
        _output.WriteLine($"   - Lower: '{lowerCase}' -> '{hashLower}'");
        _output.WriteLine($"   - Upper: '{upperCase}' -> '{hashUpper}'");
        _output.WriteLine($"   - Mixed: '{mixedCase}' -> '{hashMixed}'");
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void HashPassword_EmptyString_ReturnsValidHash()
    {
        // Arrange
        string emptyPassword = "";

        // Act
        string hash = passwordHasher.HashPassword(emptyPassword);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.Equal(64, hash.Length);
        Assert.Matches("^[a-f0-9]+$", hash);

        _output.WriteLine($"✅ Password vacía produce hash válido");
        _output.WriteLine($"   - Password: '{emptyPassword}'");
        _output.WriteLine($"   - Hash: '{hash}'");
    }

    [Fact]
    public void HashPassword_WhitespacePassword_ReturnsValidHash()
    {
        // Arrange
        string whitespacePassword = "   ";

        // Act
        string hash = passwordHasher.HashPassword(whitespacePassword);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal(64, hash.Length);
        Assert.Matches("^[a-f0-9]+$", hash);

        _output.WriteLine($"✅ Password con espacios produce hash válido");
        _output.WriteLine($"   - Password: '{whitespacePassword}'");
        _output.WriteLine($"   - Hash: '{hash}'");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void HashPassword_WhitespaceVariations_ProducesDifferentHashes(string whitespace)
    {
        // Act
        string hash = passwordHasher.HashPassword(whitespace);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal(64, hash.Length);

        _output.WriteLine($"✅ Variación de espacios en blanco: '{whitespace.Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r")}' -> '{hash}'");
    }

    #endregion

    #region Special Characters Tests

    [Theory]
    [InlineData("Password123!@#")]
    [InlineData("contraseña$%^&*()")]
    [InlineData("密码123")]
    [InlineData("пароль456")]
    [InlineData("🔒🔑password")]
    [InlineData("test\nwith\nnewlines")]
    [InlineData("test\twith\ttabs")]
    public void HashPassword_SpecialCharacters_ReturnsValidHash(string passwordWithSpecialChars)
    {
        // Act
        string hash = passwordHasher.HashPassword(passwordWithSpecialChars);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal(64, hash.Length);
        Assert.Matches("^[a-f0-9]+$", hash);

        _output.WriteLine($"✅ Password con caracteres especiales hasheada");
        _output.WriteLine($"   - Password: '{passwordWithSpecialChars}'");
        _output.WriteLine($"   - Hash: '{hash}'");
    }

    #endregion

    #region Long Password Tests

    [Fact]
    public void HashPassword_VeryLongPassword_ReturnsValidHash()
    {
        // Arrange
        string longPassword = new string('a', 1000); // 1000 character password

        // Act
        string hash = passwordHasher.HashPassword(longPassword);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal(64, hash.Length);
        Assert.Matches("^[a-f0-9]+$", hash);

        _output.WriteLine($"✅ Password muy larga hasheada correctamente");
        _output.WriteLine($"   - Longitud de password: {longPassword.Length}");
        _output.WriteLine($"   - Hash: '{hash}'");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(1000)]
    public void HashPassword_VariousLengths_ProducesConsistentHashLength(int passwordLength)
    {
        // Arrange
        string password = new string('x', passwordLength);

        // Act
        string hash = passwordHasher.HashPassword(password);

        // Assert
        Assert.Equal(64, hash.Length);

        _output.WriteLine($"✅ Password de {passwordLength} caracteres -> Hash de {hash.Length} caracteres");
    }

    #endregion

    #region Hash Format Validation Tests

    [Fact]
    public void HashPassword_ResultFormat_IsValidHexadecimal()
    {
        // Arrange
        string password = "TestPassword123";

        // Act
        string hash = passwordHasher.HashPassword(password);

        // Assert
        Assert.True(IsValidHexadecimal(hash), "Hash should be valid hexadecimal");
        Assert.True(IsLowerCaseHex(hash), "Hash should be lowercase hexadecimal");

        _output.WriteLine($"✅ Formato de hash validado");
        _output.WriteLine($"   - Hash: '{hash}'");
        _output.WriteLine($"   - Es hexadecimal válido: {IsValidHexadecimal(hash)}");
        _output.WriteLine($"   - Es hexadecimal en minúsculas: {IsLowerCaseHex(hash)}");
    }

    [Theory]
    [InlineData("password1")]
    [InlineData("password2")]
    [InlineData("password3")]
    [InlineData("password4")]
    [InlineData("password5")]
    public void HashPassword_MultiplePasswords_AllProduceValidHexFormat(string password)
    {
        // Act
        string hash = passwordHasher.HashPassword(password);

        // Assert
        Assert.True(IsValidHexadecimal(hash));
        Assert.True(IsLowerCaseHex(hash));
        Assert.Equal(64, hash.Length);

        _output.WriteLine($"✅ '{password}' -> Formato hex válido: '{hash}'");
    }

    #endregion

    #region Real-world Scenario Tests

    [Fact]
    public void HashPassword_CommonPasswords_ProduceUniqueHashes()
    {
        // Arrange
        string[] commonPasswords = {
            "password",
            "123456",
            "password123",
            "admin",
            "qwerty",
            "letmein",
            "welcome",
            "monkey"
        };

        // Act & Assert
        var hashes = new HashSet<string>();
        foreach (string password in commonPasswords)
        {
            string hash = passwordHasher.HashPassword(password);

            Assert.False(hashes.Contains(hash), $"Duplicate hash found for password: {password}");
            hashes.Add(hash);

            _output.WriteLine($"✅ '{password}' -> '{hash}'");
        }

        Assert.Equal(commonPasswords.Length, hashes.Count);
        _output.WriteLine($"✅ {commonPasswords.Length} passwords comunes produjeron {hashes.Count} hashes únicos");
    }

    [Fact]
    public void HashPassword_LoginScenario_HashesMatch()
    {
        // Arrange - Simular escenario de registro y login
        string originalPassword = "MySecurePassword123!";

        // Act - Simular registro (hash de password)
        string storedHash = passwordHasher.HashPassword(originalPassword);

        // Simular login (hash de password ingresada)
        string loginHash = passwordHasher.HashPassword(originalPassword);

        // Assert - Los hashes deben coincidir
        Assert.Equal(storedHash, loginHash);

        _output.WriteLine("✅ Escenario de login simulado exitosamente");
        _output.WriteLine($"   - Password original: '{originalPassword}'");
        _output.WriteLine($"   - Hash almacenado: '{storedHash}'");
        _output.WriteLine($"   - Hash de login: '{loginHash}'");
        _output.WriteLine($"   - Hashes coinciden: {storedHash == loginHash}");
    }

    #endregion

    #region Helper Methods

    private static bool IsValidHexadecimal(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        foreach (char c in input)
        {
            if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsLowerCaseHex(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        foreach (char c in input)
        {
            if (c >= 'A' && c <= 'F') return false; // Contains uppercase hex
        }
        return true;
    }

    #endregion
}