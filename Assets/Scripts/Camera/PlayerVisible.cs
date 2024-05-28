// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisible : MonoBehaviour
{
    private List<ObjectFade> _fadedObjects = new List<ObjectFade>();

    // Update is called once per frame
    void Update()
    {
        // Raycast from camera to player, out hit.
        Vector3 direction = BattleInfo.player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction);

        // Fade objs in the way if player isn't hit.
        RaycastHit[] hits = Physics.RaycastAll(ray);
        List<ObjectFade> hitFaders = new List<ObjectFade>();
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != BattleInfo.player)
            {
                ObjectFade fader = hit.collider.gameObject.GetComponent<ObjectFade>();
                if (fader != null)
                {
                    fader.DoFade = true;
                    hitFaders.Add(fader);
                }
            }
        }

        // Unfade objects that were hit in the previous frame but are no longer in the way
        foreach (ObjectFade fader in _fadedObjects)
        {
            if (!hitFaders.Contains(fader))
            {
                fader.DoFade = false;
            }
        }

        // Update the list of faded objects
        _fadedObjects = hitFaders;
    }
}
