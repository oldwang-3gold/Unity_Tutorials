using System.Collections;
using System.Collections.Generic;

namespace GGGBT
{
    public class BTWorkingData
    {
        internal Dictionary<int, BTActionContext> _context;
        internal Dictionary<int, BTActionContext> Context
        {
            get
            {
                return _context;
            }
        }

        public BTWorkingData()
        {
            _context = new Dictionary<int, BTActionContext>();
        }

        ~BTWorkingData()
        {
            _context = null;
        }

        public T As<T>() where T : BTWorkingData
        {
            return (T)this;
        }
    }
}

