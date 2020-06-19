using System;
﻿using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            //var wrongArgumentsActor = MyActorSystem.ActorOf(Props.Create<ValidationActor>());
            //var fakeActor = MyActorSystem.ActorOf(Props.Create(typeof(string)));



            var tailCoordinatorActor = MyActorSystem.ActorOf<TailCoordinatorActor>("tailCoordinatorActor");
            var consoleWriterActor = MyActorSystem.ActorOf(Props.Create<ConsoleWriterActor>(), "consoleWriterActor");
            var validationActor = MyActorSystem.ActorOf(Props.Create(() => new FileValidationActor(consoleWriterActor)), "validationActor");
            var consoleReaderActor = MyActorSystem.ActorOf<ConsoleReaderActor>("consoleReaderActor");

            // tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);
            
            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();
        }
    }
    #endregion
}
