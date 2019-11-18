using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Axian.ActorModel.Lesson1
{
    public class BinManagerSpec : AxTestKit
    {
        public BinManagerSpec(AxFixture fixture) : base(fixture)
        {
        }

        private IActorRef DefaultBinManager()
        {
            return Sys.ActorOf(Props.Create<BinManager>());
        }

        [Fact]
        public void _00_CreateBin_should_respond_with_bin_name()
        {
            IActorRef binManager = DefaultBinManager();
            binManager.Tell(new BinManager.CreateBin());
            var response = ExpectMsg<BinManager.CreateBinResponse>();
            Assert.NotNull(response.Bin);
        }

        [Fact]
        public void _01_CreateBin_every_bin_should_have_a_unique_name()
        {
            IActorRef binManager = DefaultBinManager();
            binManager.Tell(new BinManager.CreateBin());
            binManager.Tell(new BinManager.CreateBin());
            binManager.Tell(new BinManager.CreateBin());

            var response1 = ExpectMsg<BinManager.CreateBinResponse>();
            var response2 = ExpectMsg<BinManager.CreateBinResponse>();
            var response3 = ExpectMsg<BinManager.CreateBinResponse>();

            Assert.True(response1.Bin != response2.Bin);
            Assert.True(response2.Bin != response3.Bin);
        }

        [Fact]
        public void _02_ListBins_should_return_the_name_of_all_bins()
        {
            IActorRef binManager = DefaultBinManager();
            binManager.Tell(new BinManager.CreateBin());
            binManager.Tell(new BinManager.CreateBin());

            var response1 = ExpectMsg<BinManager.CreateBinResponse>();
            var response2 = ExpectMsg<BinManager.CreateBinResponse>();

            binManager.Tell(new BinManager.ListBins());
            var listResponse = ExpectMsg<BinManager.ListBinsResponse>();

            Assert.Contains(response1.Bin, listResponse.Bins);
            Assert.Contains(response2.Bin, listResponse.Bins);
        }

        [Fact]
        public void _03_CaptureRequest_should_not_respond_when_bin_doesnt_exist()
        {
            IActorRef binManager = DefaultBinManager();
            binManager.Tell(new BinManager.CaptureRequest("unknown", new object()));
            ExpectNoMsg();
        }
        
        [Fact]
        public void _04_CaptureRequest_should_forward_request_to_the_bin_and_respond_success()
        {
            IActorRef binManager = DefaultBinManager();
            binManager.Tell(new BinManager.CreateBin());
            var createResponse = ExpectMsg<BinManager.CreateBinResponse>();

            binManager.Tell(new BinManager.CaptureRequest(createResponse.Bin, "item"));
            ExpectMsg<Status.Success>();
        }

        [Fact]
        public void _05_ListRequests_should_not_respond_when_bin_doesnt_exist()
        {
            IActorRef binManager = DefaultBinManager();
            binManager.Tell(new BinManager.ListRequests("an-unknown-bin"));
            ExpectNoMsg();
        }

        [Fact]
        public void _06_ListRequests_should_forward_request_to_the_bin()
        {
            IActorRef binManager = DefaultBinManager();
            binManager.Tell(new BinManager.CreateBin());
            var createResponse = ExpectMsg<BinManager.CreateBinResponse>();

            binManager.Tell(new BinManager.ListRequests(createResponse.Bin));
            ExpectMsg<Bin.ListResponse>();
        }

        [Fact]
        public void _07_ListBins_should_not_return_any_expired_bins()
        {

            Config config = ConfigurationFactory.ParseString("axian.bin.ttl = 1m");

            IActorRef binManager = Sys.ActorOf(Props.Create(() => new BinManager(config)));

            binManager.Tell(new BinManager.CreateBin());
            var createResponse = ExpectMsg<BinManager.CreateBinResponse>();

            // advance the clock, causing the bin to time-out, eat a poison pill, 
            Scheduler.Advance(TimeSpan.FromMinutes(5));

            binManager.Tell(new BinManager.ListBins());
            var listResponse = ExpectMsg<BinManager.ListBinsResponse>();

            Assert.DoesNotContain(createResponse.Bin, listResponse.Bins);


        }
    }
}
