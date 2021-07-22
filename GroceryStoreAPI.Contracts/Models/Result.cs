using GroceryStoreAPI.Contracts.Enums;

namespace GroceryStoreAPI.Contracts.Models
{
    public abstract class Result
    {
        protected Result()
            : this(ResultStatus.Succeeded, "")
        {
        }

        protected Result(ResultStatus status, string errorMessage)
        {
            Status = status;
            ErrorMessage = errorMessage;
        }

        public ResultStatus Status { get; }

        public string ErrorMessage { get; }

        public bool Succeeded()
        {
            return Status == ResultStatus.Succeeded;
        }

        public bool Failed()
        {
            return !Succeeded();
        }

    }

    public sealed class Result<T> : Result
    {
        public Result(T content)
        {
            this.Content = content;
        }

        public Result(ResultStatus status, string errorMessage)
            : base(status, errorMessage)
        {
        }

        public T Content { get; }

        public static Result<T> Errored(ResultStatus status, string errorMessage)
        {
            return new Result<T>(status, errorMessage);
        }

        public static Result<T> Success(T content)
        {
            return new Result<T>(content);
        }

        public static Result<T> Success()
        {
            return new Result<T>(default);
        }

        public static Result<T> NotFound(string errorMessage)
        {
            return new Result<T>(ResultStatus.MissingResource, errorMessage);
        }
    }
}
