using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    internal class BoardData
    {
        public int Rows { get; }

        public int Cols { get; }

        public int TileSize { get; }

        public int Mines { get; }

        private int _tileCount;

        public int TileCount
        {
            get
            {
                return _tileCount;
            }
        }

        public Texture2D Texture { get; }

        public int Flags
        {
            get
            {
                return Mines;
            }
        }

        public BoardData(int rows, int cols, int tileSize, int mines, ContentManager content, string path)
        {
            Rows = rows;
            Cols = cols;
            TileSize = tileSize;
            Mines = mines;

            _tileCount = Rows * Cols;

            Texture = content.Load<Texture2D>(path);
        }
    }
}
