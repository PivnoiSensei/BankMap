using FluentValidation;
using MediatR;

namespace BankMap.Application.Common.Behaviors{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;
        //Universal validator handler
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (!_validators.Any()) return await next(ct);

            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, ct)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0) //Build validator response 
            {
                var errorMsg = string.Join(", ", failures.Select(f => f.ErrorMessage));
                //Без рефлексії, повертаємо Result<T> з помилками, які отримали від валідаторів
                //через новий метод прописаний у Result.cs
                return (TResponse)(object)Result<object>.FailureStatic(errorMsg); 
            }

            return await next(ct);
        }
    }
}


