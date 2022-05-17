using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Utils;
using Microsoft.Xna.Framework.Audio;

namespace Minesweeper
{
    enum TileState
    {
        GRASS,
        FLAGGED,

        GRASS_FLAGGED,
        BOMB_FLAGGED,

        DISPLAY_ADJACENT_BOMBS,
        HIDDEN_BOMB,
        EXPLODING_BOMB
    }

    class Tile
    {
        private const int BOMB_COUNT_TEXTURES = 8;

        public static SoundEffect FlagClearSound { get; private set; }

        public static SoundEffect FlagPlaceSound { get; private set; }

        public static SoundEffect BombSound { get; private set; }

        public static Texture2D FlagTexture { get; private set; }

        private static Texture2D[] adjacentBombCountTextures;
        private static Texture2D[] bombTextures;
        private static Texture2D[] tileShades;

        private Rectangle destRec;
        private TileState _state = TileState.GRASS;

        private int shadeTextureIndex = -1;
        private int bombTextureIndex = -1;

        public int AdjacentBombs { get; set; }

        public TileState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;

                switch (_state)
                {
                    case TileState.GRASS:
                    shadeTextureIndex = -1;
                    bombTextureIndex = -1;
                    break;

                    case TileState.DISPLAY_ADJACENT_BOMBS:
                    shadeTextureIndex = Utility.GetRandom(0, tileShades.Length);
                    break;

                    case TileState.EXPLODING_BOMB:
                    bombTextureIndex = Utility.GetRandom(0, BOMB_COUNT_TEXTURES);
                    break;
                }
            }
        }


        public Tile(Point position, int size)
        {
            destRec = new Rectangle(position, new Point(size, size));
        }

        public static void Init(ContentManager content)
        {
            BombSound = content.Load<SoundEffect>("Resources/Audio/Mine");

            adjacentBombCountTextures = new Texture2D[BOMB_COUNT_TEXTURES];
            bombTextures = new Texture2D[BOMB_COUNT_TEXTURES];

            FlagTexture = content.Load<Texture2D>("Resources/Flag");

            FlagPlaceSound = content.Load<SoundEffect>("Resources/Audio/PlaceFlag");
            FlagClearSound = content.Load<SoundEffect>("Resources/Audio/ClearFlag");


            tileShades = new Texture2D[]
            {
                content.Load<Texture2D>("Resources/Clear_Light"),
                content.Load<Texture2D>("Resources/Clear_Dark")
            };

            for (int i = 0; i < BOMB_COUNT_TEXTURES; i++)
            {
                adjacentBombCountTextures[i] = content.Load<Texture2D>($"Resources/{i + 1}");
            }

            for (int i = 0; i < BOMB_COUNT_TEXTURES; i++)
            {
                bombTextures[i] = content.Load<Texture2D>($"Resources/Mine{i + 1}");
            }
        }

        public void Reset()
        {
            State = TileState.GRASS;
            AdjacentBombs = 0;
        }

        public void Draw(SpriteBatch sb)
        {
            if (shadeTextureIndex != -1)
            {
                sb.Draw(tileShades[shadeTextureIndex], destRec, Color.White);
            }

            switch (_state)
            {
                case TileState.BOMB_FLAGGED:
                case TileState.GRASS_FLAGGED:
                sb.Draw(FlagTexture, destRec, Color.White);
                break;

                case TileState.EXPLODING_BOMB:
                sb.Draw(bombTextures[bombTextureIndex], destRec, Color.White);
                break;

                case TileState.DISPLAY_ADJACENT_BOMBS:
                if (AdjacentBombs > 0)
                {
                    sb.Draw(adjacentBombCountTextures[AdjacentBombs - 1], destRec, Color.White);
                }
                break;
            }
        }
    }
}

