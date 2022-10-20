using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileManager
{
    sealed class FileManager
    {
        // Статичные поля оформленные по кодстайлу, в первом поле объект содержащей информацию о текущем каталоге.
        private static DirectoryInfo s_currentDirectory;
        private static bool s_isExepetionCommentsNeeded;

        /// <summary>
        /// Основной метод, точка входа.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Хотите ли вы, чтобы отображалась подробная информация об ошибках (не рекомендуется для казуального пользования)" +
                " Нажмите y, если да, иначе любую кнопку.");
            s_isExepetionCommentsNeeded = Console.ReadKey(true).Key == ConsoleKey.Y ? true : false;
            while (true)
            {
                try
                {
                    ViewAndChooseDrive();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ProccesExeption(ex));
                    Console.WriteLine("Устраните проблему и попробуйте выбрать диск еще раз.");
                    Console.WriteLine("Нажмите любую кнопку чтобы продолжить");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            Menu();
            Console.WriteLine("Программа закрыта.");
        }



        /// <summary>
        /// Выводит список дисков и перемещает пользователя выбранный им корневой каталог (диск с которого начинается путь).
        /// </summary>
        /// <returns>Меняет текущий каталог на выбранный корневой каталог</returns>
        private static void ViewAndChooseDrive()
        {
            int res;
            Console.WriteLine("Вот список ваших дисков");
            DriveInfo[] AllDrives = DriveInfo.GetDrives();
            for (int i = 0; i < AllDrives.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {AllDrives[i].Name}");
            }
            Console.WriteLine("Выберите нужный вам диск указав его номер");
            while (!int.TryParse(Console.ReadLine(), out res) || res <= 0 || res > AllDrives.Length)
            {
                Console.WriteLine("Номер неправильно указан");
            }
            s_currentDirectory = new DirectoryInfo(AllDrives[res - 1].Name);
        }

        /// <summary>
        /// Основное меню, запускающая опции.
        /// </summary>
        private static void Menu()
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("Текущий путь");
                    Console.WriteLine(s_currentDirectory.FullName);
                    MenuText();
                    switch (Console.ReadLine())
                    {
                        case "1":
                            ViewAndChooseDrive();
                            break;
                        case "2":
                            ChangeDirectory();
                            break;
                        case "3":
                            GetFiles();
                            break;
                        case "4":
                            DoPrintFileText();
                            break;
                        case "5":
                            DoCopyFile();
                            break;
                        case "6":
                            DoMoveFile();
                            break;
                        case "7":
                            DoDeleteFile();
                            break;
                        case "8":
                            DoCreateFile();
                            break;
                        case "9":
                            DoConcatFiles();
                            break;
                        case "10":
                            return;
                        default:
                            Console.WriteLine("Вы ввели неподходящее число (опцию)");
                            break;
                    }

                }
                catch (Exception ex)
                {
                    ProccesExeption(ex);
                    BackToMenu();
                }
            }

        }

        /// <summary>
        /// Текст выводящийся в меню.
        /// </summary>
        private static void MenuText()
        {
            Console.WriteLine(" 1. Выбрать диск\n 2. Выбор папки\n 3. Вывести файлы в выбранной директории\n" +
                " 4. вывод содержимого текстового файла в консоль\n 5. копирование файла\n" +
                " 6. перемещение файла в выбранную пользователем директорию\n 7. Удаление файла\n" +
                " 8. Создание простого текстового файла\n 9. Конкатенация содержимого двух или более текстовых файлов" +
                " и вывод результата в консоль в кодировке UTF-8\n 10. Выйти из программы");
        }

        /// <summary>
        /// Метод, который ставится в конце методов вызываемых из меню.
        /// </summary>
        private static void BackToMenu()
        {
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Метод для сортировки и вывода исключений.
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <returns>Стоку с зависящею от настроек и типа исключения.</returns>
        private static string ProccesExeption(Exception ex)
        {
            string message = null;
            if (s_isExepetionCommentsNeeded)
            {
                message = ex.ToString();
            }
            return ex switch
            {
                // Там где прибавляется ex нужно выводить ошибку всегда.
                DirectoryNotFoundException => "Не удалось найти данную директорию" + Environment.NewLine + message,
                FileNotFoundException => "Не удалось найти данный файл" + Environment.NewLine + message,
                PathTooLongException => "Был указан слишком длинный путь" + Environment.NewLine + message,
                UnauthorizedAccessException => "Нет доступа к данному действию (UnauthorizedAccessException)" + Environment.NewLine + message,
                OutOfMemoryException => "Файл или файлы занимают слишком много памяти" + message,
                ArgumentNullException => "В качестве аргумента ничего не было передано" + Environment.NewLine + ex,
                ArgumentException => "Неверно указан аргумент" + Environment.NewLine + ex,
                IOException => $"Ошибка ввода-вывода данных:" + Environment.NewLine + ex,
                _ => $"Произошла следущая ошибка:" + Environment.NewLine + ex,
            };
        }

        /// <summary>
        /// Позволяет пользователю поменять его текущий каталог.
        /// </summary>
        private static void ChangeDirectory()
        {
            while (true)
            {
                try
                {
                    s_currentDirectory = new DirectoryInfo(SetDirectory());
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ProccesExeption(ex));
                    Console.WriteLine("Введите путь еще раз");
                    continue;
                }

            }
        }

        /// <summary>
        /// Удобно меняет текущий каталог польщователя на существующий.
        /// </summary>
        /// <returns></returns>
        private static string SetDirectory()
        {
            Console.WriteLine("Введите путь выбранной вами папки или имя подкаталога текущей директории или \"<<\", чтобы перейти в родительский каталог");
            string input = Console.ReadLine();
            while (!Directory.Exists(input) && !Directory.Exists(s_currentDirectory.FullName + "/" + input))
            {
                if (input == "<<")
                {
                    if (s_currentDirectory.Parent is null)
                    {
                        throw new Exception("Родительского каталога этой директории не существует, так как данный каталог (директория) корневой.");
                    }
                    return s_currentDirectory.Parent.FullName;
                }
                Console.WriteLine("Вы указали несуществующий путь, введите еще раз");
                input = Console.ReadLine();
            }
            return Directory.Exists(input) ? input : s_currentDirectory.FullName + "/" + input;
        }

        /// <summary>
        /// Выводит файлы текущего каталога вместе с их путями.
        /// </summary>
        private static void GetFiles()
        {
            foreach (var file in Directory.GetFiles(s_currentDirectory.FullName))
            {
                Console.WriteLine(file);

            }
            Console.WriteLine("Нажмите любую клавишу, чтобы выйти в меню");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Позвояляет пользователю выбрать файл для его прочтения в в выбранной кодировке.
        /// </summary>
        private static void DoPrintFileText()
        {
            Console.Clear();
            Console.WriteLine("Текущая директория:");
            Console.WriteLine(s_currentDirectory.FullName);
            while (true)
            {
                try
                {
                    PrintFileText();
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "menu")
                    {
                        BackToMenu();
                        return;
                    }
                    Console.WriteLine(ProccesExeption(ex));
                    Console.WriteLine("Попробуйте еще раз");
                    continue;
                }
            }
            BackToMenu();
        }

        /// <summary>
        /// Выводит текст файла текущего каталога в указанной кодировке.
        /// </summary>
        ///<exception cref="SetShortcutFilePath">Исключение для выхода в меню</exception>
        private static void PrintFileText()
        {
            Encoding encoding = ChooseEncoding();
            Console.WriteLine($"Выбрана кодировка {encoding}");
            string path = SetShortcutFilePath();
            Console.WriteLine($"Открыт файл {path}, его текст:");
            using (StreamReader streamReader = new StreamReader(File.Open(path, FileMode.Open), encoding, false))
            {
                Console.WriteLine(streamReader.ReadToEnd());

            }
        }

        /// <summary>
        /// Выбор кодировки.
        /// </summary>
        /// <returns></returns>
        private static Encoding ChooseEncoding()
        {
            Console.WriteLine("Выберите кодировку: \n 1. Unicode\n 2. UTF32\n 3. ASCII\n Введите что угодно, чтобы выбрать кодировку UTF-8");
            return Console.ReadLine() switch
            {
                "1" => Encoding.Unicode,
                "2" => Encoding.UTF32,
                "3" => Encoding.ASCII,
                _ => Encoding.UTF8,
            };
        }

        /// <summary>
        /// Метод для выбора существующего файла в текущей директории.
        /// </summary>
        /// <returns>Полный путь к выбраному файлу или исключение с текстом "menu", если пользователь захочет выйти в меню</returns>
        /// <exception cref="Exception">Исключение для выхода в меню с посылкой "menu".</exception>
        private static string SetShortcutFilePath()
        {
            Console.WriteLine("Введите имя файла в текщуей директории, который вы хотите выбрать");
            string input = Console.ReadLine();
            string fileName = s_currentDirectory.FullName + Path.DirectorySeparatorChar + input;
            while (!File.Exists(fileName))
            {
                Console.WriteLine("Повторно введите имя (вместе с расширением) доступного для текущей операции файла в текщуей директории, полный путь указывать не надо:" +
                                  " \n введите \"menu\" , если хотите вернуться к");
                input = Console.ReadLine();
                if (input == "menu")
                {
                    // для удобного выхода в меню, если в текущем каталоге нет файлов и пользователь их там не создаст.
                    throw new Exception("menu");
                }
                fileName = s_currentDirectory.FullName + Path.DirectorySeparatorChar + input;
            }
            return Path.GetFullPath(fileName);
        }

        /// <summary>
        /// Метод для копирования попыток копирования файла.
        /// </summary>
        private static void DoCopyFile()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Текущая директория:");
                Console.WriteLine(s_currentDirectory.FullName);
                Console.WriteLine("Копирование файла");
                try
                {
                    CopyFile();
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "menu")
                    {
                        BackToMenu();
                        return;
                    }
                    Console.WriteLine(ProccesExeption(ex));
                    Console.WriteLine("Попробуйте еще раз, нажмите что угодно, чтобы продолжить:");
                    Console.ReadKey(true);
                    continue;
                }
            }
            BackToMenu();
        }

        /// <summary>
        /// Метод для копирования файла.
        /// </summary>
        /// <exception cref="SetShortcutFilePath">Исключение для выхода в меню</exception>
        private static void CopyFile()
        {
            string sourceFileName = SetShortcutFilePath();
            Console.WriteLine($"Введите путь с новым названием файла (включая расширение), который будет копией выбранного файла {sourceFileName}");
            string destFileName = Console.ReadLine();
            Console.WriteLine($"Выбран путь копии {Path.GetFullPath(destFileName)}");
            if (File.Exists(destFileName))
            {
                Console.WriteLine("Вы хотите перезаписать файл? Введите yes, если да, иначе сделайте любой ввод:");
                if (Console.ReadLine() == "yes")
                {
                    File.Copy(sourceFileName, destFileName, true);
                    Console.WriteLine($"Файл {sourceFileName} скопирован и файл {destFileName} перезаписан");
                    return;
                }
            }
            File.Copy(sourceFileName, destFileName);
            Console.WriteLine($"Файл {sourceFileName} скопирован в файл {destFileName}");
        }

        /// <summary>
        /// Метод для перемещения файла или возвращения в меню.
        /// </summary>
        private static void DoMoveFile()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Текущая директория:");
                Console.WriteLine(s_currentDirectory.FullName);
                Console.WriteLine("Перемещение файла из текущей директории");
                try
                {
                    MoveFile();
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "menu")
                    {
                        BackToMenu();
                        return;
                    }
                    Console.WriteLine(ProccesExeption(ex));
                    Console.WriteLine("Попробуйте еще раз, нажмите что угодно, чтобы продолжить:");
                    Console.ReadKey(true);
                    continue;
                }
            }
            BackToMenu();
        }

        /// <summary>
        /// Метод для перемещения файла.
        /// </summary>
        /// <exception cref="SetShortcutFilePath">Исключение для выхода в меню</exception>
        private static void MoveFile()
        {
            string sourceFileName = SetShortcutFilePath();
            Console.WriteLine($"Введите директорию с новым именем и расширением файла, в которую вы хотите поместить выбранный файл {sourceFileName}");
            string destFileName = Console.ReadLine();
            Console.WriteLine($"Выбран новый путь файла {Path.GetFullPath(destFileName)}");
            Console.WriteLine($"Перемещаем {sourceFileName} в {Path.GetFullPath(destFileName)}");
            if (File.Exists(destFileName))
            {
                Console.WriteLine("В этой папке уже есть такой файл, вы хотите перезаписать файл? Введите yes, если да, иначе сделайте любой ввод:");
                if (Console.ReadLine() == "yes")
                {
                    File.Move(sourceFileName, destFileName, true);
                    Console.WriteLine($"Файл {sourceFileName} премещен в {destFileName} и перезаписан");
                    return;
                }
            }
            File.Move(sourceFileName, destFileName);
            Console.WriteLine($"Файл {sourceFileName} перемещен сюда {Path.GetFullPath(destFileName)}");
        }

        /// <summary>
        /// Метод для удаления файла или возвращения в меню.
        /// </summary>
        private static void DoDeleteFile()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Текущая директория:");
                Console.WriteLine(s_currentDirectory.FullName);
                Console.WriteLine("Удаление файла в текущей директории");
                try
                {
                    DeleteFile();
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "menu")
                    {
                        BackToMenu();
                        return;
                    }
                    Console.WriteLine(ProccesExeption(ex));
                    Console.WriteLine("Попробуйте еще раз, нажмите что угодно, чтобы продолжить:");
                    Console.ReadKey(true);
                    continue;
                }
            }
            BackToMenu();
        }

        /// <summary>
        /// Метод для удаления файла.
        /// </summary>
        /// <exception cref="SetShortcutFilePath">Исключение для выхода в меню</exception>
        private static void DeleteFile()
        {
            string filePath = SetShortcutFilePath();
            Console.WriteLine($"Вы уверены, что хотите удалить файл {filePath}. Если да нажмите y, если нет нажмите любую кнопку и вы вернетесь в меню");
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                File.Delete(filePath);
                Console.WriteLine($"Файл {filePath} удален");
            }
        }

        /// <summary>
        /// Создает файл в текущей директории.
        /// </summary>
        private static void DoCreateFile()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Текущая директория:");
                Console.WriteLine(s_currentDirectory.FullName);
                Console.WriteLine("Создание файла в текущей директории");
                try
                {
                    CreateFile(ChooseEncoding());
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "menu")
                    {
                        BackToMenu();
                        return;
                    }
                    Console.WriteLine(ProccesExeption(ex));
                    Console.WriteLine("Попробуйте еще раз, нажмите что угодно, чтобы продолжить:");
                    Console.ReadKey();
                }
            }
            BackToMenu();
        }

        /// <summary>
        /// Создает файл в указанной кодировке.
        /// </summary>
        /// <param name="encoding">Кодировка</param>
        /// <exception cref="Exception">Исключение для выхода в меню</exception>
        private static void CreateFile(Encoding encoding)
        {
            Console.WriteLine("Введите имя файла, который вы хотите создать:");
            string path = Path.Combine(s_currentDirectory.FullName, Console.ReadLine());
            Console.WriteLine($"Создается файл {path}");
            using (StreamWriter streamWriter = new StreamWriter(File.Open(path, FileMode.CreateNew), encoding))
            {
                string text = null;
                Console.WriteLine("Нажмите \"y\" если хотите ввести текст в созданном файле, нажмите Esc чтобы выйти в меню, иначе нажмите любую клавишу");
                switch (Console.ReadKey(false).Key)
                {
                    case ConsoleKey.Escape:
                        throw new Exception("menu");
                    case ConsoleKey.Y:
                        Console.WriteLine("Введите текст");
                        text = Console.ReadLine();
                        break;
                    default:
                        break;
                }
                streamWriter.WriteLine(text);
                Console.WriteLine($"Создан файл {path} в кодировке {encoding}");
            }
        }

        /// <summary>
        /// Метод для конкатенации файлов.
        /// </summary>
        private static void DoConcatFiles()
        {
            var resArr = new List<string>();
            var listOfCurrentFiles = new List<string>();
            string inputString;
            do
            {
                Console.Clear();
                Console.WriteLine("Конкатенация файлов в кодировке UTF-8");
                Console.WriteLine("Выбраны файлы:");
                foreach (var filePath in listOfCurrentFiles)
                {
                    Console.WriteLine(filePath);
                }
                Console.WriteLine("Введите полный путь файла, который вы хотите включить в конкатенацию, введите \"<<\" если хотите закончить конкатенацию файлов");
                inputString = Console.ReadLine();
                try
                {
                    using (StreamReader streamReader = new StreamReader(File.Open(inputString, FileMode.Open), Encoding.UTF8, false))
                    {
                        resArr.Add(streamReader.ReadToEnd());
                    }
                    listOfCurrentFiles.Add(Path.GetFullPath(inputString));
                    Console.WriteLine($"Файл {Path.GetFullPath(inputString)} был добавлен, нажмите любую клавишу, чтобы продолжить");
                    Console.ReadKey(true);
                }
                catch (Exception ex)
                {
                    if (inputString != "<<")
                    {
                        Console.WriteLine(ProccesExeption(ex));
                        Console.WriteLine("Не удалось добавить указанный файл, нажмите любую клавишу, чтобы продолжить");
                        Console.ReadKey(true);
                    }
                }
            }
            while (inputString != "<<");
            Console.WriteLine("Результат конкатенации:");
            foreach (var str in resArr)
            {
                Console.WriteLine(str);
            }
            Console.WriteLine("Если вы хотите записать данный текст в новый файл нажмите y иначе нажмите любую клавишу");
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                CreateFileWithText(Encoding.UTF8, resArr.ToArray());
            }
            BackToMenu();
        }

        /// <summary>
        /// Создает файл с указанным текстом.
        /// </summary>
        /// <param name="encoding">Кодирока в которой будет записан текст</param>
        /// <param name="arrayOfStrings">Текст, который нужно записать в файл</param>
        private static void CreateFileWithText(Encoding encoding, string[] arrayOfStrings)
        {
            Console.WriteLine("Введите имя файла, который вы хотите создать:");
            string path = Path.Combine(s_currentDirectory.FullName, Console.ReadLine());
            Console.WriteLine($"Создается файл {path}");
            using (StreamWriter streamWriter = new StreamWriter(File.Open(path, FileMode.CreateNew), encoding))
            {
                foreach (var str in arrayOfStrings)
                {
                    streamWriter.WriteLine(str);
                }
                Console.WriteLine($"Создан файл {path} в кодировке {encoding}");
            }
        }
    }
}



