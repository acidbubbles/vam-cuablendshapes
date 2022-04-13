using UnityEngine;

public class BlendShapes : MVRScript
{
    public override void Init()
    {
        var renderer = containingAtom.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer == null)
        {
            SuperController.LogError("The atom '" + containingAtom.name + "' does not have any SkinnedMeshRenderer");
            return;
        }

        var mesh = renderer.sharedMesh;
        if (mesh == null || mesh.blendShapeCount == 0)
        {
            SuperController.LogError("The atom '" + containingAtom.name + "' does not have any blend shapes");
            return;
        }

        var blendShapeIndex = 0;
        do
        {
            var localBlendShapeIndex = blendShapeIndex;
            var storable = new JSONStorableFloat(
                mesh.GetBlendShapeName(blendShapeIndex),
                renderer.GetBlendShapeWeight(blendShapeIndex),
                new JSONStorableFloat.SetFloatCallback(val => renderer.SetBlendShapeWeight(localBlendShapeIndex, val)),
                0f,
                100f,
                false
            );
            RegisterFloat(storable);
            CreateSlider(storable);
        }
        while (++blendShapeIndex < renderer.sharedMesh.blendShapeCount);
    }
}
