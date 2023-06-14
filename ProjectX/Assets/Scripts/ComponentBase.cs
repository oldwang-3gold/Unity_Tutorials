using System;

namespace ProjectX.Component
{
    [Serializable]
    public class ComponentBase
    {
        protected EComponent m_Type = EComponent.Base;

        public ComponentBase()
        {
            Console.WriteLine(666);
        }
    }
}

