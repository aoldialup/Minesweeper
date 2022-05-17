using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utils;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Minesweeper
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private int windowWidth;
        private int windowHeight;

        private int tileRow;
        private int tileCol;

        private bool isSoundOn = true;
        private bool isPlayingState = true;
        private bool hasPlayerWon = false;

        private Point mousePos = Point.Zero;

        private HUD hud;
        private Board board;

        private Song winSong;
        private Song loseSong;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            winSong = Content.Load<Song>("Resources/Audio/Win");
            loseSong = Content.Load<Song>("Resources/Audio/Lose");

            board = new Board(Content);

            windowWidth = board.Width;
            windowHeight = board.Height + HUD.BAR_HEIGHT;

            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.ApplyChanges();

            hud = new HUD(Content, board, isSoundOn, windowWidth, windowHeight);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            mousePos = Input.GetMousePosition();

            if (isPlayingState)
            {
                UpdatePlayingState(gameTime);
            }
            else
            {
                UpdateGameOverState();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            if (isPlayingState)
            {
                DrawPlayingState();
            }
            else
            {
                DrawGameOverState();
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void Reset()
        {
            board.Reset();

            hud.SetFlagsLeftText(board.FlagsLeft);
            hud.ResetWatch();

            isPlayingState = true;
            hasPlayerWon = false;
        }

        private void UpdateGameOverState()
        {
            if (Input.IsMouseButtonPressed(MouseButton.LEFT))
            {
                switch (hud.GetButtonPressed(mousePos, isPlayingState))
                {
                    case HUDButton.PLAY_AGAIN:
                    MediaPlayer.Stop();
                    Reset();
                    break;

                    case HUDButton.EXIT:
                    Exit();
                    break;

                    case HUDButton.TOGGLE_SOUND:
                    ToggleSound();
                    break;
                }
            }
        }

        private void DrawPlayingState()
        {
            hud.Draw(spriteBatch);
            hud.DrawButtons(spriteBatch);

            board.Draw(spriteBatch);

            hud.DrawInstructionsAnimation(spriteBatch);
        }

        private void DrawGameOverState()
        {
            hud.Draw(spriteBatch);
            board.Draw(spriteBatch);

            hud.DrawShadowSprite(spriteBatch);

            hud.DrawButtons(spriteBatch);
            hud.DrawGameOverScreen(spriteBatch, hasPlayerWon);
        }

        private void DetermineTilePressed()
        {
            tileCol = mousePos.X / board.TileSize;
            tileRow = (mousePos.Y - HUD.BAR_HEIGHT) / board.TileSize;
        }

        private void UpdatePlayingState(GameTime gameTime)
        {
            hud.Update(gameTime);
            hud.UpdateInstructionsAnimation(gameTime);

            if (Input.IsMouseButtonPressed(MouseButton.LEFT))
            {
                HandleLeftClick();
            }
            else if (Input.IsMouseButtonPressed(MouseButton.RIGHT))
            {
                HandleRightClick();
            }
        }

        private void HandleLeftClick()
        {
            hud.StopInstructionsAnimation();

            switch (hud.GetButtonPressed(mousePos, isPlayingState))
            {
                case HUDButton.EXIT:
                Exit();
                break;

                case HUDButton.TOGGLE_SOUND:
                ToggleSound();
                break;

                case HUDButton.RESET:
                Reset();
                break;

                case HUDButton.NONE:
                MineTile();
                break;
            }
        }

        private void MineTile()
        {
            if (board.Contains(mousePos))
            {
                DetermineTilePressed();

                if (board.CanTileBeMined(tileRow, tileCol))
                {
                    switch (board.GetTileState(tileRow, tileCol))
                    {
                        case TileState.HIDDEN_BOMB:
                        MineHiddenBomb();
                        break;

                        case TileState.GRASS:
                        MineGrass();
                        break;
                    }
                }
            }
        }

        private void ToggleSound()
        {
            hud.SetSoundEnabled(!isSoundOn);
            isSoundOn = !isSoundOn;

            if (isSoundOn)
            {
                MediaPlayer.Resume();
            }
            else
            {
                MediaPlayer.Pause();
            }
        }

        private void HandleRightClick()
        {
            hud.StopInstructionsAnimation();

            if (board.IsPositionValid(tileRow, tileCol))
            {
                DetermineTilePressed();

                switch (board.GetTileState(tileRow, tileCol))
                {
                    case TileState.GRASS:
                    case TileState.HIDDEN_BOMB:
                    PlaceFlag();
                    break;

                    case TileState.BOMB_FLAGGED:
                    case TileState.GRASS_FLAGGED:
                    ClearFlag();
                    break;
                }
            }
        }

        private void CheckWin()
        {
            if (board.IsZeroTile())
            {
                isPlayingState = false;
                hasPlayerWon = true;

                hud.SetWatchActive(false);
                hud.SaveBestTime();

                PlaySound(winSong);
            }
        }

        private void MineHiddenBomb()
        {
            isPlayingState = false;

            board.ExplodeBombs();

            PlaySound(Tile.BombSound);
            PlaySound(loseSong);
        }

        private void MineGrass()
        {
            board.RevealTile(tileRow, tileCol);
            hud.SetWatchActive(true);

            CheckWin();
        }

        private void PlaySound(SoundEffect soundEffect)
        {
            if (isSoundOn)
            {
                soundEffect.Play();
            }
        }

        private void PlaySound(Song song)
        {
            if (isSoundOn)
            {
                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = true;
            }
        }

        private void PlaceFlag()
        {
            if (board.PlaceFlag(tileRow, tileCol))
            {
                hud.SetFlagsLeftText(board.FlagsLeft);
                PlaySound(Tile.FlagPlaceSound);

                CheckWin();
            }
        }

        private void ClearFlag()
        {
            if (board.ClearFlag(tileRow, tileCol))
            {
                hud.SetFlagsLeftText(board.FlagsLeft);
                PlaySound(Tile.FlagClearSound);
            }
        }
    }
}