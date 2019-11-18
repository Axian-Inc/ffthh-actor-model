using Akka.Actor;
using Xunit;

namespace Axian.ActorModel.Lesson0
{
    public class GreeterSpec : AxTestKit
    {
        public GreeterSpec(AxFixture fixture) : base(fixture)
        {
        }

        // TODO: Verify the Greeter responds
        
        [Fact]
        public void UntypedGreeter_should_always_respond_hello()
        {
            // Arrange
            Props props = Props.Create<UntypedGreeter>();
            IActorRef greeter = Sys.ActorOf(props);

            // Act
            greeter.Tell("hi!");

            // Assert
            string response = ExpectMsg<string>();
            Assert.Equal("hello", response);
        }

        [Fact]
        public void ReceiveGreeter_should_always_respond_hello()
        {
            // Arrange
            Props props = Props.Create<ReceiveGreeter>();
            IActorRef greeter = Sys.ActorOf(props);

            // Act
            greeter.Tell("hi!");

            // Assert
            string response = ExpectMsg<string>();
            Assert.Equal("hello", response);
        }
    }
}
