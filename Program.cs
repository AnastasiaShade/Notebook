using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

// Сложность алгоритма добавления в словарь O(k), где k - длина слова
// Сложность алгоритма поиска слов: O(k), где k - длина слова
// Время: 2 час 20 минут
// Обдумывание: 40 минут
// Реализация: 30 минут
// Тестирование: 35 минут
// Рефакторинг: 35 минут

// Имена хранятся в префиксном дереве, где каждая буква - узел дерева. В узлах хранится количество слов, которые
// заканчиваются на данную букву. При добавлении инкрементируется счетчик для встретившихся символов, а так же создается
// новый узел, если символ впервые встречается в последовательности слов. При поиске происходит проход по дереву, пока
// не будет достигнут конец слова либо лист дерева.

namespace Notebook {
    internal class Node {
        internal int Count => _count;
        internal Dictionary<char, Node> Symbols;

        private int _count;

        /// <summary> Конструктор узла дерева </summary>
        /// <param name="root"> Является ли узел корневым </param>
        public Node(bool root = false) {
            _count = 0;
            Symbols = new Dictionary<char, Node>();
        }

        /// <summary> Добявляет слово в дерево </summary>
        /// <param name="word"> Добавляемое слово </param>
        /// <returns> Удалось ли добавить слово </returns>
        internal bool Add(string word) {
            ++_count;

            if (string.IsNullOrEmpty(word)) {
                if (Symbols.Count == 0 && _count > 1 || _count - GetCountSum() > 1) {
                    --_count;
                    return false;
                }

                return true;
            }

            if (!Symbols.ContainsKey(word[0])) {
                Symbols.Add(word[0], new Node());
            }

            var result = Symbols[word[0]].Add(word.Remove(0, 1));
            if (!result) {
                --_count;
            }

            return result;
        }

        /// <summary> Ищет все слова, начинающиеся на указанный шаблон </summary>
        /// <param name="pattern"> Шаблон </param>
        /// <returns> Количество найденных слов </returns>
        internal int Find(string pattern) {
            if (string.IsNullOrEmpty(pattern)) {
                return _count;
            }

            if (!Symbols.ContainsKey(pattern[0])) {
                return 0;
            }

            return Symbols[pattern[0]].Find(pattern.Remove(0, 1));
        }

        /// <summary> Подсчитывает количество вхождений последующих символов в слова</summary>
        /// <returns> Сумма </returns>
        private int GetCountSum() {
            var summ = 0;

            foreach (var symbol in Symbols) {
                summ += symbol.Value.Count;
            }

            return summ;
        }
    }

    internal class Program {
        private static readonly Dictionary<string, Action<string>> _actions = new Dictionary<string, Action<string>>() {
            {"add", AddName},
            {"find", FindNames}
        };

        private static readonly Node _notebook = new Node(true);

        private const string SplitPattern = @"\W+";

        /// <summary> Разбивает строку на слова </summary>
        /// <param name="source"> Исходная строка </param>
        /// <returns> Возвращает список слов. Пустые элементы игнорируются </returns>
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

        /// <summary> Добявляет имя в записную книгу </summary>
        /// <param name="name"> Добявляемое имя </param>
        /// <exception cref="ArgumentException"> Выбрасывается, когда имя уже существует в записной книге </exception>
        private static void AddName(string name) {
            if (!_notebook.Add(name)) {
                throw new ArgumentException($"Name {name} is already exists in the notebook");
            }
        }

        /// <summary> Ищет все имена, начинающиеся на указанный шаблон, и выводит их количество </summary>
        /// <param name="pattern"> Шаблон </param>
        private static void FindNames(string pattern) {
            Console.WriteLine(_notebook.Find(pattern));
        }

        /// <summary> Обрабатывает ввод пользователя и вызывает заданное действие </summary>
        /// <exception cref="InvalidDataException"> Выбрасывается, когда пользователь ничего не вводит </exception>
        /// <exception cref="ArgumentOutOfRangeException"> Выбрасывается при неправильном количестве введенных аргументов </exception>
        /// <exception cref="InvalidOperationException"> Выбрасывается при вводе несуществующего действия </exception>
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