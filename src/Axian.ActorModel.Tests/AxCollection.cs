using Xunit;

namespace Axian.ActorModel
{
    [CollectionDefinition(nameof(AxCollection))]
    public class AxCollection : ICollectionFixture<AxFixture>
    {
        // All test classes which inherit from AxTestKit will shared the
        // test context provided by AxFixture.
    }
}
