using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using View;

public class Artifact
{
    public ArtifactModel Model;
    public ArtifactView View;
    
    public Artifact(ArtifactModel model, ArtifactView view)
    {
        Model = model;
        View = view;
    }

    public async UniTask Activate(ArtifactTrigger trigger)
    {
        await Model.Activate(trigger);
    }
}
