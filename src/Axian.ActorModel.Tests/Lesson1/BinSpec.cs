using Akka.Actor;
using Akka.Configuration;
using System;
using Xunit;

namespace Axian.ActorModel.Lesson1
{
    public class BinSpec : AxTestKit
    {
        public BinSpec(AxFixture fixture) 
            : base(fixture)
        {
        }

        private IActorRef DefaultBin()
        {
            return Sys.ActorOf(Props.Create(() => new Bin()));
        }

        [Fact]
        public void _00_should_be_able_to_Put_items_in_the_bin()
        {
            IActorRef bin = DefaultBin();
            bin.Tell(new Bin.Put("any immutable object"));
            ExpectMsg<Status.Success>();
        }

        [Fact]
        public void _01_should_list_items_stored_in_the_bin()
        {
            IActorRef bin = DefaultBin();
            bin.Tell(new Bin.Put("any immutable object"));
            ExpectMsg<Status.Success>();

            bin.Tell(new Bin.List());

            var response = ExpectMsg<Bin.ListResponse>();
            Assert.Contains("any immutable object", response.Items);
        }

        [Fact]
        public void _02_should_list_items_in_reverse_chronological_order()
        {
            IActorRef bin = DefaultBin();

            bin.Tell(new Bin.Put("item1"));
            ExpectMsg<Status.Success>();

            bin.Tell(new Bin.Put("item2"));
            ExpectMsg<Status.Success>();

            bin.Tell(new Bin.Put("item3"));
            ExpectMsg<Status.Success>();

            bin.Tell(new Bin.List());
            var response = ExpectMsg<Bin.ListResponse>();

            Assert.Equal("item3", response.Items[0]);
            Assert.Equal("item2", response.Items[1]);
            Assert.Equal("item1", response.Items[2]);
        }

        [Fact]
        public void _03_should_be_able_to_configure_the_maximum_number_of_stored_items()
        {
            Config config = ConfigurationFactory.ParseString("axian.bin.max-items = 2");

            IActorRef bin = Sys.ActorOf(Props.Create(() => new Bin(config)));

            bin.Tell(new Bin.Put("item1"));
            ExpectMsg<Status.Success>();

            bin.Tell(new Bin.Put("item2"));
            ExpectMsg<Status.Success>();

            bin.Tell(new Bin.Put("item3"));
            ExpectMsg<Status.Success>();

            bin.Tell(new Bin.List());
            var response = ExpectMsg<Bin.ListResponse>();

            Assert.Equal(2, response.Items.Count);
            Assert.DoesNotContain("item1", response.Items);
        }

       
        [Fact]
        public void _04_should_be_able_to_configure_how_long_the_bin_stays_alive()
        {
            // Configure the bin to self terminate in 48 hours
            Config config = ConfigurationFactory.ParseString("axian.bin.ttl = 48h");

            IActorRef bin = Sys.ActorOf(Props.Create(() => new Bin(config)));

            // Watch for Terminated messages
            Watch(bin);

            // Advance the scheduler's clock by 1 day
            Scheduler.Advance(TimeSpan.FromDays(1));

            // Should still accept puts
            bin.Tell(new Bin.Put("item"));
            ExpectMsg<Status.Success>();

            // Advance the scheduler's clock by 1 day
            Scheduler.Advance(TimeSpan.FromDays(1));
            ExpectTerminated(bin);
        }
    }
}
