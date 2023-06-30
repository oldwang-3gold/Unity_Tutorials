using System.Collections;
using System.Collections.Generic;

namespace ProjectX.Battle.Logic
{
    public struct SkillConfig
    {
        public ESkillType Type;
        public string Name { get; set; }
        public float CD { get; set; }
    }

    public class SkillBase
    {
        private int m_OwnerId;
        private int m_SkillId;
        private int m_Level;
        private float m_CD;
        private float m_MaxCD;
        private SkillConfig m_Config;

        public SkillBase(int ownerId, int skillId, int level, float defaultcd = -1)
        {
            m_OwnerId = ownerId;
            m_SkillId = skillId;
            m_Level = level;
            m_Config = new SkillConfig();
            m_CD = defaultcd < 0 ? m_Config.CD : defaultcd;
            m_MaxCD = m_CD;
        }

        public bool CheckRelease()
        {
            if (m_Config.Type != ESkillType.Initial || m_CD > 0) return false;
            // 再判断下角色状态是否允许放技能(是否处于眩晕这种)
            return true;
        }

        /// <summary>
        /// 技能释放出来
        /// </summary>
        /// <param name="targetId"></param>
        public virtual void Release(int targetId)
        {
            // 
            Output.Execute(EBattleLogicOutput.Skill, m_OwnerId, m_SkillId, m_Level, targetId);
        }
    }
}

