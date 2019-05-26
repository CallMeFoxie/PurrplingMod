﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PurrplingMod.StateMachine.State;
using StardewValley;

namespace PurrplingMod.StateMachine
{
    public class CompanionStateMachine
    {
        public enum StateName
        {
            RESET,
            AVAILABLE,
            RECRUITED,
            UNAVAILABLE,
        }
        public CompanionManager Manager { get; private set; }
        public NPC Companion { get; private set; }
        public Dictionary<int, ICompanionState> States { get; private set; }
        private ICompanionState currentState;
        public CompanionStateMachine(CompanionManager manager, NPC companion)
        {
            this.Manager = manager;
            this.Companion = companion;
            this.States = new Dictionary<int, ICompanionState>()
            {
                [(int)StateName.RESET] = new ResetState(this),
                [(int)StateName.AVAILABLE] = new AvailableState(this),
            };
            this.ResetStateMachine();
        }

        public string Name
        {
            get
            {
                return this.Companion.Name;
            }
        }

        private void ChangeState(int state)
        {
            if (!this.States.TryGetValue(state, out ICompanionState newState))
                throw new Exception($"Unknown state: {state}");

            if (this.currentState != null)
            {
                this.currentState.Exit();
            }

            newState.Entry();
            this.currentState = newState;
        }

        public void NewDaySetup()
        {
            this.ChangeState((int)StateName.AVAILABLE);
        }

        public void ResetStateMachine()
        {
            this.ChangeState((int)StateName.RESET);
        }

        public void Dispose()
        {
            if (this.currentState != null)
                this.currentState.Exit();

            this.States.Clear();
            this.States = null;
            this.currentState = null;
            this.Companion = null;
            this.Manager = null; 
        }
    }
}