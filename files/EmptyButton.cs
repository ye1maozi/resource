using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptyButton : MaskableGraphic {

    protected EmptyButton()
    {
        useLegacyMeshGeneration = true;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}
