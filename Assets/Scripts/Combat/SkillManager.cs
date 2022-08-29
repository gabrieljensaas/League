using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public string version = "12.16.1";

    public List<PassiveList> passives;
    public List<SkillList> qSkills;
    public List<SkillList> wSkills;
    public List<SkillList> eSkills;
    public List<SkillList> rSkills;

    #region Singleton
    private static SkillManager _instance;
    public static SkillManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }
    #endregion
    private void Start()
    {
        LoadSkill(Resources.LoadAll($"Skills/{version}/Q", typeof(SkillList)), qSkills);
        LoadSkill(Resources.LoadAll($"Skills/{version}/W", typeof(SkillList)), wSkills);
        LoadSkill(Resources.LoadAll($"Skills/{version}/E", typeof(SkillList)), eSkills);
        LoadSkill(Resources.LoadAll($"Skills/{version}/R", typeof(SkillList)), rSkills);
        foreach (PassiveList skill in Resources.LoadAll($"Skills/{version}/P", typeof(PassiveList)))
            passives.Add(skill);

        static void LoadSkill(Object[] resources, List<SkillList> skills)
        {
            foreach (SkillList skill in resources)
                skills.Add(skill);
        }
    }
}
