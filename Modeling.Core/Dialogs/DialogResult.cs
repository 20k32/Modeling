using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core.Dialogs
{
    public class DialogResult(DialogAction action)
    {
        public static readonly DialogResult Default = new(DialogAction.None);

        public readonly DialogAction Action = action;
    }
}
