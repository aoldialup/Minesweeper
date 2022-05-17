using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    internal class Sprite
    {
        public Rectangle DestRec;

        public Texture2D Texture { get; set; }

        public Color Color { get; set; }

        public Sprite(ContentManager content, string path, bool initRect)
        {
            Texture = content.Load<Texture2D>(path);

            if (initRect)
            {
                DestRec = new Rectangle(0, 0, Texture.Width, Texture.Height);
            }

            Color = Color.White;
        }

        public Sprite()
        {
            Color = Color.White;
        }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Color = Color.White;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, DestRec, Color);
        }
    }
}
