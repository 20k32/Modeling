using Modeling.Core.Dialogs;
using System.Threading.Tasks;

namespace Modeling.Core.Abstractions.Dialogs
{
    public interface IParametrizedDialog<T> : IClosableDialog
    {
        ParametrizedDialogResult<T> DialogResult { get; }
    }
}
