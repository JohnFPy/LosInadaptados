using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Project.presentation.components;

namespace Project.application.components
{
    public partial class HealthTips : UserControl
    {
        private StackPanel _tipsContainer;

        public HealthTips()
        {
            InitializeComponent();
            CreateHealthTips();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _tipsContainer = this.FindControl<StackPanel>("TipsContainer");
        }

        private void CreateHealthTips()
        {
            // Limpiar el contenedor para a�adir las tarjetas program�ticamente
            _tipsContainer.Children.Clear();

            // Crear tarjetas usando el CardBuilder
            var breathingCard = CardBuilder.CreateStandard() // Sin bot�n interactivo, solo texto
                .WithTitle("Prueba la respiraci�n profunda")
                .WithDescription("Reduce el estr�s y mejora tu enfoque")
                .WithImage("icons/leaf.png") // Si tienes esta imagen disponible
                .Build();

            var stretchingCard = CardBuilder.CreateStandard()
                .WithTitle("Haz estiramientos")
                .WithDescription("Mejora tu postura y alivia la tensi�n")
                .WithImage("icons/meditation.png") // Si tienes esta imagen disponible
                .Build();

            var dietCard = CardBuilder.CreateStandard()
                .WithTitle("Mant�n una dieta equilibrada")
                .WithDescription("Nutre tu cuerpo con alimentos saludables")
                .WithImage("icons/salad.png") // Si tienes esta imagen disponible
                .Build();

            // A�adir las tarjetas al contenedor
            _tipsContainer.Children.Add(breathingCard);
            _tipsContainer.Children.Add(stretchingCard);
            _tipsContainer.Children.Add(dietCard);
        }
    }
}