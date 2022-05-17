using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Utils;

namespace Minesweeper
{
    internal class Board
    {
        private BoardData data;
        private Sprite sprite;
        private Tile[,] tiles;

        public int FlagsLeft { get; private set; }

        private int tilesCleared = 0;

        public int Width
        {
            get
            {
                return sprite.DestRec.Width;
            }
        }

        public int Height
        {
            get
            {
                return sprite.DestRec.Height;
            }
        }

        public int TileSize
        {
            get
            {
                return data.TileSize;
            }
        }

        public Board(ContentManager content)
        {
            Tile.Init(content);

            data = new BoardData(8, 10, 45, 10, content, "Resources/board_easy");
            tiles = new Tile[data.Rows, data.Cols];
            FlagsLeft = data.Flags;

            sprite = new Sprite(data.Texture);
            sprite.DestRec = new Rectangle(0, HUD.BAR_HEIGHT,
                data.Texture.Width, data.Texture.Height);

            for (int i = 0; i < data.Rows; i++)
            {
                for (int j = 0; j < data.Cols; j++)
                {
                    tiles[i, j] = new Tile(new Point(data.TileSize * j, data.TileSize * i + sprite.DestRec.Y), data.TileSize);
                }
            }

            AddBombs();
        }

        public bool IsZeroTile()
        {
            return tilesCleared == data.TileCount;
        }

        public TileState GetTileState(int row, int col)
        {
            return tiles[row, col].State;
        }

        public bool CanTileBeMined(int row, int col)
        {
            TileState state = tiles[row, col].State;

            return state == TileState.GRASS ||
                state == TileState.HIDDEN_BOMB;
        }

        public void Draw(SpriteBatch sb)
        {
            sprite.Draw(sb);

            for (int i = 0; i < data.Rows; i++)
            {
                for (int j = 0; j < data.Cols; j++)
                {
                    tiles[i, j].Draw(sb);
                }
            }
        }

        public void Reset()
        {
            FlagsLeft = data.Flags;
            tilesCleared = 0;

            for (int i = 0; i < data.Rows; i++)
            {
                for (int j = 0; j < data.Cols; j++)
                {
                    tiles[i, j].Reset();
                }
            }

            AddBombs();
        }

        public bool PlaceFlag(int row, int col)
        {
            bool succcess = true;

            if (FlagsLeft > 0)
            {
                switch (tiles[row, col].State)
                {
                    case TileState.GRASS:
                    tiles[row, col].State = TileState.GRASS_FLAGGED;
                    break;

                    case TileState.HIDDEN_BOMB:
                    tiles[row, col].State = TileState.BOMB_FLAGGED;
                    break;

                    default:
                    succcess = false;
                    break;
                }
            }
            else
            {
                succcess = false;
            }

            if (succcess)
            {
                FlagsLeft--;
                tilesCleared++;
            }

            return succcess;
        }

        public bool ClearFlag(int row, int col)
        {
            bool success = true;

            switch (tiles[row, col].State)
            {
                case TileState.GRASS_FLAGGED:
                tiles[row, col].State = TileState.GRASS;
                break;

                case TileState.BOMB_FLAGGED:
                tiles[row, col].State = TileState.HIDDEN_BOMB;
                break;

                default:
                success = false;
                break;
            }

            if (success)
            {
                FlagsLeft++;
                tilesCleared--;
            }

            return success;
        }


        private void AddBombs()
        {
            int row;
            int col;
            int bombsCreated = 0;

            while (bombsCreated < data.Mines)
            {
                row = Utility.GetRandom(0, data.Rows);
                col = Utility.GetRandom(0, data.Cols);

                if (tiles[row, col].State != TileState.HIDDEN_BOMB)
                {
                    tiles[row, col].State = TileState.HIDDEN_BOMB;
                    bombsCreated++;
                }
            }

            CountAdjacentBombs();
        }

        public void ExplodeBombs()
        {
            for (int i = 0; i < data.Rows; i++)
            {
                for (int j = 0; j < data.Cols; j++)
                {
                    if (tiles[i, j].State == TileState.HIDDEN_BOMB)
                    {
                        tiles[i, j].State = TileState.EXPLODING_BOMB;
                    }
                }
            }
        }

        private void CountAdjacentBombs()
        {
            for (int i = 0; i < data.Rows; i++)
            {
                for (int j = 0; j < data.Cols; j++)
                {
                    tiles[i, j].AdjacentBombs = CountAdjacentBombs(i, j);
                }
            }
        }

        private int CountAdjacentBombs(int row, int col)
        {
            int count = 0;

            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (IsPositionValid(i, j) && tiles[i, j].State == TileState.HIDDEN_BOMB)
                    {
                        if (!(i == row && j == col))
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        public void RevealTile(int row, int col)
        {
            tiles[row, col].State = TileState.DISPLAY_ADJACENT_BOMBS;
            tilesCleared++;

            if (tiles[row, col].AdjacentBombs == 0)
            {
                for (int i = row - 1; i <= row + 1; i++)
                {
                    for (int j = col - 1; j <= col + 1; j++)
                    {
                        if (IsPositionValid(i, j) && tiles[i, j].State != TileState.DISPLAY_ADJACENT_BOMBS)
                        {
                            RevealTile(i, j);
                        }
                    }
                }
            }
        }

        public bool IsPositionValid(int row, int col)
        {
            return row >= 0 && row < data.Rows &&
                col >= 0 && col < data.Cols;
        }

        public bool Contains(Point mousePos)
        {
            return sprite.DestRec.Contains(mousePos);
        }
    }
}