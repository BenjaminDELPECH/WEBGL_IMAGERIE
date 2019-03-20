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
#define nbSource 3
#define nbPlan 3
// =============================================================================================================

//DEFINITION DES STRUCURES (Rayon,source materiau et sphere)
// =============================================================================================================

struct Material
{
    float kd;
    float ks;
    float n;
    vec3 color;
};

struct Ray 
{
    vec3 o, v, N;
    float t;
    Material mat;
};

struct Source 
{
    vec3 pos;
    vec3 POW;
};

struct Plan
{
    vec3 n;
    float d;  
    Material mat;
    float t;
};
struct Sphere
{
    vec3 c;
    float r;
    Material mat;
    float t;
};

//FONCTION POUR CALCULER L'INTERSECTION DU RAYON ET DU PLAN
// =========================================================================================
float intersectPlan(Ray r,Plan p)
{
    float t = -(p.d + dot(p.n,r.o) )/(dot(p.n,r.v));
   return t;
    
}
float iPlane(in vec3 ro, in vec3 rd)
{
    return -ro.y/rd.y;
}
// =========================================================================================

//FONCTION POUR CALCULER L'INTERSECTION DE LA SPHERE ET DU RAYON (On retourne t1 ou t2)
// =============================================================================================================
float intersectSphere(Ray r, Sphere s)
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

//FONCTION POUR CALCULER LE PHONG MODIFIE 
//====================phong======================================
vec3 phong(Ray ray, Source source)
{
    float kd = ray.mat.kd;
    float ks = ray.mat.ks;
    float n = ray.mat.n;
    vec3 color = ray.mat.color;
    vec3 I =ray.o+(ray.t*ray.v);
    vec3 N = ray.N;
    vec3 Vi=(normalize(source.pos-I));
    vec3 Vo=(-ray.o * ray.v);
    vec3 H=(normalize(Vi+Vo));
    float cos_theta = max(0.0, dot(N,Vi));
    float cos_alpha = max(0.0, dot(N, H));
    vec3 phong = 1.8*color*source.POW * ((kd/PI)+(ks*((n+8.0)/(8.0*PI)))*pow(cos_alpha,n))*cos_theta;
    // phong = vec3(ray.t, ray.t, ray.t);
    return phong;
}
// =============================================================================================================

//FONCTION POUR INITIALISER LES PLANS,ON DEFINIT LE NOMBRE DE PLAN AINSI QUE LEUR POSITION
// =============================================================================================================
void initializePlans(inout Plan planTab[nbPlan])
{
    vec3 color1 = vec3(0.2,0.7,0.2);
    vec3 color2 = vec3(0.7,0.2,0.2);
    vec3 color3 = vec3(0.2,0.0,1.0);
    Material material1 = Material(10.0,kd,0.0, color1);
    Material material2 = Material(10.0,kd,0.0,color2);
    Material material3 = Material(10.0,kd,0.0,color3);
  
    planTab[0] = Plan(vec3(1.0,1.0,-1000.0),100.0,material1, 0.0);
    planTab[1] = Plan(vec3(-1000.0,-220.0,1.0),100.0,material2, 0.0);
    planTab[2] = Plan(vec3(planX,planY,planZ),100.0,material3, 0.0);
    
    
   }
// =============================================================================================================

//FONCTION POUR INITIALISER LES SPHERES,ON DEFINIT LE NOMBRE DE SPHERE AINSI QUE LEUR COULEUR ET POSITION
// =============================================================================================================
void initializeSpheres(inout Sphere sphereTab[nbSphere])
{
    vec3 color1 = vec3(1.0, 0.6, 0.0);
    Material material1 = Material(ks,kd,n, color1);
    sphereTab[0] = Sphere(vec3(0.0,100.0,0.0),5.0, material1, 0.0);
  
}
// =============================================================================================================

