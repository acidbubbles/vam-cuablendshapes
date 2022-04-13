using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapes : MVRScript
{
    private Transform _container;
    private List<JSONStorableFloat> _storables = new List<JSONStorableFloat>();

    public override void Init()
    {
        CreateButton("Refresh BlendShapes", true).button.onClick.AddListener(RefreshBlendShapes);

        _container = containingAtom.transform.Find("reParentObject/object/rescaleObject");

        SuperController.singleton.StartCoroutine(WaitForCUA());
    }

    private IEnumerator WaitForCUA()
    {
        yield return new WaitForEndOfFrame();

        while(this != null && !TryRegisterCUA())
        {
            yield return 0;
        }
    }

    private bool TryRegisterCUA()
    {
        var renderer = containingAtom.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer == null)
        {
            // SuperController.LogError("The atom '" + containingAtom.name + "' does not have any SkinnedMeshRenderer");
            return false;
        }

        var mesh = renderer.sharedMesh;
        if (mesh == null || mesh.blendShapeCount == 0)
        {
            // SuperController.LogError("The atom '" + containingAtom.name + "' does not have any blend shapes");
            return false;
        }

        var blendShapeIndex = 0;
        do
        {
            var localBlendShapeIndex = blendShapeIndex;
            var storable = new JSONStorableFloat(
                mesh.GetBlendShapeName(blendShapeIndex),
                renderer.GetBlendShapeWeight(blendShapeIndex),
                new JSONStorableFloat.SetFloatCallback(val => {
                    renderer.SetBlendShapeWeight(localBlendShapeIndex, val);
                }),
                0f,
                100f,
                false
            );
            RegisterFloat(storable);
            CreateSlider(storable);
            _storables.Add(storable);
        }
        while (++blendShapeIndex < renderer.sharedMesh.blendShapeCount);

        containingAtom.RestoreFromLast(this);

        return true;
    }

    private void RefreshBlendShapes()
    {
        foreach(var storable in _storables)
        {
            RemoveSlider(storable);
            DeregisterFloat(storable);
        }
        _storables.Clear();

        TryRegisterCUA();
    }
}
