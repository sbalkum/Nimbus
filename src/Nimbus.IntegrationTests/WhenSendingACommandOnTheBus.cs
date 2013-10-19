﻿using System;
using System.Threading;
using NSubstitute;

namespace Nimbus.IntegrationTests
{
    public class WhenSendingACommandOnTheBus : SpecificationFor<Bus>
    {
        private ICommandBroker _commandBroker;
        private IRequestBroker _requestBroker;
        private IEventBroker _eventBroker;

        public override Bus Given()
        {
            _commandBroker = Substitute.For<ICommandBroker>();
            _requestBroker = Substitute.For<IRequestBroker>();
            _eventBroker = Substitute.For<IEventBroker>();

            var bus = new BusBuilder(CommonResources.ConnectionString,
                                     _commandBroker,
                                     _requestBroker,
                                     _eventBroker,
                                     new[] {typeof (SomeCommand)},
                                     new[] {typeof (SomeRequest)},
                                     new[] {typeof (SomeEvent)})
                .Build();
            bus.Start();
            return bus;
        }

        public override void When()
        {
            var someCommand = new SomeCommand();
            Subject.Send(someCommand).Wait();
            Thread.Sleep(TimeSpan.FromSeconds(1));

            Subject.Stop();
        }

        [Then]
        public void SomethingShouldHappen()
        {
            _commandBroker.Received().Dispatch(Arg.Any<SomeCommand>());
        }
    }
}