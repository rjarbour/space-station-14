﻿using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Content.Shared.GameObjects.Components.VendingMachines
{
    public class SharedVendingMachineComponent : Component
    {
        public override string Name => "VendingMachine";
        public override uint? NetID => ContentNetIDs.VENDING_MACHINE;

        public List<VendingMachineInventoryEntry> Inventory = new List<VendingMachineInventoryEntry>();

        [Serializable, NetSerializable]
        public enum VendingMachineVisuals
        {
            VisualState,
        }

        [Serializable, NetSerializable]
        public enum VendingMachineVisualState
        {
            Normal,
            Off,
            Broken,
            Eject,
            Deny,
        }

        [Serializable, NetSerializable]
        public class VendingMachineEjectMessage : BoundUserInterfaceMessage
        {
            public readonly string ID;
            public VendingMachineEjectMessage(string id)
            {
                ID = id;
            }
        }

        [Serializable, NetSerializable]
        public enum VendingMachineUiKey
        {
            Key,
        }

        [Serializable, NetSerializable]
        public class InventorySyncRequestMessage : BoundUserInterfaceMessage
        {
        }

        [Serializable, NetSerializable]
        public class VendingMachineInventoryMessage : BoundUserInterfaceMessage
        {
            public readonly List<VendingMachineInventoryEntry> Inventory;
            public VendingMachineInventoryMessage(List<VendingMachineInventoryEntry> inventory)
            {
                Inventory = inventory;
            }
        }

        [Serializable, NetSerializable]
        public class VendingMachineInventoryEntry
        {
            public string ID;
            public uint Amount;
            public VendingMachineInventoryEntry(string id, uint amount)
            {
                ID = id;
                Amount = amount;
            }
        }
    }
}
