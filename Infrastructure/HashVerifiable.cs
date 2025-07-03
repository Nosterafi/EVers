using System;
using System.Security.Cryptography;

namespace Infrastructure
{
    /// <summary>
    /// Базовый класс для объектов, требующих проверки целостности через хеширование.
    /// Наследники должны реализовать логику вычисления идентификатора.
    /// </summary>
    public abstract class HashVerifiable
    {
        // Общий экземпляр SHA256 для всех объектов класса
        private static readonly SHA256 SHA256 = SHA256.Create();

        /// <summary>
        /// Текущий идентификатор объекта (хеш)
        /// </summary>
        private string СurrentId { get; set; }

        // Статический конструктор для очистки ресурсов при завершении приложения
        static HashVerifiable() =>
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => SHA256.Dispose();

        /// <summary>
        /// Устанавливает текущий идентификатор, вычисляя его через ComputeId()
        /// </summary>
        public void SetCurrentId() => СurrentId = ComputeId();

        /// <summary>
        /// Проверяет, изменился ли объект после последнего вызова SetCurrentId()
        /// </summary>
        /// <returns>
        /// true - объект не изменялся,
        /// false - объект был изменен
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Выбрасывается, если не был вызван SetCurrentId()
        /// </exception>
        public bool IsNotChanged()
        {
            _ = СurrentId ?? throw new InvalidOperationException(
                "Перед проверкой необходимо установить CurrentId через SetCurrentId()");
            return СurrentId == ComputeId();
        }

        /// <summary>
        /// Абстрактный метод для вычисления идентификатора объекта.
        /// Должен быть реализован в наследниках.
        /// </summary>
        protected abstract string ComputeId();

        /// <summary>
        /// Вычисляет SHA256 хеш для массива байт
        /// </summary>
        /// <param name="value">Данные для хеширования</param>
        /// <returns>Строковое представление хеша (hex)</returns>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается если входные данные null
        /// </exception>
        protected string ComputeHash(byte[] value)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var resultBytes = SHA256.ComputeHash(value);
            return BitConverter.ToString(resultBytes)
                .Replace("-", "")
                .ToLowerInvariant();
        }
    }
}