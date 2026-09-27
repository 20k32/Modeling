using System.Threading.Tasks;

namespace Modeling.Core.Abstractions.Dialogs
{
    public interface IClosableDialog
    {
        void Close();
        Task CloseAsync();

        void Show();
        Task ShowAsync();
    }
}
