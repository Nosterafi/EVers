using System;
using System.IO;

namespace Infrastructure
{
    // Вспомогательный класс для проверки корректности путей
    internal static class PathChecking
    {
        /// <summary>
        /// Проверяет, является ли путь абсолютным и не содержит недопустимых символов
        /// </summary>
        /// <param name="path">Проверяемый путь</param>
        /// <returns>
        /// true - путь абсолютный и валидный,
        /// false - путь относительный или содержит ошибки
        /// </returns>
        internal static bool IsCorrectAbsolutePath(string path)
        {
            try
            {
                // Проверяем, что путь абсолютный (имеет корень)
                return Path.IsPathRooted(path);
            }
            catch (ArgumentException)
            {
                // Ловим исключение при наличии недопустимых символов
                return false;
            }
        }
    }
}
