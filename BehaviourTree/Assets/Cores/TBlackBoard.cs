using System.Collections;
using System.Collections.Generic;

namespace GGGBT
{
    public class TBlackBoard
    {
        class TBlackBoardItem
        {
            private object _value;
            public void SetValue(object v)
            {
                _value = v;
            }
            public T GetValue<T>()
            {
                return (T)_value;
            }
        }

        private Dictionary<string, TBlackBoardItem> _items;

        public TBlackBoard()
        {
            _items = new Dictionary<string, TBlackBoardItem>();
        }

        ~TBlackBoard()
        {
            _items = null;
        }

        public void SetValue(string key, object v)
        {
            TBlackBoardItem item;
            if (!_items.TryGetValue(key, out item))
            {
                item = new TBlackBoardItem();
                _items.Add(key, item);
            }
            item.SetValue(v);
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            TBlackBoardItem item;
            if (_items.TryGetValue(key, out item))
            {
                return item.GetValue<T>();
            }
            return defaultValue;
        }
    }
}
