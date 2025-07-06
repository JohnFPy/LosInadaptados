using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Project.domain.services;
using Project.presentation.components;
using System;

namespace Project.application.components
{
    public partial class HealthTips : UserControl
    {
        private StackPanel _tipsContainer;
        private readonly Health_Tip_Fetcher _healthTipFetcher = new Health_Tip_Fetcher();

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

            // Obtener tips de salud desde la base de datos
            var healthTips = _healthTipFetcher.GetAllHealthTips();

            if (healthTips.Count == 0)
            {
                Console.WriteLine("No se encontraron tips de salud en la base de datos");
                return;
            }

            // Crear tarjetas usando los datos de la base de datos con CardBuilder
            foreach (var tip in healthTips)
            {
                var card = CardBuilder.CreateStandard()
                    .WithTitle(tip.Name) // T�tulo desde la base de datos
                    .WithDescription(tip.Description) // Descripci�n desde la base de datos
                    .WithImage(GetImageForTip(tip.Name)) // Asignar imagen basada en el nombre
                    .Build();

                _tipsContainer.Children.Add(card);
            }

            Console.WriteLine($"Se crearon {healthTips.Count} tarjetas de tips de salud desde la base de datos");
        }

        /// <summary>
        /// Obtiene la imagen apropiada basada en el nombre del tip
        /// </summary>
        private string GetImageForTip(string tipName)
        {
            var tipNameLower = tipName.ToLower();

            if (tipNameLower.Contains("respiraci�n") || tipNameLower.Contains("respiracion"))
                return "icons/leaf.png";

            if (tipNameLower.Contains("estiramientos") || tipNameLower.Contains("ejercicio"))
                return "icons/meditation.png";

            if (tipNameLower.Contains("dieta") || tipNameLower.Contains("alimentaci�n") || tipNameLower.Contains("alimentacion"))
                return "icons/salad.png";

            // Imagen por defecto para health tips
            return "icons/health.png";
        }

        /// <summary>
        /// Refresca los tips de salud obteniendo datos frescos de la base de datos
        /// </summary>
        public void RefreshHealthTips()
        {
            CreateHealthTips();
        }

        /// <summary>
        /// Verifica si hay conexi�n con la base de datos y tips disponibles
        /// </summary>
        public bool HasDatabaseConnection()
        {
            return _healthTipFetcher.HasHealthTips();
        }

        /// <summary>
        /// Obtiene el n�mero de tips de salud disponibles
        /// </summary>
        public int GetHealthTipsCount()
        {
            return _healthTipFetcher.GetHealthTipsCount();
        }
    }
}