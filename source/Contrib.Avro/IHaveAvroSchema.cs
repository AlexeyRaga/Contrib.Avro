using Avro;

namespace Contrib.Avro;

public interface IHaveAvroSchema
{
    static abstract Schema Schema { get; }
}
