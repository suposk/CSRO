using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.UI.Helpers
{
    public static class EditFormExtensions
    {
        /// <summary>
        /// Clears all validation messages from the <see cref="EditContext"/> of the given <see cref="EditForm"/>.
        /// </summary>
        /// <param name="editForm">The <see cref="EditForm"/> to use.</param>
        /// <param name="revalidate">
        /// Specifies whether the <see cref="EditContext"/> of the given <see cref="EditForm"/> should revalidate after all validation messages have been cleared.
        /// </param>
        /// <param name="markAsUnmodified">
        /// Specifies whether the <see cref="EditContext"/> of the given <see cref="EditForm"/> should be marked as unmodified.
        /// This will affect the assignment of css classes to a form's input controls in Blazor.
        /// </param>
        /// <remarks>
        /// This extension method should be on EditContext, but EditForm is being used until the fix for issue
        /// <see href="https://github.com/dotnet/aspnetcore/issues/12238"/> is officially released.
        /// </remarks>
        public static void ClearValidationMessages(this EditContext editContext, bool revalidate = false, bool markAsUnmodified = false)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            object GetInstanceField(Type type, object instance, string fieldName)
            {
                var fieldInfo = type.GetField(fieldName, bindingFlags);
                return fieldInfo.GetValue(instance);
            }

            //var editContext = editForm.EditContext == null
            //    ? GetInstanceField(typeof(EditForm), editForm, "_fixedEditContext") as EditContext
            //    : editForm.EditContext;

            var fieldStates = GetInstanceField(typeof(EditContext), editContext, "_fieldStates");
            var clearMethodInfo = typeof(HashSet<ValidationMessageStore>).GetMethod("Clear", bindingFlags);

            foreach (DictionaryEntry kv in (IDictionary)fieldStates)
            {
                var messageStores = GetInstanceField(kv.Value.GetType(), kv.Value, "_validationMessageStores");
                clearMethodInfo.Invoke(messageStores, null);
            }

            if (markAsUnmodified)
                editContext.MarkAsUnmodified();

            if (revalidate)
                editContext.Validate();
        }
    }
}
