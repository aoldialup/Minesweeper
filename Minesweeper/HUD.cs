using Animation2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Utils;

namespace Minesweeper
{
    enum HUDButton
    {
        NONE,
        RESET,
        TOGGLE_SOUND,
        EXIT,
        PLAY_AGAIN
    }

    internal class HUD
    {
        public const int BAR_HEIGHT = 60;

        private Texture2D soundOffTexture;
        private Texture2D soundOnTexture;
        private Sprite soundSprite;

        private Sprite bar;
        private Sprite resetGameButton;
        private Sprite exitSprite;
        private Rectangle uiFlagRec;
        private Text flagsLeftText;

        private SpriteFont font;

        private Texture2D gameOverResultsTexture;
        private Texture2D gameOverWinResultsTexture;
        private Rectangle gameOverMenuRect;

        private Texture2D tryAgainButton;
        private Texture2D playAgainButton;
        private Rectangle replayRect;

        private Vector2 gameOverTimeTakenPosition;
        private Vector2 gameOverBestTimePosition;

        private Watch watch;

        private Sprite gameOverShadowSprite;
        private Animation instructionsAnimation;

        public HUD(ContentManager content, Board board, bool isSoundOn, int windowWidth, int windowHeight)
        {
            int midWindowWidth = windowWidth / 2;
            int midWindowHeight = windowHeight / 2;
            int barHeightMiddle = BAR_HEIGHT / 2;

            bar = new Sprite(content, "Resources/HUDBar", false);
            bar.DestRec = new Rectangle(0, 0, board.Width, BAR_HEIGHT);

            LoadTextures(content);

            soundSprite = new Sprite();
            soundSprite.DestRec = new Rectangle((int)(bar.DestRec.Width * 0.80), (int)(BAR_HEIGHT * 0.35),
                (int)(soundOnTexture.Width * 0.70), (int)(soundOnTexture.Height * 0.72));
            SetSoundEnabled(isSoundOn);

            watch = new Watch("Stats.txt", font, content, bar.DestRec);

            resetGameButton = new Sprite(content, "Resources/SmileButton", false);
            resetGameButton.DestRec = new Rectangle(0, barHeightMiddle - 20, (int)(resetGameButton.Texture.Width * 0.2),
                (int)(resetGameButton.Texture.Height * 0.2));

            exitSprite = new Sprite(content, "Resources/Exit", false);
            exitSprite.DestRec = new Rectangle((int)(bar.DestRec.Width * 0.90), (int)(BAR_HEIGHT * 0.40),
                (int)(exitSprite.Texture.Width * 0.20), (int)(exitSprite.Texture.Height * 0.20));

            flagsLeftText = new Text(new Vector2(uiFlagRec.X + 150, barHeightMiddle), font);
            SetFlagsLeftText(board.FlagsLeft);

            uiFlagRec = new Rectangle((int)(bar.DestRec.Width * 0.25), (int)(bar.DestRec.Height * 0.25), 35, 35);

            gameOverMenuRect = new Rectangle((midWindowWidth - 100) - tryAgainButton.Height, midWindowHeight - 100,
                gameOverResultsTexture.Width, gameOverResultsTexture.Height);
            replayRect = new Rectangle(gameOverMenuRect.X, midWindowHeight + 140, tryAgainButton.Width, tryAgainButton.Height);

            gameOverTimeTakenPosition = new Vector2(gameOverMenuRect.Left + 70, gameOverMenuRect.Top + 85);
            gameOverBestTimePosition = new Vector2(gameOverMenuRect.Right - 100, gameOverMenuRect.Top + 85);

            gameOverShadowSprite = new Sprite(content, "Resources/GameOverBoardShadow", false);
            gameOverShadowSprite.Color *= 0.5f;
            gameOverShadowSprite.DestRec = new Rectangle(0, 0, windowWidth, windowHeight);

            instructionsAnimation = new Animation(
                content.Load<Texture2D>("Resources/Instructions"), 2, 1, 2, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 240,
                Vector2.Zero,
                0.5f, true);
            instructionsAnimation.destRec.X = midWindowWidth;
            instructionsAnimation.destRec.Y = midWindowHeight;
        }

