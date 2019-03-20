

// =====================================================
// Mouse management
// =====================================================
var mouseDown = false;
var lastMouseX = null;
var lastMouseY = null;
var rotY = 0;
var rotX = 0;

// =====================================================
window.requestAnimFrame = (function()
{
	return window.requestAnimationFrame ||
         window.webkitRequestAnimationFrame ||
         window.mozRequestAnimationFrame ||
         window.oRequestAnimationFrame ||
         window.msRequestAnimationFrame ||
         function(/* function FrameRequestCallback */ callback,/* DOMElement Element */ element)
         {
            window.setTimeout(callback, 1000/60);
         };
})();

// ==========================================
function tick() {
	requestAnimFrame(tick);
	drawScene();
}

// =====================================================
function degToRad(degrees) {
	return degrees * Math.PI / 180;
}

// =====================================================
function handleMouseDown(event) {
	mouseDown = true;
	lastMouseX = event.clientX;
	lastMouseY = event.clientY;
}


// =====================================================
function handleMouseUp(event) {
	mouseDown = false;
}


// =====================================================
function handleMouseMove(event) {
	if (!mouseDown) {				// si pas clic il ne se passe rien
		return;						
	}
	var newX = event.clientX;
	var newY = event.clientY;

	// Calculer le déplacement de la souris
	var depX = newX - lastMouseX;
	var depY = newY - lastMouseY;
	
	// Definir la rotation autour de x, y ou z
	rotY += degToRad(depX / 2); // divisé par 2 pour que ca aille moins vite
	rotX += degToRad(depY / 2);
	
	// Construire une matrice de rotation (regardez objMatrix)
	mat4.identity(objMatrix);
	mat4.rotate(objMatrix ,rotY, [0,1,0]);
	mat4.rotate(objMatrix ,rotX, [1,0,0]);
	
	// Attention les angles doivent être donnés en radians

	lastMouseX = newX;
	lastMouseY = newY;
}
