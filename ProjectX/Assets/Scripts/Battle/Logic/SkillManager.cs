using System.Collections;
using System.Collections.Generic;

namespace ProjectX.Battle.Logic
{
    public class SkillManager
    {
        private int m_OwnerId;

        public SkillManager(int ownerId, List<int> skills)
        {
            m_OwnerId = ownerId;
        }
    }
}

