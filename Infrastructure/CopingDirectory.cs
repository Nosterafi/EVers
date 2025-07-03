using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Infrastructure
{
    /// <summary>
    /// Класс для рекурсивного копирования директорий с использованием итеративного подхода
    /// </summary>
    public static class CopingDirectory
    {
        // Стек для хранения исходных директорий
        private static readonly Stack<DirectoryInfo> _originStack = new Stack<DirectoryInfo>();

        // Стек для хранения целевых директорий
        private static readonly Stack<DirectoryInfo> _targetStack = new Stack<DirectoryInfo>();

        /// <summary>
        /// Рекурсивно копирует содержимое директории в указанное место
        /// </summary>
        /// <param name="origin">Исходная директория</param>
        /// <param name="targetPath">Путь назначения</param>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если origin или targetPath равен null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Выбрасывается в случаях:
        /// - Указан некорректный путь
        /// - Исходная директория не существует
        /// - Целевая директория не существует
        /// </exception>
        /// <exception cref="IOException">
        /// Выбрасывается при ошибках ввода-вывода
        /// </exception>
        /// <remarks>
        /// Особенности реализации:
        /// - Поддерживает длинные пути в Windows (добавляет префикс \\?\)
        /// - Автоматически удаляет частично скопированные данные при ошибке
        /// - Использует итеративный подход вместо рекурсии
        /// </remarks>
        public static void CopyTo(this DirectoryInfo origin, string targetPath)
        {
            _ = origin ?? throw new ArgumentNullException(nameof(origin));
            _ = targetPath ?? throw new ArgumentNullException(nameof(targetPath));

            if (!PathChecking.IsCorrectAbsolutePath(targetPath))
                throw new ArgumentException("Указанный путь содержит недопустимые символы или не является абсолютным");

            targetPath = Path.GetFullPath(targetPath);

            if (!origin.Exists)
                throw new ArgumentException("Исходная директория не существует");

            if (!Directory.Exists(targetPath))
                throw new ArgumentException("Целевая директория не существует");

            // Формирование конечного пути
            targetPath = Path.Combine(targetPath, origin.Name);

            // Обработка длинных путей в Windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && targetPath.Length >= 200)
            {
                targetPath = @"\\?\" + targetPath;
            }

            // Инициализация стеков
            _originStack.Push(origin);
            _targetStack.Push(new DirectoryInfo(targetPath));

            try
            {
                RunCopyLoop();
            }
            catch (IOException)
            {
                // Откат изменений при ошибке
                Directory.Delete(targetPath, true);
                throw;
            }
            finally
            {
                // Гарантированная очистка стеков
                _originStack.Clear();
                _targetStack.Clear();
            }
        }

        /// <summary>
        /// Выполняет итеративное копирование содержимого директорий
        /// </summary>
        /// <exception cref="IOException">
        /// Выбрасывается при ошибках создания поддиректорий или копирования файлов
        /// </exception>
        private static void RunCopyLoop()
        {
            while (_originStack.Count > 0)
            {
                var origin = _originStack.Pop();
                var target = _targetStack.Pop();

                // Обработка поддиректорий
                foreach (var dir in origin.GetDirectories())
                {
                    _originStack.Push(dir);

                    try
                    {
                        _targetStack.Push(target.CreateSubdirectory(dir.Name));
                    }
                    catch (Exception ex)
                    {
                        throw new IOException(
                            $"Ошибка создания директории {dir.Name} по пути {target.FullName}", ex);
                    }
                }

                // Копирование файлов
                foreach (var file in origin.GetFiles())
                {
                    try
                    {
                        file.CopyTo(Path.Combine(target.FullName, file.Name));
                    }
                    catch (Exception ex)
                    {
                        throw new IOException(
                            $"Ошибка копирования файла {file.Name} в {target.FullName}", ex);
                    }
                }
            }
        }
    }
}