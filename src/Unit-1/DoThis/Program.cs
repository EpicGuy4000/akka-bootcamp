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

            var tailCoordinatorActor = MyActorSystem.ActorOf<TailCoordinatorActor>("tailCoordinatorActor");
            var consoleWriterActor = MyActorSystem.ActorOf<ConsoleWriterActor>("consoleWriterActor");
            var validationActor = MyActorSystem.ActorOf<FileValidationActor>("validationActor");
            var consoleReaderActor = MyActorSystem.ActorOf<ConsoleReaderActor>("consoleReaderActor");

            // tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);
            
            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();
        }
    }
    #endregion
}
