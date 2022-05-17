using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Utils
{
    class Utility
    {
        // Random generator for the entire program
        private static readonly Random random = new Random();

        // Pre: min is less than max, max is greater than min
        // Post: Return the result as an integer
        // Description: Generate a random number  
        public static int GetRandom(int min, int max)
        {
            // Return a random number 
            return random.Next(min, max);
        }

        // Pre: None
        // Post: Return an integer
        // Description: return the enum's length
        public static int GetEnumLength<T>()
        {
            // return the length of the enum
            return Enum.GetNames(typeof(T)).Length;
        }

        // Pre: array is not null and contains at least one element
        // Post: Return a random item from the array
        // Description: Return a random item in an array of any type
        public static T Choice<T>(T[] array)
        {
            // Return the item
            return array[random.Next(0, array.Length)];
        }

        // Pre: list is not null and contains at least one element
        // Post: Return a random item from the list
        // Description: Return a random item in a list of any type
        public static T Choice<T>(List<T> list)
        {
            // Return the item
            return list[random.Next(0, list.Count)];
        }
    }
}

