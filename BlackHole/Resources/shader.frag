#version 410 core

layout (location=3) in mat4 RotMtx;
layout (location=4) in float mass;
layout (location=5) in float dist;

in vec2 TexCoord;

out vec4 FragColor;

uniform samplerCube cubemap;

vec4 GetRay()
{
    return RotMtx * vec4(0,0,-1,0);
}

vec3 FoldRay(vec3 ray, vec2 coord)
{
    
}

void main()
{
    //vec3 ray = GetRay();
    //dir = FoldRay(ray, TexCoord);
    //vec3 dir=ray;
    FragColor = texture(cubemap, vec3(0,0,-1));
}