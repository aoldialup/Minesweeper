using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Utils;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Minesweeper
{
    internal class Watch
    {
        private const int MAX_TIME = 999;
        private const int TIME_DIGITS = 3;
        public const string NO_TIME = "_ _ _";

        private StreamReader inFile;
        private StreamWriter outFile;
        private string path;

        private Text timeText;
        private Sprite sprite;

        private bool isActive;
        private float time = 0f;
        private string bestTime;

        public Watch(string path, SpriteFont timeFont, ContentManager content, Rectangle hudBarRec)
        {
            this.path = path;

            sprite = new Sprite(content, "Resources/Watch", false);
            sprite.DestRec = new Rectangle((int)(hudBarRec.Width * 0.5), (int)(HUD.BAR_HEIGHT * 0.30),
                (int)(sprite.Texture.Width * 0.30), (int)(sprite.Texture.Height * 0.30));

            timeText = new Text(new Vector2(sprite.DestRec.X + 40, HUD.BAR_HEIGHT / 2), timeFont);

            UpdateText();
            LoadBestTime();
        }

        public void SaveBestTime()
        {
            string curTime = GetCurrentTime();

            if (bestTime == NO_TIME)
            {
                bestTime = curTime;
                Save();
            }
            else
            {
                int currentTimeInt = int.Parse(curTime);
                int bestTimeInt = int.Parse(bestTime);

                if (currentTimeInt > bestTimeInt)
                {
                    bestTime = curTime;
                    Save();
                }
            }
        }

        public string GetBestTime()
        {
            return bestTime;
        }

        public string GetCurrentTime()
        {
            return timeText.DisplayedString;
        }

        private void LoadBestTime()
        {
            try
            {
                inFile = File.OpenText(path);

                bool isValid = int.TryParse(inFile.ReadLine(), out int bestTimeInt);

                if (isValid && bestTimeInt >= 0 && bestTimeInt <= MAX_TIME)
                {
                    bestTime = bestTimeInt.ToString().PadLeft(TIME_DIGITS, '0');
                }
                else
                {
                    bestTime = NO_TIME;

                    outFile = File.CreateText(path);
                    outFile?.Close();
                }
            }
            catch (FileNotFoundException)
            {
                outFile = File.CreateText(path);
                outFile?.Close();
            }
            catch (FormatException)
            {
                outFile = File.CreateText(path);
                outFile?.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                inFile?.Close();
            }
        }

        private void Save()
        {
            try
            {
                outFile = File.CreateText(path);
                outFile.WriteLine(int.Parse(bestTime));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                outFile?.Close();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                UpdateText();
            }
        }

        private void UpdateText()
        {
            timeText.DisplayedString = ((int)time).ToString().PadLeft(3, '0');
        }

        public void Reset()
        {
            time = 0f;
            UpdateText();
        }

        public void SetActive(bool isActive)
        {
            this.isActive = isActive;
        }

        public void Draw(SpriteBatch sb)
        {
            sprite.Draw(sb);
            timeText.Draw(sb);
        }
    }
}
