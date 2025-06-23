using System;

namespace Project.presentation.components
{
    public class CardBuilder
    {
        private readonly Card _card = new Card();

        public CardBuilder WithAudioFile(string fileName)
        {
            _card.AudioFileName = fileName;
            return this;
        }

        public CardBuilder WithTitle(string title)
        {
            _card.TitleText = title;
            return this;
        }

        public CardBuilder WithDescription(string description)
        {
            _card.CardDescription = description;
            return this;
        }

        public CardBuilder WithButtonTitle()
        {
            _card.UseButtonTitle = true;
            return this;
        }

        public CardBuilder WithTextTitle()
        {
            _card.UseButtonTitle = false;
            return this;
        }

        public static CardBuilder CreateStandard()
        {
            return new CardBuilder().WithTextTitle();
        }

        public static CardBuilder CreateInteractive()
        {
            return new CardBuilder().WithButtonTitle();
        }

        public Card Build()
        {
            return _card;
        }
    }
}