using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASChurchManager.API.Membro.Infra;


public class DateTimeModelBinder : IModelBinder
{
    private static readonly string DateFormat = "yyyy-MM-dd";

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "A data é obrigatória.");
            return Task.CompletedTask;
        }

        if (DateTime.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            bindingContext.Result = ModelBindingResult.Success(date);
        }
        else
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"Formato inválido. Use {DateFormat}.");
        }

        return Task.CompletedTask;
    }
}
