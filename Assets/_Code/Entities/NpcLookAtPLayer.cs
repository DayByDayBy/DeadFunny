using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcLookAtPLayer : MonoBehaviour
{
    public Transform playerCharacter;
    public float timeToLook = 3f;
    float myTime = 0f;
    public bool isLookingAt = false;
     Transform defaulTransform;

    void Start()
    {
        defaulTransform = transform;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player)
            playerCharacter = player.transform;
    }

    public void StartLookAtPlayer()
    {
        isLookingAt = true;
    }

    public void StopLookingAt()
    {
        isLookingAt = false;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        if (!playerCharacter)
            return;

        if (isLookingAt)
        {
            myTime += Time.deltaTime;
            if (myTime > timeToLook)
            {
                myTime = timeToLook;
            }

        }
        else
        {
            myTime = 0f;
            transform.rotation = defaulTransform.rotation;
        }

        Transform headTransform = transform;
        

        Quaternion lookRotation = Quaternion.LookRotation(playerCharacter.position - headTransform.position);
        Quaternion targetTransform = headTransform.rotation * lookRotation;

        transform.rotation = Quaternion.Lerp(headTransform.rotation, targetTransform, myTime / timeToLook);
        
    }
}
