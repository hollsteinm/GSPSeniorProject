using UnityEngine;
using System.Collections;

public struct FPCamera {
	public void FPUpdate (Transform camera, Transform ship)
	{
		//Initialize vectors
		Vector3 cameraPos = ship.position;
		Vector3 lookAtVec = ship.position;
		
		//Set the camera position
		cameraPos += ship.forward * 10;
		cameraPos += ship.up * 4;
		
		//Set the lookAt position
		lookAtVec += ship.forward * 11;
		lookAtVec += ship.up * 4;
		
		//Finalize camera settings
		camera.position = cameraPos;
		camera.LookAt(lookAtVec, ship.up);
	}
}