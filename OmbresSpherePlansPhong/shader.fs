precision mediump float;

varying vec3 rayDir;
varying vec4 vColor;

uniform float n;
uniform float kd;
uniform float ks;
uniform float sphere3Y;


#define PI 3.14159

#define nbSphere 3
#define nbSource 2
#define nbPlan 3

// Structure ==================================================
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
    float toucheObjet;
    vec3 impactPoint;
    vec3 N;
    Material mat;
};


struct Plan
{
    vec3 pos;
    vec3 N;
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

// initalisations==============================================================================================================
void initializePlans(inout Plan planTab[nbPlan])
{
    vec3 color1=vec3(.6,.6,.6);
    Material material1=Material(ks,kd,n,color1);
    planTab[0]=Plan(vec3(0.,0.,-5.),vec3(0.,0.,-180.),material1);
    planTab[1]=Plan(vec3(0.,100.,0.),vec3(0.,180.,0.),material1);
    planTab[2]=Plan(vec3(-20.,100.,0.),vec3(-360.,-60.,-0.),material1);
}

// initalisations=============================================================================================================
void initializeSpheres(inout Sphere sphereTab[nbSphere])
{
    vec3 color1=vec3(.9,0.,0.);
    Material material1=Material(ks,kd,n,color1);
    sphereTab[0]=Sphere(vec3(-3.,27.,-1.25),3.5,material1);
    sphereTab[1]=Sphere(vec3(5.,27.5,-3.4),1.5,material1);
    sphereTab[2]=Sphere(vec3(-0.,sphere3Y,3.),1.5,material1);
    
}

// initalisations=============================================================================================================
void initializeSources(inout Source sourceTab[nbSource])
{
    sourceTab[0]=Source(vec3(30.,0.,40.),vec3(.5,.5,.5));
    sourceTab[1]=Source(vec3(60.,0.,10.),vec3(.5,.5,.5));
}

// initalisations==============================================================================================================
void initializeScene(inout Plan planTab[nbPlan],inout Sphere sphereTab[nbSphere],inout Source sourceTab[nbSource]){
    initializePlans(planTab);
    initializeSpheres(sphereTab);
    initializeSources(sourceTab);
}



// phong=======================================================================================================================
vec3 phong(Ray r,Source source)
{
    vec3 I=r.o+(r.t*r.v);
    vec3 N=r.N;
    vec3 Vi=(normalize(source.pos-I));
    vec3 Vo=(-r.o*r.v);
    vec3 H=(normalize(Vi+Vo));
    float cos_theta=max(0.,dot(N,Vi));
    float cos_alpha=max(0.,dot(N,H));
    vec3 phong=1.*r.mat.color*source.POW*((r.mat.kd/PI)+(r.mat.ks*((r.mat.n+8.)/(8.*PI)))*pow(cos_alpha,r.mat.n))*cos_theta;
    return phong;
}

// Intersection avec une Sphere================================================================================================
float intersectSphere(inout Ray r,Sphere s){
    vec3 oc=r.o-s.c;
    float a=dot(r.v,r.v);
	float b=dot(oc,r.v)*2.;
    float c=dot(oc,oc)-(s.r*s.r);
    float delta=b*b-4.*a*c;
    if(delta<0.) return-1.;
    else
    {
        float t1=(-b-sqrt(delta))/(2.*a);
        float t2=(-b+sqrt(delta))/(2.*a);
        if(t1<t2&&t1>0.)return t1;
        else return t2;
    }
}

// Intersection avec un Plan ===================================================================================================
float intersectPlan(inout Ray r,Plan p){
    float d=-dot(p.pos,p.N);
    float v=dot(r.v,p.N);
    float t=-(dot(r.o,p.N)+d)/v;
    if(t>0.)return t;
    else return-1.;
}


//verification de la presence d'objets entre le point d'impact et la source lumineuse ===========================================
bool verifOmbre(in Ray retourRayon,in Sphere sphereTab[nbSphere],in Plan planTab[nbPlan]){
    float tS,tP;

    for(int j=0;j<nbSphere;j++)
    {
        tS=intersectSphere(retourRayon,sphereTab[j]);
        if(tS>0.&&tS<1.){
            return true;
        }
    }
    
    for(int j=0;j<nbPlan;j++)
    {
        tP=intersectPlan(retourRayon,planTab[j]);
        if(tP>0.&&tP<1.){
            return true;
        }
    }
    
}

// Calcul de l'eclairage , construction d'un rayon de retour pour l'envoyer à la fonction verifOmbre() ============================
void eclairage(inout vec3 phongvar,in Ray r,in Source sourceTab[nbSource],in Sphere sphereTab[nbSphere],in Plan planTab[nbPlan]){
    Ray retourRayon;bool ombre;float tS;float tP;vec3 newPhong;
    for(int i=0;i<nbSource;i++)
    {
        retourRayon.o=r.impactPoint+.25*r.N;
        retourRayon.v=sourceTab[i].pos;
        ombre=false;
        ombre=verifOmbre(retourRayon,sphereTab,planTab);
        if(!ombre)
        {
            newPhong=phong(r,sourceTab[i]);
            phongvar+=newPhong;
        }
    }
}

//  Donne aux rayons les propriétés de l'objet qu'il touche ==========================================================================
void setRay(inout Ray r,float t,vec3 centerObj,Material matObj){
    r.t=t;
    r.toucheObjet=1.;
    r.impactPoint=r.o+r.v*t;
    r.N=(normalize(r.impactPoint-centerObj));
    r.mat=matObj;
}

//===================================================================================================================================
vec4 intersect(inout Ray r,in Sphere sphereTab[nbSphere],in Plan planTab[nbPlan],in Source sourceTab[nbSource]){
    float tP,tS;vec3 phongvar;
    for(int i=0;i<nbPlan;i++){
        tP=intersectPlan(r,planTab[i]);
        if(tP>0.&&tP<r.t){
            setRay(r,tP,planTab[i].N,planTab[i].mat);
        }
    }
    for(int i=0;i<nbSphere;i++){
        tS=intersectSphere(r,sphereTab[i]);
        if(tS>0.&&tS<r.t){
            setRay(r,tS,sphereTab[i].c,sphereTab[i].mat);
        }
    }
    eclairage(phongvar,r,sourceTab,sphereTab,planTab);
    return vec4(phongvar*r.mat.color,1.);
}

//======================================================================================================================================
void main(void){
    vec3 color1=vec3(0.,0.,0.);
    Material material1=Material(ks,kd,n,color1);
    Ray r=Ray(vec3(0.),rayDir,10.e+20,0.,vec3(0.),vec3(0.),material1);
    Sphere sphereTab[nbSphere];
    Plan planTab[nbPlan];
    Source sourceTab[nbSource];
    initializeScene(planTab,sphereTab,sourceTab);
    gl_FragColor=intersect(r,sphereTab,planTab,sourceTab);
}

