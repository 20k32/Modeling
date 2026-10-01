namespace Modeling.Core.Abstractions.Converters
{
    public interface IConverter<Tsource, Tdestination>
    {
        Tdestination Convert(Tsource source);
        Tsource ConvertBack(Tdestination destination);
    }
}
