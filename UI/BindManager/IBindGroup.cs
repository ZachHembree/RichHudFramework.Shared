﻿using System.Collections.Generic;
using VRage;
using BindDefinitionData = VRage.MyTuple<string, string[]>;

namespace RichHudFramework
{
    namespace UI
    {
        // <summary>
        /// A collection of unique keybinds.
        /// </summary>
        public interface IBindGroup : IReadOnlyList<IBind>
        {
            /// <summary>
            /// Returns the bind with the name given, if it exists.
            /// </summary>
            IBind this[string name] { get; }

            /// <summary>
            /// Bind group name
            /// </summary>
            string Name { get; }

            /// <summary>
            /// Index of the bind group in its associated client
            /// </summary>
            int Index { get; }

            /// <summary>
            /// Unique identifer
            /// </summary>
            object ID { get; }

            /// <summary>
            /// Returns true if the group contains a bind with the given name.
            /// </summary>
            bool DoesBindExist(string name);

            /// <summary>
            /// Returns true if the specified alias conflicts with other combos
            /// </summary>
            bool DoesComboConflict(IBind bind, int alias = 0);

            /// <summary>
            /// Returns true if the given list of controls conflicts with any existing combos
            /// </summary>
            bool DoesComboConflict(IReadOnlyList<IControl> newCombo, IBind currentBind = null, int alias = 0);

            /// <summary>
            /// Determines if given combo is equivalent to any existing combos
            /// </summary>
            bool DoesComboConflict(IReadOnlyList<int> newCombo, IBind currentBind = null, int alias = 0);

            /// <summary>
            /// Attempts to load bind combinations from bind data. Will not register new binds.
            /// </summary>
            bool TryLoadBindData(IReadOnlyList<BindDefinition> bindData);

            /// <summary>
            /// Registers a list of binds using the names given paired with associated control indices.
            /// </summary>
            void RegisterBinds(BindGroupInitializer bindData);

            /// <summary>
            /// Registers a list of binds using the names given.
            /// </summary>
            void RegisterBinds(IReadOnlyList<string> bindNames);

            /// <summary>
            /// Returns the bind with the name given, if it exists.
            /// </summary>
            IBind GetBind(string name);

            /// <summary>
            /// Adds a bind with the given name and the given key combo. Throws an exception if the bind is invalid.
            /// </summary>
            IBind AddBind(string bindName, IReadOnlyList<ControlHandle> combo, IReadOnlyList<IReadOnlyList<ControlHandle>> aliases = null);

            /// <summary>
            /// Adds a bind with the given name and the given key combo. Throws an exception if the bind is invalid.
            /// </summary>
            IBind AddBind(string bindName, IReadOnlyList<int> combo, IReadOnlyList<IReadOnlyList<int>> aliases = null);

            /// <summary>
            /// Tries to register an empty bind using the given name.
            /// </summary>
            bool TryRegisterBind(string bindName, out IBind newBind);

            /// <summary>
            /// Tries to register a bind using the given name and the given key combo.
            /// </summary>
            bool TryRegisterBind(string bindName, out IBind newBind, IReadOnlyList<int> combo, IReadOnlyList<IReadOnlyList<int>> aliases = null);

            /// <summary>
            /// Tries to register a bind using the given name and the given key combo.
            /// </summary>
            bool TryRegisterBind(string bindName, out IBind newBind, IReadOnlyList<ControlHandle> combo, IReadOnlyList<IReadOnlyList<ControlHandle>> aliases = null);

            /// <summary>
            /// Retrieves the set of key binds as an array of BindDefinition
            /// </summary>
            BindDefinition[] GetBindDefinitions();

            /// <summary>
            /// Retrieves the set of key binds as an array of BindDefinition
            /// </summary>
            BindDefinitionData[] GetBindData();

            /// <summary>
            /// Clears bind subscribers for the entire group
            /// </summary>
            void ClearSubscribers();
        }

        public enum BindGroupAccessors : int
        {
            /// <summary>
            /// out: string
            /// </summary>
            Name = 1,

            /// <summary>
            /// object
            /// </summary>
            ID = 2,

            /// <summary>
            /// in: MyTuple{IReadOnlyList{int}, int}, out: bool
            /// </summary>
            DoesComboConflict = 3,

            /// <summary>
            /// in: string, out: int
            /// </summary>
            TryRegisterBindName = 4,

            /// <summary>
            /// in:  MyTuple{string, int[]}, out: int
            /// </summary>
            TryRegisterBindWithIndices = 5,

            /// <summary>
            /// in: MyTuple{string, string[]}, out: int
            /// </summary>
            TryRegisterBindWithNames = 6,

            /// <summary>
            /// in: IReadOnlyList{BindDefinitionData}, Out: BindMembers[]
            /// </summary>
            TryLoadBindData = 7,

            /// <summary>
            /// out: BindDefinitionData[]
            /// </summary>
            GetBindData = 8,

            /// <summary>
            /// in: int, out: bool
            /// </summary>
            DoesBindExist = 9,

            /// <summary>
            /// in: string, out: Vector2I
            /// </summary>
            GetBindFromName = 10,

            /// <summary>
            /// in: IReadOnlyList{string}
            /// </summary>
            RegisterBindNames = 11,

            /// <summary>
            /// in: IReadOnlyList{MyTuple{string, IReadOnlyList{int}}}
            /// </summary>
            RegisterBindIndices = 12,

            /// <summary>
            /// in: IReadOnlyList{BindDefinitionData}
            /// </summary>
            RegisterBindDefinitions = 13,

            /// <summary>
            /// in: MyTuple{string, IReadOnlyList{int}}, out: Vector2I
            /// </summary>
            AddBindWithIndices = 14,

            /// <summary>
            /// in: MyTuple{string, IReadOnlyList{string}}, out: Vector2I
            /// </summary>
            AddBindWithNames = 15,

            /// <summary>
            /// void
            /// </summary>
            ClearSubscribers = 16,
        }
    }
}