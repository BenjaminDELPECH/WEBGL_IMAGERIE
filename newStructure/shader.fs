
//constante ============================================
precision mediump float;
varying vec3 rayDir;
varying vec4 vColor;

uniform float n;
uniform float kd;
uniform float ks;

uniform float planX;
uniform float planY;
uniform float planZ;
uniform float planD;

#define PI 3.14159
#define nbSphere 1
#define nbSource 1
#define nbPlan 1

struct Material
{
    float kd;
    float ks;
    float n;
    vec3 color;
};

struct Ray{
    vec3 o,v;
    float t;
    float hit;
    vec3 hitPoint;
    vec3 normal;
    Material mat;
};

struct Plan
{
    vec3 position;
    vec3 normal;
    Material mat;
};

struct Sphere{
    vec3 c;
    float r;
    Material mat;
};
struct Source
{
    vec3 pos;
    vec3 POW;
};

void initializePlans(inout Plan planTab[nbPlan])
{
    vec3 color1=vec3(.0,.7,.0);
    Material material1=Material(ks,kd,n,color1);
    planTab[0]=Plan(vec3(0.,100.,-20.),vec3(planX, planY, planZ),material1);
}
void initializeSpheres(inout Sphere sphereTab[nbSphere])
{
    vec3 color1=vec3(1.,.6,0.);
    Material material1=Material(ks,kd,n,color1);
    sphereTab[0]=Sphere(vec3(0.,100.,-12.),10.,material1);
}
void initializeSources(inout Source sourceTab[nbSource])
{
    // sourceTab[1] = Source(vec3(-30.0,140.0,-190.0),vec3(1.0,1.0,1.0));
    sourceTab[0]=Source(vec3(100.,0.,0.),vec3(1.,1.,1.));
    
}
vec3 phong(Ray r,Source source)
{
    float kd=r.mat.kd;
    float ks=r.mat.ks;
    float n=r.mat.n;
    vec3 color=r.mat.color;
    vec3 I=r.o+(r.t*r.v);
    vec3 N=r.normal;
    vec3 Vi=(normalize(source.pos-I));
    vec3 Vo=(-r.o*r.v);
    vec3 H=(normalize(Vi+Vo));
    float cos_theta=max(0.,dot(N,Vi));
    float cos_alpha=max(0.,dot(N,H));
    vec3 phong=1.0*color*source.POW*((kd/PI)+(ks*((n+8.)/(8.*PI)))*pow(cos_alpha,n))*cos_theta;
    return phong;
}
void intersectSphere(inout Ray r,Sphere s){
    float t;
    vec3 oc=r.o-s.c;
    float a=dot(r.v,r.v);
    float b=dot(oc,r.v)*2.;
    float c=dot(oc,oc)-(s.r*s.r);
    float delta=b*b-4.*a*c;
    if(delta>0.)
    {
        float t1=(-b-sqrt(delta))/(2.*a);
        float t2=(-b+sqrt(delta))/(2.*a);
        if(t1<t2&&t1>0.)t=t1;
        else t=t2;
    }
    if(t>0.&&t<r.t){
        r.t=t;
        r.hit=1.;
        r.hitPoint=vec3(
            r.o.x+r.v.x*t,
            r.o.y+r.v.y*t,
            r.o.z+r.v.z*t
        );
        
        r.normal=normalize(r.hitPoint-s.c);
        r.mat=s.mat;
    }
}

void intersectPlane(inout Ray r,Plan p){
    float d=-dot(p.position,p.normal);
    float v=dot(r.v,p.normal);
    float t=-(dot(r.o,p.normal)+d)/v;
    if(t>0.&&t<r.t){
        r.t=t;
        r.hit=1.;
        r.hitPoint=vec3(
            r.o.x+r.v.x*t,
            r.o.y+r.v.y*t,
            r.o.z+r.v.z*t
        );
        vec3 I = r.hitPoint;
        vec3 N = (normalize(I - p.normal));
        r.normal = N;
        r.mat=p.mat;
    }
}

vec4 eclairage(in Ray r,Source sourceTab[nbSource]){
    vec3 phongVar = vec3(0., 0., 0.);
    for(int i=0;i<nbSource;i++){
        phongVar+=phong(r,sourceTab[i]);
    }
    return vec4(phongVar,1.);
}

void intersect(inout Ray r,in Sphere sphereTab[nbSphere],in Plan planTab[nbPlan]){
   
    for(int i=0;i<nbPlan;i++)intersectPlane(r,planTab[i]);
     for(int i=0;i<nbSphere;i++)intersectSphere(r,sphereTab[i]);
}

void main(void){
    vec3 color1=vec3(0.,0.,0.);
    Material material1=Material(ks,kd,n,color1);
    Ray r;
    r.o=vec3(0.);
    r.v=rayDir;
    r.t=10.e+20;
    r.hit=0.;
    r.hitPoint=vec3(0.);
    r.normal=vec3(0.);
    r.mat=material1;
    Sphere sphereTab[nbSphere];
    Plan planTab[nbPlan];
    Source sourceTab[nbSource];
    initializePlans(planTab);
    initializeSpheres(sphereTab);
    initializeSources(sourceTab);
    intersect(r,sphereTab,planTab);
    if(r.hit>0.){
    gl_FragColor=eclairage(r,sourceTab);
    }else{
         gl_FragColor=vec4(0.);
    }
}

