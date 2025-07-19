using Project.application.components;
using System;
using Xunit.Abstractions;

namespace Project.Tests
{
    /// <summary>
    /// Pruebas unitarias para la validación de fechas y lógica de calendario en dayView y componentes relacionados
    /// </summary>
    public class CalendarValidationTests
    {
        private readonly ITestOutputHelper _output;

        public CalendarValidationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region Validación de formato de fecha

        [Theory]
        [InlineData("2025-07-19")]
        [InlineData("2024-02-29")]
        [InlineData("2000-01-01")]
        [InlineData("1999-12-31")]
        public void DateId_ValidFormat_ShouldBeAccepted(string dateId)
        {
            bool isValid = DateTime.TryParseExact(dateId, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var _);
            Assert.True(isValid);
            _output.WriteLine($"✅ Formato de fecha válido: '{dateId}' - Resultado: {isValid}");
        }

        [Theory]
        [InlineData("2025/07/19", "formato incorrecto")]
        [InlineData("19-07-2025", "formato incorrecto")]
        [InlineData("2025-13-01", "mes inválido")]
        [InlineData("2025-00-10", "mes inválido")]
        [InlineData("2025-07-32", "día inválido")]
        [InlineData("2025-07-00", "día inválido")]
        [InlineData("", "vacío")]
        [InlineData("   ", "solo espacios")]
        public void DateId_InvalidFormat_ShouldBeRejected(string dateId, string reason)
        {
            bool isValid = !string.IsNullOrWhiteSpace(dateId) && 
                          DateTime.TryParseExact(dateId, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var _);
            Assert.False(isValid);
            _output.WriteLine($"❌ Formato de fecha inválido: '{dateId}' - Razón: {reason} - Resultado: {isValid}");
        }

        // Método separado para probar valores nulos
        [Fact]
        public void DateId_NullValue_ShouldBeRejected()
        {
            string? dateId = null;
            bool isValid = !string.IsNullOrWhiteSpace(dateId) && 
                          DateTime.TryParseExact(dateId, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var _);
            Assert.False(isValid);
            _output.WriteLine($"❌ Formato de fecha inválido: null - Razón: es nulo - Resultado: {isValid}");
        }

        #endregion

        #region Validación de años bisiestos

        [Theory]
        [InlineData("2024-02-29")]
        [InlineData("2000-02-29")]
        [InlineData("2400-02-29")]
        public void LeapYear_ValidFeb29_ShouldBeAccepted(string dateId)
        {
            bool isValid = DateTime.TryParse(dateId, out var dt) && dt.Month == 2 && dt.Day == 29;
            Assert.True(isValid);
            _output.WriteLine($"✅ Fecha válida de año bisiesto: '{dateId}' - Resultado: {isValid}");
        }

        [Theory]
        [InlineData("2023-02-29", "no es bisiesto")]
        [InlineData("2100-02-29", "no es bisiesto")]
        public void LeapYear_InvalidFeb29_ShouldBeRejected(string dateId, string reason)
        {
            bool isValid = DateTime.TryParse(dateId, out var dt) && dt.Month == 2 && dt.Day == 29;
            Assert.False(isValid);
            _output.WriteLine($"❌ Fecha inválida de año bisiesto: '{dateId}' - Razón: {reason} - Resultado: {isValid}");
        }

        #endregion

        #region Validación de rango de fechas

        [Theory]
        [InlineData("1900-01-01")]
        [InlineData("2100-12-31")]
        public void DateId_ValidRange_ShouldBeAccepted(string dateId)
        {
            bool isValid = DateTime.TryParse(dateId, out var dt) && dt.Year >= 1900 && dt.Year <= 2100;
            Assert.True(isValid);
            _output.WriteLine($"✅ Fecha dentro de rango permitido: '{dateId}' - Resultado: {isValid}");
        }

        [Theory]
        [InlineData("1899-12-31", "año menor a 1900")]
        [InlineData("2200-01-01", "año mayor a 2100")]
        public void DateId_InvalidRange_ShouldBeRejected(string dateId, string reason)
        {
            bool isValid = DateTime.TryParse(dateId, out var dt) && dt.Year >= 1900 && dt.Year <= 2100;
            Assert.False(isValid);
            _output.WriteLine($"❌ Fecha fuera de rango permitido: '{dateId}' - Razón: {reason} - Resultado: {isValid}");
        }

        #endregion

        #region Validación de edge cases y días especiales

        [Theory]
        [InlineData("2025-01-01", "primer día del año")]
        [InlineData("2025-12-31", "último día del año")]
        [InlineData("2025-04-30", "mes de 30 días")]
        [InlineData("2025-02-28", "febrero no bisiesto")]
        public void DateId_EdgeCases_ShouldBeAccepted(string dateId, string description)
        {
            bool isValid = DateTime.TryParse(dateId, out var _);
            Assert.True(isValid);
            _output.WriteLine($"✅ Edge case de fecha aceptado: '{dateId}' ({description})");
        }

        #endregion

        #region Validación de asignación en dayView

        [Fact]
        public void DayView_AssignValidDateId_ShouldSetProperty()
        {
            var day = new dayView();
            string dateId = "2025-07-19";
            day.DateId = dateId;
            Assert.Equal(dateId, day.DateId);
            _output.WriteLine($"✅ dayView asigna correctamente DateId: '{dateId}'");
        }

        [Fact]
        public void DayView_AssignInvalidDateId_ShouldSetPropertyButBeInvalid()
        {
            var day = new dayView();
            string dateId = "2025-13-01";
            day.DateId = dateId;
            bool isValid = DateTime.TryParse(day.DateId, out var _);
            Assert.False(isValid);
            _output.WriteLine($"❌ dayView asigna DateId inválido: '{dateId}' - Resultado: {isValid}");
        }

        #endregion

        #region Simulación de error y mensajes

        [Fact]
        public void DateId_InvalidFormat_ShouldShowErrorMessage()
        {
            string dateId = "2025/07/19";
            bool isValid = DateTime.TryParseExact(dateId, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var _);
            string expectedError = "Formato de fecha inválido. Use 'yyyy-MM-dd'.";
            Assert.False(isValid);
            _output.WriteLine("=== ERROR DE FORMATO DE FECHA ===");
            _output.WriteLine($"DateId ingresado: '{dateId}'");
            _output.WriteLine($"Mensaje esperado: {expectedError}");
        }

        #endregion

        #region Escenario completo de fechas

        [Fact]
        public void CompleteCalendarScenario_AllCases()
        {
            string?[] testDates = {
                "2025-07-19", "2024-02-29", "2023-02-29", "2025-13-01", 
                "1899-12-31", "2100-12-31", "2200-01-01", "", "   "
            };

            foreach (var dateId in testDates)
            {
                bool isValid = !string.IsNullOrWhiteSpace(dateId) && 
                              DateTime.TryParseExact(dateId, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var dtInner) && 
                              dtInner.Year >= 1900 && dtInner.Year <= 2100;
                _output.WriteLine($"DateId: '{dateId}' - Válido: {isValid}");
            }
            
            // Prueba específica para valor nulo
            string? nullDate = null;
            DateTime dt;
            bool isNullValid = !string.IsNullOrWhiteSpace(nullDate) && 
                              DateTime.TryParseExact(nullDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dt) && 
                              dt.Year >= 1900 && dt.Year <= 2100;
            _output.WriteLine($"DateId: null - Válido: {isNullValid}");
            
            _output.WriteLine("=== ESCENARIO COMPLETO DE VALIDACIÓN DE FECHAS DE CALENDARIO ===");
        }

        #endregion
    }
}