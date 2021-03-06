using Content.Server.GameObjects.EntitySystems;
using Content.Shared.GameObjects.Components.Research;
using Content.Shared.Research;
using Robust.Server.GameObjects.Components.UserInterface;
using Robust.Server.Interfaces.GameObjects;
using Robust.Server.Interfaces.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Server.GameObjects.Components.Research
{
    [RegisterComponent]
    [ComponentReference(typeof(IActivate))]
    public class ResearchConsoleComponent : SharedResearchConsoleComponent, IActivate
    {
        private BoundUserInterface _userInterface;
        private ResearchClientComponent _client;
        public override void Initialize()
        {
            base.Initialize();
            _userInterface = Owner.GetComponent<ServerUserInterfaceComponent>().GetBoundUserInterface(ResearchConsoleUiKey.Key);
            _userInterface.OnReceiveMessage += UserInterfaceOnOnReceiveMessage;
            _client = Owner.GetComponent<ResearchClientComponent>();
        }

        private void UserInterfaceOnOnReceiveMessage(ServerBoundUserInterfaceMessage message)
        {
            if (!Owner.TryGetComponent(out TechnologyDatabaseComponent database)) return;

            switch (message.Message)
            {
                case ConsoleUnlockTechnologyMessage msg:
                    var protoMan = IoCManager.Resolve<IPrototypeManager>();
                    if (!protoMan.TryIndex(msg.Id, out TechnologyPrototype tech)) break;
                    if(!_client.Server.CanUnlockTechnology(tech)) break;
                    if (_client.Server.UnlockTechnology(tech))
                    {
                        database.SyncWithServer();
                        database.Dirty();
                        UpdateUserInterface();
                    }

                    break;

                case ConsoleServerSyncMessage msg:
                    database.SyncWithServer();
                    UpdateUserInterface();
                    break;

                case ConsoleServerSelectionMessage msg:
                    if (!Owner.TryGetComponent(out ResearchClientComponent client)) break;
                    client.OpenUserInterface(message.Session);
                    break;
            }
        }

        /// <summary>
        ///     Method to update the user interface on the clients.
        /// </summary>
        public void UpdateUserInterface()
        {
            _userInterface.SetState(GetNewUiState());
        }

        private ResearchConsoleBoundInterfaceState GetNewUiState()
        {
            var points = _client.ConnectedToServer ? _client.Server.Point : 0;
            var pointsPerSecond = _client.ConnectedToServer ? _client.Server.PointsPerSecond : 0;

            return new ResearchConsoleBoundInterfaceState(points, pointsPerSecond);
        }

        /// <summary>
        ///     Open the user interface on a certain player session.
        /// </summary>
        /// <param name="session">Session where the UI will be shown</param>
        public void OpenUserInterface(IPlayerSession session)
        {
            _userInterface.Open(session);
        }

        void IActivate.Activate(ActivateEventArgs eventArgs)
        {
            if (!eventArgs.User.TryGetComponent(out IActorComponent actor))
                return;

            OpenUserInterface(actor.playerSession);
            return;
        }
    }
}
