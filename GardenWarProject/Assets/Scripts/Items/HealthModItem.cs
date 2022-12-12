using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Inventory
{
    public class HealthModItem : Item
    {
        private IActiveLifeable lifeable;

        protected override void OnItemAddedEffects(Entity entity)
        {
            lifeable = entity.GetComponent<IActiveLifeable>();
            lifeable?.IncreaseMaxHpRPC(((HealthModItemSO)AssociatedItemSO()).healthMod);
            Debug.Log($"Gained {((HealthModItemSO)AssociatedItemSO()).healthMod} hp");
        }

        protected override void OnItemAddedEffectsFeedback(Entity entity)
        {
            
        }

        protected override void OnItemRemovedEffects(Entity entity)
        {
            lifeable?.DecreaseMaxHpRPC(((HealthModItemSO)AssociatedItemSO()).healthMod);
            Debug.Log($"Removed {((HealthModItemSO)AssociatedItemSO()).healthMod} hp");
        }

        protected override void OnItemRemovedEffectsFeedback(Entity entity)
        {
            
        }

        public override void OnItemActivatedEffects(int targetIndex, Vector3 positions)
        {
            Debug.Log("Item Activated Effect");
        }

        public override void OnItemActivatedFeedbackEffects(int targetIndex, Vector3 position)
        {
            Debug.Log("Item Activated Feedback Effect");
        }
    }
}