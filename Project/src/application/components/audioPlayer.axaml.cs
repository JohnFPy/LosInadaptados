using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Project.application.components;
using System;

namespace Project.presentation.components
{
    public partial class AudioPlayer : UserControl
    {
        private StackPanel _cardsContainer;

        public AudioPlayer()
        {
            InitializeComponent();
            CreateCards();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _cardsContainer = this.FindControl<StackPanel>("CardsContainer");
        }

        private void CreateCards()
        {
            // Create all cards with interactive (button) titles
            var pianoCard = CardBuilder.CreateInteractive()  // Changed from CreateStandard to CreateInteractive
                .WithAudioFile("relaxingPiano.mp3")
                .WithTitle("Piano relajante")
                .WithDescription("Calma tu mente en minutos")
                .Build();

            // This one was already interactive
            var japanCard = CardBuilder.CreateInteractive()
                .WithAudioFile("japanBeat.wav")
                .WithTitle("Ritmo japonés")
                .WithDescription("Energía con inspiración oriental")
                .Build();

            // Changed this one from standard to interactive
            var folkCard = CardBuilder.CreateInteractive()  // Changed from CreateStandard to CreateInteractive
                .WithAudioFile("ethnoFolk.wav")
                .WithTitle("Ethno folk")
                .WithDescription("Sonidos tradicionales del mundo")
                .Build();

            // Add all cards to the container
            _cardsContainer.Children.Add(pianoCard);
            _cardsContainer.Children.Add(japanCard);
            _cardsContainer.Children.Add(folkCard);
        }
    }
}