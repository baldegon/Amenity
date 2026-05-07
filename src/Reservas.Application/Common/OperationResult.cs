namespace Reservas.Application.Common;

public class OperationResult<T>
{
    private OperationResult(bool isSuccess, T? value, string? errorMessage, OperationErrorType? errorType)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public bool IsSuccess { get; }

    public T? Value { get; }

    public string? ErrorMessage { get; }

    public OperationErrorType? ErrorType { get; }

    public static OperationResult<T> Success(T value) => new(true, value, null, null);

    public static OperationResult<T> Failure(string errorMessage, OperationErrorType errorType = OperationErrorType.Validation) => new(false, default, errorMessage, errorType);

    public static OperationResult<T> NotFound(string errorMessage) => new(false, default, errorMessage, OperationErrorType.NotFound);
}

public enum OperationErrorType
{
    Validation = 1,
    NotFound = 2
}
