using Modeling.Core.Dialogs;
using System.Threading.Tasks;

namespace Modeling.Core.Abstractions.Dialogs
{
    public interface IDialog : IClosableDialog
    {
        DialogResult DialogResult { get; }
    }
}
