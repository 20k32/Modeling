using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Modeling.Core.Constants;
using Modeling.Core.Dispatching;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Core.Messages.Canvas.Settings;
using Modeling.Core.Messages.Settings;
using Modeling.Core.Miscellaneous;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.ViewModels
{
    public sealed partial class DrawingViewModel : ObservableObject
    {
        [RelayCommand]
        void Initialize()
        {
            Logger.LoadedInformation("Main page");

            InitializeCanvas();

            InitializeDrawingSession();

            InitializeSettings();
        }

        [RelayCommand]
        void ClearWorkingArea()
        {
            ClearCanvas();
        }

        [RelayCommand]
        async Task InitializeCanvasAsync()
        {
            ClearCanvas();
            await LoadSettingsAsync();
        }
        void InitializeCanvas()
        {
            WeakReferenceMessenger.Default.Send(new InitializeCanvasControlMessage(this));
        }

        void InitializeDrawingSession()
        {
            WeakReferenceMessenger.Default.Send(new InitializeDrawingSessionMessage(this));
        }

        void ClearCanvas()
        {
            WeakReferenceMessenger.Default.Send(new ClearCanvasMessage(this, DrawingConstants.DEFAULT_DRAWING_COLOR));
        }

        void InitializeSettings()
        {
            WeakReferenceMessenger.Default.Send(new InitializeSettingsMessage(this));
        }

        async Task LoadSettingsAsync()
        {
            await WeakReferenceMessenger.Default.Send(new LoadSettingsAsyncMessage(this));
        }
    }
}
