#version 130
uniform vec3 camp;
in vec3 vPosition; //Входные переменные vPosition - позция вершины
out vec3 dir,org;     //Выходные переменные; 
in vec3 camera;
void main() 
{ 
gl_Position = vec4(vPosition, 1.0); //Позиция окна 
dir = normalize(vec3(vPosition.x * 1.66667, vPosition.y, -2.0)); 
org=camp;
//org = camera;
}