using System;
using System.ComponentModel;
using System.Drawing;

namespace Quixo
{
    /// <summary>
    /// This class represents a move within a Quixo game.
    /// </summary>
    [Serializable]
    public sealed class Move:INotifyPropertyChanged
    {
        private readonly Player player = Player.None;
        public Player Player { get { return player; } }
        private Point source = Point.Empty;
        public Point Source { get { return source; } }
        private Point destination = Point.Empty;
        public Point Destination { get { return destination; } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Move(Player player, Point source, Point destination) =>
            (this.player, this.source, this.destination) = (player, source, destination);

        public string Print() => $"Player {this.player}: {this.source} to {this.destination}";

    }
}