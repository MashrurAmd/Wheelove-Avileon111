using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct QuesAnswer
{
    public string questions;
    public List<string> options;
    public string answers;
}

[Serializable]
public class TestData
{
    public string testsName;
    public List<QuesAnswer> quesAnswers;
}

[CreateAssetMenu(fileName = "QuestionData", menuName = "Question/QuesData")]

public class QuesData : ScriptableObject
{
    public List<TestData> tests;
}
