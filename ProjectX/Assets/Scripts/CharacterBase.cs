using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectX.Component;

namespace ProjectX.Character
{
    public class CharacterBase
    {
        protected Transform m_Transform;
        public Transform Transform { get { return m_Transform; } }

        protected Animator m_Animator;
        protected Dictionary<EComponent, ComponentBase> m_AllComponents;

        public CharacterBase(Transform transform)
        {
            m_AllComponents = new Dictionary<EComponent, ComponentBase>();
            m_Transform = transform;
        }

        public T GetComponent<T>(EComponent etype) where T : ComponentBase
        {
            ComponentBase component;
            if (!m_AllComponents.TryGetValue(etype, out component))
            {
                return default(T);
            }
            return (T)component;
        }

        public void AddComponent(EComponent etype, ComponentBase component)
        {
            if (m_AllComponents.ContainsKey(etype))
            {
                return;
            }
            m_AllComponents.Add(etype, component);
        }
    }
}

