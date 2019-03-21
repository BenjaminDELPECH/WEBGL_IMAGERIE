//  DEFINITION DES VARIABLES
// =============================================================================================================
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
#define nbSphere 5
#define nbSource 2
#define nbPlan 2
// =============================================================================================================

//DEFINITION DES STRUCURES (Rayon,source materiau et sphere)
// =============================================================================================================
struct Material{
    float kd;
    float ks;
    float n;
    vec3 color;
};
struct Ray
{
    vec3 o,v,N;
    float t;
    Material mat;
};

struct Source{
    vec3 pos;
    vec3 POW;
};
struct Plan{
    vec3 n;
    float d;
    vec3 color;
    float t;
};

struct Sphere{
    vec3 c;
    float r;
    Material mat;
    float t;
};

float intersectPlan(Ray r,Plan p)
{
    float t;
    t=(dot(r.o,p.n)+p.d)/dot(r.v,p.n);
    if(t<0.)return-1.;
    else return t;
}

//FONCTION POUR CALCULER L'INTERSECTION DE LA SPHERE ET DU RAYON (On retourne t1 ou t2)
// =============================================================================================================
float intesectSphere(Ray r,Sphere s)
{
    vec3 oc=r.o-s.c;
    float a=dot(r.v,r.v);
    float b=dot(oc,r.v)*2.;
    float c=dot(oc,oc)-(s.r*s.r);
    
    float delta=b*b-4.*a*c;
    
    if(delta<0.)return-1.;
    else
    {
        float t1=(-b-sqrt(delta))/(2.*a);
        float t2=(-b+sqrt(delta))/(2.*a);
        if(t1<t2&&t1>0.)return t1;
        else return t2;
    }
}
// =============================================================================================================

//FONCTION POUR CALCULER LE PHONG MODIFIE
//====================phong======================================
vec3 phong(Ray ray,Source source)
{
    float kd=ray.mat.kd;
    float ks=ray.mat.ks;
    float n=ray.mat.n;
    vec3 color=ray.mat.color;
    vec3 I=ray.o+(ray.t*ray.v);
    vec3 N=ray.N;
    vec3 Vi=(normalize(source.pos-I));
    vec3 Vo=(-ray.o*ray.v);
    vec3 H=(normalize(Vi+Vo));
    float cos_theta=max(0.,dot(N,Vi));
    float cos_alpha=max(0.,dot(N,H));
    vec3 phong=1.8*color*source.POW*((kd/PI)+(ks*((n+8.)/(8.*PI)))*pow(cos_alpha,n))*cos_theta;
    // phong = vec3(ray.t, ray.t, ray.t);
    return phong;
}
// =============================================================================================================

//FONCTION POUR INITIALISER LES PLANS,ON DEFINIT LE NOMBRE DE PLAN AINSI QUE LEUR POSITION
// =============================================================================================================
void initializePlans(inout Plan planTab[nbPlan]){
    
    // vec3 color1 = vec3(0.0, 1.0, 0.6);
    // vec3 color2 = vec3(0.0,0.0,1.0);
    // planTab[0] = Plan(vec3(60.0,600.0,50.0),vec3(1.0,100.0,100.0),color1);
    // planTab[1] = Plan(vec3(100.0,300.0,50.0),vec3(0.600,1.0,0.0),color2);
    
}
// =============================================================================================================

//FONCTION POUR INITIALISER LES SPHERES,ON DEFINIT LE NOMBRE DE SPHERE AINSI QUE LEUR COULEUR ET POSITION
// =============================================================================================================
void initializeSpheres(inout Sphere sphereTab[nbSphere]){
    vec3 color1=vec3(1.,.6,0.);
    Material material1=Material(ks,kd,n,color1);
    sphereTab[0]=Sphere(vec3(-20.,380.,10.),10.,material1,0.);
    sphereTab[1]=Sphere(vec3(0.,330.,0.),10.,material1,0.);
    
    sphereTab[2]=Sphere(vec3(90.,400.,0.),76.,material1,0.);
    
}
// =============================================================================================================

//FONCTION POUR INITIALISER LA OU LES SOURCES DE LUMIERES
// =============================================================================================================
void initializeSources(inout Source sourceTab[nbSource]){
    sourceTab[0]=Source(vec3(-500.,0.,0.),vec3(1.,1.,1.));
    sourceTab[1]=Source(vec3(-200.,150.,-30.),vec3(1.,1.,1.));
    
}
// =============================================================================================================
// void illuminateOmbre(in Ray r, in Source sourceTab[nbSource], in Sphere sphereTab[nbSphere], inout vec3 phongvar){
    //         bool ombre=false;
    //         vec3 I, retourRayon;
    //         float t;
    //         for(int j=0;j<nbSource;j++)
    //         {
        //             I=r.o+(r.t*r.v);
        //             I=I+.010*r.N;
        //             retourRayon=Ray(I,sourceTab[j].pos,r.N,0.,r.mat);
        
        //             ombre=false;
        //             for(int k=0;k<nbSphere;k++)
        //             {
            //                 t=intesectSphere(retourRayon,sphereTab[k]);
            //                 if(t>.015&&t<1.){
                //                     ombre=true;
            //                 }
        //             }
        //             if(!ombre){
            //                 newPhong=phong(r,sourceTab[j]);
            //                 phongvar+=newPhong;
        //             }
    //         }
