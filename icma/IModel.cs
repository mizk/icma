using libicma.options;

namespace icma
{
    public interface IModel:ICloneable
    {
        string ID { get; }
       

        bool IsValid { get; }
    }
}
