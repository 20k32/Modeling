using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Models.Settings
{
    public sealed class RefreshRateItem(int value)
    {
        public int Value { get; } = value;
    }
}
