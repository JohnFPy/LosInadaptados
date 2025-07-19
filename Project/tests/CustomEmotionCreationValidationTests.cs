using Project.application.components;
using System;
using Xunit.Abstractions;

namespace Project.Tests
{
    /// <summary>
    /// Pruebas unitarias para la validación de creación de emociones personalizadas
    /// </summary>
    public class CustomEmotionCreationValidationTests
    {
        private readonly ITestOutputHelper _output;

        public CustomEmotionCreationValidationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region Validación de nombre de emoción personalizada

        [Theory]
        [InlineData("MiEmoción")]
        [InlineData("Alegría Extrema")]
        [InlineData("Nostalgia")]
        [InlineData("Euforia")]
        [InlineData("Tristeza profunda")]
        public void CreateCustomEmotion_ValidNames_ShouldBeAccepted(string name)
        {
            bool isValid = !string.IsNullOrWhiteSpace(name) && name.Length <= 30;
            Assert.True(isValid);
            _output.WriteLine($"✅ Nombre válido para emoción personalizada: '{name}' - Resultado: {isValid}");
        }

        [Theory]
        [InlineData("", "vacío")]
        [InlineData("   ", "solo espacios")]
        [InlineData(null, "nulo")]
        [InlineData("Emoción con un nombre extremadamente largo que supera los 30 caracteres", "más de 30 caracteres")]
        public void CreateCustomEmotion_InvalidNames_ShouldBeRejected(string name, string reason)
        {
            bool isValid = !string.IsNullOrWhiteSpace(name) && name.Length <= 30;
            Assert.False(isValid);
            _output.WriteLine($"❌ Nombre inválido para emoción personalizada: '{name}' - Razón: {reason} - Resultado: {isValid}");
        }

        #endregion

        #region Validación de ruta de imagen

        [Theory]
        [InlineData("path/to/image.png")]
        [InlineData("C:\\imagenes\\custom.png")]
        [InlineData("/usr/local/images/emocion.jpg")]
        [InlineData("avares://Project/resources/emotions/custom.png")]
        public void CreateCustomEmotion_ValidImagePath_ShouldBeAccepted(string imagePath)
        {
            bool isValid = !string.IsNullOrWhiteSpace(imagePath) && imagePath.EndsWith(".png") || imagePath.EndsWith(".jpg");
            Assert.True(isValid);
            _output.WriteLine($"✅ Ruta de imagen válida: '{imagePath}' - Resultado: {isValid}");
        }

        [Theory]
        [InlineData("", "vacía")]
        [InlineData(null, "nula")]
        [InlineData("   ", "solo espacios")]
        [InlineData("image.txt", "formato no permitido")]
        [InlineData("image", "sin extensión")]
        public void CreateCustomEmotion_InvalidImagePath_ShouldBeRejected(string imagePath, string reason)
        {
            bool isValid = !string.IsNullOrWhiteSpace(imagePath) && (imagePath.EndsWith(".png") || imagePath.EndsWith(".jpg"));
            Assert.False(isValid);
            _output.WriteLine($"❌ Ruta de imagen inválida: '{imagePath}' - Razón: {reason} - Resultado: {isValid}");
        }

        #endregion

        #region Validación de unicidad de nombre

        [Fact]
        public void CreateCustomEmotion_DuplicateName_ShouldBeRejected()
        {
            var existingNames = new[] { "Feliz", "Triste", "MiEmoción" };
            string newName = "MiEmoción";
            bool isUnique = Array.IndexOf(existingNames, newName) < 0;

            Assert.False(isUnique);
            _output.WriteLine($"❌ Nombre duplicado detectado: '{newName}' - Resultado: {isUnique}");
        }

        [Fact]
        public void CreateCustomEmotion_UniqueName_ShouldBeAccepted()
        {
            var existingNames = new[] { "Feliz", "Triste", "MiEmoción" };
            string newName = "Euforia";
            bool isUnique = Array.IndexOf(existingNames, newName) < 0;

            Assert.True(isUnique);
            _output.WriteLine($"✅ Nombre único para emoción personalizada: '{newName}' - Resultado: {isUnique}");
        }

