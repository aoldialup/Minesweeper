using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Utils
{
    public enum MouseButton
    {
        LEFT,
        RIGHT,
        MIDDLE,
        XBUTTON_1,
        XBUTTON_2,
    }

    public static class Input
    {
        private static MouseState prevMouse;
        private static MouseState curMouse;

        private static KeyboardState prevKb;
        private static KeyboardState curKb;

        public static bool IsKeyDown(Keys key)
        {
            return curKb.IsKeyDown(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return curKb.IsKeyDown(key) && !prevKb.IsKeyDown(key);
        }

        public static bool IsMouseButtonPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LEFT:
                return curMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed;

                case MouseButton.RIGHT:
                return curMouse.RightButton == ButtonState.Pressed && prevMouse.RightButton != ButtonState.Pressed;

                case MouseButton.MIDDLE:
                return curMouse.MiddleButton == ButtonState.Pressed && prevMouse.MiddleButton != ButtonState.Pressed;

                case MouseButton.XBUTTON_1:
                return curMouse.XButton1 == ButtonState.Pressed && prevMouse.XButton1 != ButtonState.Pressed;

                case MouseButton.XBUTTON_2:
                return curMouse.XButton2 == ButtonState.Pressed && prevMouse.XButton2 != ButtonState.Pressed;

                default:
                throw new ArgumentException("Invalid mouse button entered");
            }
        }

        public static bool IsMouseButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LEFT:
                return curMouse.LeftButton == ButtonState.Pressed;

                case MouseButton.RIGHT:
                return curMouse.RightButton == ButtonState.Pressed;

                case MouseButton.MIDDLE:
                return curMouse.MiddleButton == ButtonState.Pressed;

                case MouseButton.XBUTTON_1:
                return curMouse.XButton1 == ButtonState.Pressed;

                case MouseButton.XBUTTON_2:
                return curMouse.XButton2 == ButtonState.Pressed;

                default:
                throw new ArgumentException("Invalid mouse button entered");
            }
        }

        public static Point GetMousePosition()
        {
            return curMouse.Position;
        }

        public static void Update()
        {
            prevMouse = curMouse;
            prevKb = curKb;

            curKb = Keyboard.GetState();
            curMouse = Mouse.GetState();
        }

        public static int GetHorizontalScrollWheelValue()
        {
            return curMouse.HorizontalScrollWheelValue;
        }

        public static int GetScrollWheelValue()
        {
            return curMouse.ScrollWheelValue;
        }

        public static Keys[] GetPressedKeys()
        {
            return curKb.GetPressedKeys();
        }
    }
}