using System;
using System.Collections.Generic;
using System.Data.SQLite;  // Необходимо добавить пакет System.Data.SQLite

class Program
{
    static void Main(string[] args)
    {
        // Создание или подключение к базе данных SQLite
        string connectionString = "Data Source=example.db;Version=3;";
        using (var connection = new SQLiteConnection(connectionString))
        {
            string want;
            int stop = 0;
            connection.Open();
            CreateTable(connection);
            while (stop == 0)
            {
                try
                {
                    Console.WriteLine("1.1 - добавить студента, 1.2 - добавить учителя, 1.3 - добавить курс, 1.4 - добавить экзамен, 1.5 - добавить оценку студенту, \n" +
                    "2.1 - изменить информацию о студенте, 2.2 - изменить информацию о преподавателях, 2.3 - изменить информацию о курсах, \n" +
                    "3.1 - удалить студента, 3.2 - удалить преподавателя, 3.3 - удалить курс, 3.4 - удалить экзамен, \n" +
                    "4 - получить список студентов по курсу, \n" +
                    "5 - получить список курсов,читаемых данным преподавателем, " +
                    "6 - получить список студентов данного курса, \n" +
                    "7 - получить список оценок по данному курсу, \n" +
                    "8 - средний бал данного студента по данному курсу, \n" +
                    "9 - средний балл данного студента, \n" +
                    "10 - средний бал по данному факультету, \n" +
                    "stop - завершить работу");
                    want = Console.ReadLine();
                    if (want[0] == '1' & want.Length == 3)
                    {
                        if (want[2] == '1' & want.Length == 3) { InsertStudent(connection, Console.ReadLine(), Console.ReadLine(), Console.ReadLine(), int.Parse(Console.ReadLine())); }
                        else if (want[2] == '2' & want.Length == 3) { InsertTeacher(connection, Console.ReadLine(), Console.ReadLine(), Console.ReadLine()); }
                        else if (want[2] == '3' & want.Length == 3) { InsertCourse(connection, Console.ReadLine(), Console.ReadLine(), int.Parse(Console.ReadLine())); }
                        else if (want[2] == '4' & want.Length == 3) { InsertExam(connection, Console.ReadLine(), int.Parse(Console.ReadLine())); }
                        else if (want[2] == '5' & want.Length == 3) { InsertGrade(connection, int.Parse(Console.ReadLine()), int.Parse(Console.ReadLine()), int.Parse(Console.ReadLine())); }
                    }
                    else if (want[0] == '2' & want.Length == 3)
                    {
                        if (want[2] == '1') { UpdateStudent(connection, int.Parse(Console.ReadLine()), Console.ReadLine(), Console.ReadLine(), Console.ReadLine(), int.Parse(Console.ReadLine())); }
                        else if (want[2] == '2') { UpdateTeacher(connection, int.Parse(Console.ReadLine()), Console.ReadLine(), Console.ReadLine(), Console.ReadLine()); }
                        else if (want[2] == '3') { UpdateCourse(connection, int.Parse(Console.ReadLine()), Console.ReadLine(), Console.ReadLine(), int.Parse(Console.ReadLine())); }
                    }
                    else if (want[0] == '3' & want.Length == 3)
                    {
                        if (want[2] == '1') { DeleteStudent(connection, int.Parse(Console.ReadLine())); }
                        else if (want[2] == '2') { DeleteTeacher(connection, int.Parse(Console.ReadLine())); }
                        else if (want[2] == '3') { DeleteCourse(connection, int.Parse(Console.ReadLine())); }
                        else if (want[2] == '4') { DeleteExam(connection, int.Parse(Console.ReadLine())); }
                    }
                    else if (want[0] == '4' & want.Length == 1) { PrintStudentsByDepartment(connection, Console.ReadLine()); }
                    else if (want[0] == '5' & want.Length == 1) { PrintCoursesByTeacher(connection, int.Parse(Console.ReadLine())); }
                    else if (want[0] == '6' & want.Length == 1) { PrintStudentsByCourse(connection, int.Parse(Console.ReadLine())); }
                    else if (want[0] == '7' & want.Length == 1) { GradesByCourse(connection, int.Parse(Console.ReadLine())); }
                    else if (want[0] == '8' & want.Length == 1) { AverageScoreByCourse(connection, int.Parse(Console.ReadLine()), int.Parse(Console.ReadLine())); }
                    else if (want[0] == '9' & want.Length == 1) { AverageScoreByStudent(connection, int.Parse(Console.ReadLine())); }
                    else if (want[0] == '1' & want[1] == '0' & want.Length == 2) { AverageScoreByDepartment(connection, Console.ReadLine()); }
                    else if (want == "stop")
                    {
                        connection.Close();
                        Console.WriteLine("Соединение с базой данных закрыто, Adios");
                        stop = 1;
                    }
                    else { Console.WriteLine("Вы неправильно ввели число, попробуйте повторить попытку..."); }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }
    }


    static void CreateTable(SQLiteConnection connection)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS Students (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT,
            surname TEXT,
            department TEXT,
            age INTEGER
            );

            CREATE TABLE IF NOT EXISTS Grades (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                student_id INTEGER,
                exam_id INTEGER,
                grade INTEGER,
                CONSTRAINT fk_student_id FOREIGN KEY (student_id) REFERENCES Students (id) ON DELETE SET NULL ON UPDATE CASCADE,
                CONSTRAINT fk_exam_id FOREIGN KEY (exam_id) REFERENCES Exams (id) ON DELETE SET NULL ON UPDATE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Teachers (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT,
                surname TEXT,
                department TEXT
            );

            CREATE TABLE IF NOT EXISTS Courses (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                title TEXT,
                description TEXT,
                teacher_id INTEGER,
                CONSTRAINT fk_teacher_id FOREIGN KEY (teacher_id) REFERENCES Teachers (id) ON DELETE SET NULL ON UPDATE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Exams (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                session_date DATE,
                course_id INTEGER,
                CONSTRAINT fk_course_id FOREIGN KEY (course_id) REFERENCES Courses (id) ON DELETE SET NULL ON UPDATE CASCADE
            );

            CREATE TABLE IF NOT EXISTS StudentCourses (
                student_id INTEGER,
                course_id INTEGER,
                PRIMARY KEY (student_id, course_id),
                CONSTRAINT fk_course_id FOREIGN KEY (course_id) REFERENCES Courses (id),
                CONSTRAINT fk_student_id FOREIGN KEY (student_id) REFERENCES Students (id)
            );

            CREATE TABLE IF NOT EXISTS GradesByCourse (
                grade_id INTEGER,
                course_id INTEGER,
                PRIMARY KEY (course_id),
                CONSTRAINT fk_course_id FOREIGN KEY (course_id) REFERENCES Courses (id),
                CONSTRAINT fk_grade_id FOREIGN KEY (grade_id) REFERENCES Grades (id)
            );";
            command.ExecuteNonQuery();
            Console.WriteLine("Таблицы успешно созданы.");
        }
    }
    static void UpdateStudent(SQLiteConnection connection, int id, string name, string surname, string department, int age)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = $"UPDATE Students SET name = @name, surname = @surname, department = @department, age = @age WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@department", department);
            command.Parameters.AddWithValue("@age", age);
            command.ExecuteNonQuery();
            Console.WriteLine($"Данные о студенте с ID {id} изменены");
        }
    }
    static void UpdateTeacher(SQLiteConnection connection, int id, string name, string surname, string department)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = $"UPDATE Students SET name = @name, surname = @surname, department = @department WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@department", department);
            command.ExecuteNonQuery();
            Console.WriteLine($"Данные о студенте с ID {id} изменены");
        }
    }
    static void UpdateCourse(SQLiteConnection connection, int id, string title, string description, int teacher_id)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = $"UPDATE Courses SET title = @title, description = @description, teacher_id = @teacher_id WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@teacher_id", teacher_id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Данные о студенте с ID {id} изменены");
        }
    }
    static void UpdateExam(SQLiteConnection connection, int id, int date, int course_id)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = $"UPDATE Students SET date = @date, course_id = @course_id WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@date", date);
            command.Parameters.AddWithValue("@course_id", course_id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Данные о студенте с ID {id} изменены");
        }
    }
    static void AverageScoreByCourse(SQLiteConnection connection, int course_id, int student_id)
    {
        using (var command = new SQLiteCommand($"SELECT AVG(Grades.grade) as grade FROM Grades JOIN Exams ON Grades.exam_id =  Exams.id WHERE Exams.course_id = '{course_id}' and grades.student_id = '{student_id}'", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"Средний балл студента с ID {student_id} по ID курсу {course_id} - {reader["grade"]}");
            }
        }
    }
    static void AverageScoreByStudent(SQLiteConnection connection, int id)
    {
        using (var command = new SQLiteCommand($"SELECT AVG(grade) as grade FROM Grades where student_id = {id}", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"Средний балл студента с ID {id} - {reader["grade"]}");
            }
        }
    }
    static void GradesByCourse(SQLiteConnection connection, int id)
    {
        using (var command = new SQLiteCommand($"SELECT Grades.* FROM Grades JOIN Exams ON Grades.exam_id =  Exams.id WHERE Exams.course_id = '{id}'", connection))
        using (var reader = command.ExecuteReader())
        {
            Console.WriteLine($"Оценки по ID курсу {id}");
            while (reader.Read())
            {
                Console.WriteLine($"ID Оценки: {reader["id"]}, Оценка: {reader["grade"]}");
            }
        }
    }
    static void AverageScoreByDepartment(SQLiteConnection connection, string department)
    {
        using (var command = new SQLiteCommand($"SELECT AVG(grade) from Grades where student_id in (SELECT id FROM Students where department = '{department}')", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"Cредний балл по факультету {department}: {reader["AVG(grade)"]} ");

            }

        }
    }
    static void PrintStudentsByCourse(SQLiteConnection connection, int id)
    {
        using (var command = new SQLiteCommand($"SELECT DISTINCT Students.* FROM Grades JOIN Students ON Grades.student_id == Students.id WHERE exam_id IN (SELECT DISTINCT id FROM Exams WHERE course_id = {id})", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"Имя: {reader["name"]}, Фамилия {reader["surname"]}");
            }
        }
    }
    static void PrintCoursesByTeacher(SQLiteConnection connection, int id)
    {
            using (var command = new SQLiteCommand($"SELECT * FROM Courses WHERE teacher_id = '{id}'", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["id"]}, Название: {reader["title"]}, ID учителя: {reader["teacher_id"]}, Описание курса:  {reader["description"]}");
                }
            }
    }
    static void PrintStudentsByDepartment(SQLiteConnection connection, string department)
    {
        using (var command = new SQLiteCommand($"SELECT * FROM Students WHERE department = '{department}'", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["id"]}, Имя: {reader["name"]}, Фамилия: {reader["surname"]}, Факультет: {reader["department"]}, Возраст: {reader["age"]}");
            }
        }
    }
    static void DeleteExam(SQLiteConnection connection, int id)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "DELETE FROM Exams WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Экзамен {id} удален.");
        }
    }
    static void DeleteTeacher(SQLiteConnection connection, int id)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "DELETE FROM Teachers WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Учитель {id} удален.");
        }
    }
    static void DeleteStudent(SQLiteConnection connection, int id)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "DELETE FROM Students WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Ученик {id} удален.");
        }
    }
    static void СhangeStudent(SQLiteConnection connection, int id, string name, string surname, string department, int age)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "INSERT INTO Students (name, surname, department, age) VALUES (@name, @surname, @department, @age)";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@department", department);
            command.Parameters.AddWithValue("@age", age);
            command.ExecuteNonQuery();
            Console.WriteLine($"Студент {name} {surname} добавлен.");
        }
    }
    static void GetGrades(SQLiteConnection connection)
    {
        using (var command = new SQLiteCommand("SELECT * FROM Grades", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["id"]}, ID студента: {reader["student_id"]}, Id экзамена: {reader["exam_id"]}, Оценка: {reader["grade"]}");
            }
        }
    }
    static void InsertGrade(SQLiteConnection connection, int student_id, int exam_id, int grade)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "INSERT INTO Grades (student_id, exam_id, grade) VALUES (@student_id, @exam_id, @grade)";
            command.Parameters.AddWithValue("@student_id", student_id);
            command.Parameters.AddWithValue("@exam_id", exam_id);
            command.Parameters.AddWithValue("@grade", grade);
            command.ExecuteNonQuery();
            Console.WriteLine($"Данная оценка добавлен.");
        }
    }
    static void DeleteCourse(SQLiteConnection connection, int id)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "DELETE FROM Courses WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Курс {id} удален.");
        }
    }
    static void InsertStudent(SQLiteConnection connection, string name, string surname, string department, int age)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "INSERT INTO Students (name, surname, department, age) VALUES (@name, @surname, @department, @age)";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@department", department);
            command.Parameters.AddWithValue("@age", age);
            command.ExecuteNonQuery();
            Console.WriteLine($"Студент {name} {surname} добавлен.");
        }
    }
    static void InsertTeacher(SQLiteConnection connection, string name, string surname, string department)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "INSERT INTO Teachers (name, surname, department) VALUES (@name, @surname, @department)";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@department", department);
            command.ExecuteNonQuery();
            Console.WriteLine($"Преподаватель {name} {surname} добавлен.");
        }
    }
    static void InsertCourse(SQLiteConnection connection, string title, string description, int teacher_id)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "INSERT INTO Courses (title, description, teacher_id) VALUES (@title, @description, @teacher_id);";
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@teacher_id", teacher_id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Курс {title} добавлен.");
            
        }
    }
    static void InsertExam(SQLiteConnection connection, string session_date, int course_id)
    {
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "INSERT INTO Exams (session_date, course_id) VALUES (@session_date, @course_id)";
            command.Parameters.AddWithValue("@session_date", session_date);
            command.Parameters.AddWithValue("@course_id", course_id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Данный экзамен добавлен.");
        }
    }
    static void GetStudents(SQLiteConnection connection)
    {
        using (var command = new SQLiteCommand("SELECT * FROM Students", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["id"]}, Имя: {reader["name"]}, Возраст: {reader["age"]}");
            }
        }
    } 
    static void GetCourses(SQLiteConnection connection)
    {
        using (var command = new SQLiteCommand("SELECT * FROM Courses", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["id"]}, Название: {reader["title"]}, ID учителя: {reader["teacher_id"]}, Описание курса:  {reader["description"]}");
            }
        }
    }
    static void GetTeachers(SQLiteConnection connection)
    {
        using (var command = new SQLiteCommand("SELECT * FROM teachers", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["id"]}, Имя: {reader["name"]}, Фамилия: {reader["surname"]}, Отдел: {reader["department"]}");
            }
        }
    }
    static void GetExams(SQLiteConnection connection)
    {
        using (var command = new SQLiteCommand("SELECT * FROM exams", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["id"]}, Дата: {reader["session_date"]}, Курс: {reader["course_id"]}");
            }
        }
    }
}