//     }
void illumineOmbre(inout vec3 phongvar,in Ray r,in Sphere sphereTab[nbSphere],in Source sourceTab[nbSource]){
    bool ombre;float t;
    vec3 newPhong;
    for(int j=0;j<nbSource;j++)
    {
        vec3 I=r.o+(r.t*r.v);
        I=I+.010*r.N;
        Ray retourRayon=Ray(I,sourceTab[j].pos,vec3(0.),0.,r.mat);
        ombre=false;
        for(int k=0;k<nbSphere;k++)
        {
            t=intesectSphere(retourRayon,sphereTab[k]);
            if(t>.015&&t<1.){
                ombre=true;
            }
        }
        if(!ombre){
            newPhong=phong(r,sourceTab[j]);
            phongvar+=newPhong;
        }
    }
}

//FONCTION POUR AFFICHER LA SCENE A PARTIR DU RAYON,DU TABLEAU DE SPHERE ET DES SOURCES
// =============================================================================================================
vec4 Illumination(Ray r,in Sphere sphereTab[nbSphere],in Plan planTab[nbPlan],in Source sourceTab[nbSource]){
    vec3 phongvar;
    vec3 newPhong;
    float tmin=-1.;
    float t;
    vec3 I;
    Ray retourRayon;
    Sphere mySphere;
    Plan myPlan;
    float newT;
    
    float tminS=-1.;
    for(int i=0;i<nbSphere;i++)
    {
        newT=intesectSphere(r,sphereTab[i]);
        if(newT>0.){
            if(tminS<0.||(newT<tminS)){
                tminS=newT;
                mySphere=sphereTab[i];
                mySphere.t=tminS;
            }
        }
    }
    
    float tminP=-1.;
    for(int i=0;i<nbPlan;i++)
    {
        newT=intersectPlan(r,planTab[i]);
        if(newT>0.){
            if(tminP<0.||(newT<tminP)){
                tminP=newT;
                myPlan=planTab[i];
                myPlan.t=tminP;
            }
        }
    }
    
    
    vec3 centerObj;
    float myT;
    Material myMat;
    // TYPE OF OBJECT CODE:
    //rien : 0; sphere : 1; plan : 2
    int typeOfObject;
    if(tminS==-1.&&tminP==-1.)typeOfObject=0;
    else if(tminP==-1.&&tminS!=-1.)typeOfObject=1;
    else if(tminP!=-1.&&tminS==-1.)typeOfObject=2;
    else if(tminP!=-1.&&tminS!=-1.){
        if(tminP<tminS)typeOfObject=2;
        else typeOfObject=1;
    }
    
    if(typeOfObject==0)phongvar=vec3(0.,0.,0.);
    if(typeOfObject==1)centerObj=mySphere.c;myT=mySphere.t;myMat=mySphere.mat;
    if(typeOfObject==2)centerObj=mySphere.c;myT=mySphere.t;myMat=mySphere.mat;
    
    r.t=myT;
    I=r.o+(myT*r.v);
    vec3 N=(normalize(I-centerObj));
    r.N=N;
    r.mat=myMat;
    
    illumineOmbre(phongvar,r,sphereTab,sourceTab);
    return vec4(phongvar*mySphere.mat.color,1.);
    
}
// =============================================================================================================

//on initialise la couleur du fond de notre canvas avec fragcolor, puis on definit un rayon, on cree ensuite un tableau de sphere a partir du nombre de spheres puis un tableau de sources a partir du nombre de sources, ensuite on appelle les fonction d'initialisation de sphere et de source puis on affiche la scene avec nos spheres eclairees
// ======================================================= main ======================================================
void main(void){
    vec3 color1=vec3(0.,0.,0.);
    Material material1=Material(ks,kd,n,color1);
    Ray r=Ray(vec3(0.,0.,0.),rayDir,vec3(0.,0.,0.),1.,material1);
    Sphere sphereTab[nbSphere];
    Source sourceTab[nbSource];
    
    Plan planTab[nbPlan];
    initializePlans(planTab);
    initializeSpheres(sphereTab);
    initializeSources(sourceTab);
    
    gl_FragColor=Illumination(r,sphereTab,planTab,sourceTab);
}
// =============================================================================================================
