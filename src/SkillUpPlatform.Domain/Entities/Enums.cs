namespace SkillUpPlatform.Domain.Entities;

public enum UserRole
{
    Student = 1,
    ContentCreator = 2,
    Admin = 3
}

public enum DifficultyLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3
}

public enum ContentType
{
    Video = 1,
    Document = 2,
    Article = 3,
    Quiz = 4,
    InteractiveContent = 5
}

public enum AssessmentType
{
    SkillAssessment = 1,
    KnowledgeTest = 2,
    PersonalityTest = 3,
    CareerAssessment = 4
}

public enum QuestionType
{
    MultipleChoice = 1,
    TrueFalse = 2,
    ShortAnswer = 3,
    Essay = 4
}

public enum LearningPathStatus
{
    NotStarted = 1,
    InProgress = 2,
    Completed = 3,
    Paused = 4
}

public enum ResourceType
{
    CVTemplate = 1,
    CoverLetterTemplate = 2,
    InterviewQuestions = 3,
    CareerGuide = 4,
    SkillTemplate = 5
}
