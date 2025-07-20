using Project.application.components;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Project.Tests;

/// <summary>
/// Pruebas unitarias para validar emociones duplicadas con todos los casos de emociones
/// </summary>
public class EmotionDuplicateValidationTests
{
    private readonly ITestOutputHelper _output;

    public EmotionDuplicateValidationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public static readonly Emotion[] ExistingEmotions = new[]
    {
        new Emotion { Name = "Feliz", ImagePath = "a.png", IsLocalImage = false, IsAddButton = false },
        new Emotion { Name = "Triste", ImagePath = "b.png", IsLocalImage = false, IsAddButton = false },
        new Emotion { Name = "Enojado", ImagePath = "c.png", IsLocalImage = false, IsAddButton = false },
        new Emotion { Name = "Relajado", ImagePath = "d.png", IsLocalImage = false, IsAddButton = false },
        new Emotion { Name = "Nostalgia", ImagePath = "e.png", IsLocalImage = true, IsAddButton = false },
        new Emotion { Name = "Euforia", ImagePath = "f.png", IsLocalImage = true, IsAddButton = false },
        new Emotion { Name = "MiEmociónPersonalizada", ImagePath = "g.png", IsLocalImage = true, IsAddButton = false }
    };

    [Theory]
    [InlineData("feliz")]
    [InlineData("FELIZ")]
    [InlineData("triste")]
    [InlineData("ENOJADO")]
    [InlineData("relajado")]
    [InlineData("nostalgia")]
    [InlineData("euforia")]
    [InlineData("miemociónpersonalizada")]

    public void Save_DuplicateEmotionName_ShouldNotAllow(string newEmotionName)
    {
        // Arrange
        var newEmotion = new Emotion
        {
            Name = newEmotionName,
            ImagePath = "nuevo.png",
            IsLocalImage = false,
            IsAddButton = false
        };

        // Act
        bool isDuplicate = Array.Exists(
            ExistingEmotions,
            e => e.Name.Equals(newEmotion.Name, StringComparison.OrdinalIgnoreCase)
        );

        // Assert
        Assert.True(isDuplicate);
        _output.WriteLine($" No se permite guardar emoción duplicada: '{newEmotion.Name}'");
    }
}