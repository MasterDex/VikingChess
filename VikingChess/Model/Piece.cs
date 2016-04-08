using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace VikingChess.Model
{
    /// <summary>
    /// The Piece class represents a gamepiece on the <see cref="Board"/>
    /// </summary>
    public class Piece
    {
        /// <summary>
        /// The <see cref="Enums.Color"/> of the <see cref="Piece"/>
        /// </summary>
        private Enums.Color color;
        /// <summary>
        /// The <see cref="Enums.PieceType"/> of the <see cref="Piece"/>
        /// </summary>
        private Enums.PieceType type;
        /// <summary>
        /// The <see cref="Piece"/>'s <see cref="Enums.Rank"/> on the <see cref="Board"/>
        /// </summary>
        private Enums.Rank rank;
        /// <summary>
        /// The <see cref="Piece"/>'s <see cref="Enums.File"/> on the <see cref="Board"/>
        /// </summary>
        private Enums.File file;
        /// <summary>
        /// An <see cref="List{T}"/> containing all legal moves for the <see cref="Piece"/> from its current location on the <see cref="Board"/>
        /// </summary>
        private List<Square> legalMoves = new List<Square>();

        #region Constructors

        /// <summary>
        /// Blank Constructor.
        /// </summary>
        public Piece()
        {

        }

        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="color"><see cref="color"/></param>
        /// <param name="type"><see cref="type"/></param>
        public Piece(Enums.Color color, Enums.PieceType type)
        {
            setColor(color);
            setType(type);
        }

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="color"><see cref="color"/></param>
        /// <param name="type"><see cref="type"/></param>
        /// <param name="rank"><see cref="rank"/></param>
        /// <param name="file"><see cref="file"/></param>
        public Piece(Enums.Color color, Enums.PieceType type, Enums.Rank rank, Enums.File file)
        {
            setColor(color);
            setType(type);
            setLocation(rank, file);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="previousPiece">The <see cref="Piece"/> to copy</param>
        public Piece(Piece previousPiece)
        {
            if (previousPiece != null)
            {
                color = previousPiece.color;
                type = previousPiece.type;
                file = previousPiece.file;
                rank = previousPiece.rank;
            }
        }

        #endregion

        #region Getters and Setters

        /// <summary>
        /// Returns the <see cref="color"/> of the <see cref="Piece"/>
        /// </summary>
        /// <returns><see cref="color"/></returns>
        public Enums.Color getColor()
        {
            return color;
        }

        /// <summary>
        /// Returns the <see cref="type"/> of the <see cref="Piece"/>
        /// </summary>
        /// <returns><see cref="type"/></returns>
        public Enums.PieceType getType()
        {
            return type;
        }

        /// <summary>
        /// Returns the rank of the <see cref="Piece"/> on the board.
        /// </summary>
        /// <returns><see cref="rank"/></returns>
        public Enums.Rank getRank()
        {
            return rank;
        }

        /// <summary>
        /// Returns the file of the <see cref="Piece"/> on the board.
        /// </summary>
        /// <returns><see cref="file"/></returns>
        public Enums.File getFile()
        {
            return file;
        }

        /// <summary>
        /// Returns a <see cref="Tuple{T1, T2}"/> containing the rank and file of the <see cref="Piece"/>
        /// </summary>
        /// <returns>A <see cref="Tuple{T1, T2}"/> containing the rank and file of the <see cref="Piece"/></returns>
        public Tuple<Enums.Rank, Enums.File> getLocation()
        {
            return new Tuple<Enums.Rank, Enums.File>(rank, file);
        }

        /// <summary>
        /// Sets the rank of the <see cref="Piece"/>
        /// </summary>
        /// <param name="rank"><see cref="rank"/></param>
        public void setRank(Enums.Rank rank)
        {
            this.rank = rank;
        }

        /// <summary>
        /// Sets the file of the <see cref="Piece"/>
        /// </summary>
        /// <param name="file"><see cref="file"/></param>
        public void setFile(Enums.File file)
        {
            this.file = file;
        }

        /// <summary>
        /// Sets the rank and file of this <see cref="Piece"/>
        /// </summary>
        /// <param name="rank"><see cref="rank"/></param>
        /// <param name="file"><see cref="file"/></param>
        public void setLocation(Enums.Rank rank, Enums.File file)
        {
            setRank(rank);
            setFile(file);
        }

        /// <summary>
        /// Sets the <see cref="color"/> of the <see cref="Piece"/>.
        /// </summary>
        /// <param name="color">The <see cref="color"/> of the <see cref="Piece"/>. Can be either <see cref="Enums.Color.BLACK"/> or <see cref="Enums.Color.WHITE"/>.</param>
        public void setColor(Enums.Color color)
        {
            this.color = color;
        }

        /// <summary>
        /// Sets the <see cref="type"/> of the <see cref="Piece"/>.
        /// </summary>
        /// <param name="type">The <see cref="type"/> of the <see cref="Piece"/>. Can be either <see cref="Enums.PieceType.PAWN"/> or <see cref="Enums.PieceType.KING"/>.</param>
        public void setType(Enums.PieceType type)
        {
            this.type = type;
        }

        #endregion

        #region Additional Methods

        /// <summary>
        /// Returns true or false if the <see cref="Piece"/> can move to the <see cref="Square"/>
        /// </summary>
        /// <param name="square">The <see cref="Square"/> to check against</param>
        /// <returns>true/false</returns>
        public Boolean canMoveTo(Square square)
        {
            // If the piece is a KING
            if (getType().Equals(Enums.PieceType.KING) )
            {
                // Return true if the square is empty, false otherwise
                return square.getPiece() == null ? true : false;
            }

            // If the square is NOT a CORNER or a THRONE and the piece is a PAWN
            if (!(square.getSquareType().Equals(Enums.SquareType.CORNER)) && !(square.getSquareType().Equals(Enums.SquareType.THRONE)) && (getType().Equals(Enums.PieceType.PAWN)))
            {
                // Return true if the square is empty, false otherwise
                return square.getPiece() == null ? true : false;
            }
            else return false;
        }

        /// <summary>
        /// Returns an List containing all legal Moves for the <see cref="Piece"/> from its current <see cref="Square"/>.
        /// </summary>
        /// <param name="board">The <see cref="Board"/> that represents the state to check from.</param>
        /// <returns>An <see cref="List{T}"/> containing all <see cref="Square"/>s that the <see cref="Piece"/> can move to.</returns>
        public List<Square> generateLegalMoves(Board board)
        {
            // Get a tuple containing the integer co-ordinates of the Piece on the board
            Tuple<int, int> position = board.GetPosition(getLocation());
            int row = position.Item1;
            int column = position.Item2;
            
            // Reset the List to ensure no outdated legal moves exist.
            legalMoves.Clear();

            // Generate Up Moves
            for (int i = row - 1; i >= 0; --i)
            {
                Square square = board.GetSquare(i, column);
                if(square != null)
                {
                    if (canMoveTo(square))
                    {
                        legalMoves.Add(square);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            row = position.Item1;
            column = position.Item2;
            // Generate Down Moves
            for (int i = row + 1; i < board.GetSize(); ++i)
            {
                Square square = board.GetSquare(i, column);
                if (square != null)
                {
                    if (canMoveTo(square))
                    {
                        legalMoves.Add(square);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            row = position.Item1;
            column = position.Item2;
            // Generate Left Moves
            for (int i = column - 1; i >= 0; --i)
            {
                Square square = board.GetSquare(row, i);
                if (square != null)
                {
                    if (canMoveTo(square))
                    {
                        legalMoves.Add(square);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            row = position.Item1;
            column = position.Item2;
            // Generate Right Moves
            for (int i = column + 1; i < board.GetSize(); ++i)
            {
                Square square = board.GetSquare(row, i);
                if (square != null)
                {
                    if (canMoveTo(square))
                    {
                        legalMoves.Add(square);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return legalMoves;
        }

        /// <summary>
        /// <see cref="ToString"/> override
        /// </summary>
        /// <returns>A <see cref="String"/> representation of the <see cref="Piece"/></returns>
        public override string ToString()
        {
            string theString = "";

            theString += "\n | Color : " + getColor().ToString();
            theString += "\n | Type  : " + getType().ToString();
            theString += "\n | Location  : " + getFile() + (int)getRank();

            return theString;
        }

        #endregion
    }
}
