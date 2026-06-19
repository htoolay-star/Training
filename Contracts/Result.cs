namespace Contracts
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public bool IsError => !IsSuccess;

        public string Code { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Target { get; set; } = string.Empty;

        public EnumRespType Type { get; set; }

        public T Data { get; set; } = default!;

        public EnumRespType GetEnumRespType() => Type;

        public static Result<T> Success(T data, string message = "Success") =>
            new() { IsSuccess = true, Type = EnumRespType.Success, Data = data, Message = message };

        public static Result<T> Success(string message = "Success") =>
            new() { IsSuccess = true, Type = EnumRespType.Success, Message = message };

        public static Result<T> Error(string message = "Some error occurred.", T? data = default) =>
            new() { IsSuccess = false, Data = data!, Message = message, Type = EnumRespType.Error };

        public static Result<T> ValidationError(string message, T? data = default) =>
            new() { IsSuccess = false, Data = data!, Message = message, Type = EnumRespType.ValidationError };

        public static Result<T> SystemError(string message, T? data = default) =>
            new() { IsSuccess = false, Data = data!, Message = message, Type = EnumRespType.SystemError };

        public static Result<T> NotFoundError(string message = "Not found.", T? data = default) =>
            new() { IsSuccess = false, Data = data!, Message = message, Type = EnumRespType.NotFound };

        public static Result<T> DuplicateRecordError(string message = "Duplicate record.", T? data = default) =>
            new() { IsSuccess = false, Data = data!, Message = message, Type = EnumRespType.DuplicateRecord };

        public static Result<T> InvalidDataError(string message = "Invalid data.", T? data = default) =>
            new() { IsSuccess = false, Data = data!, Message = message, Type = EnumRespType.InvalidData };

        public static Result<T> BadRequestError(string message = "User input error.", T? data = default) =>
            new() { IsSuccess = false, Data = data!, Message = message, Type = EnumRespType.BadRequest };

        public static Result<T> SetResponse(string code, EnumRespType type, T? data = default) =>
            new()
            {
                IsSuccess = type == EnumRespType.Success,
                Code = code,
                Type = type,
                Data = data!
            };
    }

    public enum EnumRespType
    {
        None,
        Success,
        Error,
        ValidationError,
        SystemError,
        NotFound,
        DuplicateRecord,
        InvalidData,
        BadRequest,
        Warning
    }
}
