using Hedgehog;
using Hedgehog.Linq;
using Gen = Hedgehog.Linq.Gen;
using Range = Hedgehog.Linq.Range;

namespace Contrib.Avro.CodeGen.Tests;

public static class G
{
    public static Gen<int> Int32 => Gen.Int32(Range.LinearBoundedInt32());
    public static Gen<string> String => Gen.Alpha.String(Range.LinearInt32(0, 100));

    public static Gen<IList<T>> IList<T>(this Gen<T> valueGen, Range<int> range) =>
        valueGen.Enumerable(range).Select(x => (IList<T>)x.ToList());

    public static Gen<IList<T>> IList<T>(this Gen<T> valueGen, int minItems = 0, int maxItems = 5) =>
        valueGen.IList(Range.LinearInt32(minItems, maxItems));

    public static Gen<Choice<T1, T2>> ChoiceGen<T1, T2>(Gen<T1> leftGen, Gen<T2> rightGen) =>
        Gen.Choice([
            leftGen.Select(Choice.FromValue<T1, T2>),
            rightGen.Select(Choice.FromValue<T1, T2>)
        ]);

    public static Gen<Choice<T1, T2, T3>> ChoiceGen<T1, T2, T3>(Gen<T1> gen1, Gen<T2> gen2, Gen<T3> gen3) =>
        Gen.Choice([
            gen1.Select(Choice.FromValue<T1, T2, T3>),
            gen2.Select(Choice.FromValue<T1, T2, T3>),
            gen3.Select(Choice.FromValue<T1, T2, T3>)
        ]);

    public static Gen<IDictionary<string, T>> Dictionary<T>(this Gen<T> valueGen) =>
        valueGen
            .Enumerable(Range.Constant(0, 5))
            .Select(xs => xs.Select((x, i) => (Item: x, Index: i))
                .ToDictionary(x => x.Index.ToString(), x => x.Item))
                .Select(x => (IDictionary<string, T>)x);

}
