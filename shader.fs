precision mediump float;
varying vec3 rayDir;
varying vec4 vColor;

uniform float n;
uniform float kd;
uniform float ks;


#define PI 3.14159
#define nbSphere 3
#define nbSource 2
// =============================================================================================================




// =============================================================================================================
struct Ray {
	vec3 o, v;
	float t;
};

struct Source {
	vec3 pos;
	vec3 POW;
};

struct Material{
	float kd;
	float ks;
	float n;
	vec3 color;
};

struct Sphere {
	vec3 c;
	float r;
	Material mat;
	float tmin;
};
// =============================================================================================================




// =============================================================================================================
float intesectSphere(Ray r, Sphere s)
{	
	vec3 oc = r.o - s.c;
	float a = dot(r.v,r.v);
	float b = dot(oc,r.v) * 2.0;
	float c = dot(oc,oc)-(s.r * s.r);
	
	float delta = b * b - 4.0 * a * c;
	
	if(delta < 0.0) return -1.0;
	else
	{
		float t1 = (-b-sqrt(delta))/(2.0*a);
		float t2 = (-b+sqrt(delta))/(2.0*a);
		if(t1 < t2 && t1 > 0.0) return t1;
		else return t2;
	}
}
// =============================================================================================================





//====================phong======================================
vec3 phong(Sphere sphere, Ray ray, Source source){
	
	float kd = sphere.mat.kd;
	float ks = sphere.mat.ks;
	float n = sphere.mat.n;
	vec3 color = sphere.mat.color;

	vec3 I =ray.o+(sphere.tmin*ray.v);
	vec3 N =(normalize(I-sphere.c));
	vec3 Vi=(normalize(source.pos-I));
	vec3 Vo=(-ray.o * ray.v);
	vec3 H=(normalize(Vi+Vo));
	vec3 phong = source.POW * ((kd/PI)+(ks*((n+8.0)/(8.0*PI)))*pow(dot(N,H),n))*dot(N,Vi);
	
	return phong;
}
// =============================================================================================================





// =============================================================================================================
 void initializeSpheres(inout Sphere sphereTab[nbSphere]){
	vec3 color1 = vec3(1.0, 0.6, 0.0);
	Material material1 = Material(ks,kd,n, color1);
	sphereTab[0] = Sphere(vec3(-0.0,350.0,0.0),0.0, material1, -1.0);
	sphereTab[1] = Sphere(vec3(-100.0,300.0,50.0),50.0, material1, -1.0);
	sphereTab[2] = Sphere(vec3(100.0,300.0,50.0),50.0, material1, -1.0);

}
// =============================================================================================================





// =============================================================================================================
void initializeSources(inout Source sourceTab[nbSource]){
	sourceTab[0] = Source(vec3(20.0,295.0,50.0),vec3(1.0,1.0,1.0));
	sourceTab[1] = Source(vec3(20.0,295.0,-250.0),vec3(1.0,1.0,1.0));
	
}
// =============================================================================================================





// =============================================================================================================
void displayScene(Ray r,in Sphere sphereTab[nbSphere],in Source sourceTab[nbSource]){
	vec3 phongvar;
	vec3 newPhong;
	float tmin = -1.0;
	for(int i=0;i< nbSphere;i++)
	{
		float k = float(i);
		// vec3 color1 = vec3(1.0, 1.0, 1.0);
		// Material material1 = Material(vec3(1.0,1.0,1.0),0.06,130.0, color1);
		// sphereTab[i] = Sphere(vec3(1.2*k*2.0,k*220.0,20.0),20.0,  material1, -1.0);
		r.t = intesectSphere(r,sphereTab[i]);
		if(r.t > 0.0){
			if(tmin < 0.0 || (r.t < tmin)){
				tmin = r.t;
				sphereTab[i].tmin = tmin;
				phongvar = vec3(0.0,0.0,0.0);
				newPhong = vec3(0.0,0.0,0.0);
				for(int j=0;j< 2 ;j++)
				{
					newPhong = phong(sphereTab[i], r, sourceTab[j]);
					if(newPhong.x > 0.0){
						phongvar += newPhong;
					}
				}
				gl_FragColor = vec4(phongvar * sphereTab[i].mat.color, 1.0);
			}
		}
	}
}
// =============================================================================================================





// ======================================================= main ======================================================
void main(void) {
	gl_FragColor = vec4(0.0,0.0,0.0,1.0);
	Ray r = Ray(vec3(0.0,0.0,0.0), rayDir,-1.0);
	Sphere sphereTab[nbSphere];
	Source sourceTab[nbSource];
	
	initializeSpheres(sphereTab);
	initializeSources(sourceTab);
	displayScene(r, sphereTab, sourceTab);
}
// =============================================================================================================
