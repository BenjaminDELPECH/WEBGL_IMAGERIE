//  DEFINITION DES VARIABLES
// =============================================================================================================
precision mediump float;
varying vec3 rayDir;
varying vec4 vColor;

uniform float n;
uniform float kd;
uniform float ks;


#define PI 3.14159
#define nbSphere 5
#define nbSource 2
#define nbPlan 2
// =============================================================================================================



//DEFINITION DES STRUCURES (Rayon,source materiau et sphere)
// =============================================================================================================
struct Ray 
{
    vec3 o, v;
    float t;
};

struct Source 
{
    vec3 pos;
    vec3 POW;
};
struct Material
{
    float kd;
    float ks;
    float n;
    vec3 color;
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
// =========================================================================================



//FONCTION POUR CALCULER L'INTERSECTION DE LA SPHERE ET DU RAYON (On retourne t1 ou t2)
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






//FONCTION POUR CALCULER LE PHONG MODIFIE 
//====================phong======================================
vec3 phong(Sphere sphere, Ray ray, Source source)
{
    float kd = sphere.mat.kd;
    float ks = sphere.mat.ks;
    float n = sphere.mat.n;
    vec3 color = sphere.mat.color;
    vec3 I =ray.o+(sphere.t*ray.v);
    vec3 N =(normalize(I-sphere.c));
    vec3 Vi=(normalize(source.pos-I));
    vec3 Vo=(-ray.o * ray.v);
    vec3 H=(normalize(Vi+Vo));
    float cos_theta = max(0.0, dot(N,Vi));
    float cos_alpha = max(0.0, dot(N, H));
    vec3 phong = 1.8*color*source.POW * ((kd/PI)+(ks*((n+8.0)/(8.0*PI)))*pow(cos_alpha,n))*cos_theta;
    return phong;
}
// =============================================================================================================

//FONCTION POUR CALCULER LE PHONG MODIFIE 
//====================phong======================================
vec3 phongPlan(Plan plan, Ray ray, Source source)
{
    float kd = plan.mat.kd;
    float ks = plan.mat.ks;
    float n = plan.mat.n;
    vec3 color = plan.mat.color;
    vec3 I =ray.o+(plan.t*ray.v);
    vec3 N =(normalize(I-plan.n));
    vec3 Vi=(normalize(source.pos-I));
    vec3 Vo=(-ray.o * ray.v);
    vec3 H=(normalize(Vi+Vo));
    float cos_theta = max(0.0, dot(N,Vi));
    float cos_alpha = max(0.0, dot(N, H));
    vec3 phong = 1.8*color*source.POW * ((kd/PI)+(ks*((n+8.0)/(8.0*PI)))*pow(cos_alpha,n))*cos_theta;
    return phong;
}
// =============================================================================================================


//FONCTION POUR INITIALISER LES PLANS,ON DEFINIT LE NOMBRE DE PLAN AINSI QUE LEUR POSITION
// =============================================================================================================
void initializePlans(inout Plan planTab[nbPlan])
{
    vec3 color1 = vec3(1.0, 0.0, 1.0);
    vec3 color2 = vec3(0.0,0.0,1.0);
    Material material1 = Material(ks,kd,n, color1);
    Material material2 = Material(ks,kd,n,color2);
    planTab[0] = Plan(vec3(0.0,-1.0,7.0),1.0,material1, 0.0);
    
}
// =============================================================================================================



//FONCTION POUR INITIALISER LES SPHERES,ON DEFINIT LE NOMBRE DE SPHERE AINSI QUE LEUR COULEUR ET POSITION
// =============================================================================================================
void initializeSpheres(inout Sphere sphereTab[nbSphere])
{
    vec3 color1 = vec3(1.0, 0.6, 0.0);
    Material material1 = Material(ks,kd,n, color1);
    sphereTab[0] = Sphere(vec3(-20.0,380.0,10.0),10.0, material1, 0.0);
    sphereTab[1] = Sphere(vec3(0.0,330.0,0.0),10.0, material1, 0.0);
    sphereTab[2] = Sphere(vec3(90.0,400.0,0.0),76.0, material1, 0.0);
}
// =============================================================================================================




//FONCTION POUR INITIALISER LA OU LES SOURCES DE LUMIERES
// =============================================================================================================
void initializeSources(inout Source sourceTab[nbSource])
{
    sourceTab[0] = Source(vec3(40.0,150.0,300.0),vec3(1.0,1.0,1.0));
    sourceTab[1] = Source(vec3(0.0,1500.0,-300.0),vec3(1.0,1.0,1.0));
    
}
// =============================================================================================================




//FONCTION POUR TROUVER SI IL Y A INTERSECTION AVEC UN OBJET SUR LA TRAJECTOIRE DU RAYON VERS LE POINT EN QUESTION
// =============================================================================================================
bool findOmbre(Ray r, in Sphere sphere, in Sphere sphereTab[nbSphere], in Source source)
{
    float t;
    vec3 I;
    Ray retourRayon;
    bool ombre;
    
    I = r.o+(sphere.t*r.v);
    vec3 N =(normalize(I-sphere.c));
    I=I + 0.01*N;
    retourRayon = Ray(I, source.pos, 0.0);
    ombre = false;
    for(int k=0;k< nbSphere;k++)
    {   
        t = intesectSphere(retourRayon,sphereTab[k]);
        if(t > 0.015 && t < 1.0000){
            ombre = true;
        }
    }
    return ombre;
}
// =============================================================================================================

//FONCTION POUR TROUVER SI IL Y A INTERSECTION AVEC UN OBJET SUR LA TRAJECTOIRE DU RAYON VERS LE POINT EN QUESTION
// =============================================================================================================
bool findOmbrePlan(Ray r,Plan plan,  in Plan planTab[nbSphere], in Source source)
{
    float t;
    vec3 I;
    Ray retourRayon;
    bool ombre;
    
    I = r.o+(plan.t*r.v);
    vec3 N =(normalize(I-plan.n));
    I=I + 0.01*N;
    retourRayon = Ray(I, source.pos, 0.0);
    ombre = false;
    for(int k=0;k< nbPlan;k++)
    {   
        t = intersectPlan(retourRayon,planTab[k]);
        if(t > 0.015 && t < 1.0000){
            ombre = true;
        }
    }
    return ombre;
}
// =============================================================================================================




//FONCTION POUR AFFICHER LA SCENE A PARTIR DU RAYON,DU TABLEAU DE SPHERE ET DES SOURCES
// =============================================================================================================
vec4 Illumination(Ray r, in Sphere sphereTab[nbSphere], in Plan planTab[nbPlan], in Source sourceTab[nbSource])
{
    vec3 phongvar;
    vec3 newPhong;
    float tmin = -1.0;
    float t, tPlan;
    Plan testPlan = planTab[0];

    for(int i=0;i< nbSphere;i++)
    {
       r.t = intesectSphere(r,  sphereTab[i]);
       

        if(r.t > 0.0){
            if(tmin < 0.0 || (r.t < tmin)){
                tmin = r.t;
                sphereTab[i].t = tmin;
                phongvar = vec3(0.0,0.0,0.0);
                newPhong = vec3(0.0,0.0,0.0);
                bool ombre = false;
                for(int j=0; j< nbSource ;j++)
                {   
                    bool ombre = findOmbre(r,sphereTab[i],  sphereTab, sourceTab[j]);
                    if(!ombre){
                        newPhong = phong(sphereTab[i], r, sourceTab[j]);
                        phongvar += newPhong;
                    }
                }
            }
         }
    }
    return vec4(phongvar, 1.0);
}
// =============================================================================================================

//FONCTION POUR AFFICHER LA SCENE A PARTIR DU RAYON,DU TABLEAU DE SPHERE ET DES SOURCES
// =============================================================================================================
vec4 IlluminationPlan(Ray r, in Sphere sphereTab[nbSphere], in Plan planTab[nbPlan], in Source sourceTab[nbSource])
{
    vec3 phongvar;
    vec3 newPhong;
    float tmin = -1.0;
    float t, tPlan;
    Plan testPlan = planTab[0];

    for(int i=0;i< nbSphere;i++)
    {
       t = intersectPlan(r,  planTab[i]);
       
        if(r.t > 0.0){
            if(tmin < 0.0 || (t < tmin)){
                tmin = r.t;
                planTab[i].t = tmin;
                for(int j=0; j< nbSource ;j++)
                {   
                    newPhong = phongPlan(planTab[i], r, sourceTab[j]);
                    phongvar += newPhong;
                    
                }
            }
        }
    }
    return vec4(phongvar, 1.0);
}
// =============================================================================================================





//on initialise la couleur du fond de notre canvas avec fragcolor, puis on definit un rayon, on cree ensuite un tableau de sphere a partir du nombre de spheres puis un tableau de sources a partir du nombre de sources, ensuite on appelle les fonction d'initialisation de sphere et de source puis on affiche la scene avec nos spheres eclairees
// ======================================================= main ======================================================
void main(void) 
{
    Ray r = Ray(vec3(0.0,0.0,0.0), rayDir,1.0);
    Sphere sphereTab[nbSphere];
    Source sourceTab[nbSource];
    Plan planTab[nbPlan];
   

    gl_FragColor = vec4(0.1,0.1,0.1,1.0);
    initializePlans(planTab);
    initializeSpheres(sphereTab);
    initializeSources(sourceTab);
    
    gl_FragColor = IlluminationPlan(r, sphereTab,planTab, sourceTab);
}