using UnityEngine;

namespace KidStyo.Helper
{
    public static class UnityHelper
    {
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
    }
}