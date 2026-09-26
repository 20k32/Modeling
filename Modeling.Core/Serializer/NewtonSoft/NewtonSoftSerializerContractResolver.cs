using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Modeling.Core.Settings;
using Newtonsoft.Json.Serialization;
using System;

namespace Modeling.Core.Serializer.NewtonSoft
{
    sealed class NewtonSoftSerializerContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (objectType == typeof(IDrawingSettings))
            {
                contract.Converter = Ioc.Default.GetRequiredService<DrawingSettingsJsonConverter>();
            }

            return contract;
        }
    }
}
