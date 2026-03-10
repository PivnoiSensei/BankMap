- [x] Файл `BankMap.WebApi/DepInjection.cs` не використовується. Забув прибрати?
- [x] 2. `ImportBranchesHandler` та `UpdateBranchStatusHandler` викликають `IBranchManager` напряму замість `IBranchService`. Хендлер повинен викликати сервіс.
- [x] 3. Чому DI класи мають назву `IServiceCollection`?
- [x] 4. У проекті `BankMap.WebApi` присутні nuget'и EntityFrameworkCore. Їх там не повинно бути, їх місце в інфраструктурі.
- [x] 5. Усі сутності в одному файлі. Треба розбити на різні файли, один клас - один файл. Те саме стосується `ImportDto` та інших файлів.
- [x] 6. В деякі методи не передається `CancellationToken`. Наприклад в `_dbContext.Branches.FindAsync` та `next`.
- [x] 7. Хендлери повинні бути маленькі, а для мапінгу викликати мапери в сервісі. Сервіс приймає запит, виконує бізнес-логіку і готує результат для відповіді.
- [x] 8. Мені не подобається рефлексія в `ValidationBehavior`.
   ```csharp
   return (TResponse)typeof(TResponse)
       .GetMethod("Failure", new[] { typeof(string) })
       ?.Invoke(null, new object[] { errorMsg })!;
   ```
   Можна додати інтерфейс для `Result` з методом `static abstract TSelf Failure(string error)`. А потім замість рефлексії використати цей метод `TResponse.Failure(errorMsg)` попередньо додавши constraint `TResponse : IResultFailure<TResponse>`.
- [x] 9. `BranchMapper` повинен бути в окремій вкладці, а не в Services.
10. Не треба створювати query/command в контроллері, за виключенням порожніх. У цьому методі command повинен бути в аргументах методу з атрибутом `[FromBody]`
   ```csharp
   public async Task<IActionResult> UpdateBranchStatus(int id, [FromBody] UpdateBranchStatusBody body, CancellationToken ct)
      => await SendAsync(new UpdateBranchStatusCommand(id, body.IsTemporaryClosed), ct);
   ```
11. Також не треба додавати `CancellationToken` в методи контроллеру. Його можна дістати в `ApiControllerBase` з `HttpContext.RequestAborted`.
- [x] 12. Проекти повинні лежати в папці `src/`, як вказано в завданні.
