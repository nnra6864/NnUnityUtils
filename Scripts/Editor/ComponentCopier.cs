// https://gist.github.com/adammyhre/705c0eb2bc83de7c1b8639fb6a79305d
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

/// <summary>
/// A Unity editor extension for copying and pasting all components between GameObjects.
/// The tool supports handling multiple components of the same type and correctly restores
/// their data using the Unity Undo system. It provides menu items to copy components
/// from the currently selected GameObject and paste them onto other selected GameObjects.
/// </summary>
public class ComponentsCopier {
    static Component[] copiedComponents;
    static Dictionary<Type, int> componentCounters = new Dictionary<Type, int>();

    [MenuItem("GameObject/TransferComponents/Copy %#&C")]
    static void Copy() {
        var activeObject = Selection.activeGameObject;
        if (activeObject == null) return;

        copiedComponents = activeObject.GetComponents<Component>();
        componentCounters.Clear();
    }

    [MenuItem("GameObject/TransferComponents/Paste %#&P")]
    static void Paste() {
        if (copiedComponents == null) {
            Debug.LogError("Nothing copied!"); 
            return;
        }

        foreach (var target in Selection.gameObjects) {
            if (!target) continue;

            Undo.RegisterCompleteObjectUndo(target, $"{target.name}: Paste All Components");

            foreach (var copied in copiedComponents) {
                if (!copied) continue;

                ComponentUtility.CopyComponent(copied);
                Type componentType = copied.GetType();
                var targetComponents = target.GetComponents(componentType);

                if (targetComponents.Length > 0 && componentCounters.ContainsKey(componentType)) {
                    componentCounters[componentType]++;
                } else {
                    componentCounters[componentType] = 0;
                }

                int index = componentCounters[componentType];
                if (targetComponents.Length - index > 0) {
                    var targetComponent = targetComponents[index];
                    Undo.RecordObject(targetComponent, $"Paste {componentType} Values");
                    PasteComponent(() => ComponentUtility.PasteComponentValues(targetComponent), componentType);
                } else {
                    PasteComponent(() => ComponentUtility.PasteComponentAsNew(target), componentType);
                }
            }
        }

        copiedComponents = null;
    }

    static void PasteComponent(Func<bool> pasteAction, Type componentType) {
        if (pasteAction()) {
            Debug.Log($"Successfully pasted: {componentType}");
        } else {
            Debug.LogError($"Failed to copy: {componentType}");
        }
    }
}
#endif