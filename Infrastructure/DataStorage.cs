using Newtonsoft.Json;
using System;
using System.IO;

namespace Infrastructure
{
    /// <summary>
    /// Статический класс для работы с JSON-хранилищем данных
    /// </summary>
    public static class DataStorage
    {
        /// <summary>
        /// Сохраняет объект в JSON-файл по указанному абсолютному пути
        /// </summary>
        /// <param name="obj">Объект для сохранения</param>
        /// <param name="absolutePath">Абсолютный путь к файлу</param>
        /// <exception cref="ArgumentNullException">
        /// Возникает, если obj или absolutePath равен null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Возникает при некорректном пути
        /// </exception>
        /// <exception cref="IOException">
        /// Возникает при ошибках записи в файл
        /// </exception>
        /// <remarks>
        /// Метод выполняет следующие проверки:
        /// 1. Валидация входных параметров
        /// 2. Проверка корректности пути
        /// 3. Сериализация объекта в JSON
        /// 4. Запись в файл с обработкой ошибок
        /// </remarks>
        public static void SaveJson(this object obj, string absolutePath)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));
            _ = absolutePath ?? throw new ArgumentNullException(nameof(absolutePath));

            if (!PathChecking.IsCorrectAbsolutePath(absolutePath))
                throw new ArgumentException("Указанный путь содержит недопустимые символы или не является абсолютным");

            var jsonStr = JsonConvert.SerializeObject(obj);

            try
            {
                File.WriteAllText(Path.GetFullPath(absolutePath), jsonStr);
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка сохранения объекта в файл {absolutePath}", ex);
            }
        }

        /// <summary>
        /// Читает и десериализует объект из JSON-файла
        /// </summary>
        /// <typeparam name="ResultType">Тип возвращаемого объекта</typeparam>
        /// <param name="absolutePath">Абсолютный путь к файлу</param>
        /// <returns>Десериализованный объект указанного типа</returns>
        /// <exception cref="ArgumentNullException">
        /// Возникает, если absolutePath равен null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Возникает при некорректном пути или несоответствии типа
        /// </exception>
        /// <exception cref="IOException">
        /// Возникает при ошибках чтения файла
        /// </exception>
        /// <remarks>
        /// Метод выполняет следующие операции:
        /// 1. Валидация пути
        /// 2. Чтение файла
        /// 3. Десериализация JSON с проверкой типа
        /// </remarks>
        public static ResultType ReadFromFile<ResultType>(string absolutePath)
        {
            _ = absolutePath ?? throw new ArgumentNullException(nameof(absolutePath));

            if (!PathChecking.IsCorrectAbsolutePath(absolutePath))
                throw new ArgumentException("Указанный путь содержит недопустимые символы или не является абсолютным");

            var jsonStr = string.Empty;

            try
            {
                jsonStr = File.ReadAllText(absolutePath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка чтения файла {absolutePath}", ex);
            }

            try
            {
                return JsonConvert.DeserializeObject<ResultType>(jsonStr);
            }
            catch (JsonSerializationException)
            {
                throw new ArgumentException("Указанный тип не соответствует содержимому файла");
            }
        }
    }
}