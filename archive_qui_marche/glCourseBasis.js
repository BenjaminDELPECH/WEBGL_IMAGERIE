// =====================================================
var gl;
var shadersLoaded = 0;
var vertShaderTxt;
var fragShaderTxt;
var shaderProgram = null;
var vertexBuffer;
var mvMatrix = mat4.create();
var pMatrix = mat4.create();
var objMatrix = mat4.create();
mat4.identity(objMatrix);


// =====================================================
function webGLStart() {
	var canvas = document.getElementById("WebGL-test");
	canvas.onmousedown = handleMouseDown;
	document.onmouseup = handleMouseUp;
	document.onmousemove = handleMouseMove;

	initGL(canvas);
	initBuffers();
	loadShaders('shader');

	gl.clearColor(0.7, 0.7, 0.7, 1.0);
	gl.enable(gl.DEPTH_TEST);

	//	drawScene();
	tick();
}

// =====================================================
function initGL(canvas) {
	try {
		gl = canvas.getContext("experimental-webgl");
		gl.viewportWidth = canvas.width;
		gl.viewportHeight = canvas.height;
		gl.viewport(0, 0, canvas.width, canvas.height);
	} catch (e) {}
	if (!gl) {
		console.log("Could not initialise WebGL");
	}
}

// =====================================================
function initBuffers() {
	vertexBuffer = gl.createBuffer();
	gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
	vertices = [
		-1.0, -1.0, 0.0,
		-1.0, 1.0, 0.0,
		1.0, 1.0, 0.0,
		1.0, -1.0, 0.0
	];
	gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
	vertexBuffer.itemSize = 3;
	vertexBuffer.numItems = 4;
}


// =====================================================
function loadShaders(shader) {
	loadShaderText(shader, '.vs');
	loadShaderText(shader, '.fs');
}

// =====================================================
function loadShaderText(filename, ext) { // technique car lecture asynchrone...
	var xhttp = new XMLHttpRequest();
	xhttp.onreadystatechange = function () {
		if (xhttp.readyState == 4 && xhttp.status == 200) {
			if (ext == '.vs') {
				vertShaderTxt = xhttp.responseText;
				shadersLoaded++;
			}
			if (ext == '.fs') {
				fragShaderTxt = xhttp.responseText;
				shadersLoaded++;
			}
			if (shadersLoaded == 2) {
				initShaders(vertShaderTxt, fragShaderTxt);
				shadersLoaded = 0;
			}
		}
	}
	xhttp.open("GET", filename + ext, true);
	xhttp.send();
}

// =====================================================
function initShaders(vShaderTxt, fShaderTxt) {

	vshader = gl.createShader(gl.VERTEX_SHADER);
	gl.shaderSource(vshader, vShaderTxt);
	gl.compileShader(vshader);
	if (!gl.getShaderParameter(vshader, gl.COMPILE_STATUS)) {
		console.log(gl.getShaderInfoLog(vshader));
		return null;
	}

	fshader = gl.createShader(gl.FRAGMENT_SHADER);
	gl.shaderSource(fshader, fShaderTxt);
	gl.compileShader(fshader);
	if (!gl.getShaderParameter(fshader, gl.COMPILE_STATUS)) {
		console.log(gl.getShaderInfoLog(fshader));
		return null;
	}

	shaderProgram = gl.createProgram();
	gl.attachShader(shaderProgram, vshader);
	gl.attachShader(shaderProgram, fshader);

	gl.linkProgram(shaderProgram);

	if (!gl.getProgramParameter(shaderProgram, gl.LINK_STATUS)) {
		console.log("Could not initialise shaders");
	}

	gl.useProgram(shaderProgram);

	shaderProgram.vertexPositionAttribute = gl.getAttribLocation(shaderProgram, "aVertexPosition");
	gl.enableVertexAttribArray(shaderProgram.vertexPositionAttribute);

	shaderProgram.pMatrixUniform = gl.getUniformLocation(shaderProgram, "uPMatrix");
	shaderProgram.mvMatrixUniform = gl.getUniformLocation(shaderProgram, "uMVMatrix");

	shaderProgram.n = gl.getUniformLocation(shaderProgram, "n");
	gl.uniform1f(shaderProgram.n, 100.0);

	shaderProgram.ks = gl.getUniformLocation(shaderProgram, "ks");
	gl.uniform1f(shaderProgram.ks, 1.5);

	shaderProgram.kd = gl.getUniformLocation(shaderProgram, "kd");
	gl.uniform1f(shaderProgram.kd, 0.20);
}


// =====================================================
function setMatrixUniforms() {
	if (shaderProgram != null) {
		gl.uniformMatrix4fv(shaderProgram.pMatrixUniform, false, pMatrix);
		gl.uniformMatrix4fv(shaderProgram.mvMatrixUniform, false, mvMatrix);
	}
}


function drawScene() {
	gl.clear(gl.COLOR_BUFFER_BIT);

	if (shaderProgram != null) {
		gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
		gl.vertexAttribPointer(shaderProgram.vertexPositionAttribute,
			vertexBuffer.itemSize, gl.FLOAT, false, 0, 0);

		mat4.perspective(45, gl.viewportWidth / gl.viewportHeight, 0.1, 200.0, pMatrix);
		mat4.identity(mvMatrix);
		mat4.translate(mvMatrix, [0.0, 0.0, -2.0]);
		mat4.multiply(mvMatrix, objMatrix);





		gl.drawArrays(gl.TRIANGLE_FAN, 0, vertexBuffer.numItems);
	}
}

$(document).ready(function () {
	var test = $("#sliderN").val();
	$("#labelN").html(test);
	$("#sliderN").on("change mousemove", function () {
		$("#labelN").html(this.value);
		gl.uniform1f(shaderProgram.n, this.value);
	});

	var test2 = $("#sliderKs").val();
	$("#labelKs").html(test2);
	$("#sliderKs").on("change mousemove", function () {
		$("#labelKs").html(this.value);
		gl.uniform1f(shaderProgram.ks, this.value);
	});

	var test3 = $("#sliderKd").val();
	$("#labelKd").html(test3);
	$("#sliderKd").on("change mousemove", function () {
		$("#labelKd").html(this.value);
		gl.uniform1f(shaderProgram.kd, this.value);
	});
});