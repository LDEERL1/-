using System.Collections.Generic;

namespace HangmanGame
{
    public class Categories
    {
        public string Category { get; set; }
        public List<string> Words { get; set; }

        public static List<Categories> GetDefaultCategories()
        {
            return new List<Categories>
            {
                new Categories
                {
                    Category = "Программирование",
                    Words = new List<string> { "программист", "разработчик", "алгоритм", "программа", "компьютер", "интерация" }
                },
                new Categories
                {
                    Category = "Животные",
                    Words = new List<string> { "броненосец", "муравьед", "калибри", "землеройка", "капибара", "трясогузка" }
                },
                new Categories
                {
                    Category = "Еда",
                    Words = new List<string> { "пахлава", "халва", "шакшука", "онигири", "сендвич", "шашлык" }
                },
                new Categories
                {
                    Category = "Литература",
                    Words = new List<string> { "ревизор", "хамелеон", "дубровский", "недоросль", "бесприданница", "одиссея" }
                }
            };
        }
    }
}
