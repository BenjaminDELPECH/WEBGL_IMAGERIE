//  DEFINITION DES VARIABLES
// =============================================================================================================
precision mediump float;
varying vec3 rayDir;
varying vec4 vColor;

uniform float n;
uniform float kd;
uniform float ks;


#define PI 3.14159
#define nbSphere 4
#define nbSource 2
#define nbPlan 2
// =============================================================================================================



//DEFINITION DES STRUCURES (Rayon,source materiau et sphere)
// =============================================================================================================
struct Ray {
    vec3 o, v;
    float t;
};

struct Source {
    vec3 pos;
    vec3 POW;
};
struct Plan {
    vec3 normalp;
    vec3 posp;
    vec3 color; 
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

//FONCTION POUR CALCULER L'INTERSECTION DU PLAN ET DU RAYON(on retourne k pour savoir si le rayon traverse ou touche le plan ou pas, si k est compris entre 0 et 1 le rayon traverse et/ou touche le plan
//==============================================================================================================
bool intersectPlan(Ray r,Plan p)
{
    //on calcule d'abord la distance totale entre le point d'origine du rayon et d'arrivee que l'on appellera distrayont
    vec3 ra= r.v *r.t; 
    vec3 distrayont = r.o - ra;

    //on calcule le point d'origine du plan - le point d'origine du rayon que l'on appellera distrayonp
    vec3 distrayonp = p.posp-r.o;

    //Ensuite on fait un produit scalaire de la valeur de la distance totale du rayon  par la normale du plan 
    float vp = dot(distrayont,p.normalp);

    //puis un produit scalaire de la valeur de la distance du rayon(par rapport au plan)  par la la normale du plan
    float wp = dot(distrayonp,p.normalp);

        //on divise les deux 
        float k = wp/vp;
    if(k<=0.0 && k>=1.0) return false ;
    else return true;



}





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




//FONCTION POUR INITIALISER LES PLANS,ON DEFINIT LE NOMBRE DE PLAN AINSI QUE LEUR POSITION
// =============================================================================================================
void initializePlans(inout Plan planTab[nbPlan]){

    vec3 color1 = vec3(0.0, 1.0, 0.6);

    vec3 color2 = vec3(0.0,0.0,1.0);
    planTab[0] = Plan(vec3(0.0,600.0,50.0),vec3(1.0,100.0,100.0),color1);
    planTab[1] = Plan(vec3(100.0,300.0,50.0),vec3(0.600,1.0,0.0),color2);


}
// =============================================================================================================


//FONCTION POUR INITIALISER LES SPHERES,ON DEFINIT LE NOMBRE DE SPHERE AINSI QUE LEUR COULEUR ET POSITION
// =============================================================================================================
void initializeSpheres(inout Sphere sphereTab[nbSphere]){
    vec3 color1 = vec3(1.0, 0.6, 0.0);
    Material material1 = Material(ks,kd,n, color1);
    sphereTab[0] = Sphere(vec3(-0.0,900.0,0.0),50.0, material1, -1.0);
    sphereTab[1] = Sphere(vec3(-100.0,400.0,50.0),50.0, material1, -1.0);
    sphereTab[2] = Sphere(vec3(100.0,400.0,50.0),50.0, material1, -1.0);
    
    sphereTab[3] = Sphere(vec3(0.0,500.0,-50.0),50.0, material1, -1.0);
}
// =============================================================================================================




//FONCTION POUR INITIALISER LA OU LES SOURCES DE LUMIERES
// =============================================================================================================
void initializeSources(inout Source sourceTab[nbSource]){
    sourceTab[0] = Source(vec3(20.0,200.0,-120.0),vec3(1.0,1.0,1.0));
    sourceTab[1] = Source(vec3(20.0,295.0,-250.0),vec3(1.0,1.0,1.0));

}
// =============================================================================================================




//FONCTION POUR AFFICHER LA SCENE A PARTIR DU RAYON,DU TABLEAU DE SPHERE ET DES SOURCES
// =============================================================================================================
void displayScene(Ray r,in Sphere sphereTab[nbSphere],in Plan planTab[nbPlan],in Source sourceTab[nbSource]){
    vec3 phongvar;
    vec3 newPhong;
    float tmin = -1.0;
//==dispensable====
    //boucle pour afficher les plans

    for (int i=0;i<nbPlan;i++)
    {
        if (intersectPlan(r,planTab[i]) == false){
         
        gl_FragColor = vec4(planTab[i].color,0.80);
}
    }
//==dispensable====


    //boucle pour afficher les spheres

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




//on initialise la couleur du fond de notre canvas avec fragcolor, puis on definit un rayon, on cree ensuite un tableau de sphere a partir du nombre de spheres puis un tableau de sources a partir du nombre de sources, ensuite on appelle les fonction d'initialisation de sphere et de source puis on affiche la scene avec nos spheres eclairees
// ======================================================= main ======================================================
void main(void) {
    gl_FragColor = vec4(0.0,0.0,0.0,1.0);
    Ray r = Ray(vec3(0.0,0.0,0.0), rayDir,1.0);
    Sphere sphereTab[nbSphere];
    Source sourceTab[nbSource];

    Plan planTab[nbPlan];
    initializePlans(planTab);
    initializeSpheres(sphereTab);
    initializeSources(sourceTab);
    displayScene(r, sphereTab,planTab, sourceTab);
}
// =============================================================================================================
