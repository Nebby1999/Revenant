using UnityEngine;

namespace RevenantMod.Components
{
    public class RevenantBlendShapeController : MonoBehaviour
    {
        public Transform pupilTransform;
        public SkinnedMeshRenderer skinnedMeshRenderer;

        private void LateUpdate()
        {
            float weight = 0;

            float scaleX = pupilTransform.localScale.x;

            weight = (scaleX - 1) * 0.85f;
            skinnedMeshRenderer.SetBlendShapeWeight(0, weight);

            weight = -(scaleX - 1) * 1.25f;
            skinnedMeshRenderer.SetBlendShapeWeight(1, weight);
        }
    }
}