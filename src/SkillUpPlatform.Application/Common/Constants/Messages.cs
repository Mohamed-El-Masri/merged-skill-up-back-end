namespace SkillUpPlatform.Application.Common.Constants;

public static class ErrorMessages
{
    public const string UserNotFound = "User not found";
    public const string InvalidCredentials = "Invalid email or password";
    public const string EmailAlreadyExists = "Email already exists";
    public const string LearningPathNotFound = "Learning path not found";
    public const string ContentNotFound = "Content not found";
    public const string AssessmentNotFound = "Assessment not found";
    public const string ResourceNotFound = "Resource not found";
    public const string UnauthorizedAccess = "Unauthorized access";
    public const string ValidationFailed = "Validation failed";
    public const string InternalServerError = "An internal server error occurred";
}

public static class SuccessMessages
{
    public const string UserCreated = "User created successfully";
    public const string UserUpdated = "User updated successfully";
    public const string LoginSuccessful = "Login successful";
    public const string LearningPathCreated = "Learning path created successfully";
    public const string ContentCreated = "Content created successfully";
    public const string AssessmentSubmitted = "Assessment submitted successfully";
    public const string ProgressUpdated = "Progress updated successfully";
}

public static class CacheKeys
{
    public const string UserPrefix = "user:";
    public const string LearningPathPrefix = "learning_path:";
    public const string ContentPrefix = "content:";
    public const string AssessmentPrefix = "assessment:";
    public const string ResourcePrefix = "resource:";
    
    public static string UserKey(int userId) => $"{UserPrefix}{userId}";
    public static string LearningPathKey(int learningPathId) => $"{LearningPathPrefix}{learningPathId}";
    public static string ContentKey(int contentId) => $"{ContentPrefix}{contentId}";
    public static string AssessmentKey(int assessmentId) => $"{AssessmentPrefix}{assessmentId}";
    public static string ResourceKey(int resourceId) => $"{ResourcePrefix}{resourceId}";
}
