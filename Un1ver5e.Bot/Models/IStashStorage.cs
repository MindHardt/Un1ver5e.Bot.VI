using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot.Models
{
    /// <summary>
    /// Represents a storage for <see cref="IStashData"/> objects in correlations to <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStashStorage<T>
    {
        /// <summary>
        /// Gets a <see cref="IStashData"/> object which is associated with <paramref name="snowflake"/> if it exists.
        /// </summary>
        /// <param name="snowflake"></param>
        /// <returns></returns>
        public IStashData? Get(T key);
        /// <summary>
        /// Gets a <see cref="IStashData"/> object which is associated with <paramref name="snowflake"/> if it exists and removes it from this <see cref="IStashStorage{T}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IStashData? Pop(T key);
        /// <summary>
        /// Stashed <paramref name="data"/> and associates it with <paramref name="snowflake"/>.
        /// </summary>
        /// <param name="snowflake"></param>
        /// <param name="data"></param>
        public void Stash(T key, IStashData data);
    }
}
