namespace VikingChess.Model
{
    /// <summary>
    /// A helper class containing enumerators.
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// Blank Constructor
        /// </summary>
        private Enums()
        {

        }

        /// <summary>
        /// Represents the File of a <see cref="Piece"/> on the <see cref="Board"/>
        /// </summary>
        public enum File { A, B, C, D, E, F, G, H, I, J, K }

        /// <summary>
        /// Represents the Rank of a <see cref="Piece"/> on the <see cref="Board"/>
        /// </summary>
        public enum Rank { None, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Eleven }

        /// <summary>
        /// Represents the color of a <see cref="Piece"/> or a <see cref="Player"/>
        /// </summary>
        public enum Color { WHITE, BLACK }

        /// <summary>
        /// Represents the type of a <see cref="Piece"/>
        /// </summary>
        public enum PieceType { PAWN, KING }

        /// <summary>
        /// Represents the type of a <see cref="Square"/>
        /// </summary>
        public enum SquareType { CORNER, THRONE, REGULAR }

        /// <summary>
        /// Represents the type of a <see cref="Player"/>
        /// </summary>
        public enum PlayerType { HUMAN, CPU }
    }
}