//FONCTION POUR INITIALISER LA OU LES SOURCES DE LUMIERES
// =============================================================================================================
void initializeSources(inout Source sourceTab[nbSource])
{
    // sourceTab[1] = Source(vec3(-30.0,140.0,-190.0),vec3(1.0,1.0,1.0));
    sourceTab[0] = Source(vec3(0.0,0.0,0.0),vec3(1.0,1.0,1.0));
    
  
    
}
// =============================================================================================================

// ===========================================================

void rayonToucheObjet(inout Ray r, float myT, vec3 myCenter, Material myMat){
    vec3 I,N;

    I =r.o+(myT*r.v);
    N =(normalize(I-myCenter));
    r.N = N;
    r.t = myT;
    r.mat = myMat;

}

//FONCTION POUR AFFICHER LA SCENE A PARTIR DU RAYON,DU TABLEAU DE SPHERE ET DES SOURCES
// =============================================================================================================
vec4 IlluminationTest(inout Ray r, in Sphere sphereTab[nbSphere], in Plan planTab[nbPlan], in Source sourceTab[nbSource])
{
    vec3 phongvar;
    vec3 newPhong;
    float tMin = -1.0;
    float tP, tS, newT;
    Sphere mySphere;
    Plan myPlan;
   
     tS = -1.0;tMin = -1.0;
    for(int i=0;i< nbSphere;i++)
            {
            tS = intersectSphere(r,  sphereTab[i]);
           
            if(tMin < 0.0 || (tS < tMin))
            {
                mySphere = sphereTab[i];
                mySphere.t = tS;
            }
            
        }
    tMin = -1.0;
     for(int i=0;i< nbPlan;i++)
    {
        tP = intersectPlan(r,  planTab[i]);
       
        
       if((tMin < 0.0 || (tP < tMin)))
        {
            tMin = tP;
            myPlan = planTab[i];
            myPlan.t = tP;
        }
       
    }
        
    phongvar = vec3(0.0, 0.0, 0.0);
        
        if(tP != -1.0 && tS != -1.0){
            if(tS<tP){
                rayonToucheObjet(r,mySphere.t , mySphere.c, mySphere.mat);
               
            }else{
           rayonToucheObjet(r,myPlan.t , myPlan.n, myPlan.mat);
           
            }
            }else if(tP != -1.0 &&  tS == -1.0){
                  rayonToucheObjet(r,myPlan.t , myPlan.n, myPlan.mat);
}
            else if(tP == -1.0 &&  tS != -1.0){
                  rayonToucheObjet(r,mySphere.t , mySphere.c, mySphere.mat);
            }
            else{
                phongvar = vec3(0.0, 0.0, 0.0);
            }
            
            for(int i=0; i< 2 ;i++)
            {   
                newPhong = phong(r, sourceTab[i]);
              
                phongvar+=newPhong;
                
             }
       return vec4(phongvar, 1.0);
}
// =============================================================================================================
//on initialise la couleur du fond de notre canvas avec fragcolor, puis on definit un rayon, on cree ensuite un tableau de sphere a partir du nombre de spheres puis un tableau de sources a partir du nombre de sources, ensuite on appelle les fonction d'initialisation de sphere et de source puis on affiche la scene avec nos spheres eclairees
// ======================================================= main ======================================================
void main(void) 
{
    vec3 color1 = vec3(0.0, 0.0, 0.0);
    Material material1 = Material(ks,kd,n, color1);
    Ray r = Ray(vec3(0.0,0.0,0.0), rayDir,vec3(0.0, 0.0, 0.0), 1.0, material1);
    Sphere sphereTab[nbSphere];
    Source sourceTab[nbSource];
    Plan planTab[nbPlan];
   

    gl_FragColor = vec4(0.1,0.1,0.1,1.0);
    initializePlans(planTab);
    initializeSpheres(sphereTab);
    initializeSources(sourceTab);
    
    gl_FragColor = IlluminationTest(r, sphereTab,planTab, sourceTab);
}