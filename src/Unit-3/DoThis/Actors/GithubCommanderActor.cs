using System;
using System.Linq;
using Akka.Actor;
using Akka.Routing;

namespace GithubActors.Actors
{
    /// <summary>
    /// Top-level actor responsible for coordinating and launching repo-processing jobs
    /// </summary>
    public class GithubCommanderActor : ReceiveActor, IWithUnboundedStash
    {
        #region Message classes

        public class CanAcceptJob
        {
            public CanAcceptJob(RepoKey repo)
            {
                Repo = repo;
            }

            public RepoKey Repo { get; private set; }
        }

        public class AbleToAcceptJob
        {
            public AbleToAcceptJob(RepoKey repo)
            {
                Repo = repo;
            }

            public RepoKey Repo { get; private set; }
        }

        public class UnableToAcceptJob
        {
            public UnableToAcceptJob(RepoKey repo)
            {
                Repo = repo;
            }

            public RepoKey Repo { get; private set; }
        }

        #endregion

        private IActorRef _coordinator;
        private IActorRef _canAcceptJobSender;
        private int _pendingJobReplies;

        public GithubCommanderActor()
        {
            Ready();
        }

        private void Asking()
        {
            Receive<CanAcceptJob>(_ => Stash.Stash());
            
            Receive<UnableToAcceptJob>(job =>
            {
                _pendingJobReplies--;

                if (_pendingJobReplies == 0)
                {
                    _canAcceptJobSender.Tell(job);
                    
                    Become(Ready);
                    Stash.UnstashAll();
                }
            });

            Receive<AbleToAcceptJob>(job =>
            {
                _canAcceptJobSender.Tell(job);

                //start processing messages
                Sender.Tell(new GithubCoordinatorActor.BeginJob(job.Repo));

                //launch the new window to view results of the processing
                Context.ActorSelection(ActorPaths.MainFormActor.Path).Tell(new MainFormActor.LaunchRepoResultsWindow(job.Repo, Sender));
                
                Become(Ready);
                Stash.UnstashAll();
            });
        }

        private void Ready()
        {
            Receive<CanAcceptJob>(job =>
            {
                _coordinator.Tell(job);
                
                _canAcceptJobSender = Sender;
                _pendingJobReplies = 3; // number of routees
                Become(Asking);
            });
        }

        protected override void PreStart()
        {
            var coordinator1 = Context.ActorOf<GithubCoordinatorActor>(ActorPaths.GithubCoordinatorActor.Name + "1");
            var coordinator2 = Context.ActorOf<GithubCoordinatorActor>(ActorPaths.GithubCoordinatorActor.Name + "2");
            var coordinator3 = Context.ActorOf<GithubCoordinatorActor>(ActorPaths.GithubCoordinatorActor.Name + "3");
            
            _coordinator = Context.ActorOf(Props.Empty.WithRouter(new BroadcastGroup(coordinator1.Path.ToString(), coordinator2.Path.ToString(), coordinator3.Path.ToString())));
            
            base.PreStart();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            //kill off the old coordinator so we can recreate it from scratch
            _coordinator.Tell(PoisonPill.Instance);
            base.PreRestart(reason, message);
        }

        public IStash Stash { get; set; }
    }
}
