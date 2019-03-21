attribute vec2 aVertexPosition;
varying vec3 rayDir;

void main(void) {
	rayDir = vec3 (18.0*aVertexPosition.x, 50.0, 12.0*aVertexPosition.y);
	gl_Position = vec4(aVertexPosition, 0.0, 1.0);
}
