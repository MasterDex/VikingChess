using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingChess.Model
{
    /// <summary>
    /// The <see cref="Square"/> class represents a <see cref="Square"/> on the <see cref="Board"/>
    /// </summary>
    public class Square
    {
        /// <summary>
        /// The <see cref="Enums.File"/> of the <see cref="Square"/>
        /// </summary>
        Enums.File file;
        /// <summary>
        /// The <see cref="Enums.Rank"/> of the <see cref="Square"/>
        /// </summary>
        Enums.Rank rank;
        /// <summary>
        /// The <see cref="Piece"/> (if any) on the <see cref="Square"/>
        /// </summary>
        Piece piece;
        /// <summary>
        /// The <see cref="Enums.SquareType"/> of the <see cref="Square"/>
        /// </summary>
        Enums.SquareType type;
        /// <summary>
        /// A <see cref="Square"/> <see cref="Array"/> of the neighbours of the <see cref="Square"/>
        /// </summary>
        Square[] neighbours = new Square[4];

        /// <summary>
        /// Blank Constructor
        /// </summary>
        public Square()
        {

        }

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="rank">The <see cref="rank"/> of this <see cref="Square"/> on the <see cref="Board"/>.</param>
        /// <param name="file">The <see cref="file"/> of this <see cref="Square"/> on the <see cref="Board"/>.</param>
        /// <param name="piece">The <see cref="Piece"/> (if any) occupying this <see cref="Square"/>.</param>
        /// <param name="type">The <see cref="type"/> of this <see cref="Square"/>.</param>
        public Square(Enums.Rank rank, Enums.File file, Piece piece, Enums.SquareType type)
        {
            setLocation(rank, file);
            setPiece(piece);
            setType(type);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="previousSquare">The <see cref="Square"/> to copy</param>
        public Square(Square previousSquare)
        {
            file = previousSquare.file;
            rank = previousSquare.rank;
            piece = new Piece(previousSquare.getPiece());
            type = previousSquare.type;
            neighbours = previousSquare.neighbours;
        }

        /// <summary>
        /// Returns the <see cref="Enums.Rank"/> of the <see cref="Square"/> on the <see cref="Board"/>.
        /// </summary>
        /// <returns><see cref="rank"/></returns>
        public Enums.Rank getRank()
        {
            return rank;
        }

        /// <summary>
        /// Returns the <see cref="Enums.File"/> of the <see cref="Square"/> on the <see cref="Board"/>.
        /// </summary>
        /// <returns><see cref="file"/></returns>
        public Enums.File getFile()
        {
            return file;
        }

        /// <summary>
        /// Returns the <see cref="Piece"/> occupying the <see cref="Square"/>.
        /// </summary>
        /// <returns><see cref="piece"/></returns>
        public Piece getPiece()
        {
            if (piece != null)
            {
                return piece;
            }
            else return null;
            
        }

        /// <summary>
        /// Returns the <see cref="Enums.SquareType"/> of <see cref="Square"/>.
        /// </summary>
        /// <returns><see cref="type"/></returns>
        public Enums.SquareType getSquareType()
        {
            return type;
        }

        /// <summary>
        /// Returns a <see cref="Tuple{T1, T2}"/> with the <see cref="rank"/> and <see cref="file"/> of the <see cref="Square"/>
        /// </summary>
        /// <returns>A <see cref="Tuple{T1, T2}"/> with the <see cref="rank"/> and <see cref="file"/> of the <see cref="Square"/></returns>
        public Tuple<Enums.Rank, Enums.File> getLocation()
        {
            return new Tuple<Enums.Rank, Enums.File>(rank, file);
        }

        /// <summary>
        /// Returns an <see cref="Array"/> containing the neighbours of the <see cref="Square"/>.
        /// </summary>
        /// <returns><see cref="neighbours"/></returns>
        public Square[] getNeighbours()
        {
            return neighbours;
        }

        /// <summary>
        /// Sets the <see cref="rank"/> of the <see cref="Square"/>
        /// </summary>
        /// <param name="rank"><see cref="rank"/></param>
        public void setRank(Enums.Rank rank)
        {
            this.rank = rank;
        }

        /// <summary>
        /// Sets the <see cref="file"/> of the <see cref="Square"/>
        /// </summary>
        /// <param name="file"><see cref="file"/></param>
        public void setFile(Enums.File file)
        {
            this.file = file;
        }

        /// <summary>
        /// Sets the <see cref="rank"/> and <see cref="file"/> of the <see cref="Square"/>
        /// </summary>
        /// <param name="rank"><see cref="rank"/></param>
        /// <param name="file"><see cref="file"/></param>
        public void setLocation(Enums.Rank rank, Enums.File file)
        {
            setRank(rank);
            setFile(file);
        }

        /// <summary>
        /// Sets the <see cref="Piece"/> occupying the <see cref="Square"/>.
        /// </summary>
        /// <param name="piece"><see cref="piece"/></param>
        public void setPiece(Piece piece)
        {
            this.piece = piece;
        }

        /// <summary>
        /// Sets the <see cref="type"/> of the <see cref="Square"/>.
        /// </summary>
        /// <param name="type"><see cref="type"/></param>
        public void setType(Enums.SquareType type)
        {
            this.type = type;
        }

        /// <summary>
        /// Sets the neighbour squares of the <see cref="Square"/>
        /// </summary>
        /// <param name="leftSquare">The left neighbour <see cref="Square"/></param>
        /// <param name="rightSquare">The right neighbour <see cref="Square"/></param>
        /// <param name="topSquare">The top neighbour <see cref="Square"/></param>
        /// <param name="bottomSquare">The bottom neighbour <see cref="Square"/></param>
        public void setNeighbours(Square leftSquare, Square rightSquare, Square topSquare, Square bottomSquare)
        {
            neighbours[0] = leftSquare;
            neighbours[1] = rightSquare;
            neighbours[2] = topSquare;
            neighbours[3] = bottomSquare;
        }

        /// <summary>
        /// ToString method
        /// </summary>
        /// <returns>A String representation of the <see cref="Square"/></returns>
        public override string ToString()
        {
            string theString = "";

            theString += "\n | Location  : " + getFile() + (int)getRank();
            if (getPiece() != null)
            {
                theString += "\n | Piece : " + getPiece().ToString();
            }
            else
            {
                theString += "\n | Piece : null ";
            }
            theString += "\n | Type  : " + getSquareType().ToString();

            return theString;
        }

    }
}
