attribute vec2 aVertexPosition;
varying vec3 rayDir;

void main(void) {
	rayDir = vec3 (90.0*aVertexPosition.x, 50.0, 60.0*aVertexPosition.y);
	gl_Position = vec4(aVertexPosition, 0.0, 1.0);
}
