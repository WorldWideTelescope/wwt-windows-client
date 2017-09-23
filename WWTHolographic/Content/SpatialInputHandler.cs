using Windows.UI.Input.Spatial;

namespace WWTHolographic.Common
{
    // Sample gesture handler.
    // Hooks up events to recognize a tap gesture, and keeps track of input using a boolean value.
    public class SpatialInputHandler
    {
        // API objects used to process gesture input, and generate gesture events.
        private SpatialInteractionManager interactionManager;

        // Used to indicate that a Pressed input event was received this frame.
        private SpatialInteractionSourceState sourceState;

        private SpatialInteractionSource LastController;
        public SpatialInteractionSource LeftController;
        public SpatialInteractionSource RightController;

        // Creates and initializes a GestureRecognizer that listens to a Person.
        public SpatialInputHandler()
        {
            // The interaction manager provides an event that informs the app when
            // spatial interactions are detected.
            interactionManager = SpatialInteractionManager.GetForCurrentView();

            // Bind a handler to the SourcePressed event.
            interactionManager.SourcePressed += this.OnSourcePressed;
            interactionManager.SourceDetected += InteractionManager_SourceDetected;
            interactionManager.SourceLost += InteractionManager_SourceLost;
            interactionManager.InteractionDetected += InteractionManager_InteractionDetected;
            //
            // TODO: Expand this class to use other gesture-based input events as applicable to
            //       your app.
            //
        }

        private void InteractionManager_InteractionDetected(SpatialInteractionManager sender, SpatialInteractionDetectedEventArgs args)
        {
            int x = 1;
        }

        private void InteractionManager_SourceLost(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            LastController = args.State.Source;

            if (LastController.Handedness == SpatialInteractionSourceHandedness.Left)
            {
                LeftController = null;
            }

            if (LastController.Handedness == SpatialInteractionSourceHandedness.Right)
            {
                RightController = null;
            }
        }

        private void InteractionManager_SourceDetected(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            LastController = args.State.Source;

            if (LastController.Handedness == SpatialInteractionSourceHandedness.Left)
            {
                LeftController = LastController;
            }

            if (LastController.Handedness == SpatialInteractionSourceHandedness.Right)
            {
                RightController = LastController;
            }
        }
        // Checks if the user performed an input gesture since the last call to this method.
        // Allows the main update loop to check for asynchronous changes to the user
        // input state.
        public SpatialInteractionSourceState CheckForInput()
        {
            SpatialInteractionSourceState sourceState = this.sourceState;
            this.sourceState = null;
            return sourceState;
        }

        public void OnSourcePressed(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            sourceState = args.State;

            //
            // TODO: In your app or game engine, rewrite this method to queue
            //       input events in your input class or event handler.
            //
        }

        
    }
}