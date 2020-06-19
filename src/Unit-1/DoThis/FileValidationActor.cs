using System.IO;
using Akka.Actor;

namespace WinTail
{
    public class FileValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;
        
        public FileValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var stringMessage = message as string;

            if (string.IsNullOrEmpty(stringMessage))
            {
                _consoleWriterActor.Tell(new Messages.NullInputError("No input received."));
            }
            else
            {
                if (IsFileUri(stringMessage))
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess($"Starting processing for {stringMessage}"));
                    Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor").Tell(new TailCoordinatorActor.StartTail(stringMessage, _consoleWriterActor));
                    return;
                }

                _consoleWriterActor.Tell(new Messages.ValidationError($"{stringMessage} is not an existing URI on disk."));
            }
            
            Sender.Tell(new Messages.ContinueProcessing());
        }

        private static bool IsFileUri(string path) => File.Exists(path);
    }
}