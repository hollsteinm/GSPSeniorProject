/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/

// The target we are following
var target : Transform;
// The distance in the x-z plane to the target
var distance = 40.0;
// the height we want the camera to be above the target
var height = 10.0;
// the rate at which we want the camera to follow
var dampRate = 0.1;

// Place the script in the Camera-Control group in the component menu
@script AddComponentMenu("Camera-Control/Smooth Follow")


function LateUpdate () {
	// Early out if we don't have a target
	if (!target)
		return;
	
	// Calculate the position the camera should be in
	var wantedHeight : Vector3 =  target.up * height;
	var wantedDistance : Vector3 = target.forward * -distance;
	
	// Set the position of the camera on the x-z plane to:
	// distance meters behind the target
	var wantedPosition : Vector3 = target.position + wantedHeight + wantedDistance;
	var cameraMoveVec : Vector3 = wantedPosition - transform.position;
	
	transform.position += cameraMoveVec * dampRate;
	
	// Always look at the target
	transform.LookAt (target, target.up);
}