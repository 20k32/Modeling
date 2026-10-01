namespace Modeling.Core.Abstractions.Converters
{
    public interface IParametrizedConverter<Tsource, Tdestination, Tparameter>
    {
        Tdestination Convert(Tsource source, Tparameter parameter);
        Tsource ConvertBack(Tdestination destination, Tparameter parameter);
    }
}
