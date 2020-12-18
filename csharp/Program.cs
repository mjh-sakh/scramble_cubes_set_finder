using System;
using System.Collections.Generic;
using System.Linq;


namespace CubeSetSearch
{

    class ScrambleCubes
    {
        private string all_letters = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя";
        private string excluded_letters = "хжъё";
        private List<char> letters = new List<char> { };
        private List<string> _words;
        private int maxLetterRepetition = 4;
        private int numberOfCubesInSet = 10;
        private int _cubeSize = 6;
        private static readonly Random rnd = new Random();

        public class Cube
        {
            private readonly char[] _sides;
            private readonly int _size;

            public Cube(List<char> pool, int size = 6)
            {
                _sides = new char[size];
                _size = size;
                generate(pool);
            }

            private void generate(List<char> pool)
            {
                for (int ix = 0; ix < _size;)
                {
                    var i = rnd.Next(pool.Count - 1);
                    var c = pool[i];
                    if (!_sides.Contains(c))
                    {
                        _sides[ix] = c;
                        pool.Remove(c);
                        ix++;
                    }
                }
            }

            public Cube(string chars)
            {
                _size = chars.Length;
                _sides = new char[_size];
                for (int i = 0; i < _size; i++)
                {
                    _sides[i] = chars[i];
                }
            }

            public bool checkChar(char c) => _sides.Contains(c);

            public void print()
            {
                Console.Write("[ ");
                foreach (var side in _sides)
                {
                    Console.Write($"{side} ");
                }
                Console.WriteLine("]");
            }
        }

        public class CubeSet
        {
            private readonly int _size;
            private readonly Cube[] _cubes;
            private List<char> _pool;
            private int _cubeSize;

            public CubeSet(int size, List<char> letters, int cubeSize = 6)
            {
                _size = size;
                _cubes = new Cube[size];
                _pool = new List<char>(letters);
                _cubeSize = cubeSize;
                generate(_pool);
            }

            private void generate(List<char> pool)
            {
                for (int ix = 0; ix < _size; ix++)
                {
                    var cube = new Cube(pool, _cubeSize);
                    _cubes[ix] = cube;
                }
            }

            public CubeSet(int size, string manualEntryString)
            {
                _size = size;
                _cubes = new Cube[size];
                var sides = manualEntryString.Length / size;

                for (int i = 0; i < size; i++)
                {
                    var cubeString = manualEntryString.Substring(i * sides, sides);
                    _cubes[i] = new Cube(cubeString);
                }
            }

            private List<Cube> findCubesWithChar(char c)
            {
                var cubesWithChar = new List<Cube> { };
                foreach (var cube in _cubes)
                {
                    if (cube.checkChar(c)) cubesWithChar.Add(cube);
                }
                return cubesWithChar;
            }

            public bool checkCanWriteWord(string word)
            {
                var cubesWithChars = new Dictionary<char, List<Cube>> { };
                foreach (var c in word.Distinct())
                {
                    var cubesWithChar = findCubesWithChar(c);
                    if (cubesWithChar.Count == 0) return false;
                    cubesWithChars.Add(c, cubesWithChar);
                }
                var selectedCubes = cubesWithChars.Values.Aggregate(new List<Cube> { }, (curr, next) => curr.Union(next).ToList());
                if (selectedCubes.Count < word.Length) return false;

                var sorted_word = word.OrderBy(c => cubesWithChars[c].Count);
                foreach (var c in sorted_word)
                {
                    bool noCubeFoundFlag = true;
                    foreach (var cube in cubesWithChars[c])
                    {
                        if (selectedCubes.Contains(cube))
                        {
                            selectedCubes.Remove(cube);
                            noCubeFoundFlag = false;
                            break;
                        }
                    }
                    if (noCubeFoundFlag) return false;
                }
                return true;
            }

            public void print()
            {
                foreach (var cube in _cubes)
                {
                    cube.print();
                }
            }
        }

        public ScrambleCubes(List<string> input_words, int cubeSize = 6)
        {

            foreach (var c in all_letters)
            {
                if (!excluded_letters.Contains(c)) letters.Add(c);
            }
            for (int i = 0; i < maxLetterRepetition; i++)
            {
                letters.AddRange(letters);
            }
            _words = input_words;
            _cubeSize = cubeSize;
        }

        public IEnumerable<CubeSet> GenerateCubeSet()
        {
            while (true)
            {
                yield return new CubeSet(numberOfCubesInSet, letters, _cubeSize);
            }
        }

        public void RunScoring()
        {
            int count = 0;
            int bestScore = 0;
            var startTime = DateTime.Now;
            foreach (var cubeSet in GenerateCubeSet())
            {
                int score = 0;
                foreach (var word in _words)
                {
                    if (cubeSet.checkCanWriteWord(word)) score++;
                }
                if (score >= bestScore)
                {
                    Console.WriteLine($"Current best score is {score}.");
                    cubeSet.print();
                    bestScore = score;
                }
                count++;
                if (count % 10_000 == 0) Console.WriteLine($"{ count / 10_000}0k evaluated in {DateTime.Now.Subtract(startTime)}.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var words = new List<string> { "мандарин", "снеговик", "вьюга", "оливье", "елка", "звезда", "салют", "фейерверк", "куранты", "шарики",
             "подарки", "мороз", "снегопад", "метель", "санки", "аргомак", "ледянка", "коньки", "письмо", "конфеты",
             "бык", "мороз", "зима", "сосулька", "лед", "сноуборд", "декабрь", "январь", "февраль", "шуба", "батарея",
             "сугроб", "снегурочка", "горка", "каникулы", "шапка", "перчатки", "лопата", "гололед", "валенки",
             "снегирь", "радость", "скользко", "темно", "спячка", "берлога", "буря", "крещение", "узор"};

            var SC = new ScrambleCubes(words);
            SC.RunScoring();
        }
    }
}
