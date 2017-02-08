using System;

namespace CommonCode.Drawing
{
    /// <summary>
    /// A StateEdge defines a condition that will cause a state machine to change states.
    /// </summary>
    /// <returns>A number representing which state to switch to.</returns>
    public delegate int StateEdge();

    public class AnimationStateMachine
    {
        State[] states;
        int currentState;

        //A state contains an animation and a list of conditions that will cause the machine to change states.
        struct State
        {
            public string Name;
            public Animation Animation;
            public StateEdge[] Edges;

            public State(string name, Animation anim, StateEdge[] edges)
            {
                Name = name;
                Animation = anim;
                if (edges != null)
                    Edges = edges;
                else
                    Edges = new StateEdge[0];
            }
        }

        public AnimationStateMachine(string[] names, Animation[] animations, StateEdge[][] edges)
        {
            if (animations.Length != edges.Length || animations.Length != names.Length)
                throw new ArgumentException("The number of names, animations, and edge groups must match");

            states = new State[animations.Length];
            for (int i = 0; i < states.Length; i++)
            {
                states[i] = new State(null, animations[i], edges[i]);
            }
        }

        /// <summary>
        /// Returns the current state's animation.
        /// </summary>
        public Animation GetAnimation()
        {
            return states[currentState].Animation;
        }

        /// <summary>
        /// Sets the current state of the machine.
        /// </summary>
        /// <param name="targetState">The name of the state the machine should be set to.</param>
        /// <returns>Boolean value representing whether or not the named state was found.</returns>
        public bool SetState(string targetState)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].Name == targetState)
                {
                    currentState = i;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the name of the current state.
        /// </summary>
        public string GetState() { return states[currentState].Name; }

        /// <summary>
        /// Causes the machine to check for changes in state.  Does not update internal animations.
        /// </summary>
        public void UpdateMachine()
        {
            foreach (StateEdge edge in states[currentState].Edges)
            {
                int result = edge.Invoke();
                if (result != -1)
                {
                    currentState = result;
                    break;
                }
            }
        }
    }
}
