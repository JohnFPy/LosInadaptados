using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Project.application.components;
using System;

namespace Project.presentation.components
{
    public partial class audioStatistics : UserControl
    {
        private Project.application.components.audioStatistics? _viewModel;
        private StackPanel? _statisticsContainer;

        public audioStatistics()
        {
            InitializeComponent();
            SetupComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SetupComponent()
        {
            // Crear una instancia del ViewModel y asignarla como DataContext
            _viewModel = new Project.application.components.audioStatistics();
            this.DataContext = _viewModel;

            // Obtener referencia al contenedor
            _statisticsContainer = this.FindControl<StackPanel>("StatisticsContainer");

            // Crear las cards iniciales
            RefreshStatistics();
        }

        /// <summary>
        /// Actualiza las estad�sticas obteniendo datos frescos de la base de datos
        /// </summary>
        public void RefreshStatistics()
        {
            if (_viewModel != null && _statisticsContainer != null)
            {
                _viewModel.CreateStatisticsCards(_statisticsContainer);
            }
        }

        /// <summary>
        /// Se ejecuta cuando el control se vuelve visible
        /// </summary>
        protected override void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            // Actualizar estad�sticas cada vez que el control se hace visible
            RefreshStatistics();
        }

        /// <summary>
        /// M�todo p�blico para forzar actualizaci�n desde c�digo externo
        /// </summary>
        public void ForceRefresh()
        {
            RefreshStatistics();
        }
    }
}