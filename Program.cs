using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

// Сложность алгоритма добавления в словарь O(1) в лучшем случае, O(N * log(N)) - в худшем случае
// Сложность алгоритма поиска слов: O(sqrt (N))
// Время: 1 час 50 минут
// Обдумывание: 40 минут
// Реализация: 30 минут
// Тестирование: 25 минут
// Рефакторинг: 15 минут

// Имена хранятся в словаре, в которм ключом выступает первый символ имени, а значением - список имен.
// Поиск прыжками: при поиске осуществляется прыжок по индексам, равный sqrt(array_length). За начальную позицию диапазона
// принимается индекс, при котором слово будет меньше или равно заданному шаблону, а за конечную - слово, которое больше
// шаблона и не содержит его. Далее начальная и конечная позиции корректируются, пока все слова в диапазоне не будут
// содержать шаблон.

namespace Notebook {
    internal class Program {
        private static readonly Dictionary<char, List<string>>     _notebook = new Dictionary<char, List<string>>();
        private static readonly Dictionary<string, Action<string>> _actions  = new Dictionary<string, Action<string>>() {
            { "add", AddName },
            { "find", FindNames }
        };

        private const string SplitPattern = @"\W+";

        /// <summary> Разбивает строку на слова </summary>
        /// <param name="source">Исходная строка</param>
        /// <returns>Возвращает список слов. Пустые элементы игнорируются.</returns>
        private static List<string> ParseStringToWords(string source) {
            var parsedData = Regex.Split(source, SplitPattern).ToList();

            for (var i = 0; i < parsedData.Count;) {
                if (string.IsNullOrEmpty(parsedData[i])) {
                    parsedData.RemoveAt(i);
                    continue;
                }

                ++i;
            }

            return parsedData;
        }
        
        /// <summary>Добявляет имя в записную книгу</summary>
        /// <param name="name">Добявляемое имя</param>
        /// <exception cref="ArgumentException">Выбрасывается, когда имя уже существует в записной книге</exception>
        private static void AddName(string name) {
            var letter = name[0];
            
            if (!_notebook.ContainsKey(letter)) {
                _notebook.Add(letter, new List<string>() {name});
                return;
            }

            if (!_notebook[letter].Contains(name)) {
                _notebook[letter].Add(name);
                _notebook[letter].Sort();
            }
            else {
                throw new ArgumentException($"Name {name} is already exists in the notebook");
            }
        }
        
        /// <summary>Ищет все имена, начинающиеся на указанный шаблон, и выводит их количество</summary>
        /// <param name="pattern">Шаблон</param>
        private static void FindNames(string pattern) {
            var names = _notebook[pattern[0]];
            int first = 0;
            int last = names.Count - 1;

            if (pattern.Length == 1) {
                Console.WriteLine(names.Count);
            }
            
            int arrayLength = names.Count;
            int jumpStep = (int) Math.Sqrt(arrayLength);
            int currentPos = jumpStep;

            while (Math.Min(currentPos, arrayLength) < arrayLength) {
                var pos = Math.Min(currentPos, arrayLength) - 1;

                if (names[pos].CompareTo(pattern) <= 0) {
                    first = pos;
                }
                else if (!names[pos].StartsWith(pattern)) {
                    last = pos;
                    break;
                }

                currentPos += jumpStep;
            }

            while (!names[first].StartsWith(pattern)) {
                ++first;
            }

            while (!names[last].StartsWith(pattern)) {
                --last;
            }

            Console.WriteLine(last - first);
        }

        /// <summary>Обрабатывает ввод пользователя и вызывает заданное действие</summary>
        /// <exception cref="InvalidDataException">Выбрасывается, когда пользователь ничего не вводит</exception>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается при неправильном количестве введенных аргументов</exception>
        /// <exception cref="InvalidOperationException">Выбрасывается при вводе несуществующего действия</exception>
        private static void MakeAction() {
            Console.Write("Enter action: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) {
                throw new InvalidDataException("Input cant be empty!");
            }

            var words = ParseStringToWords(input);

            if (words.Count != 2) {
                throw new ArgumentOutOfRangeException("There are should be only two arguments!");
            }

            if (!_actions.ContainsKey(words[0].ToLower())) {
                throw new InvalidOperationException("There are no such action in the action list!");
            }

            _actions[words[0].ToLower()].Invoke(words[1].ToLower());
        }

        private static void Main() {
            AddName("james");
            AddName("harry");
            AddName("anna");
            AddName("alisa");
            AddName("jordge");
            AddName("jacob");
            AddName("andrew");
            
            while (true) {
                try {
                    MakeAction();
                }
                catch (Exception err) {
                    Console.WriteLine(err);
                }
                Console.WriteLine();
            }
        }
    }
}