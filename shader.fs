
// ======================================================= structure de donnée ======================================================

precision mediump float;
varying vec3 rayDir;
varying vec4 vColor;

struct ray {
	vec3 o, v;
	float t;
};

struct sphere {
	vec3 c;
	float r;
	vec4 color;
};


// ======================================================= fonction intersectSphere ======================================================

float intesectSphere(inout ray r, sphere s)
{
	float a = dot(r.v,r.v);
	float b = dot((r.o-s.c),r.v)*2.0;
	float c = dot((r.o-s.c),(r.o-s.c))-(s.r*s.r);
	
	float delta = b*b-4.0*a*c;
	
	if(delta < 0.0)
	{
		return -1.0;
	}
	else
	{
		// valeur de t 
		float t1 = (-b-sqrt(delta))/2.0*a;
		float t2 = (-b+sqrt(delta))/2.0*a;
		if(t1 < t2 && t1 > 0.0)
		{
			return t1;
		}
		else
		{
			return t2;
		}
	}
}


// ======================================================= main ======================================================
void main(void) {

	sphere sphereTab[100];
	vec4 colors[20];
	
	
	//set background.
	gl_FragColor = vec4(0.0,0.0,0.0,1.0);
	
	// sphere (x, y, z) ----- y = profondeur, donc ne pas trop diminuer cette valeure
	//    -120 <-----------------x-------------------> 120 
	//    -75 bas ---------------y----------------haut 75

	vec4 color1 = vec4(fract(sin(1.0)*1.0), 0.6, 0.0, 1.0);

	sphereTab[0] = sphere(vec3(-50.0,200.0,20.0),10.0, color1);
	sphereTab[1] = sphere(vec3(-20.0,300.0,20.0),10.0, color1);
	sphereTab[2] = sphere(vec3(10.0,400.0,20.0),10.0, color1);
	sphereTab[3] = sphere(vec3(40.0,500.0,20.0),10.0, color1);
	sphereTab[4] = sphere(vec3(70.0,600.0,20.0),10.0, color1);
	sphereTab[5] = sphere(vec3(100.0,700.0,20.0),10.0, color1);
	sphereTab[6] = sphere(vec3(130.0,800.0,20.0),10.0, color1);
	
	

	
	
	ray r = ray(vec3(0.0,0.0,0.0), rayDir,-1.0);
	
	for(int i=0;i< 20;i++)
	{
		float k = float(i);
		sphereTab[i] = sphere(vec3(1.2*k*2.0,k*220.0,20.0),10.0, color1);
		r.t = intesectSphere(r,sphereTab[i]);
	
		if(r.t != -1.0)
		{
			gl_FragColor = sphereTab[i].color;
		}
	}	
}