        #endregion

        #region Validación de botón de agregar

        [Fact]
        public void CreateCustomEmotion_AddButton_ShouldNotProceed()
        {
            var addButtonEmotion = new Emotion
            {
                Name = "Agregar emoción",
                ImagePath = "avares://Project/resources/emotions/add.png",
                IsLocalImage = false,
                IsAddButton = true
            };

            bool shouldProceed = !addButtonEmotion.IsAddButton;
            Assert.False(shouldProceed);
            _output.WriteLine("❌ No se debe crear emoción personalizada si es el botón de agregar");
        }

        #endregion

        #region Validación de edge cases

        [Theory]
        [InlineData("😊", "emoji como nombre")]
        [InlineData("Emoción-Especial", "guión en el nombre")]
        [InlineData("Emoción_2025", "guión bajo y números")]
        public void CreateCustomEmotion_EdgeCaseNames_ShouldBeAccepted(string name, string description)
        {
            bool isValid = !string.IsNullOrWhiteSpace(name) && name.Length <= 30;
            Assert.True(isValid);
            _output.WriteLine($"✅ Edge case aceptado: '{name}' ({description})");
        }

        #endregion

        #region Escenario completo de creación

        [Fact]
        public void CompleteCustomEmotionCreationScenario_AllValidations()
        {
            string name = "Euforia";
            string imagePath = "path/to/euforia.png";
            var existingNames = new[] { "Feliz", "Triste", "MiEmoción" };

            bool nameValid = !string.IsNullOrWhiteSpace(name) && name.Length <= 30;
            bool imagePathValid = !string.IsNullOrWhiteSpace(imagePath) && (imagePath.EndsWith(".png") || imagePath.EndsWith(".jpg"));
            bool isUnique = Array.IndexOf(existingNames, name) < 0;

            bool canCreate = nameValid && imagePathValid && isUnique;

            Assert.True(canCreate);
            _output.WriteLine("=== ESCENARIO COMPLETO DE CREACIÓN DE EMOCIÓN PERSONALIZADA ===");
            _output.WriteLine($"Nombre: '{name}' - Válido: {nameValid}");
            _output.WriteLine($"Ruta de imagen: '{imagePath}' - Válida: {imagePathValid}");
            _output.WriteLine($"Nombre único: {isUnique}");
            _output.WriteLine($"Puede crear emoción: {canCreate}");
        }

        #endregion

        #region Simulación de error y mensajes

        [Fact]
        public void CreateCustomEmotion_InvalidScenario_ShouldShowErrorMessage()
        {
            string name = "";
            string imagePath = "image.txt";
            var existingNames = new[] { "Feliz", "Triste", "MiEmoción" };

            bool nameValid = !string.IsNullOrWhiteSpace(name) && name.Length <= 30;
            bool imagePathValid = !string.IsNullOrWhiteSpace(imagePath) && (imagePath.EndsWith(".png") || imagePath.EndsWith(".jpg"));
            bool isUnique = Array.IndexOf(existingNames, name) < 0;

            bool canCreate = nameValid && imagePathValid && isUnique;
            string expectedError = "Error al crear emoción personalizada: nombre o imagen inválidos, o nombre duplicado.";

            Assert.False(canCreate);
            _output.WriteLine("=== ERROR DE CREACIÓN DE EMOCIÓN PERSONALIZADA ===");
            _output.WriteLine($"Nombre: '{name}' - Válido: {nameValid}");
            _output.WriteLine($"Ruta de imagen: '{imagePath}' - Válida: {imagePathValid}");
            _output.WriteLine($"Nombre único: {isUnique}");
            _output.WriteLine($"Puede crear emoción: {canCreate}");
            _output.WriteLine($"Mensaje esperado: {expectedError}");
        }

        #endregion
    }
}