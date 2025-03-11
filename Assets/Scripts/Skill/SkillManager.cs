using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : SingletonDestroy<SkillManager>
{
    public SkillBase[] skills;

    public void Init()
    {
        skills = GetComponents<SkillBase>();

        for(int i = 0; i < skills.Length; i++)
        {
            skills[i].SkillID = 1000000 + i;
        }
    }

    public SkillBase GetSkill(int id)
    {
        for(int i = 0;i < skills.Length;i++)
        {
            if (skills[i].SkillID == id)
                return skills[i];
        }

        return null;
    }
}
