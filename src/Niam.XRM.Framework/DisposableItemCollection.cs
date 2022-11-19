using System;
using System.Collections.Generic;
using System.Linq;

namespace Niam.XRM.Framework;

public class DisposableItemCollection
{
    private readonly List<object> _collection = new();

    public IDisposable Add<T>(T item) where T : class
    {
        _collection.Add(item);
        return new Subscription<T>(this, item);
    }
    
    public void Remove<T>(T item) where T : class
    {
        _collection.Remove(item);
    }

    public IEnumerable<T> GetAll<T>() => _collection.OfType<T>();

    private class Subscription<T> : IDisposable where T : class
    {
        private readonly DisposableItemCollection _collection;
        private readonly T _item;

        public Subscription(DisposableItemCollection collection, T item)
        {
            _collection = collection;
            _item = item;
        }
        
        public void Dispose()
        {
            _collection.Remove(_item);
        }
    }
}