using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite; //библиотека для работы с СУБД SQLite
using System.IO;

/* Для работы с базой данных необходимо добавить в проект соответствующую
             * библиотеку, с помощью которой вы сможете работать с СУБД SQLite. Зайдите
             * на вкладку "СРЕДСТВА" в верхнем меню среды Visual Studio, в контекстном
             * меню выберите "Диспетчер пакетов NuGet", далее "Управление пакетами NuGet
             * для решения...". В открывшемся окне нажмите "Обзор", в поисковой строке
             * введите следующий запрос: "System.Data.SQLite". Кликните по выданному 
             * запросу System.Data.SQLite (значок - синий квадрат с перышком",
             * в правом окошке пометьте галочкой свой проект, нажмите "Установить".
             * Необходимо будет принять условия лицензионного соглашения, кликаем
             * "Принять". Ждем, далее прописываем в using'ах файла Program.cs
             * следующую строчку: "using System.Data.SQLite;".
             * Успех: вы подключили библиотеку для работы с СУБД SQLite!
             */

namespace ConsoleApp2
{
    class Program
    {
        static void EMain(string[] args)
        {
            //проверяем, создан ли файл с БД
            if (File.Exists(@"C:\Users\Anton\Desktop\Test.db"))
                Console.WriteLine("DB exists, OK");
            else
            {
                //создаем файл базы данных (формата .db) в указанной директории
                SQLiteConnection.CreateFile(@"C:\Users\Anton\Desktop\Test.db"); 
                Console.WriteLine("DB was created successfully");
            }

            //создаем соединение с БД
            SQLiteConnection connect
                = new SQLiteConnection(@"Data Source=C:\Users\Anton\Desktop\Test.db; Version=3;"); 
            connect.Open(); //открываем БД

            SQLiteCommand command; //объявляем переменную запроса

            //Названия полей, таблиц и значений заключаем между совокупностями символов \"
            //с целью предовратить проблемы с кодировкой (если у вас все равно выдает ошибку, попробуйте
            //наоборот убрать эти символы, возможно конкретно у вас проблем с кодировкой нет)

            //Обратите внимание на то, что методы ExecuteNonQuery() и ExecuteReader() разные, так как
            //созданы с разной целью:
            //ExecuteNonQuery() - вы отсылаете запрос без возврата данных
            //ExecuteReader() - вы отсылаете запрос с условием, что СУБД вернет поток данных


            //инициализируем запрос (просто инициализируем)
            command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS \"Empl\" (\"id\" INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "\"name\" TEXT, \"info\" TEXT)", connect); 
            command.ExecuteNonQuery(); //выполняем запрос

            command = new SQLiteCommand("INSERT INTO \"Empl\"(\"name\", \"info\") " +
                "VALUES ('\"Kolobok\"', '\"ochen krutoy\"')", connect);
            command.ExecuteNonQuery();

            command = new SQLiteCommand("INSERT INTO \"Empl\"(\"name\", \"info\") " +
               "VALUES ('\"Kolobok\"', '\"ochen ne krutoy\"')", connect);
            command.ExecuteNonQuery();

            command = new SQLiteCommand("SELECT * FROM \"Empl\"", connect);
            SQLiteDataReader reader; //объявляем переменную чтения возвращаемого потока данных (ридер)
            reader = command.ExecuteReader(); //выполняем запрос и заливаем весь возвращаемый поток данных в ридер
            while (reader.Read()) //вытягиваем данные из ридера до тех пор, пока ридер не останется пустым
            {
                //в квадратных скобках пишем названия полей таблицы таким же образом, как они указаны в запросе
                Console.WriteLine("ID " + reader["id"] + " NAME " + reader["name"]
                    + " INFO " + reader["info"]); 
            }
            reader.Close(); //закрываем ридер

            connect.Close(); //закрываем базу данных
            Console.ReadLine();
        }
    }
}