        private void LoadTextures(ContentManager content)
        {
            soundOffTexture = content.Load<Texture2D>("Resources/SoundOff");
            soundOnTexture = content.Load<Texture2D>("Resources/SoundOn");

            tryAgainButton = content.Load<Texture2D>("Resources/GameOver_TryAgain");
            playAgainButton = content.Load<Texture2D>("Resources/GameOver_PlayAgain");

            gameOverResultsTexture = content.Load<Texture2D>("Resources/GameOver_Results");
            gameOverWinResultsTexture = content.Load<Texture2D>("Resources/GameOver_WinResults");

            font = content.Load<SpriteFont>("Font");
        }

        public void DrawShadowSprite(SpriteBatch sb)
        {
            gameOverShadowSprite.Draw(sb);
        }

        public void Draw(SpriteBatch sb)
        {
            bar.Draw(sb);

            resetGameButton.Draw(sb);

            sb.Draw(Tile.FlagTexture, uiFlagRec, Color.White);
            flagsLeftText.Draw(sb);

            watch.Draw(sb);
        }

        public void Update(GameTime gameTime)
        {
            watch.Update(gameTime);
        }

        public void DrawButtons(SpriteBatch sb)
        {
            soundSprite.Draw(sb);
            exitSprite.Draw(sb);
        }

        public void DrawInstructionsAnimation(SpriteBatch sb)
        {
            instructionsAnimation.Draw(sb, Color.White, Animation.FLIP_NONE);
        }

        public void UpdateInstructionsAnimation(GameTime gameTime)
        {
            instructionsAnimation.Update(gameTime);
        }

        public void StopInstructionsAnimation()
        {
            instructionsAnimation.isAnimating = false;
        }

        public void DrawGameOverScreen(SpriteBatch sb, bool hasWon)
        {
            if (hasWon)
            {
                sb.Draw(gameOverWinResultsTexture, gameOverMenuRect, Color.White);
                sb.Draw(playAgainButton, replayRect, Color.White);

                sb.DrawString(font, watch.GetCurrentTime(), gameOverTimeTakenPosition, Color.White);
            }
            else
            {
                sb.Draw(gameOverResultsTexture, gameOverMenuRect, Color.White);
                sb.Draw(tryAgainButton, replayRect, Color.White);

                sb.DrawString(font, Watch.NO_TIME, gameOverTimeTakenPosition, Color.White);
            }

            sb.DrawString(font, watch.GetBestTime(), gameOverBestTimePosition, Color.White);
        }

        public void SetFlagsLeftText(int flagsLeft)
        {
            flagsLeftText.DisplayedString = flagsLeft.ToString();
        }

        public HUDButton GetButtonPressed(Point mousePos, bool isGameStatePlaying)
        {
            if (resetGameButton.DestRec.Contains(mousePos))
            {
                return HUDButton.RESET;
            }

            if (soundSprite.DestRec.Contains(mousePos))
            {
                return HUDButton.TOGGLE_SOUND;
            }

            if (exitSprite.DestRec.Contains(mousePos))
            {
                return HUDButton.EXIT;
            }

            if (replayRect.Contains(mousePos) && !isGameStatePlaying)
            {
                return HUDButton.PLAY_AGAIN;
            }

            return HUDButton.NONE;
        }

        public void SetSoundEnabled(bool isEnabled)
        {
            if (isEnabled)
            {
                soundSprite.Texture = soundOnTexture;
            }
            else
            {
                soundSprite.Texture = soundOffTexture;
            }
        }

        public void ResetWatch()
        {
            watch.Reset();
        }

        public void SaveBestTime()
        {
            watch.SaveBestTime();
        }

        public void SetWatchActive(bool isActive)
        {
            watch.SetActive(isActive);
        }
    }
}
