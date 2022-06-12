using System;

namespace labaFour {
    public static class TextEditor {

        private static FileSearch CreateSearcher() {

            FileSearch searcher;

            while (true) {

                Console.WriteLine("Enter directory:");
                var directory = Console.ReadLine();

                try {

                    searcher = new FileSearch(directory);
                    break;
                }
                catch (Exception exception) {

                    Console.WriteLine(exception.Message);
                    Console.WriteLine("Try again.");
                    Console.WriteLine("\n");
                }
            }

            return searcher;
        }

        private static void FindFile(FileSearch searcher) {

            Console.Clear();
            Console.WriteLine("Show all files in current directory     0");
            Console.WriteLine("Find file by tags                       1");
            Console.WriteLine("Add tags to files                       2");
            Console.WriteLine("EXIT                                    3");
            Console.WriteLine("\n");

            while (true) {

                Console.WriteLine("Choose option");

                switch (Console.ReadLine()) {

                    case "0":
                        searcher.GetFiles();
                        break;
                    case "1":

                        var fileS = new FileStream($"{searcher.DirName + "/search.bin"}", FileMode.OpenOrCreate, FileAccess.Read);
                        if (fileS.Length != 0) {

                            searcher.Deserialize(fileS);
                            fileS.Close();
                        }
                        else {

                            Console.WriteLine("No files found.");
                            fileS.Close();
                            break;
                        }

                        Console.WriteLine("Enter tags:");
                        string[] tags = new string[2];
                        tags[0] = Console.ReadLine();
                        tags[1] = Console.ReadLine();

                        try {

                            searcher.Search(tags);
                            break;
                        }
                        catch (Exception exception) {

                            Console.WriteLine(exception.Message);
                            break;
                        }
                    case "2":

                        var fileD = new FileStream($"{searcher.DirName + "/search.bin"}", FileMode.OpenOrCreate, FileAccess.Read);
                        if (fileD.Length != 0) {

                            searcher.Deserialize(fileD);
                        }
                        fileD.Close();

                        Console.WriteLine("Enter file name:");
                        var name = Console.ReadLine();
                        try {

                            searcher.AddFile(name);
                        }
                        catch (Exception exception) {

                            Console.WriteLine(exception.Message);
                            break;
                        }

                        fileD = new FileStream($"{searcher.DirName + "/search.bin"}", FileMode.OpenOrCreate, FileAccess.Write);
                        searcher.Serialize(fileD);
                        fileD.Close();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Unknown option.");
                        break;
                }

                Console.WriteLine("\n");
            }
        }

        private static void Editor(FileSearch searcher) {

            Console.WriteLine("Enter file name:");
            var name = Console.ReadLine();
            name = searcher.DirName + "/" + name;

            if (!File.Exists(name)) {

                var file = File.Create(name);
                file.Close();
            }

            Console.Clear();

            var textFile = new TextFile(name);
            var textR = new StreamReader(textFile.Name);
            textFile.Content = textR.ReadToEnd();
            Console.WriteLine(textFile.Content);
            textR.Close();

            using (var textW = new StreamWriter(textFile.Name, true)) {

                var key = new ConsoleKeyInfo();
                var text = "";
                var caretaker = new Caretaker();

                while (true) {

                    caretaker.SaveState(textFile);

                    text += Console.ReadLine();
                    textFile.Content += $"\n{text}";

                    key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Delete) {

                        caretaker.RestoreState(textFile);
                        Console.Clear();
                        Console.WriteLine(textFile.Content);
                        text = "";
                    }
                    else if (key.Key == ConsoleKey.Escape) {

                        textW.WriteLine(text);
                        textW.Flush();
                        Console.WriteLine("EEditing finished.");
                        return;
                    }
                    else {

                        textW.WriteLine(text);
                        textW.Flush();
                        text = key.Key.ToString().ToLower();
                    }
                }
            }
        }

        public static void Program() {

            var searcher = CreateSearcher();

            while (true) {

                Console.Clear();
                Console.WriteLine("Search files     0");
                Console.WriteLine("Text editor      1");
                Console.WriteLine("EXIT             2");
                Console.WriteLine("\n");
                Console.WriteLine("Choose option");

                switch (Console.ReadLine()) {

                    case "0":
                        Console.Clear();
                        FindFile(searcher);
                        break;
                    case "1":
                        Console.Clear();
                        Editor(searcher);
                        break;
                    case "2":
                        return;
                    default:
                        Console.WriteLine("Unknown option.");
                        break;
                }
            }
        }
    }
}